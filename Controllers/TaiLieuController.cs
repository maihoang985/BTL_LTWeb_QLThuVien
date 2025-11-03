using Library_Manager.Filters;
using Library_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_Manager.Controllers
{
    [Authorization("QTV,QLB,QLT,QLM")]
    public class TaiLieuController : Controller
    {
        private readonly QlthuVienContext _context;

        public TaiLieuController(QlthuVienContext context)
        {
            _context = context;
        }

        #region Các chức năng cơ bản (Index, Details, PopulateSelectList, Delete)
        // GET: TaiLieu
        public IActionResult Index(int? page, string searchString, string category, string publisher, string language, string returnUrl)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            IQueryable<TTaiLieu> taiLieus = _context.TTaiLieu
                .Include(t => t.MaDdNavigation).Include(t => t.MaNnNavigation).Include(t => t.MaNxbNavigation)
                .Include(t => t.MaThLNavigation).Include(t => t.MaTkNavigation);

            if (!string.IsNullOrEmpty(searchString))
            {
                taiLieus = taiLieus.Where(tl =>
                    tl.TenTl.ToLower().Contains(searchString.ToLower()) ||
                    tl.MaTl.ToLower().Contains(searchString.ToLower()) ||
                    (tl.MaNxbNavigation != null && tl.MaNxbNavigation.TenNxb.ToLower().Contains(searchString.ToLower())) ||
                    (tl.MaThLNavigation != null && tl.MaThLNavigation.TenThL.ToLower().Contains(searchString.ToLower())));
            }

            if (!string.IsNullOrEmpty(category))
                taiLieus = taiLieus.Where(tl => tl.MaThLNavigation.TenThL == category);

            if (!string.IsNullOrEmpty(publisher))
                taiLieus = taiLieus.Where(tl => tl.MaNxbNavigation.TenNxb == publisher);

            if (!string.IsNullOrEmpty(language))
                taiLieus = taiLieus.Where(tl => tl.MaNnNavigation.TenNn == language);

            taiLieus = taiLieus.OrderBy(tl => tl.MaTl);

            var pagedTaiLieus = new PagedList.Core.PagedList<TTaiLieu>(taiLieus, pageNumber, pageSize);

            ViewBag.CurrentFilter = searchString;

            ViewBag.Categories = _context.TTheLoai.Select(c => c.TenThL).Distinct().ToList();
            ViewBag.Publishers = _context.TNhaXuatBan.Select(p => p.TenNxb).Distinct().ToList();
            ViewBag.Languages = _context.TNgonNgu.Select(l => l.TenNn).Distinct().ToList();

            ViewBag.SelectedCategory = category;
            ViewBag.SelectedPublisher = publisher;
            ViewBag.SelectedLanguage = language;

            ViewBag.ReturnUrl = returnUrl;

            return View(pagedTaiLieus);
        }

        // GET: TaiLieu/Details/5
        public async Task<IActionResult> Details(string id, string returnUrl)
        {
            if (id == null) { return NotFound(); }
            var tTaiLieu = await _context.TTaiLieu
                .Include(t => t.MaDdNavigation)
                .Include(t => t.MaNnNavigation)
                .Include(t => t.MaNxbNavigation)
                .Include(t => t.MaThLNavigation)
                .Include(t => t.MaTkNavigation)
                .Include(t => t.TTaiLieuTacGia)
                .Include(t => t.TBanSao)
                .FirstOrDefaultAsync(m => m.MaTl == id);
            if (tTaiLieu == null) { return NotFound(); }
            ViewBag.ReturnUrl = returnUrl;
            return View(tTaiLieu);
        }

        // HÀM CHUNG: Tải các danh sách SelectList
        private void PopulateSelectList(TTaiLieu tTaiLieu = null)
        {
            ViewData["MaDd"] = new SelectList(_context.TDinhDang, "MaDd", "TenDd", tTaiLieu?.MaDd);
            ViewData["MaNn"] = new SelectList(_context.TNgonNgu, "MaNn", "TenNn", tTaiLieu?.MaNn);
            ViewData["MaNxb"] = new SelectList(_context.TNhaXuatBan, "MaNxb", "TenNxb", tTaiLieu?.MaNxb);
            ViewData["MaThL"] = new SelectList(_context.TTheLoai, "MaThL", "TenThL", tTaiLieu?.MaThL);
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoan, "MaTk", "MaTk", tTaiLieu?.MaTk);

            ViewData["TacGiaList"] = new SelectList(
                _context.TTacGia.Select(tg => new { tg.MaTg, FullName = tg.HoDem + " " + tg.Ten }).OrderBy(x => x.FullName),
                "MaTg", "FullName"
            );
        }

        // GET: TaiLieu/Edit/5
        public async Task<IActionResult> Edit(string id, string returnUrl)
        {
            if (id == null) { return NotFound(); }
            var tTaiLieu = await _context.TTaiLieu
                .Include(t => t.TTaiLieuTacGia)
                .ThenInclude(ttg => ttg.MaTgNavigation)
                .FirstOrDefaultAsync(m => m.MaTl == id);

            if (tTaiLieu == null) { return NotFound(); }
            PopulateSelectList(tTaiLieu);
            ViewBag.ReturnUrl = returnUrl;
            return View(tTaiLieu);
        }

        // POST: TaiLieu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tTaiLieu = await _context.TTaiLieu.FindAsync(id);
            if (tTaiLieu != null) { _context.TTaiLieu.Remove(tTaiLieu); }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TTaiLieuExists(string id)
        {
            return _context.TTaiLieu.Any(e => e.MaTl == id);
        }
        #endregion

        // ------------------------------------------------------------------------------------------------
        // GET: TaiLieu/Create
        // ------------------------------------------------------------------------------------------------
        public IActionResult Create(string returnUrl)
        {
            PopulateSelectList();
            ViewBag.ReturnUrl = returnUrl;
            // Khởi tạo Model rỗng để View không bị lỗi NullReferenceException
            return View(new TTaiLieu());
        }

        // ------------------------------------------------------------------------------------------------
        // POST: TaiLieu/Create (ĐÃ SỬA VÀ TỐI ƯU HÓA)
        // ------------------------------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("MaTl,MaNxb,MaNn,MaThL,MaDd,TenTl,LanXuatBan,NamXuatBan,SoTrang,KhoCo,MaTk")] TTaiLieu tTaiLieu,
            ICollection<TTaiLieuTacGia> TTaiLieuTacGia)
        {
            // 1. LOẠI TRỪ các thuộc tính điều hướng khỏi ModelState
            ModelState.Remove("MaDdNavigation"); ModelState.Remove("MaNnNavigation"); ModelState.Remove("MaTkNavigation");
            ModelState.Remove("MaNxbNavigation"); ModelState.Remove("MaThLNavigation");

            for (int i = 0; i < TTaiLieuTacGia?.Count; i++)
            {
                ModelState.Remove($"TTaiLieuTacGia[{i}].MaTgNavigation");
                ModelState.Remove($"TTaiLieuTacGia[{i}].MaTlNavigation");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Đảm bảo MaTl có giá trị (tự động tạo nếu người dùng không nhập)
                    if (string.IsNullOrEmpty(tTaiLieu.MaTl))
                    {
                        tTaiLieu.MaTl = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
                    }

                    // Thêm Tài liệu chính
                    _context.Add(tTaiLieu);

                    // Thêm các tác giả vào Collection
                    if (TTaiLieuTacGia != null)
                    {
                        foreach (var author in TTaiLieuTacGia)
                        {
                            author.MaTl = tTaiLieu.MaTl; // Gán khóa ngoại MaTl
                            _context.TTaiLieuTacGia.Add(author);
                        }
                    }

                    await _context.SaveChangesAsync();

                    // THÀNH CÔNG: Chuyển hướng về trang Index
                    return RedirectToAction(nameof(Index), new { saveSuccess = true });
                }
                catch (Exception ex)
                {
                    // Lỗi DB khi lưu
                    TempData["StatusMessage"] = "danger";
                    TempData["Message"] = "Lỗi hệ thống khi tạo mới: " + ex.Message;
                }
            }
            else
            {
                // Lỗi Model Binding/Validation
                TempData["StatusMessage"] = "danger";
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                   .Select(x => $"{x.Key}: {string.Join("; ", x.Value.Errors.Select(e => e.ErrorMessage))}");

                TempData["Message"] = $"Dữ liệu không hợp lệ. Vui lòng kiểm tra: <ul><li>{string.Join("</li><li>", errors)}</li></ul>";
            }

            // Trả về View khi thất bại

            // Gán lại dữ liệu Tác giả từ form (để View hiển thị đúng)
            if (TTaiLieuTacGia != null)
            {
                tTaiLieu.TTaiLieuTacGia = new List<TTaiLieuTacGia>();
                foreach (var item in TTaiLieuTacGia)
                {
                    item.MaTgNavigation = await _context.TTacGia.AsNoTracking().FirstOrDefaultAsync(t => t.MaTg == item.MaTg);
                    tTaiLieu.TTaiLieuTacGia.Add(item);
                }
            }

            PopulateSelectList(tTaiLieu);
            return View(tTaiLieu);
        }

        // ------------------------------------------------------------------------------------------------
        // POST: TaiLieu/Edit/5 (Đã được Fix)
        // ------------------------------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, 
            [Bind("MaTl,MaNxb,MaNn,MaThL,MaDd,TenTl,LanXuatBan,NamXuatBan,SoTrang,KhoCo,MaTk")] TTaiLieu tTaiLieu,
            ICollection<TTaiLieuTacGia> TTaiLieuTacGia)
        {
            if (id != tTaiLieu.MaTl) { return NotFound(); }

            // 1. Loại trừ Navigation Properties
            ModelState.Remove("MaDdNavigation"); ModelState.Remove("MaNnNavigation"); ModelState.Remove("MaTkNavigation");
            ModelState.Remove("MaNxbNavigation"); ModelState.Remove("MaThLNavigation");
            for (int i = 0; i < TTaiLieuTacGia?.Count; i++) { ModelState.Remove($"TTaiLieuTacGia[{i}].MaTgNavigation"); ModelState.Remove($"TTaiLieuTacGia[{i}].MaTlNavigation"); }

            if (ModelState.IsValid)
            {
                var originalTaiLieu = await _context.TTaiLieu
                    .Include(t => t.TTaiLieuTacGia).FirstOrDefaultAsync(m => m.MaTl == id);
                if (originalTaiLieu == null) { return NotFound(); }

                try
                {
                    // Cập nhật thuộc tính đơn lẻ an toàn
                    _context.Entry(originalTaiLieu).CurrentValues.SetValues(tTaiLieu);

                    // Xử lý Collection Tác giả
                    originalTaiLieu.TTaiLieuTacGia.Clear();
                    if (TTaiLieuTacGia != null && TTaiLieuTacGia.Any())
                    {
                        foreach (var author in TTaiLieuTacGia) { author.MaTl = originalTaiLieu.MaTl; originalTaiLieu.TTaiLieuTacGia.Add(author); }
                    }
                    await _context.SaveChangesAsync();

                    // THAY ĐỔI: Sử dụng TempData và return View để hiển thị thông báo ngay tại trang Edit
                    TempData["StatusMessage"] = "success";
                    TempData["Message"] = "Thông tin Tài liệu đã được lưu thành công.";

                    // Reload đối tượng để View hiển thị đúng trạng thái mới
                    originalTaiLieu = await _context.TTaiLieu
                        .Include(t => t.TTaiLieuTacGia).ThenInclude(ttg => ttg.MaTgNavigation).FirstOrDefaultAsync(m => m.MaTl == id);

                    PopulateSelectList(originalTaiLieu);
                    
                    return View(originalTaiLieu);

                    // Hoặc uncomment dòng dưới nếu bạn muốn về Index:
                    // return RedirectToAction(nameof(Index), new { saveSuccess = true });
                }
                catch (DbUpdateConcurrencyException) { TempData["StatusMessage"] = "danger"; TempData["Message"] = "Lỗi xung đột dữ liệu. Vui lòng thử lại."; }
                catch (Exception ex) { TempData["StatusMessage"] = "danger"; TempData["Message"] = "Lỗi hệ thống khi lưu dữ liệu: " + ex.Message; }
            }
            else
            {
                TempData["StatusMessage"] = "danger";
                var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => $"{x.Key}: {string.Join("; ", x.Value.Errors.Select(e => e.ErrorMessage))}");
                TempData["Message"] = $"Dữ liệu không hợp lệ. Vui lòng kiểm tra: <ul><li>{string.Join("</li><li>", errors)}</li></ul>";
            }

            // Xử lý khi Validation thất bại
            if (TTaiLieuTacGia != null)
            {
                var tTaiLieuDisplay = new TTaiLieu();
                _context.Entry(tTaiLieuDisplay).CurrentValues.SetValues(tTaiLieu);
                tTaiLieuDisplay.TTaiLieuTacGia = new List<TTaiLieuTacGia>();
                foreach (var item in TTaiLieuTacGia) { item.MaTgNavigation = await _context.TTacGia.AsNoTracking().FirstOrDefaultAsync(t => t.MaTg == item.MaTg); tTaiLieuDisplay.TTaiLieuTacGia.Add(item); }
                tTaiLieu = tTaiLieuDisplay;
            }
            PopulateSelectList(tTaiLieu);
            
            return View(tTaiLieu);
        }

        // GET: TaiLieu/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            // 1. Kiểm tra ID
            if (id == null)
            {
                return NotFound();
            }

            // 2. Truy vấn tài liệu và các thông tin liên quan cần thiết cho View xác nhận
            var tTaiLieu = await _context.TTaiLieu
                .Include(t => t.MaNxbNavigation)
                .Include(t => t.MaThLNavigation)
                .Include(t => t.MaNnNavigation)
                .Include(t => t.MaDdNavigation)
                .Include(t => t.MaTkNavigation)
                .Include(t => t.TBanSao) // Cần include TBanSaos để đếm số lượng bản sao hiển thị trên trang Delete
                .FirstOrDefaultAsync(m => m.MaTl == id);

            // 3. Kiểm tra tài liệu tồn tại
            if (tTaiLieu == null)
            {
                return NotFound();
            }

            // 4. Trả về View Delete.cshtml
            return View(tTaiLieu);
        }

        #region Code giữ nguyên cho AJAX Actions
        // 1. Tác giả
        [HttpPost][Authorization("QLT")] public async Task<IActionResult> CreateNewTacGiaAjax([FromBody] TacGiaModel model) { if (string.IsNullOrEmpty(model.HoDem) || string.IsNullOrEmpty(model.Ten)) { return Json(new { success = false, message = "Họ đệm và Tên không được để trống." }); } var newMaTg = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(); var newTacGia = new TTacGia { MaTg = newMaTg, HoDem = model.HoDem, Ten = model.Ten }; try { _context.TTacGia.Add(newTacGia); await _context.SaveChangesAsync(); return Json(new { success = true, maTg = newTacGia.MaTg, hoDem = newTacGia.HoDem, ten = newTacGia.Ten, fullName = newTacGia.HoDem + " " + newTacGia.Ten }); } catch (Exception ex) { return Json(new { success = false, message = "Lỗi Database: " + ex.Message }); } }

        // 2. Nhà Xuất Bản
        [HttpPost][Authorization("QLT")] public async Task<IActionResult> CreateNewNxbAjax([FromBody] NxbModel model) { if (string.IsNullOrEmpty(model.TenNxb)) { return Json(new { success = false, message = "Tên Nhà xuất bản không được để trống." }); } var newMaNxb = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(); var newNxb = new TNhaXuatBan { MaNxb = newMaNxb, TenNxb = model.TenNxb }; try { _context.TNhaXuatBan.Add(newNxb); await _context.SaveChangesAsync(); return Json(new { success = true, maNxb = newNxb.MaNxb, tenNxb = newNxb.TenNxb }); } catch (Exception ex) { return Json(new { success = false, message = "Lỗi Database: " + ex.Message }); } }

        // 3. Ngôn ngữ
        [HttpPost][Authorization("QLT")] public async Task<IActionResult> CreateNewNgonNguAjax([FromBody] NnModel model) { if (string.IsNullOrEmpty(model.TenNn)) { return Json(new { success = false, message = "Tên Ngôn ngữ không được để trống." }); } var newMaNn = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(); var newNn = new TNgonNgu { MaNn = newMaNn, TenNn = model.TenNn }; try { _context.TNgonNgu.Add(newNn); await _context.SaveChangesAsync(); return Json(new { success = true, maNn = newNn.MaNn, tenNn = newNn.TenNn }); } catch (Exception ex) { return Json(new { success = false, message = "Lỗi Database: " + ex.Message }); } }

        // 4. Thể loại
        [HttpPost][Authorization("QLT")] public async Task<IActionResult> CreateNewTheLoaiAjax([FromBody] ThLModel model) { if (string.IsNullOrEmpty(model.TenThL)) { return Json(new { success = false, message = "Tên Thể loại không được để trống." }); } var newMaThL = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(); var newThL = new TTheLoai { MaThL = newMaThL, TenThL = model.TenThL }; try { _context.TTheLoai.Add(newThL); await _context.SaveChangesAsync(); return Json(new { success = true, maThL = newThL.MaThL, tenThL = newThL.TenThL }); } catch (Exception ex) { return Json(new { success = false, message = "Lỗi Database: " + ex.Message }); } }

        // 5. Định dạng
        [HttpPost][Authorization("QLT")] public async Task<IActionResult> CreateNewDinhDangAjax([FromBody] DdModel model) { if (string.IsNullOrEmpty(model.TenDd)) { return Json(new { success = false, message = "Tên Định dạng không được để trống." }); } var newMaDd = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(); var newDd = new TDinhDang { MaDd = newMaDd, TenDd = model.TenDd }; try { _context.TDinhDang.Add(newDd); await _context.SaveChangesAsync(); return Json(new { success = true, maDd = newDd.MaDd, tenDd = newDd.TenDd }); } catch (Exception ex) { return Json(new { success = false, message = "Lỗi Database: " + ex.Message }); } }

        public class TacGiaModel { public string HoDem { get; set; } public string Ten { get; set; } }
        public class NxbModel { public string TenNxb { get; set; } }
        public class NnModel { public string TenNn { get; set; } }
        public class ThLModel { public string TenThL { get; set; } }
        public class DdModel { public string TenDd { get; set; } }
        #endregion
    }
}