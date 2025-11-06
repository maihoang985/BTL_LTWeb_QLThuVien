using Library_Manager.Filters;
using Library_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http; // Cần thiết cho HttpContext.Session.GetString

namespace Library_Manager.Controllers
{
    [Authorization("QTV,QLB,QLT,QLM")]
    [Route("Giao-dich-muon-tra")]
    public class GiaoDichMuonTraController : Controller
    {
        private readonly QlthuVienContext _context;

        public GiaoDichMuonTraController(QlthuVienContext context)
        {
            _context = context;
        }

        // --- Index và Details (Giữ nguyên) ---

        // GET: GiaoDichMuonTra
        [Route("Danh-sach")]
        public IActionResult Index(int? page, string searchString, string returnUrl)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            IQueryable<TGiaoDichMuonTra> giaoDiches = _context.TGiaoDichMuonTra
                .Include(t => t.MaTbdNavigation)
                    .ThenInclude(TheBanDocController => TheBanDocController.MaBdNavigation)
                .Include(t => t.MaTkNavigation);

            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();

                giaoDiches = giaoDiches.Where(gd =>
                    gd.MaTbd.ToLower().Contains(searchLower) ||
                    gd.MaGd.ToLower().Contains(searchLower) ||
                    (gd.TrangThai != null && gd.TrangThai.ToLower().Contains(searchLower)) ||
                    EF.Functions.Like(gd.NgayMuon.Year.ToString(), $"%{searchString}%") ||
                    (
                        gd.MaTbdNavigation != null &&
                        gd.MaTbdNavigation.MaBdNavigation != null &&
                        (
                            gd.MaTbdNavigation.MaBdNavigation.Ten.ToLower().Contains(searchLower) ||
                            gd.MaTbdNavigation.MaBdNavigation.HoDem.ToLower().Contains(searchLower) ||
                            gd.MaTbdNavigation.MaBdNavigation.MaBd.ToLower().Contains(searchLower)
                        )
                    )
                );
            }

