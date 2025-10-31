using Library_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Library_Manager.Controllers
{
    public class TheBanDocController : Controller
    {
        private readonly QlthuVienContext _context;

        public TheBanDocController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: TheBanDoc
        public IActionResult Index(int? page, string searchString)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            IQueryable<TTheBanDoc> theBanDocs = _context.TTheBanDocs
                .Include(t => t.MaBdNavigation);

            if (!string.IsNullOrEmpty(searchString))
            {
                theBanDocs = theBanDocs.Where(t =>
                    t.MaTbd.Contains(searchString) ||
                    t.MaBd.Contains(searchString) ||
                    t.MaBdNavigation.HoDem.ToLower().Contains(searchString.ToLower()) ||
                    t.MaBdNavigation.Ten.ToLower().Contains(searchString.ToLower()) ||
                    t.TrangThai.Contains(searchString));
            }

            theBanDocs = theBanDocs.OrderBy(t => t.MaTbd);

            var pagedTheBanDocs = new PagedList<TTheBanDoc>(theBanDocs, pageNumber, pageSize);

            ViewBag.CurrentFilter = searchString;
            return View(pagedTheBanDocs);
        }

        // GET: TheBanDoc/Details/5
        public async Task<IActionResult> Details(string id, string returnUrl = null)
        {
            if (id == null) { return NotFound(); }

            var tTheBanDoc = await _context.TTheBanDocs
                .Include(t => t.MaBdNavigation)
                .Include(t => t.MaTkNavigation)
                .ThenInclude(t => t.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaTbd == id);

            if (tTheBanDoc == null) { return NotFound(); }

            ViewBag.ReturnUrl = returnUrl;
            return View(tTheBanDoc);
        }

        // GET: TheBanDoc/Create
        public async Task<IActionResult> Create()
        {
            await PopulateBanDocChuaCoTheDropDownList();
            return View();
        }

        // POST: TheBanDoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTbd,MaBd,NgayCap,NgayHetHan,TrangThai")] TTheBanDoc tTheBanDoc)
        {
            var maTk = HttpContext.Session.GetString("MaTk");
            ModelState.Remove("MaTk");

            if (string.IsNullOrEmpty(maTk))
            {
                ModelState.AddModelError(string.Empty, "Bạn phải đăng nhập để thực hiện chức năng này.");
            }
            else
            {
                tTheBanDoc.MaTk = maTk;
            }

            ModelState.Remove("MaTk");

            if (ModelState.IsValid)
            {
                _context.Add(tTheBanDoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateBanDocChuaCoTheDropDownList(tTheBanDoc.MaBd);
            return View(tTheBanDoc);
        }

        // Helper private
        private async Task PopulateBanDocChuaCoTheDropDownList(object selectedBanDoc = null)
        {
            var maBdDaCoThe = _context.TTheBanDocs.Select(t => t.MaBd);
            var banDocChuaCoThe = await _context.TBanDocs
                .Where(b => !maBdDaCoThe.Contains(b.MaBd))
                .Select(b => new { MaBd = b.MaBd, HoTen = b.HoDem + " " + b.Ten })
                .OrderBy(b => b.HoTen)
                .ToListAsync();
            ViewData["MaBd"] = new SelectList(banDocChuaCoThe, "MaBd", "HoTen", selectedBanDoc);
        }


        // GET: TheBanDoc/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) { return NotFound(); }

            var tTheBanDoc = await _context.TTheBanDocs.FindAsync(id);
            if (tTheBanDoc == null) { return NotFound(); }

            // Logic lấy Họ Tên Bạn đọc (Giữ nguyên)
            var banDoc = await _context.TBanDocs.FindAsync(tTheBanDoc.MaBd);
            if (banDoc != null)
            {
                ViewBag.HoTenBanDoc = banDoc.HoDem + " " + banDoc.Ten;
            }
            else
            {
                ViewBag.HoTenBanDoc = "Lỗi: Không tìm thấy bạn đọc";
            }

            return View(tTheBanDoc);
        }

        // POST: TheBanDoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaTbd,MaBd,NgayCap,NgayHetHan,TrangThai")] TTheBanDoc tTheBanDoc)
        {
            if (id != tTheBanDoc.MaTbd) { return NotFound(); }

            // 1. Loại trừ Navigation Properties (giống TaiLieu)
            ModelState.Remove("MaTk"); // MaTk không có trên form
            ModelState.Remove("MaBdNavigation");
            ModelState.Remove("MaTkNavigation");

            if (ModelState.IsValid)
            {
                // Logic quan trọng: Phải lấy lại MaTk (người tạo thẻ) gốc vì nó không được post lên
                var originalMaTk = await _context.TTheBanDocs
                    .Where(t => t.MaTbd == id)
                    .Select(t => t.MaTk)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (originalMaTk == null)
                {
                    // Thẻ đã bị xóa
                    TempData["StatusMessage"] = "danger";
                    TempData["Message"] = "Lỗi: Không tìm thấy thẻ gốc để cập nhật.";
                    // Vẫn phải trả về View với thông tin
                    var banDocDisplay = await _context.TBanDocs.FindAsync(tTheBanDoc.MaBd);
                    ViewBag.HoTenBanDoc = banDocDisplay != null ? (banDocDisplay.HoDem + " " + banDocDisplay.Ten) : "Lỗi";
                    return View(tTheBanDoc);
                }

                try
                {
                    // Gán lại MaTk gốc cho đối tượng
                    tTheBanDoc.MaTk = originalMaTk;

                    // Cập nhật thẻ
                    _context.Update(tTheBanDoc);
                    await _context.SaveChangesAsync();

                    // THÀNH CÔNG: Sử dụng TempData và return View (giống TaiLieu)
                    TempData["StatusMessage"] = "success";
                    TempData["Message"] = "Thông tin Thẻ Bạn đọc đã được lưu thành công.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TTheBanDocs.Any(e => e.MaTbd == id)) { return NotFound(); }

                    TempData["StatusMessage"] = "danger";
                    TempData["Message"] = "Lỗi xung đột dữ liệu. Thẻ này vừa được chỉnh sửa bởi người khác. Vui lòng tải lại trang.";
                }
                catch (Exception ex)
                {
                    TempData["StatusMessage"] = "danger";
                    TempData["Message"] = "Lỗi hệ thống khi lưu dữ liệu: " + ex.Message;
                }
            }
            else
            {
                // Lỗi Model Binding/Validation (giống TaiLieu)
                TempData["StatusMessage"] = "danger";
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                    .Select(x => $"{x.Key}: {string.Join("; ", x.Value.Errors.Select(e => e.ErrorMessage))}");
                TempData["Message"] = $"Dữ liệu không hợp lệ. Vui lòng kiểm tra: <ul><li>{string.Join("</li><li>", errors)}</li></ul>";
            }

            // Xử lý khi THẤT BẠI hoặc THÀNH CÔNG (luôn return View)
            // Tải lại HoTen cho ô input readonly
            var banDoc = await _context.TBanDocs.FindAsync(tTheBanDoc.MaBd);
            ViewBag.HoTenBanDoc = banDoc != null ? (banDoc.HoDem + " " + banDoc.Ten) : "Lỗi";

            return View(tTheBanDoc);
        }

        #region Hàm Delete (Giữ nguyên)
        // GET: TheBanDoc/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) { return NotFound(); }

            var tTheBanDoc = await _context.TTheBanDocs
                .Include(t => t.MaBdNavigation)
                .FirstOrDefaultAsync(m => m.MaTbd == id);

            if (tTheBanDoc == null) { return NotFound(); }

            return View(tTheBanDoc);
        }

        // POST: TheBanDoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tTheBanDoc = await _context.TTheBanDocs.FindAsync(id);
            if (tTheBanDoc != null)
            {
                _context.TTheBanDocs.Remove(tTheBanDoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TTheBanDocExists(string id)
        {
            return _context.TTheBanDocs.Any(e => e.MaTbd == id);
        }
        #endregion
    }
}