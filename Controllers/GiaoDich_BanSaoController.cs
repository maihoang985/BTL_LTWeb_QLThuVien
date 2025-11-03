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
    public class GiaoDich_BanSaoController : Controller
    {
        private readonly QlthuVienContext _context;

        public GiaoDich_BanSaoController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: GiaoDichBanSao
        public async Task<IActionResult> Index()
        {
            var qlthuVienContext = _context.TGiaoDichBanSao.Include(t => t.MaBsNavigation).Include(t => t.MaGdNavigation);
            return View(await qlthuVienContext.ToListAsync());
        }

        // GET: GiaoDichBanSao/Details/5
        public async Task<IActionResult> Details(string id, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tGiaoDichBanSao = await _context.TGiaoDichBanSao
                .Where(t => t.MaGd == id)
                .Include(t => t.MaBsNavigation)
                    .ThenInclude(t => t.MaTlNavigation)
                .Include(t => t.MaGdNavigation)
                    .ThenInclude(t => t.MaTbdNavigation)
                        .ThenInclude(t => t.MaBdNavigation)
                .ToListAsync();

            if (tGiaoDichBanSao == null)
            {
                return NotFound();
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(tGiaoDichBanSao);
        }

        // GET: GiaoDichBanSao/Create
        public IActionResult Create()
        {
            ViewData["MaBs"] = new SelectList(_context.TBanSao, "MaBs", "MaBs");
            ViewData["MaGd"] = new SelectList(_context.TGiaoDichMuonTra, "MaGd", "MaGd");
            return View();
        }

        // POST: GiaoDichBanSao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGd,MaBs,TinhTrangMuon,TinhTrangTra")] TGiaoDichBanSao tGiaoDichBanSao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tGiaoDichBanSao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaBs"] = new SelectList(_context.TBanSao, "MaBs", "MaBs", tGiaoDichBanSao.MaBs);
            ViewData["MaGd"] = new SelectList(_context.TGiaoDichMuonTra, "MaGd", "MaGd", tGiaoDichBanSao.MaGd);
            return View(tGiaoDichBanSao);
        }

        // GET: GiaoDichBanSao/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tGiaoDichBanSao = await _context.TGiaoDichBanSao.FindAsync(id);
            if (tGiaoDichBanSao == null)
            {
                return NotFound();
            }
            ViewData["MaBs"] = new SelectList(_context.TBanSao, "MaBs", "MaBs", tGiaoDichBanSao.MaBs);
            ViewData["MaGd"] = new SelectList(_context.TGiaoDichMuonTra, "MaGd", "MaGd", tGiaoDichBanSao.MaGd);
            return View(tGiaoDichBanSao);
        }

        // POST: GiaoDichBanSao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaGd,MaBs,TinhTrangMuon,TinhTrangTra")] TGiaoDichBanSao tGiaoDichBanSao)
        {
            if (id != tGiaoDichBanSao.MaGd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tGiaoDichBanSao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TGiaoDichBanSaoExists(tGiaoDichBanSao.MaGd))
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
            ViewData["MaBs"] = new SelectList(_context.TBanSao, "MaBs", "MaBs", tGiaoDichBanSao.MaBs);
            ViewData["MaGd"] = new SelectList(_context.TGiaoDichMuonTra, "MaGd", "MaGd", tGiaoDichBanSao.MaGd);
            return View(tGiaoDichBanSao);
        }

        // GET: GiaoDichBanSao/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tGiaoDichBanSao = await _context.TGiaoDichBanSao
                .Include(t => t.MaBsNavigation)
                .Include(t => t.MaGdNavigation)
                .FirstOrDefaultAsync(m => m.MaGd == id);
            if (tGiaoDichBanSao == null)
            {
                return NotFound();
            }

            return View(tGiaoDichBanSao);
        }

        // POST: GiaoDichBanSao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tGiaoDichBanSao = await _context.TGiaoDichBanSao.FindAsync(id);
            if (tGiaoDichBanSao != null)
            {
                _context.TGiaoDichBanSao.Remove(tGiaoDichBanSao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TGiaoDichBanSaoExists(string id)
        {
            return _context.TGiaoDichBanSao.Any(e => e.MaGd == id);
        }
    }
}
