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
            // Nếu đã đăng nhập (có Session "UserName") thì chuyển về trang chủ
            if (HttpContext.Session.GetString("UserName") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // ==========================================================
        // KHỐI 2: ACTION LOGIN (XỬ LÝ ĐĂNG NHẬP)
        // ==========================================================
        [HttpPost]
        public IActionResult Login(TTaiKhoan user) // 'user' này chứa TenDangNhap và MatKhau từ Form
        {
            // --- BƯỚC 1: TRUY VẤN TÀI KHOẢN ---
            // SỬA ĐỔI: Dùng .Include() để tải thông tin Nhân Viên (MaNvNavigation)
            // và thông tin Vai Trò (MaVtNavigation) ngay lập tức.
            var taiKhoan = _context.TTaiKhoans
                .Include(tk => tk.MaNvNavigation)  // JOIN tới bảng TNhanVien
                .Include(tk => tk.MaVtNavigation)  // JOIN tới bảng TVaiTro
                .FirstOrDefault(x => x.TenDangNhap == user.TenDangNhap);

            // Kiểm tra tài khoản không tồn tại
            if (taiKhoan == null)
            {
                ViewBag.Error = "Tên đăng nhập không tồn tại.";
                return View();
            }

            // --- BƯỚC 2: KIỂM TRA VÀ HASH MẬT KHẨU (Giữ nguyên logic của bạn) ---
            // ✅ Kiểm tra xem mật khẩu trong DB có phải là Base64 không
            if (!PasswordHelper.IsBase64String(taiKhoan.MatKhau))
            {
                // Nếu không phải Base64 → nghĩa là chưa mã hóa → mã hóa lại ngay
                taiKhoan.MatKhau = PasswordHelper.HashPassword(taiKhoan.TenDangNhap, taiKhoan.MatKhau);
                _context.Update(taiKhoan);
                _context.SaveChanges();
            }

            // So sánh mật khẩu đã nhập (từ form 'user') với mật khẩu hash (từ CSDL 'taiKhoan')
            bool isValid = PasswordHelper.VerifyPassword(taiKhoan.TenDangNhap, user.MatKhau, taiKhoan.MatKhau);

            // --- BƯỚC 3: XỬ LÝ NẾU ĐĂNG NHẬP THÀNH CÔNG ---
            if (isValid)
            {
                // SỬA ĐỔI: Lấy thông tin từ các navigation properties đã được Include()

                // 1. Lấy Họ Tên từ bảng Nhân Viên (TNhanVien)
                string hoTen = "Không rõ";
                if (taiKhoan.MaNvNavigation != null) // Kiểm tra nếu có nhân viên liên kết
                {
                    hoTen = taiKhoan.MaNvNavigation.HoDem + " " + taiKhoan.MaNvNavigation.Ten;
                }

                // 2. Lấy Tên Vai Trò từ bảng Vai Trò (TVaiTro)
                string tenVaiTro = taiKhoan.MaVt; // Mặc định là Mã VT
                if (taiKhoan.MaVtNavigation != null) // Kiểm tra nếu có vai trò liên kết
                {
                    tenVaiTro = taiKhoan.MaVtNavigation.TenVt;
                }

                // 3. LƯU THÔNG TIN VÀO SESSION
                // (Dùng đúng key "hoTen" và "tenVaiTro" (viết thường) như file Navbar đang đọc)
                HttpContext.Session.SetString("hoTen", hoTen);
                HttpContext.Session.SetString("tenVaiTro", tenVaiTro);

                // Lưu các thông tin khác để kiểm tra quyền
                HttpContext.Session.SetString("UserName", taiKhoan.TenDangNhap.ToString());
                HttpContext.Session.SetString("UserRole", taiKhoan.MaVt.ToString()); // Mã vai trò (QTV, QLB...)
                HttpContext.Session.SetString("MaNv", taiKhoan.MaNv);

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

            // (Hoặc xóa từng key để cẩn thận hơn)
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("UserRole");
            HttpContext.Session.Remove("hoTen");
            HttpContext.Session.Remove("tenVaiTro");
            HttpContext.Session.Remove("MaNv");

            // Trở về trang đăng nhập
            return RedirectToAction("Login", "Account");
        }
    }
}
