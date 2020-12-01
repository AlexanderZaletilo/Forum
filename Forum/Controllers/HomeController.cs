using Forum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Forum.Controllers
{
    public class HomeController : Controller
    {
     
        private readonly ILogger<HomeController> _logger;
        private readonly ForumContext _db;
        public HomeController(ILogger<HomeController> logger, ForumContext context)
        {
            _logger = logger;
            _db = context;
        }
        [Route("Topic/Show")]
        [Route("Home/Index")]
        [Route("")]
        public IActionResult Index()
        {
            return View(_db.Topics.OrderByDescending(topic => topic.MessageCount).Select(delegate (Topic topic)
            {
                _db.Entry(topic).Reference(t => t.User).Load();
                return topic;
            }));
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int code)
        {
            _logger.LogInformation("Error request with code {0} and ip {1} with url {2}", code, HttpContext.Connection.RemoteIpAddress.ToString(),
                HttpContext.Features.Get<IHttpRequestFeature>().RawTarget);
            if (code >= 500)
                _logger.LogError("Server error with code{0} and ip {1} with request url{2}", code, HttpContext.Connection.RemoteIpAddress.ToString(),
                    HttpContext.Features.Get<IHttpRequestFeature>().RawTarget);
            ViewData["code"] = code;
            return View();
        }
    }
}