            giaoDiches = giaoDiches.OrderBy(gd => gd.MaTbd);
            var pagedGiaoDiches = new PagedList<TGiaoDichMuonTra>(giaoDiches, pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;
            ViewBag.ReturnUrl = returnUrl;
            return View(pagedGiaoDiches);
        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream

>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        // GET: GiaoDichMuonTra/Details/5
        [Route("Chi-tiet/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var tGiaoDichMuonTra = await _context.TGiaoDichMuonTra
                .Include(t => t.MaTbdNavigation)
                    .ThenInclude(BanDocController => BanDocController.MaBdNavigation)
                .Include(t => t.MaTkNavigation)
                    .ThenInclude(NhanVienController => NhanVienController.MaNvNavigation)
                .Include(t => t.TGiaoDichBanSao)
                    .ThenInclude(gdbs => gdbs.MaBsNavigation)
                        .ThenInclude(bs => bs.MaTlNavigation)
                .FirstOrDefaultAsync(m => m.MaGd == id);

            if (tGiaoDichMuonTra == null) return NotFound();

            return View(tGiaoDichMuonTra);
        }


<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
        // --- HÀNH ĐỘNG CREATE ĐÃ ĐIỀU CHỈNH ---

>>>>>>> Stashed changes
=======
        // --- HÀNH ĐỘNG CREATE ĐÃ ĐIỀU CHỈNH ---

>>>>>>> Stashed changes
        // GET: GiaoDichMuonTra/Create
        [Route("Tao-moi")]
        public IActionResult Create(string returnUrl)
        {
            var defaultGd = new TGiaoDichMuonTra
            {
                NgayMuon = DateOnly.FromDateTime(DateTime.Now),
                NgayHenTra = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
            };

            ViewBag.ReturnUrl = returnUrl;
            return View(defaultGd);
        }

        // POST: GiaoDichMuonTra/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        [Route("Tao-moi")]
        public async Task<IActionResult> Create([Bind("MaGd,MaTbd,MaTk,NgayMuon,NgayHenTra,NgayTra,TrangThai")] TGiaoDichMuonTra tGiaoDichMuonTra)
=======
        public async Task<IActionResult> Create(
            [Bind("MaTbd,NgayHenTra")] TGiaoDichMuonTra tGiaoDichMuonTra,
            [FromForm] List<string> selectedBanSaoList)
>>>>>>> Stashed changes
=======
        public async Task<IActionResult> Create(
            [Bind("MaTbd,NgayHenTra")] TGiaoDichMuonTra tGiaoDichMuonTra,
            [FromForm] List<string> selectedBanSaoList)
>>>>>>> Stashed changes
        {
            // Tự động sinh MaGD
            string newMaGd;
            try
            {
                newMaGd = await GenerateNewMaGd();
                tGiaoDichMuonTra.MaGd = newMaGd;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi sinh mã
                TempData["StatusMessage"] = "danger";
                TempData["Message"] = $"Lỗi hệ thống khi sinh mã: <strong>{ex.Message}</strong>";
                return View(tGiaoDichMuonTra);
            }

            // 2. Lấy MaTK từ người dùng đang đăng nhập
            var loggedInMaTk = HttpContext.Session.GetString("MaTk");
            if (string.IsNullOrEmpty(loggedInMaTk))
            {
                TempData["StatusMessage"] = "danger";
                TempData["Message"] = "Lỗi hệ thống: <strong>Không tìm thấy Mã Tài khoản nhân viên đang đăng nhập.</strong> Vui lòng đăng nhập lại.";
                return View(tGiaoDichMuonTra);
            }
            tGiaoDichMuonTra.MaTk = loggedInMaTk;

            // 3. Mặc định Ngày Mượn là ngày hôm nay
            tGiaoDichMuonTra.NgayMuon = DateOnly.FromDateTime(DateTime.Now);

            // 4. Mặc định TrangThai
            tGiaoDichMuonTra.TrangThai = "Đang mượn";
            tGiaoDichMuonTra.NgayTra = null;

            if (selectedBanSaoList == null || !selectedBanSaoList.Any())
            {
                TempData["StatusMessage"] = "danger";
                TempData["Message"] = "Dữ liệu không hợp lệ: <strong>Giao dịch phải có ít nhất một bản sao tài liệu được chọn.</strong>";
                // Không return RedirectToAction, trả về View để hiển thị lỗi ngay trên trang
                return View(tGiaoDichMuonTra);
            }


            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Add(tGiaoDichMuonTra);
                    await _context.SaveChangesAsync();

                    foreach (var maBs in selectedBanSaoList)
                    {
                        var gdbs = new TGiaoDichBanSao
                        {
                            MaGd = tGiaoDichMuonTra.MaGd,
                            MaBs = maBs,
                            TinhTrang = false // 0 = Đang mượn
                        };
                        _context.TGiaoDichBanSao.Add(gdbs);

                        // TODO: Cập nhật trạng thái Bản sao trong bảng tBanSao
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // THÔNG BÁO THÀNH CÔNG VÀ CHUYỂN HƯỚNG VỀ INDEX
                    TempData["StatusMessage"] = "success";
                    TempData["Message"] = $"Đã tạo mới Giao dịch: <strong>{tGiaoDichMuonTra.MaGd}</strong> thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    TempData["StatusMessage"] = "danger";
                    // Lấy lỗi sâu hơn
                    string errorMessage = ex.InnerException?.Message ?? ex.Message;
                    TempData["Message"] = "Lỗi hệ thống khi lưu: <strong>" + errorMessage + "</strong>";

                    // Trả về View để hiển thị lỗi ngay trên trang
                    return View(tGiaoDichMuonTra);
                }
            }
            // Xử lý ModelState không hợp lệ
            TempData["StatusMessage"] = "danger";
            var errors = ModelState.Where(x => x.Value.Errors.Any())
                   .Select(x => $"{x.Key}: {string.Join("; ", x.Value.Errors.Select(e => e.ErrorMessage))}");
            TempData["Message"] = $"Dữ liệu không hợp lệ. Vui lòng kiểm tra: <ul><li><strong>{string.Join("</strong></li><li><strong>", errors)}</strong></li></ul>";

            return View(tGiaoDichMuonTra);
        }


        // --- CÁC ACTIONS HỖ TRỢ CHO AJAX ---

        // Action: Tìm Thẻ Bạn đọc đang hoạt động (ĐÃ SỬA LỖI ĐỊNH DẠNG NGÀY)
        [HttpGet]
        public async Task<IActionResult> SearchActiveTheBanDoc(string searchTerm)
<<<<<<< Updated upstream
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return Json(new { success = false, message = "Vui lòng nhập từ khóa tìm kiếm." });
            }

            var searchLower = searchTerm.Trim().ToLower();

            // 1. Thực hiện truy vấn LINQ: Lấy dữ liệu thô
            var activeCardsQuery = await _context.TTheBanDoc
                .Include(t => t.MaBdNavigation)
                .Where(t => t.TrangThai.ToLower() == "hoạt động" && (
                    t.MaTbd.ToLower().Contains(searchLower) ||
                    (t.MaBdNavigation.HoDem + " " + t.MaBdNavigation.Ten).ToLower().Contains(searchLower)
                ))
                .Select(t => new
                {
                    t.MaTbd,
                    HoTen = t.MaBdNavigation.HoDem + " " + t.MaBdNavigation.Ten,
                    t.NgayHetHan // Lấy DateOnly? thô
                })
                .Take(10)
                .ToListAsync(); // Thực thi truy vấn

