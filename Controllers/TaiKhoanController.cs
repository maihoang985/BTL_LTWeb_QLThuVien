using Library_Manager.Filters;
using Library_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using Library_Manager.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_Manager.Controllers
{
    //[Authorization("QTV")]
    public class TaiKhoanController : Controller
    {
        private readonly QlthuVienContext _context;

        public TaiKhoanController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: TTaiKhoans
        public IActionResult Index(int? page, string searchString, string roleFilter)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            IQueryable<TTaiKhoan> taiKhoans = _context.TTaiKhoans
                .Include(t => t.MaNvNavigation)
                .Include(t => t.MaVtNavigation);

            if (!string.IsNullOrEmpty(searchString))
            {
                taiKhoans = taiKhoans.Where(tk =>
                    tk.TenDangNhap.ToLower().Contains(searchString.ToLower()) ||
                    tk.MaTk.Contains(searchString) ||
                    tk.MaNv.Contains(searchString));
            }

            // 2. MỚI: Lọc theo vai trò (Role Filter)
            if (!string.IsNullOrEmpty(roleFilter))
            {
                taiKhoans = taiKhoans.Where(tk => tk.MaVt == roleFilter);
            }

            taiKhoans = taiKhoans.OrderBy(tk => tk.MaTk);

            var pagedTaiKhoans = new PagedList<TTaiKhoan>(taiKhoans, pageNumber, pageSize);

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentRoleFilter = roleFilter; // Gửi vai trò đang được chọn về View
            ViewBag.VaiTros = _context.TVaiTros.ToList(); // Gửi danh sách vai trò về View

            return View(pagedTaiKhoans);
        }

        // GET: TTaiKhoans/Details/5
        public async Task<IActionResult> Details(string id, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTaiKhoan = await _context.TTaiKhoans
                .Include(t => t.MaNvNavigation)
                .Include(t => t.MaVtNavigation)
                .FirstOrDefaultAsync(m => m.MaTk == id);
            if (tTaiKhoan == null)
            {
                return NotFound();
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(tTaiKhoan);
        }

        // GET: TTaiKhoans/Create
        public IActionResult Create()
        {
            ViewData["MaNv"] = new SelectList(_context.TNhanViens, "MaNv", "MaNv");
            ViewData["MaVt"] = new SelectList(_context.TVaiTros, "MaVt", "MaVt");
            return View();
        }

        // POST: TTaiKhoans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTk,MaNv,MaVt,TenDangNhap,MatKhau,TrangThai,NgayTao")] TTaiKhoan tTaiKhoan)
        {
            if (ModelState.IsValid)
            {
                // ✅ Băm mật khẩu trước khi lưu
                tTaiKhoan.MatKhau = PasswordHelper.HashPassword(tTaiKhoan.TenDangNhap, tTaiKhoan.MatKhau);

                _context.Add(tTaiKhoan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNv"] = new SelectList(_context.TNhanViens, "MaNv", "MaNv", tTaiKhoan.MaNv);
            ViewData["MaVt"] = new SelectList(_context.TVaiTros, "MaVt", "MaVt", tTaiKhoan.MaVt);
            return View(tTaiKhoan);
        }

        // GET: TTaiKhoans/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTaiKhoan = await _context.TTaiKhoans.FindAsync(id);
            if (tTaiKhoan == null)
            {
                return NotFound();
            }
            ViewData["MaNv"] = new SelectList(_context.TNhanViens, "MaNv", "MaNv", tTaiKhoan.MaNv);
            ViewData["MaVt"] = new SelectList(_context.TVaiTros, "MaVt", "MaVt", tTaiKhoan.MaVt);
            return View(tTaiKhoan);
        }



        // POST: TTaiKhoans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaTk,MaNv,MaVt,TenDangNhap,MatKhau,TrangThai,NgayTao")] TTaiKhoan tTaiKhoan)
        {
            if (id != tTaiKhoan.MaTk)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.TTaiKhoans.AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.MaTk == id);
                    if (existing == null)
                    {
                        return NotFound();
                    }

                    // ✅ Nếu mật khẩu thay đổi => băm lại
                    if (tTaiKhoan.MatKhau != existing.MatKhau)
                    {
                        // (tùy chọn) kiểm tra chuỗi có phải Base64 hoặc đã băm chưa
                        if (!PasswordHelper.IsBase64String(tTaiKhoan.MatKhau))
                        {
                            tTaiKhoan.MatKhau = PasswordHelper.HashPassword(tTaiKhoan.TenDangNhap, tTaiKhoan.MatKhau);
                        }
                    }

                    _context.Update(tTaiKhoan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TTaiKhoanExists(tTaiKhoan.MaTk))
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

            ViewData["MaNv"] = new SelectList(_context.TNhanViens, "MaNv", "MaNv", tTaiKhoan.MaNv);
            ViewData["MaVt"] = new SelectList(_context.TVaiTros, "MaVt", "MaVt", tTaiKhoan.MaVt);
            return View(tTaiKhoan);
        }


        // GET: TTaiKhoans/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTaiKhoan = await _context.TTaiKhoans
                .Include(t => t.MaNvNavigation)
                .Include(t => t.MaVtNavigation)
                .FirstOrDefaultAsync(m => m.MaTk == id);
            if (tTaiKhoan == null)
            {
                return NotFound();
            }

            return View(tTaiKhoan);
        }

        // POST: TTaiKhoans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tTaiKhoan = await _context.TTaiKhoans.FindAsync(id);
            if (tTaiKhoan != null)
            {
                _context.TTaiKhoans.Remove(tTaiKhoan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TTaiKhoanExists(string id)
        {
            return _context.TTaiKhoans.Any(e => e.MaTk == id);
        }
    }
}
