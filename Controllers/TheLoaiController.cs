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
    public class TheLoaiController : Controller
    {
        private readonly QlthuVienContext _context;

        public TheLoaiController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: TheLoai
        public IActionResult Index(int? page, string searchString)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            // Giữ ở dạng IQueryable để linh hoạt lọc và sắp xếp
            IQueryable<TTheLoai> theLoais = _context.TTheLoais;

            // Tìm kiếm theo mã hoặc tên thể loại
            if (!string.IsNullOrEmpty(searchString))
            {
                theLoais = theLoais.Where(tl =>
                    tl.TenThL.ToLower().Contains(searchString.ToLower()) ||
                    tl.MaThL.ToLower().Contains(searchString.ToLower()));
            }

            // Sắp xếp theo mã thể loại
            theLoais = theLoais.OrderBy(tl => tl.MaThL);

            // Tạo danh sách phân trang
            var pagedTheLoais = new PagedList<TTheLoai>(theLoais, pageNumber, pageSize);

            // Giữ lại giá trị tìm kiếm để hiển thị lại trên view
            ViewBag.CurrentFilter = searchString;

            return View(pagedTheLoais);
        }

        // GET: TheLoai/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTheLoai = await _context.TTheLoais
                .FirstOrDefaultAsync(m => m.MaThL == id);
            if (tTheLoai == null)
            {
                return NotFound();
            }

            return View(tTheLoai);
        }

        // GET: TheLoai/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TheLoai/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaThL,TenThL")] TTheLoai tTheLoai)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tTheLoai);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tTheLoai);
        }

        // GET: TheLoai/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTheLoai = await _context.TTheLoais.FindAsync(id);
            if (tTheLoai == null)
            {
                return NotFound();
            }
            return View(tTheLoai);
        }

        // POST: TheLoai/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaThL,TenThL")] TTheLoai tTheLoai)
        {
            if (id != tTheLoai.MaThL)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tTheLoai);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TTheLoaiExists(tTheLoai.MaThL))
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
            return View(tTheLoai);
        }

        // GET: TheLoai/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTheLoai = await _context.TTheLoais
                .FirstOrDefaultAsync(m => m.MaThL == id);
            if (tTheLoai == null)
            {
                return NotFound();
            }

            return View(tTheLoai);
        }

        // POST: TheLoai/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tTheLoai = await _context.TTheLoais.FindAsync(id);
            if (tTheLoai != null)
            {
                _context.TTheLoais.Remove(tTheLoai);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TTheLoaiExists(string id)
        {
            return _context.TTheLoais.Any(e => e.MaThL == id);
        }
    }
}
