using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library_Manager.Models;
using PagedList.Core;

namespace Library_Manager.Controllers
{
    public class TBanDocController : Controller
    {
        private readonly QlthuVienContext _context;

        public TBanDocController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: TBanDoc
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.TBanDocs.ToListAsync());
        //}
        
        public IActionResult Index(int? page, string searchString) // Bỏ async và await
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            // 1. Giữ ở dạng IQueryable (Không dùng ToList() hoặc ToListAsync())
            IQueryable<TBanDoc> banDocs = _context.TBanDocs;

            if (!string.IsNullOrEmpty(searchString))
            {
                banDocs = banDocs.Where(bd =>
                    bd.Ten.ToLower().Contains(searchString.ToLower()) ||
                    bd.HoDem.ToLower().Contains(searchString.ToLower()) ||
                    bd.MaBd.Contains(searchString));
            }

            // Sắp xếp
            banDocs = banDocs.OrderBy(bd => bd.MaBd);

            // 2. ToPagedList() sẽ tự xử lý việc thực thi truy vấn phân trang
            var pagedBanDocs = new PagedList<TBanDoc>(banDocs, pageNumber, pageSize);

            // Hoặc sử dụng ToPagedList() nếu bạn đã cài đặt package PagedList.Core.Mvc
            // var pagedBanDocs = banDocs.ToPagedList(pageNumber, pageSize);

            // Truyền lại giá trị tìm kiếm để hiển thị lại trong View
            ViewBag.CurrentFilter = searchString;

            return View(pagedBanDocs);
        }

        // GET: TBanDoc/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tBanDoc = await _context.TBanDocs
                .FirstOrDefaultAsync(m => m.MaBd == id);
            if (tBanDoc == null)
            {
                return NotFound();
            }

            return View(tBanDoc);
        }

        // GET: TBanDoc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TBanDoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaBd,HoDem,Ten,NgaySinh,GioiTinh,DiaChi,Sdt,Email")] TBanDoc tBanDoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tBanDoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tBanDoc);
        }

        // GET: TBanDoc/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tBanDoc = await _context.TBanDocs.FindAsync(id);
            if (tBanDoc == null)
            {
                return NotFound();
            }
            return View(tBanDoc);
        }

        // POST: TBanDoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaBd,HoDem,Ten,NgaySinh,GioiTinh,DiaChi,Sdt,Email")] TBanDoc tBanDoc)
        {
            if (id != tBanDoc.MaBd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tBanDoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TBanDocExists(tBanDoc.MaBd))
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
            return View(tBanDoc);
        }

        // GET: TBanDoc/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tBanDoc = await _context.TBanDocs
                .FirstOrDefaultAsync(m => m.MaBd == id);
            if (tBanDoc == null)
            {
                return NotFound();
            }

            return View(tBanDoc);
        }

        // POST: TBanDoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tBanDoc = await _context.TBanDocs.FindAsync(id);
            if (tBanDoc != null)
            {
                _context.TBanDocs.Remove(tBanDoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TBanDocExists(string id)
        {
            return _context.TBanDocs.Any(e => e.MaBd == id);
        }
    }
}
