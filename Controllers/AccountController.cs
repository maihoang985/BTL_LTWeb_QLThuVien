using Library_Manager.Helpers;
using Library_Manager.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library_Manager.Controllers
{
    public class AccountController : Controller
    {
        //QlthuVienContext db = new QlthuVienContext();

        private readonly QlthuVienContext _context;

        // Inject DbContext qua constructor
        public AccountController(QlthuVienContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

      
        [HttpPost]
        public IActionResult Login(TTaiKhoan user)
        {
            //if (HttpContext.Session.GetString("UserName") == null)
            //{
                var u = _context.TTaiKhoans.Where(x => x.TenDangNhap.Equals(user.TenDangNhap) && x.MatKhau.Equals(user.MatKhau)).FirstOrDefault();
                if (u == null)
                {
                    ViewBag.Error = "Tên đăng nhập không tồn tại.";
                    return View();
                }

                // ✅ Kiểm tra xem mật khẩu trong DB có phải là Base64 không
                if (!PasswordHelper.IsBase64String(u.MatKhau))
                {
                    // Nếu không phải Base64 → nghĩa là chưa mã hóa → mã hóa lại ngay
                    u.MatKhau = PasswordHelper.HashPassword(u.TenDangNhap, u.MatKhau);
                    _context.Update(u);
                    _context.SaveChanges();
                }
            if (u != null)
                {
                // So sánh mật khẩu đã nhập với mật khẩu hash trong DB
                bool isValid = PasswordHelper.VerifyPassword(u.TenDangNhap, user.MatKhau, u.MatKhau);
                if (isValid) {
                    HttpContext.Session.SetString("UserName", u.TenDangNhap.ToString());
                    HttpContext.Session.SetString("UserRole", u.MaVt.ToString());
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Sai mật khẩu!");
            }
            else
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại!");
            }
            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Login", "Account");
        }
    }
}
