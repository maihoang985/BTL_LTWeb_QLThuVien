using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library_Manager.Models;

namespace Library_Manager.Controllers
{
    public class TaiLieu_TacGiaController : Controller
    {
        private readonly QlthuVienContext _context;

        public TaiLieu_TacGiaController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: TaiLieu_TacGia
        public async Task<IActionResult> Index()
        {
            var qlthuVienContext = _context.TTaiLieuTacGia.Include(t => t.MaTgNavigation).Include(t => t.MaTlNavigation);
            return View(await qlthuVienContext.ToListAsync());
        }

        // GET: TaiLieu_TacGia/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTaiLieuTacGia = await _context.TTaiLieuTacGia
                .Include(t => t.MaTgNavigation)
                .Include(t => t.MaTlNavigation)
                .FirstOrDefaultAsync(m => m.MaTl == id);
            if (tTaiLieuTacGia == null)
            {
                return NotFound();
            }

            return View(tTaiLieuTacGia);
        }

        // GET: TaiLieu_TacGia/Create
        public IActionResult Create()
        {
            ViewData["MaTg"] = new SelectList(_context.TTacGia, "MaTg", "MaTg");
            ViewData["MaTl"] = new SelectList(_context.TTaiLieus, "MaTl", "MaTl");
            return View();
        }

        // POST: TaiLieu_TacGia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTl,MaTg,VaiTro")] TTaiLieuTacGia tTaiLieuTacGia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tTaiLieuTacGia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTg"] = new SelectList(_context.TTacGia, "MaTg", "MaTg", tTaiLieuTacGia.MaTg);
            ViewData["MaTl"] = new SelectList(_context.TTaiLieus, "MaTl", "MaTl", tTaiLieuTacGia.MaTl);
            return View(tTaiLieuTacGia);
        }

        // GET: TaiLieu_TacGia/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTaiLieuTacGia = await _context.TTaiLieuTacGia.FindAsync(id);
            if (tTaiLieuTacGia == null)
            {
                return NotFound();
            }
            ViewData["MaTg"] = new SelectList(_context.TTacGia, "MaTg", "MaTg", tTaiLieuTacGia.MaTg);
            ViewData["MaTl"] = new SelectList(_context.TTaiLieus, "MaTl", "MaTl", tTaiLieuTacGia.MaTl);
            return View(tTaiLieuTacGia);
        }

        // POST: TaiLieu_TacGia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaTl,MaTg,VaiTro")] TTaiLieuTacGia tTaiLieuTacGia)
        {
            if (id != tTaiLieuTacGia.MaTl)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tTaiLieuTacGia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TTaiLieuTacGiaExists(tTaiLieuTacGia.MaTl))
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
            ViewData["MaTg"] = new SelectList(_context.TTacGia, "MaTg", "MaTg", tTaiLieuTacGia.MaTg);
            ViewData["MaTl"] = new SelectList(_context.TTaiLieus, "MaTl", "MaTl", tTaiLieuTacGia.MaTl);
            return View(tTaiLieuTacGia);
        }

        // GET: TaiLieu_TacGia/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTaiLieuTacGia = await _context.TTaiLieuTacGia
                .Include(t => t.MaTgNavigation)
                .Include(t => t.MaTlNavigation)
                .FirstOrDefaultAsync(m => m.MaTl == id);
            if (tTaiLieuTacGia == null)
            {
                return NotFound();
            }

            return View(tTaiLieuTacGia);
        }

        // POST: TaiLieu_TacGia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tTaiLieuTacGia = await _context.TTaiLieuTacGia.FindAsync(id);
            if (tTaiLieuTacGia != null)
            {
                _context.TTaiLieuTacGia.Remove(tTaiLieuTacGia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TTaiLieuTacGiaExists(string id)
        {
            return _context.TTaiLieuTacGia.Any(e => e.MaTl == id);
        }
    }
}
