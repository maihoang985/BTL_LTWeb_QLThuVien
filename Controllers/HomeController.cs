using Library_Manager.Filters;
using Library_Manager.Models;
using Library_Manager   .Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Library_Manager.Controllers
{
    public class HomeController : Controller
    {
        private readonly QlthuVienContext _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, QlthuVienContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorization("QTV,QLB,QLT,QLM")]
        public IActionResult Index()
        {
            // 1. Số liệu tổng quan (Key Metrics)
            var totalReaders = _context.TBanDocs.Count();
            var totalCopies = _context.TBanSaos.Count();
            var totalLoans = _context.TGiaoDichMuonTras.Count(); // Tổng số giao dịch

            // 2. Sách quá hạn
            var overdueLoans = _context.TGiaoDichMuonTras
                .Where(g => g.NgayHenTra.Date < DateTime.Now.Date && g.NgayTra == null)
                .Count();

            // 3. Sách có sẵn (có thể tính toán phức tạp hơn, nhưng đây là cách đơn giản)
            var availableCopies = _context.TBanSaos
                .Where(bs => bs.TinhTrang == "Sẵn sàng") // Cần định nghĩa TinhTrang này trong DB
                .Count();

            // 4. Các biểu đồ: Tính toán giao dịch trong 7 ngày qua
            var recentLoans = _context.TGiaoDichMuonTras
                .Where(g => g.NgayMuon >= DateTime.Now.AddDays(-7))
                .GroupBy(g => g.NgayMuon.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToList();

            // Truyền dữ liệu qua ViewModel hoặc ViewBag
            ViewBag.TotalReaders = totalReaders;
            ViewBag.TotalCopies = totalCopies;
            ViewBag.TotalLoans = totalLoans;
            ViewBag.OverdueLoans = overdueLoans;
            ViewBag.AvailableCopies = availableCopies;
            ViewBag.RecentLoansData = recentLoans; // Dữ liệu cho biểu đồ

            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
