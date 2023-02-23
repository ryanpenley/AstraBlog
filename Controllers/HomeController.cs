using AstraBlog.Data;
using AstraBlog.Models;
using AstraBlog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Drawing.Printing;
using X.PagedList;

namespace AstraBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogPostService _blogPostService;
        private readonly IEmailSender _emailService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBlogPostService blogPostService, IEmailSender emailService)
        {
            _logger = logger;
            _blogPostService = blogPostService;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index(int? pageNum, string? swalMessage = null)
        {
            int pageSize = 3;
            int page = pageNum?? 1;





            IPagedList<BlogPost> model = (await _blogPostService.GetRecentPostsAsync()).ToPagedList(page,pageSize);

            ViewData["SwalMessage"] = swalMessage;

            return View(model);
        }

        public async Task<IActionResult> PopularPosts(int? pageNum)
        {

            // add pageSize and page = pageNum

            int pagesize = 3;
            int page = pageNum ?? 1;
            ViewData["Page"] = page;

            IPagedList<BlogPost> model = (await _blogPostService.GetPopularPostsAsync()).ToPagedList(page,pagesize);

            return View(model); 
        }
        public async Task<IActionResult> RecentPosts(int? pageNum)
        {

            // add pageSize and page = pageNum

            int pagesize = 3;
            int page = pageNum ?? 1;
            ViewData["Page"] = page;

            IPagedList<BlogPost> model = (await _blogPostService.GetRecentPostsAsync()).ToPagedList(page,pagesize);

            return View(model); 
        }

        public IActionResult Contact() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactMe(EmailData emailData)
        {
            if (ModelState.IsValid)
            {
                string? swalMessage = string.Empty;

                try
                {
                    emailData.Body = ($"""<strong>{emailData.EmailSenderName}</strong> sent a message:<br><br>{emailData.Body}<br><br><strong>Their email is:<a href="mailto:{emailData.SenderEmailAddress}">{emailData.SenderEmailAddress}</a></strong>""");

                    await _emailService.SendEmailAsync("ryanpenley@gmail.com", "Message from Humble Hacker Contact Me", emailData.Body!);

                    //swalMessage = "Success: Your Email has been sent!";

                    return RedirectToAction(nameof(Index), new { swalMessage });
                }
                catch (Exception)
                {
                    //swalMessage = "Error! Your Email Failed to Send!";
                    return RedirectToAction(nameof(Index), new { swalMessage });
                    throw;
                }
            }

            return View(emailData);
        }


        public IActionResult SearchIndex(string? searchString, int? pageNum)
        {
            int pageSize = 5;
            int page = pageNum ?? 1;

            IPagedList<BlogPost> model = (_blogPostService.SearchBlogPosts(searchString)).ToPagedList(page, pageSize);


            return View(nameof(Index), model);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}