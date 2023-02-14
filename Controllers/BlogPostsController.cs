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

namespace AstraBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;


        public BlogPostsController(ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BlogPosts *****************************************************************************************
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            List<BlogPost> posts = new List<BlogPost>();

            posts = await _context.BlogPosts.Include(c => c.Category).ToListAsync();


            return View(posts);
        }

        // GET: BlogPosts/Details/5 **********************************************************************************
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
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

            IEnumerable<Category> categoriesList = await _context.Categories
                                                                 .ToListAsync();
            
            IEnumerable<Tag> tagsList = await _context.Tags
                                                      .ToListAsync();


            ViewData["CategoryList"] = new SelectList(categoriesList, "Id", "Name");
            ViewData["TagsList"] = new MultiSelectList(tagsList, "Id", "Name");

            return View();
        }

        // POST: BlogPosts/Create ******************************************************************************
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,IsPublished,ImageData,ImageType,CategoryId")] BlogPost blogPost, int selectedCategory, IEnumerable<int> selectedTags
            )
        {


            if (ModelState.IsValid)
            {
                //TODO: Slug BlogPost



                // Format Date
                blogPost.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);



                blogPost.CategoryId= selectedCategory;



                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TagsList"] = new MultiSelectList(_context.Categories, "Id", "Name");
            
            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name");
            return View(blogPost);
        }






        // GET: BlogPosts/Edit/5 ******************************************************************************************************
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }



            var blogPost = await _context.BlogPosts
                                         .Include(b => b.Category)
                                         .FirstOrDefaultAsync(b => b.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }


            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name");
            return View(blogPost);
        }





        // POST: BlogPosts/Edit/5 **********************************************************************************************************
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,IsPublished,ImageData,ImageType,CategoryId")] BlogPost blogPost, int selectedCategory)
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


                    blogPost.CategoryId = selectedCategory;


                    _context.Update(blogPost);


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5 ******************************************************************************************
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            if (_context.BlogPosts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BlogPosts'  is null.");
            }
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
          return (_context.BlogPosts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