            // 2. Định dạng dữ liệu (in-memory) và trả về Json
            var formattedCards = activeCardsQuery.Select(t => new
            {
                t.MaTbd,
                t.HoTen,
                // Ép kiểu an toàn (kiểm tra HasValue)
                NgayHetHan = t.NgayHetHan.HasValue
                             ? t.NgayHetHan.Value.ToDateTime(TimeOnly.MinValue).ToString("dd/MM/yyyy")
                             : ""
            }).ToList();

            return Json(new { success = true, data = formattedCards });
        }

        // Action: Tìm Bản sao tài liệu đang SẴN CÓ
        [HttpGet]
        public async Task<IActionResult> SearchAvailableBanSao(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return Json(new { success = false, message = "Vui lòng nhập từ khóa tìm kiếm." });
            }

            var searchLower = searchTerm.Trim().ToLower();

            var availableCopies = await _context.TBanSao
                .Include(bs => bs.MaTlNavigation)
                .Where(bs => bs.MaBs.ToLower().Contains(searchLower)
                            || (bs.MaTlNavigation != null && bs.MaTlNavigation.TenTl.ToLower().Contains(searchLower)))
                .Where(bs => !_context.TGiaoDichBanSao.Any(gdbs => gdbs.MaBs == bs.MaBs && gdbs.TinhTrang == false))
                .Select(bs => new
                {
                    MaBs = bs.MaBs,
                    TenTaiLieu = (bs.MaTlNavigation != null ? bs.MaTlNavigation.TenTl : "Không rõ tên tài liệu"),
                    TrangThai = "Sẵn có"
                })
                .Take(10)
                .ToListAsync();

            return Json(new { success = true, data = availableCopies });
        }

        // --- HÀM PRIVATE HỖ TRỢ SINH MÃ ---

        private async Task<string> GenerateNewMaGd()
        {
            var pMaGd = new SqlParameter("@NewMaGD", System.Data.SqlDbType.Char, 12)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC SP_GenerateNewMaGD @NewMaGD OUTPUT", pMaGd);

            return pMaGd.Value != DBNull.Value ? pMaGd.Value.ToString().Trim() : throw new Exception("Không thể sinh Mã Giao dịch mới.");
        }


        // --- Edit, Delete (Giữ nguyên) ---

        // GET: GiaoDichMuonTra/Edit/5
        [Route("Chinh-sua/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

=======
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return Json(new { success = false, message = "Vui lòng nhập từ khóa tìm kiếm." });
            }

            var searchLower = searchTerm.Trim().ToLower();

            // 1. Thực hiện truy vấn LINQ: Lấy dữ liệu thô
            var activeCardsQuery = await _context.TTheBanDoc
                .Include(t => t.MaBdNavigation)
                .Where(t => t.TrangThai.ToLower() == "hoạt động" && (
                    t.MaTbd.ToLower().Contains(searchLower) ||
                    (t.MaBdNavigation.HoDem + " " + t.MaBdNavigation.Ten).ToLower().Contains(searchLower)
                ))
                .Select(t => new
                {
                    t.MaTbd,
                    HoTen = t.MaBdNavigation.HoDem + " " + t.MaBdNavigation.Ten,
                    t.NgayHetHan // Lấy DateOnly? thô
                })
                .Take(10)
                .ToListAsync(); // Thực thi truy vấn

            // 2. Định dạng dữ liệu (in-memory) và trả về Json
            var formattedCards = activeCardsQuery.Select(t => new
            {
                t.MaTbd,
                t.HoTen,
                // Ép kiểu an toàn (kiểm tra HasValue)
                NgayHetHan = t.NgayHetHan.HasValue
                             ? t.NgayHetHan.Value.ToDateTime(TimeOnly.MinValue).ToString("dd/MM/yyyy")
                             : ""
            }).ToList();

            return Json(new { success = true, data = formattedCards });
        }

        // Action: Tìm Bản sao tài liệu đang SẴN CÓ
        [HttpGet]
        public async Task<IActionResult> SearchAvailableBanSao(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return Json(new { success = false, message = "Vui lòng nhập từ khóa tìm kiếm." });
            }

            var searchLower = searchTerm.Trim().ToLower();

            var availableCopies = await _context.TBanSao
                .Include(bs => bs.MaTlNavigation)
                .Where(bs => bs.MaBs.ToLower().Contains(searchLower)
                            || (bs.MaTlNavigation != null && bs.MaTlNavigation.TenTl.ToLower().Contains(searchLower)))
                .Where(bs => !_context.TGiaoDichBanSao.Any(gdbs => gdbs.MaBs == bs.MaBs && gdbs.TinhTrang == false))
                .Select(bs => new
                {
                    MaBs = bs.MaBs,
                    TenTaiLieu = (bs.MaTlNavigation != null ? bs.MaTlNavigation.TenTl : "Không rõ tên tài liệu"),
                    TrangThai = "Sẵn có"
                })
                .Take(10)
                .ToListAsync();

            return Json(new { success = true, data = availableCopies });
        }

        // --- HÀM PRIVATE HỖ TRỢ SINH MÃ ---

        private async Task<string> GenerateNewMaGd()
        {
            var pMaGd = new SqlParameter("@NewMaGD", System.Data.SqlDbType.Char, 12)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC SP_GenerateNewMaGD @NewMaGD OUTPUT", pMaGd);

            return pMaGd.Value != DBNull.Value ? pMaGd.Value.ToString().Trim() : throw new Exception("Không thể sinh Mã Giao dịch mới.");
        }


        // --- Edit, Delete (Giữ nguyên) ---

        // GET: GiaoDichMuonTra/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

