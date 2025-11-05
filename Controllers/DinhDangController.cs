using Library_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient; // Thêm
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data; // Thêm

namespace Library_Manager.Controllers
{
    public class DinhDangController : Controller
    {
        private readonly QlthuVienContext _context;

        public DinhDangController(QlthuVienContext context)
        {
            _context = context;
        }

        // =======================================================
        // GET: DinhDang
        // =======================================================
        public IActionResult Index(int? page, string searchString)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            IQueryable<TDinhDang> dinhDangs = _context.TDinhDang;

            if (!string.IsNullOrEmpty(searchString))
            {
                dinhDangs = dinhDangs.Where(dd =>
                    dd.TenDd.ToLower().Contains(searchString.ToLower()) ||
                    dd.MaDd.ToLower().Contains(searchString.ToLower()));
            }

            dinhDangs = dinhDangs.OrderBy(dd => dd.MaDd);
            var pagedDinhDangs = new PagedList<TDinhDang>(dinhDangs, pageNumber, pageSize);

            ViewBag.CurrentFilter = searchString;

            return View(pagedDinhDangs);
        }

        // =======================================================
        // GET: DinhDang/Details/5
        // =======================================================
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) { return NotFound(); }

            var tDinhDang = await _context.TDinhDang
                .FirstOrDefaultAsync(m => m.MaDd == id);
            if (tDinhDang == null) { return NotFound(); }

            return View(tDinhDang);
        }

        // =======================================================
        // GET: DinhDang/Create
        // =======================================================
        public IActionResult Create()
        {
            return View();
        }

        // =======================================================
        // POST: DinhDang/Create (THÊM LOGIC BẮT LỖI TRÙNG LẶP VÀ CHUẨN HÓA THÔNG BÁO)
        // =======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDd,TenDd")] TDinhDang tDinhDang)
        {
            // Bỏ kiểm tra Navigation Property nếu có (mặc dù Định dạng không có, nên giữ an toàn)
            // ModelState.Remove("MaDdNavigation"); 

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(tDinhDang);
                    await _context.SaveChangesAsync();

                    TempData["StatusMessage"] = "success";
                    TempData["Message"] = $"Đã tạo mới Định dạng: <strong>{tDinhDang.TenDd}</strong> với Mã DĐ: <strong>{tDinhDang.MaDd}</strong>";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx) // Bắt lỗi DB Update (bao gồm trùng lặp)
                {
                    // Kiểm tra lỗi trùng lặp UNIQUE KEY (Mã lỗi SQL 2627 hoặc 2601)
                    if (dbEx.InnerException is SqlException sqlEx &&
                        (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                    {
                        TempData["StatusMessage"] = "danger";
                        // Lỗi trùng lặp có thể xảy ra với MaDd (PK) hoặc TenDd (UNIQUE)
                        TempData["Message"] = "Không thể lưu. Mã hoặc Tên Định dạng đã tồn tại.";
                    }
                    else
                    {
                        // Lỗi DB khác
                        TempData["StatusMessage"] = "danger";
                        string innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                        TempData["Message"] = $"Lỗi hệ thống khi tạo mới: <strong>{innerMessage}</strong>";
                    }
                }
                catch (Exception ex)
                {
                    // Lỗi hệ thống chung
                    TempData["StatusMessage"] = "danger";
                    string innerMessage = ex.InnerException?.Message ?? ex.Message;
                    TempData["Message"] = $"Lỗi hệ thống khi tạo mới: <strong>{innerMessage}</strong>";
                }
            }

            // LỖI VALIDATION hoặc LỖI DB: Trả về View để hiển thị thông báo
            if (!ModelState.IsValid)
            {
                TempData["StatusMessage"] = "danger";
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                  .Select(x => $"{x.Key}: {string.Join("; ", x.Value.Errors.Select(e => e.ErrorMessage))}");
                TempData["Message"] = $"Dữ liệu không hợp lệ. Vui lòng kiểm tra: <ul><li><strong>{string.Join("</strong></li><li><strong>", errors)}</strong></li></ul>";
            }

            return View(tDinhDang);
        }

        // =======================================================
        // GET: DinhDang/Edit/5
        // =======================================================
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) { return NotFound(); }

            var tDinhDang = await _context.TDinhDang.FindAsync(id);
            if (tDinhDang == null) { return NotFound(); }
            return View(tDinhDang);
        }

        // =======================================================
        // POST: DinhDang/Edit/5 (THÊM LOGIC BẮT LỖI TRÙNG LẶP VÀ CHUẨN HÓA THÔNG BÁO)
        // =======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaDd,TenDd")] TDinhDang tDinhDang)
        {
            if (id != tDinhDang.MaDd) { return NotFound(); }

            if (ModelState.IsValid)
            {
                try
                {
                    // Logic cập nhật tương tự NXB/Tác giả để tránh lỗi tracking
                    var originalDinhDang = await _context.TDinhDang
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.MaDd == id);

                    if (originalDinhDang == null) { return NotFound(); }

                    // Ánh xạ trường được phép sửa
                    originalDinhDang.TenDd = tDinhDang.TenDd;

                    _context.Update(originalDinhDang);
                    await _context.SaveChangesAsync();

                    // THÀNH CÔNG: Set TempData
                    TempData["StatusMessage"] = "success";
                    TempData["Message"] = $"Thông tin Định dạng <strong>{originalDinhDang.TenDd}</strong> đã được cập nhật thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TDinhDangExists(tDinhDang.MaDd))
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
                catch (DbUpdateException dbEx) // Bắt lỗi DB Update (bao gồm trùng lặp tên DĐ)
                {
                    // Kiểm tra lỗi trùng lặp UNIQUE KEY (Mã lỗi SQL 2627 hoặc 2601)
                    if (dbEx.InnerException is SqlException sqlEx &&
                        (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                    {
                        TempData["StatusMessage"] = "danger";
                        TempData["Message"] = $"Không thể lưu. Tên Định dạng <strong>{tDinhDang.TenDd}</strong> đã tồn tại.";
                    }
                    else
                    {
                        // LỖI DB khác
                        TempData["StatusMessage"] = "danger";
                        string innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                        TempData["Message"] = $"Lỗi hệ thống khi lưu: <strong>{innerMessage}</strong>";
                    }
                }
                catch (Exception ex)
                {
                    // LỖI HỆ THỐNG: Set TempData
                    TempData["StatusMessage"] = "danger";
                    string innerMessage = ex.InnerException?.Message ?? ex.Message;
                    TempData["Message"] = $"Lỗi hệ thống khi lưu: <strong>{innerMessage}</strong>";
                }
            }
            else
            {
                // LỖI VALIDATION: Set TempData
                TempData["StatusMessage"] = "danger";
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                   .Select(x => $"{x.Key}: {string.Join("; ", x.Value.Errors.Select(e => e.ErrorMessage))}");
                TempData["Message"] = $"Dữ liệu không hợp lệ. Vui lòng kiểm tra: <ul><li><strong>{string.Join("</strong></li><li><strong>", errors)}</strong></li></ul>";
            }

            // LUÔN LUÔN: Return View để hiển thị thông báo
            return View(tDinhDang);
        }

        // =======================================================
        // GET: DinhDang/Delete/5
        // =======================================================
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) { return NotFound(); }

            var tDinhDang = await _context.TDinhDang
                .FirstOrDefaultAsync(m => m.MaDd == id);
            if (tDinhDang == null) { return NotFound(); }

            return View(tDinhDang);
        }

        // =======================================================
        // POST: DinhDang/Delete/5 (THÊM XỬ LÝ LỖI KHÓA NGOẠI VÀ CHUẨN HÓA THÔNG BÁO)
        // =======================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tDinhDang = await _context.TDinhDang.FindAsync(id);

            if (tDinhDang != null)
            {
                try
                {
                    _context.TDinhDang.Remove(tDinhDang);
                    await _context.SaveChangesAsync();

                    TempData["StatusMessage"] = "success";
                    TempData["Message"] = $"Đã xóa Định dạng có Mã: <strong>{id}</strong> thành công.";
                }
                catch (DbUpdateException dbEx) // Bắt lỗi khóa ngoại
                {
                    // Lỗi: Ràng buộc Khóa Ngoại (Foreign Key Constraint)
                    TempData["StatusMessage"] = "danger";
                    TempData["Message"] = $"Không thể xóa Định dạng <strong>{id}</strong> vì đang có tài liệu tham chiếu đến. Vui lòng xóa các tài liệu liên quan trước.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Lỗi hệ thống khác
                    TempData["StatusMessage"] = "danger";
                    TempData["Message"] = $"Lỗi hệ thống khi xóa: <strong>{ex.Message}</strong>";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TDinhDangExists(string id)
        {
            return _context.TDinhDang.Any(e => e.MaDd == id);
        }
    }
}