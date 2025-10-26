using Library_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_Manager.Controllers
{
    public class TacGiaController : Controller
    {
        private readonly QlthuVienContext _context;

        public TacGiaController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: TacGia
        public IActionResult Index(int? page, string searchString)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            // 1. Giữ ở dạng IQueryable (không dùng ToList)
            IQueryable<TTacGia> tacGias = _context.TTacGia;

            // 2. Nếu có chuỗi tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                tacGias = tacGias.Where(tg =>
                    tg.Ten.ToLower().Contains(searchString.ToLower()) ||
                    tg.HoDem.ToLower().Contains(searchString.ToLower()) ||
                    tg.MaTg.Contains(searchString));
            }

            // 3. Sắp xếp
            tacGias = tacGias.OrderBy(tg => tg.MaTg);

            // 4. Phân trang
            var pagedTacGias = new PagedList<TTacGia>(tacGias, pageNumber, pageSize);
            // Hoặc: var pagedTacGias = tacGias.ToPagedList(pageNumber, pageSize);

            // 5. Truyền lại giá trị tìm kiếm để hiển thị lại
            ViewBag.CurrentFilter = searchString;

            return View(pagedTacGias);
        }


        // GET: TacGia/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTacGium = await _context.TTacGia
                .FirstOrDefaultAsync(m => m.MaTg == id);
            if (tTacGium == null)
            {
                return NotFound();
            }

            return View(tTacGium);
        }

        // GET: TacGia/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TacGia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTg,HoDem,Ten")] TTacGia tTacGium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tTacGium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tTacGium);
        }

        // GET: TacGia/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTacGium = await _context.TTacGia.FindAsync(id);
            if (tTacGium == null)
            {
                return NotFound();
            }
            return View(tTacGium);
        }

        // POST: TacGia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaTg,HoDem,Ten")] TTacGia tTacGium)
        {
            if (id != tTacGium.MaTg)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tTacGium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TTacGiumExists(tTacGium.MaTg))
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
            return View(tTacGium);
        }

        // GET: TacGia/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTacGium = await _context.TTacGia
                .FirstOrDefaultAsync(m => m.MaTg == id);
            if (tTacGium == null)
            {
                return NotFound();
            }

            return View(tTacGium);
        }

        // POST: TacGia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tTacGium = await _context.TTacGia.FindAsync(id);
            if (tTacGium != null)
            {
                _context.TTacGia.Remove(tTacGium);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TTacGiumExists(string id)
        {
            return _context.TTacGia.Any(e => e.MaTg == id);
        }
    }
}
