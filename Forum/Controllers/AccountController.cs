using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Forum.ViewModels;
using Forum.Models;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
namespace Forum.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ForumContext _db;
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration conf;
        private readonly Forum.Services.IMailSender mailSender;
        private readonly ILogger<AccountController> _logger;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
                                 ForumContext context, IWebHostEnvironment env, IConfiguration configuration,
                                 Forum.Services.IMailSender sender, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = context;
            this.env = env;
            conf = configuration;
            mailSender = sender;
            _logger = logger;
        }
        [Authorize]
        [Route("Account/Show/{name}")]
        [HttpGet]
        public async Task<IActionResult> Show(string name)
        {
            User user = await _userManager.FindByNameAsync(name);
            if(user == null)
                return StatusCode(404);
            User requester = await _userManager.FindByNameAsync(User.Identity.Name);
            if (name != User.Identity.Name)
                user.DialogMessages = (from msg in _db.PrivateMessages
                                       where (msg.SenderId == user.Id && msg.ReceiverId == requester.Id) ||
                                             (msg.SenderId == requester.Id && msg.ReceiverId == user.Id)
                                       orderby msg.Published
                                       select msg).Select(delegate (PrivateMessage msg, int i)
                                      {
                                          _db.Entry(msg).Reference(m => m.Sender).Load();
                                          return msg;
                                      }).ToList();
            else
            {
                user.DialogMessages = _db.PrivateMessages.
                                      Where(msg => msg.SenderId == user.Id || msg.ReceiverId == user.Id).AsEnumerable().
                                      GroupBy(msg => msg.SenderId == user.Id ? msg.ReceiverId : msg.SenderId).
                                      Select( group => group.OrderByDescending(msg => msg.Published).First()).
                                      Select(delegate (PrivateMessage msg, int i)
                                      {
                                          _db.Entry(msg).Reference(m => m.Sender).Load();
                                          _db.Entry(msg).Reference(m => m.Receiver).Load();
                                          return msg;
                                      }).
                                      OrderByDescending(msg => msg.Published).ToList();
            }
                return View(user);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendPrivateMessage(string name, string text)
        {
            User sender = await _userManager.FindByNameAsync(User.Identity.Name);
            User receiver = await _userManager.FindByNameAsync(name);
            if (receiver == null)
                return StatusCode(404);
            PrivateMessage msg = new PrivateMessage()
            {
                Published = DateTime.UtcNow,
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                Text = text
            };
            await _db.PrivateMessages.AddAsync(msg);
            await _db.SaveChangesAsync();
            return StatusCode(200);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadProfile(string name, IFormFile ifile)
        {

            User user = await _userManager.FindByNameAsync(name);
            if(user == null)
                return StatusCode(404);
            if (ifile != null)
            {
                string uploadsFolder = Path.Combine(env.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ifile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                if (user.ProfileImage != conf["DefaultProfilePath"])
                    System.IO.File.Delete(Path.Combine(uploadsFolder, user.ProfileImage));
                user.ProfileImage = uniqueFileName;
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ifile.CopyTo(fileStream);
                }
                await _db.SaveChangesAsync();
                _logger.LogInformation("{0} has new image at {1}", user.UserName, filePath);
            }
            return RedirectToAction("Show", new { name = name });
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByNameAsync(model.Login);
                if (user.EmailConfirmed)
                {
                    var result =
                        await _signInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                            return Redirect(model.ReturnUrl);
                        else
                            return RedirectToAction("Index", "Home");
                    }
                    else
                        ModelState.AddModelError("", "Wrong login and (or) password.");
                }
                else
                    ModelState.AddModelError("", "First verify your email.");
            }
            return View(model);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {        
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.Login,
                    ProfileImage = conf["DefaultProfilePath"],
                    Registered = DateTime.Today
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    mailSender.Send(user.UserName, user.Email, user.Id, code);
                    _logger.LogInformation("{0} just created account.", user.UserName);
                    return RedirectToAction("Login");
                }
                else
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Verify(string token, string id)
        {
            if (id == null || token == null)
            {
                return StatusCode(404);
            }
            var codeDecodedBytes = WebEncoders.Base64UrlDecode(token);
            token = Encoding.UTF8.GetString(codeDecodedBytes);
            var result = await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(id), token);
            if (result.Succeeded)
            {
                _logger.LogInformation("Succesful verification for userId {0}", id);
                return RedirectToAction("Login");
            }
            else
            {
                _logger.LogInformation("Unsuccesful verification attempt");
                return StatusCode(404);
            }
        }
    }
}
