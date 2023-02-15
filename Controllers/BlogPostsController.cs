using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AstraBlog.Data;
using AstraBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AstraBlog.Services.Interfaces;
using AstraBlog.Services;

namespace AstraBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IImageService _imageService;
        readonly IBlogPostService _blogPostService;

        public BlogPostsController(ApplicationDbContext context, UserManager<BlogUser> userManager, IImageService imageService, IBlogPostService blogPostService)
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
            _blogPostService = blogPostService;
        }


        // GET: AdminPage *********************************************************************************
        public async Task<IActionResult> AdminPage()
        {

            var blogPosts = await _blogPostService.GetBlogPostsAsync();


            return View(blogPosts);
        }




        // GET: BlogPosts/Details/5 **********************************************************************************
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(id.Value);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create ********************************************************************************
        public async Task<IActionResult> Create()
        {
            // Query and present the list of categories for the logged in user
            string? userId = _userManager.GetUserId(User);
            
            IEnumerable<Tag> tagsList = await _context.Tags
                                                      .ToListAsync();


            ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
            ViewData["TagsList"] = new MultiSelectList(tagsList, "Id", "Name");

            return View(new BlogPost());
        }

        // POST: BlogPosts/Create ******************************************************************************
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,IsPublished,ImageFile,CategoryList")] BlogPost blogPost, int selectedCategory, IEnumerable<int> selectedTags
            )
        {


            if (ModelState.IsValid)
            {
                //TODO: Slug BlogPost



                // Format Date
                blogPost.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);

                // Reformat if Image was Updated
                if (blogPost.ImageFile != null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                    blogPost.ImageType = blogPost.ImageFile.ContentType;
                }


                blogPost.CategoryId = selectedCategory;

                // calls service to save new blogpost
                await _blogPostService.AddBlogPostAsync(blogPost);



                return RedirectToAction(nameof(Index));
            }
            
            ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
            return View(blogPost);
        }






        // GET: BlogPosts/Edit/5 ******************************************************************************************************
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }



            var blogPost = await _blogPostService.GetBlogPostAsync(id.Value);
            if (blogPost == null)
            {
                return NotFound();
            }


            ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
            return View(blogPost);
        }





        // POST: BlogPosts/Edit/5 **********************************************************************************************************
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,IsPublished,ImageFile,ImageData,ImageType,CategoryList")] BlogPost blogPost, int selectedCategory)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Reformat Created Date
                    blogPost.Created = DataUtility.GetPostGresDate(blogPost.Created);
                    blogPost.Updated = DataUtility.GetPostGresDate(DateTime.UtcNow);

                    // Reformat if Image was Updated
                    if (blogPost.ImageFile != null)
                    {
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                        blogPost.ImageType = blogPost.ImageFile.ContentType;
                    }



                    blogPost.CategoryId = selectedCategory;



                    await _blogPostService.UpdateBlogPostAsync(blogPost);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5 ******************************************************************************************
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(id.Value);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5 ********************************************************************************************
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var blogPost = await _blogPostService.GetBlogPostAsync(id);
            if (blogPost != null)
            {
                await _blogPostService.DeleteBlogPostAsync(blogPost);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> BlogPostExists(int id)
        {
            return (await _blogPostService.GetBlogPostsAsync()).Any(b => b.Id == id);
        }
    }
}
