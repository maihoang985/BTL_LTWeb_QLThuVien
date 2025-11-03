using Library_Manager.Filters;
using Library_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core; // Thêm dòng này
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_Manager.Controllers
{
    [Authorization("QTV,QLB,QLT,QLM")]
    public class NhanVienController : Controller
    {
        private readonly QlthuVienContext _context;

        public NhanVienController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: NhanVien
        public IActionResult Index(int? page, string searchString)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            // 1. Giữ dạng IQueryable để dễ xử lý phân trang
            IQueryable<TNhanVien> nhanViens = _context.TNhanVien;

            // 2. Nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                nhanViens = nhanViens.Where(nv =>
                    nv.MaNv.Contains(searchString) ||
                    nv.HoDem.ToLower().Contains(searchString.ToLower()) ||
                    nv.Ten.ToLower().Contains(searchString.ToLower()) ||
                    nv.Email.ToLower().Contains(searchString.ToLower()));
            }

            // 3. Sắp xếp
            nhanViens = nhanViens.OrderBy(nv => nv.MaNv);

            // 4. Phân trang
            var pagedNhanViens = new PagedList<TNhanVien>(nhanViens, pageNumber, pageSize);
            // Nếu bạn có cài `PagedList.Core.Mvc` thì có thể dùng cách ngắn gọn hơn:
            // var pagedNhanViens = nhanViens.ToPagedList(pageNumber, pageSize);

            // 5. Truyền giá trị tìm kiếm để hiển thị lại trong View
            ViewBag.CurrentFilter = searchString;

            return View(pagedNhanViens);
        }


        // GET: NhanVien/Details/5
        public async Task<IActionResult> Details(string id, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tNhanVien = await _context.TNhanVien
                .FirstOrDefaultAsync(m => m.MaNv == id);
            if (tNhanVien == null)
            {
                return NotFound();
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(tNhanVien);
        }

        // GET: NhanVien/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NhanVien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("QTV")]
        public async Task<IActionResult> Create([Bind("MaNv,HoDem,Ten,NgaySinh,GioiTinh,DiaChi,Sdt,Email,PhuTrach")] TNhanVien tNhanVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tNhanVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tNhanVien);
        }

        // GET: NhanVien/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tNhanVien = await _context.TNhanVien.FindAsync(id);
            if (tNhanVien == null)
            {
                return NotFound();
            }
            return View(tNhanVien);
        }

        // POST: NhanVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaNv,HoDem,Ten,NgaySinh,GioiTinh,DiaChi,Sdt,Email,PhuTrach")] TNhanVien tNhanVien)
        {
            if (id != tNhanVien.MaNv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tNhanVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TNhanVienExists(tNhanVien.MaNv))
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
            return View(tNhanVien);
        }

        // GET: NhanVien/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tNhanVien = await _context.TNhanVien
                .FirstOrDefaultAsync(m => m.MaNv == id);
            if (tNhanVien == null)
            {
                return NotFound();
            }

            return View(tNhanVien);
        }

        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tNhanVien = await _context.TNhanVien.FindAsync(id);
            if (tNhanVien != null)
            {
                _context.TNhanVien.Remove(tNhanVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TNhanVienExists(string id)
        {
            return _context.TNhanVien.Any(e => e.MaNv == id);
        }
    }
}
