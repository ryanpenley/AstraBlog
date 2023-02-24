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
using AstraBlog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace AstraBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IBlogPostService _blogPostService;

        public TagsController(ApplicationDbContext context, UserManager<BlogUser> userManager, IBlogPostService blogPostService)
        {
            _context = context;
            _userManager = userManager;
            _blogPostService = blogPostService;
        }

        // GET: Tags
        public async Task<IActionResult> Index()
        {
            IEnumerable<Tag> model = await _blogPostService.GetTagsAsync();

            return View(model);
        }

        // GET: Tags/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id, int? pageNum)
        {
            if (id == null || _context.Tags == null)
            {
                return NotFound();
            }

            Tag tag = await _blogPostService.GetTagAsync(id.Value);

            if (tag == null)
            {
                return NotFound();
            }

            int page = pageNum ?? 1;

            ViewData["Page"] = page;

            return View(tag);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Tag tag)
        {
            if (ModelState.IsValid)
            {
              await _blogPostService.AddNewTagAsync(tag);
          
              return RedirectToAction(nameof(Index));
            }

            return View(tag);
        }

        // GET: Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tags == null)
            {
                return NotFound();
            }

            Tag tag = await _blogPostService.GetTagAsync(id.Value);

            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        // POST: Tags/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Tag tag)
        {
            if (id != tag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(tag.Id))
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
            return View(tag);
        }

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tags == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tags == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tags'  is null.");
            }
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TagExists(int id)
        {
          return (_context.Tags?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
