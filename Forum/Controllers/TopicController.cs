using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Models;
using Forum.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Forum.Controllers
{
    [Authorize]
    public class TopicController : Controller
    {
        readonly UserManager<User> _userManager;
        readonly ForumContext _db;
        private readonly ILogger<TopicController> _logger;
        public TopicController(ForumContext context, UserManager<User> userManager, ILogger<TopicController> logger)
        {
            _db = context;
            _userManager = userManager;
            _logger = logger;
        }
        [Route("Topic/Show/{id:int}/Page-{page:int:min(1)}")]
        [Route("Topic/Show/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> Show(int id, int page=1)
        {
            int pageSize = 4;
            Topic topic;
            try {
                topic = _db.Topics.First(t => t.Id == id);
            }
            catch(Exception _)
            {
                return StatusCode(404);
            }
            _db.Entry(topic).Reference(t => t.User).Load();
            var count = await _db.Messages.CountAsync(msg => msg.TopicId == id);
            topic.Messages = await _db.Messages.Where(msg => msg.TopicId == id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            foreach (var msg in topic.Messages)
                _db.Entry(msg).Reference(m => m.User).Load();
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            return View(new TopicShowViewModel() 
            {
                Topic = topic,
                Id = topic.Id,
                PageViewModel = pageViewModel
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddMessage(TopicShowViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                Topic topic = _db.Topics.FirstOrDefault(t => t.Id == model.Id);
                Message msg = new Message
                {
                    Text = model.Text,
                    UserId = user.Id,
                    TopicId = topic.Id,
                    Published = DateTime.UtcNow,
                };
                topic.MessageCount++;
                user.MessageCount++;
                await _db.Messages.AddAsync(msg);
                await _db.SaveChangesAsync();
                _logger.LogInformation("To topic {0} user {1} has added message with id {2}", topic.Id, user.UserName, msg.Id);
                return RedirectToAction("Show", "Topic", new { id = topic.Id });
            }
            else
                return View("~/Views/Topic/Show.cshtml", model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(TopicCreationViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                Topic topic = new Topic
                {
                    Name = model.Name,
                    UserId = user.Id,
                    MessageCount = 1,
                    Opened = DateTime.UtcNow,
                };
                var result = await _db.Topics.AddAsync(topic);
                user.MessageCount += 1;
                await _db.SaveChangesAsync();
                Message msg = new Message
                {
                    UserId = user.Id,
                    TopicId = result.Entity.Id,
                    Text = model.InitialMessage,
                    Published = topic.Opened
                };
                await _db.Messages.AddAsync(msg);
                await _db.SaveChangesAsync();
                _logger.LogInformation("User {0} created topic {1}", user.UserName, topic.Id);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}
