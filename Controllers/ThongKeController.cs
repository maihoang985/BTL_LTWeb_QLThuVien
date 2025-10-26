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
        public IActionResult TheoThang()
        {
            return View();
        }

        // Action trả dữ liệu JSON cho chart
        [HttpGet]
        public async Task<JsonResult> GetMonthlyBorrowingData(int year)
        {
            // Lấy dữ liệu từ database, nhóm theo tháng
            var monthlyData = await _context.TGiaoDichMuonTras
                .Where(g => g.NgayMuon.Year == year)
                .GroupBy(g => g.NgayMuon.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    Count = group.Count()
                })
                .ToListAsync();

            // Đảm bảo đủ 12 tháng (những tháng không có dữ liệu => Count = 0)
            var chartData = Enumerable.Range(1, 12)
                .Select(month => new
                {
                    Month = month,
                    Count = monthlyData.FirstOrDefault(d => d.Month == month)?.Count ?? 0
                })
                .ToList();

            return Json(chartData);
        }
    }
}
