using Library_Manager.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library_Manager.Controllers
{
    public class AccountController : Controller
    {
        QlthuVienContext db = new QlthuVienContext();

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
            if (HttpContext.Session.GetString("UserName") == null)
            {
                var u = db.TTaiKhoans.Where(x => x.TenDangNhap.Equals(user.TenDangNhap) && x.MatKhau.Equals(user.MatKhau)).FirstOrDefault();
                if (u != null)
                {
                    HttpContext.Session.SetString("UserName", u.TenDangNhap.ToString());
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Login", "Account");
        }
    }
}
