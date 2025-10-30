using Library_Manager.Helpers;
using Library_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // BẮT BUỘC: Thêm using này để có .Include()
using System.Linq;

namespace Library_Manager.Controllers
{
    public class AccountController : Controller
    {
        private readonly QlthuVienContext _context;

        // Inject DbContext qua constructor
        public AccountController(QlthuVienContext context)
        {
            _context = context;
        }

        // ==========================================================
        // KHỐI 1: ACTION LOGIN (HIỂN THỊ FORM)
        // ==========================================================
        [HttpGet]
        public IActionResult Login()
        {
            // SỬA ĐỔI: Kiểm tra Session "MaTk" (hoặc "UserName")
            if (HttpContext.Session.GetString("MaTk") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // ==========================================================
        // KHỐI 2: ACTION LOGIN (XỬ LÝ ĐĂNG NHẬP)
        // ==========================================================
        [HttpPost]
        public IActionResult Login(TTaiKhoan user)
        {
            // --- BƯỚC 1: TRUY VẤN TÀI KHOẢN ---
            var taiKhoan = _context.TTaiKhoans
                .Include(tk => tk.MaNvNavigation)  // JOIN tới TNhanVien
                .Include(tk => tk.MaVtNavigation)  // JOIN tới TVaiTro
                .FirstOrDefault(x => x.TenDangNhap == user.TenDangNhap);

            if (taiKhoan == null)
            {
                ViewBag.Error = "Tên đăng nhập không tồn tại.";
                return View();
            }

            // --- BƯỚC 2: KIỂM TRA VÀ HASH MẬT KHẨU (Giữ nguyên logic của bạn) ---
            if (!PasswordHelper.IsBase64String(taiKhoan.MatKhau))
            {
                taiKhoan.MatKhau = PasswordHelper.HashPassword(taiKhoan.TenDangNhap, taiKhoan.MatKhau);
                _context.Update(taiKhoan);
                _context.SaveChanges();
            }

            bool isValid = PasswordHelper.VerifyPassword(taiKhoan.TenDangNhap, user.MatKhau, taiKhoan.MatKhau);

            // --- BƯỚC 3: XỬ LÝ NẾU ĐĂNG NHẬP THÀNH CÔNG ---
            if (isValid)
            {
                // 1. Lấy Họ Tên
                string hoTen = taiKhoan.MaNvNavigation != null
                             ? taiKhoan.MaNvNavigation.HoDem + " " + taiKhoan.MaNvNavigation.Ten
                             : "Không rõ";

                // 2. Lấy Tên Vai Trò
                string tenVaiTro = taiKhoan.MaVtNavigation != null
                                 ? taiKhoan.MaVtNavigation.TenVt
                                 : taiKhoan.MaVt;

                // 3. LƯU TẤT CẢ THÔNG TIN CẦN THIẾT VÀO SESSION
                HttpContext.Session.SetString("UserName", taiKhoan.TenDangNhap.ToString());
                HttpContext.Session.SetString("UserRole", taiKhoan.MaVt.ToString()); // Dùng cho [Authorization]

                // === BỔ SUNG KEY MA TK VÀ THÔNG TIN CÁ NHÂN ===
                HttpContext.Session.SetString("MaTk", taiKhoan.MaTk.ToString());     // KEY QUAN TRỌNG CHO HOMECONTROLLER
                HttpContext.Session.SetString("MaNv", taiKhoan.MaNv ?? "");          // Mã Nhân viên (nếu có)
                HttpContext.Session.SetString("hoTen", hoTen);                       // Họ tên đầy đủ (cho Navbar)
                HttpContext.Session.SetString("tenVaiTro", tenVaiTro);               // Tên vai trò (cho Navbar)
                // ==============================================

                // Lưu tài khoản vào TempData để sử dụng trong HomeController (nếu cần)
                HttpContext.Session.SetString("MaTk", taiKhoan.MaTk);

                // Chuyển về trang chủ
                return RedirectToAction("Index", "Home");
            }

            // --- BƯỚC 4: XỬ LÝ NẾU SAI MẬT KHẨU ---
            ModelState.AddModelError("", "Sai mật khẩu!");
            return View(user);
        }

        // ==========================================================
        // KHỐI 3: ACTION LOGOUT
        // ==========================================================
        public IActionResult Logout()
        {
            // Xóa tất cả Session
            HttpContext.Session.Clear();

            // Trở về trang đăng nhập
            return RedirectToAction("Login", "Account");
        }
    }
}