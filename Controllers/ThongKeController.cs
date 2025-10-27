using Microsoft.AspNetCore.Mvc;
using Library_Manager.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Library_Manager.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly QlthuVienContext _context;

        public ThongKeController(QlthuVienContext context)
        {
            _context = context;
        }
        //Hiển thị trang biểu đồ
        public IActionResult LuotMuonTheoThang()
        {
            // Mặc định lấy năm hiện tại để hiển thị ban đầu
            int nam = DateTime.Now.Year;
            ViewBag.Nam = nam;
            return View();
        }

        //API: Lấy dữ liệu thống kê theo năm
       [HttpGet]
        public IActionResult GetLuotMuonTheoThang(int year)
        {
            var thongKe = _context.TGiaoDichMuonTras
                .Where(g => g.NgayMuon.Year == year)
                .GroupBy(g => g.NgayMuon.Month)
                .Select(g => new
                {
                    Thang = g.Key,
                    SoLuotMuon = g.Count()
                })
                .OrderBy(x => x.Thang)
                .ToList();

            return Json(thongKe);
        }

        
    }
}