>>>>>>> Stashed changes
            var tGiaoDichMuonTra = await _context.TGiaoDichMuonTra.FindAsync(id);
            if (tGiaoDichMuonTra == null) return NotFound();

            ViewData["MaTbd"] = new SelectList(_context.TTheBanDoc, "MaTbd", "MaTbd", tGiaoDichMuonTra.MaTbd);
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoan, "MaTk", "MaTk", tGiaoDichMuonTra.MaTk);
            return View(tGiaoDichMuonTra);
        }

        // POST: GiaoDichMuonTra/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Chinh-sua/{id}")]
        public async Task<IActionResult> Edit(string id, [Bind("MaGd,MaTbd,MaTk,NgayMuon,NgayHenTra,NgayTra,TrangThai")] TGiaoDichMuonTra tGiaoDichMuonTra)
        {
            if (id != tGiaoDichMuonTra.MaGd) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tGiaoDichMuonTra);
                    await _context.SaveChangesAsync();

                    TempData["StatusMessage"] = "success";
                    TempData["Message"] = $"Thông tin Giao dịch <strong>{tGiaoDichMuonTra.MaGd}</strong> đã được cập nhật thành công.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TGiaoDichMuonTraExists(tGiaoDichMuonTra.MaGd))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["StatusMessage"] = "danger";
                        TempData["Message"] = "Lỗi xung đột dữ liệu. Vui lòng thử lại.";
                        return View(tGiaoDichMuonTra);
                    }
                }
                catch (Exception ex)
                {
                    TempData["StatusMessage"] = "danger";
                    TempData["Message"] = "Lỗi hệ thống khi lưu: <strong>" + ex.Message + "</strong>";
                    return View(tGiaoDichMuonTra);
                }
                return RedirectToAction(nameof(Index));
            }

            TempData["StatusMessage"] = "danger";
            var errors = ModelState.Where(x => x.Value.Errors.Any())
                   .Select(x => $"{x.Key}: {string.Join("; ", x.Value.Errors.Select(e => e.ErrorMessage))}");
            TempData["Message"] = $"Dữ liệu không hợp lệ. Vui lòng kiểm tra: <ul><li><strong>{string.Join("</strong></li><li><strong>", errors)}</strong></li></ul>";

            return View(tGiaoDichMuonTra);
        }

        // GET: GiaoDichMuonTra/Delete/5
        [Route("Xoa/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var tGiaoDichMuonTra = await _context.TGiaoDichMuonTra
                .Include(t => t.MaTbdNavigation)
                .Include(t => t.MaTkNavigation)
                .FirstOrDefaultAsync(m => m.MaGd == id);
            if (tGiaoDichMuonTra == null) return NotFound();

            return View(tGiaoDichMuonTra);
        }

        // POST: GiaoDichMuonTra/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Xoa/{id}")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tGiaoDichMuonTra = await _context.TGiaoDichMuonTra.FindAsync(id);

            if (tGiaoDichMuonTra != null)
            {
                try
                {
                    _context.TGiaoDichMuonTra.Remove(tGiaoDichMuonTra);
                    await _context.SaveChangesAsync();

                    TempData["StatusMessage"] = "success";
                    TempData["Message"] = $"Đã xóa Giao dịch có Mã: <strong>{id}</strong> thành công.";
                }
                catch (DbUpdateException ex)
                {
                    TempData["StatusMessage"] = "danger";
                    // Thông báo lỗi nếu có ràng buộc ngoại
                    TempData["Message"] = $"Không thể xóa giao dịch <strong>{id}</strong> vì có thể đang có các bản sao liên quan. Vui lòng kiểm tra các bản sao trong giao dịch.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["StatusMessage"] = "danger";
                    TempData["Message"] = $"Lỗi hệ thống khi xóa: <strong>{ex.Message}</strong>";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TGiaoDichMuonTraExists(string id)
        {
            return _context.TGiaoDichMuonTra.Any(e => e.MaGd == id);
        }
    }
}