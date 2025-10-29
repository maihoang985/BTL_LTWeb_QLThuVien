using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library_Manager.Models;
using PagedList.Core; // thêm thư viện phân trang

namespace Library_Manager.Controllers
{
    public class TheBanDocController : Controller
    {
        private readonly QlthuVienContext _context;

        public TheBanDocController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: TheBanDoc
        public IActionResult Index(int? page, string searchString)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            IQueryable<TTheBanDoc> theBanDocs = _context.TTheBanDocs
                .Include(t => t.MaBdNavigation);

            if (!string.IsNullOrEmpty(searchString))
            {
                theBanDocs = theBanDocs.Where(t =>
                    t.MaTbd.Contains(searchString) ||
                    t.MaBd.Contains(searchString) ||
                    t.MaBdNavigation.HoDem.ToLower().Contains(searchString.ToLower()) ||
                    t.MaBdNavigation.Ten.ToLower().Contains(searchString.ToLower()));
            }

            theBanDocs = theBanDocs.OrderBy(t => t.MaTbd);

            var pagedTheBanDocs = new PagedList<TTheBanDoc>(theBanDocs, pageNumber, pageSize);

            ViewBag.CurrentFilter = searchString;
            return View(pagedTheBanDocs);
        }

        // GET: TheBanDoc/Details/5
        public async Task<IActionResult> Details(string id, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTheBanDoc = await _context.TTheBanDocs
                .Include(t => t.MaBdNavigation)
                .FirstOrDefaultAsync(m => m.MaTbd == id);
            if (tTheBanDoc == null)
            {
                return NotFound();
            }

            // Dùng ViewBag hoặc ViewData để truyền returnUrl sang View
            ViewBag.ReturnUrl = returnUrl;

            return View(tTheBanDoc);
        }

        // GET: TheBanDoc/Create
        public IActionResult Create()
        {
            ViewData["MaBd"] = new SelectList(_context.TBanDocs, "MaBd", "MaBd");
            return View();
        }

        // POST: TheBanDoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTbd,MaBd,NgayCap,NgayHetHan,TrangThai")] TTheBanDoc tTheBanDoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tTheBanDoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaBd"] = new SelectList(_context.TBanDocs, "MaBd", "MaBd", tTheBanDoc.MaBd);
            return View(tTheBanDoc);
        }

        // GET: TheBanDoc/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTheBanDoc = await _context.TTheBanDocs.FindAsync(id);
            if (tTheBanDoc == null)
            {
                return NotFound();
            }
            ViewData["MaBd"] = new SelectList(_context.TBanDocs, "MaBd", "MaBd", tTheBanDoc.MaBd);
            return View(tTheBanDoc);
        }

        // POST: TheBanDoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaTbd,MaBd,NgayCap,NgayHetHan,TrangThai")] TTheBanDoc tTheBanDoc)
        {
            if (id != tTheBanDoc.MaTbd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tTheBanDoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TTheBanDocExists(tTheBanDoc.MaTbd))
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
            ViewData["MaBd"] = new SelectList(_context.TBanDocs, "MaBd", "MaBd", tTheBanDoc.MaBd);
            return View(tTheBanDoc);
        }

        // GET: TheBanDoc/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTheBanDoc = await _context.TTheBanDocs
                .Include(t => t.MaBdNavigation)
                .FirstOrDefaultAsync(m => m.MaTbd == id);
            if (tTheBanDoc == null)
            {
                return NotFound();
            }

            return View(tTheBanDoc);
        }

        // POST: TheBanDoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tTheBanDoc = await _context.TTheBanDocs.FindAsync(id);
            if (tTheBanDoc != null)
            {
                _context.TTheBanDocs.Remove(tTheBanDoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TTheBanDocExists(string id)
        {
            return _context.TTheBanDocs.Any(e => e.MaTbd == id);
        }
    }
}
