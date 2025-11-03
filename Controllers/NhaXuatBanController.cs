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
    public class NhaXuatBanController : Controller
    {
        private readonly QlthuVienContext _context;

        public NhaXuatBanController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: NhaXuatBan
        public IActionResult Index(int? page, string searchString)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            // 1. Giữ ở dạng IQueryable (không dùng ToList hay ToListAsync)
            IQueryable<TNhaXuatBan> nhaXuatBans = _context.TNhaXuatBan;

            // 2. Tìm kiếm theo Mã hoặc Tên Nhà Xuất Bản
            if (!string.IsNullOrEmpty(searchString))
            {
                nhaXuatBans = nhaXuatBans.Where(nxb =>
                    nxb.TenNxb.ToLower().Contains(searchString.ToLower()) ||
                    nxb.MaNxb.ToLower().Contains(searchString.ToLower()));
            }

            // 3. Sắp xếp theo mã
            nhaXuatBans = nhaXuatBans.OrderBy(nxb => nxb.MaNxb);

            // 4. Phân trang
            var pagedNXBs = new PagedList<TNhaXuatBan>(nhaXuatBans, pageNumber, pageSize);
            // Hoặc dùng:
            // var pagedNXBs = nhaXuatBans.ToPagedList(pageNumber, pageSize);

            // 5. Giữ lại giá trị tìm kiếm để hiển thị lại trong view
            ViewBag.CurrentFilter = searchString;

            return View(pagedNXBs);
        }


        // GET: NhaXuatBan/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tNhaXuatBan = await _context.TNhaXuatBan
                .FirstOrDefaultAsync(m => m.MaNxb == id);
            if (tNhaXuatBan == null)
            {
                return NotFound();
            }

            return View(tNhaXuatBan);
        }

        // GET: NhaXuatBan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NhaXuatBan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNxb,TenNxb")] TNhaXuatBan tNhaXuatBan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tNhaXuatBan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tNhaXuatBan);
        }

        // GET: NhaXuatBan/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tNhaXuatBan = await _context.TNhaXuatBan.FindAsync(id);
            if (tNhaXuatBan == null)
            {
                return NotFound();
            }
            return View(tNhaXuatBan);
        }

        // POST: NhaXuatBan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaNxb,TenNxb")] TNhaXuatBan tNhaXuatBan)
        {
            if (id != tNhaXuatBan.MaNxb)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tNhaXuatBan);
                    await _context.SaveChangesAsync();

                    // THÀNH CÔNG: Set TempData
                    TempData["StatusMessage"] = "success";
                    TempData["Message"] = "Thông tin Nhà Xuất Bản đã được cập nhật thành công.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TNhaXuatBanExists(tNhaXuatBan.MaNxb))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // LỖI XUNG ĐỘT: Set TempData
                        TempData["StatusMessage"] = "danger";
                        TempData["Message"] = "Lỗi xung đột dữ liệu. Vui lòng thử lại.";
                    }
                }
                catch (Exception ex)
                {
                    // LỖI HỆ THỐNG: Set TempData
                    TempData["StatusMessage"] = "danger";
                    TempData["Message"] = "Lỗi hệ thống khi lưu: " + ex.Message;
                }
            }
            else
            {
                // LỖI VALIDATION: Set TempData
                TempData["StatusMessage"] = "danger";
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                   .Select(x => $"{x.Key}: {string.Join("; ", x.Value.Errors.Select(e => e.ErrorMessage))}");
                TempData["Message"] = $"Dữ liệu không hợp lệ. Vui lòng kiểm tra: <ul><li>{string.Join("</li><li>", errors)}</li></ul>";
            }

            // LUÔN LUÔN: Return View để hiển thị thông báo
            return View(tNhaXuatBan);
        }

        // GET: NhaXuatBan/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tNhaXuatBan = await _context.TNhaXuatBan
                .FirstOrDefaultAsync(m => m.MaNxb == id);
            if (tNhaXuatBan == null)
            {
                return NotFound();
            }

            return View(tNhaXuatBan);
        }

        // POST: NhaXuatBan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tNhaXuatBan = await _context.TNhaXuatBan.FindAsync(id);
            if (tNhaXuatBan != null)
            {
                _context.TNhaXuatBan.Remove(tNhaXuatBan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TNhaXuatBanExists(string id)
        {
            return _context.TNhaXuatBan.Any(e => e.MaNxb == id);
        }
    }
}
