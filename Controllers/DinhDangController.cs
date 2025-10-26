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
    public class DinhDangController : Controller
    {
        private readonly QlthuVienContext _context;

        public DinhDangController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: DinhDang
        public IActionResult Index(int? page, string searchString) // Bỏ async và await
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            // 1. Giữ ở dạng IQueryable (Không dùng ToList hoặc ToListAsync)
            IQueryable<TDinhDang> dinhDangs = _context.TDinhDangs;

            // 2. Lọc theo chuỗi tìm kiếm (nếu có)
            if (!string.IsNullOrEmpty(searchString))
            {
                dinhDangs = dinhDangs.Where(dd =>
                    dd.TenDd.ToLower().Contains(searchString.ToLower()) ||
                    dd.MaDd.ToLower().Contains(searchString.ToLower()));
            }

            // 3. Sắp xếp theo mã định dạng
            dinhDangs = dinhDangs.OrderBy(dd => dd.MaDd);

            // 4. Áp dụng phân trang
            var pagedDinhDangs = new PagedList<TDinhDang>(dinhDangs, pageNumber, pageSize);
            // Hoặc nếu bạn dùng PagedList.Core.Mvc có sẵn: 
            // var pagedDinhDangs = dinhDangs.ToPagedList(pageNumber, pageSize);

            // 5. Truyền lại giá trị tìm kiếm để giữ nguyên trên giao diện
            ViewBag.CurrentFilter = searchString;

            return View(pagedDinhDangs);
        }


        // GET: DinhDang/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tDinhDang = await _context.TDinhDangs
                .FirstOrDefaultAsync(m => m.MaDd == id);
            if (tDinhDang == null)
            {
                return NotFound();
            }

            return View(tDinhDang);
        }

        // GET: DinhDang/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DinhDang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDd,TenDd")] TDinhDang tDinhDang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tDinhDang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tDinhDang);
        }

        // GET: DinhDang/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tDinhDang = await _context.TDinhDangs.FindAsync(id);
            if (tDinhDang == null)
            {
                return NotFound();
            }
            return View(tDinhDang);
        }

        // POST: DinhDang/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaDd,TenDd")] TDinhDang tDinhDang)
        {
            if (id != tDinhDang.MaDd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tDinhDang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TDinhDangExists(tDinhDang.MaDd))
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
            return View(tDinhDang);
        }

        // GET: DinhDang/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tDinhDang = await _context.TDinhDangs
                .FirstOrDefaultAsync(m => m.MaDd == id);
            if (tDinhDang == null)
            {
                return NotFound();
            }

            return View(tDinhDang);
        }

        // POST: DinhDang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tDinhDang = await _context.TDinhDangs.FindAsync(id);
            if (tDinhDang != null)
            {
                _context.TDinhDangs.Remove(tDinhDang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TDinhDangExists(string id)
        {
            return _context.TDinhDangs.Any(e => e.MaDd == id);
        }
    }
}
