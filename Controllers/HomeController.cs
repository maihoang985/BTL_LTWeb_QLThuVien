using Library_Manager.Filters;
using Library_Manager.Models;
using Library_Manager.Models.Authentication;
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
            string? currentMaTk = HttpContext.Session.GetString("MaTk");

            // =======================================================
            // KHỐI A: THÔNG TIN NHÂN VIÊN/TÀI KHOẢN CÁ NHÂN
            // =======================================================
            var account = _context.TTaiKhoans
                .Include(tk => tk.MaNvNavigation)
                .Include(tk => tk.MaVtNavigation)
                .FirstOrDefault(tk => tk.MaTk == currentMaTk);

            ViewBag.CurrentAccount = account;
            ViewBag.CurrentEmployee = account?.MaNvNavigation;


            // =======================================================
            // KHỐI B: SỐ LIỆU TỔNG QUAN CHO CÁC KHỐI CHỨC NĂNG
            // =======================================================

            // 1. Quản lý Mượn/Trả
            ViewBag.TotalLoans = _context.TGiaoDichMuonTras.Count(g => g.NgayTra == null);
            ViewBag.OverdueLoans = _context.TGiaoDichMuonTras
                .Count(g => g.NgayHenTra.Date < DateTime.Now.Date && g.NgayTra == null);
            ViewBag.LoansToday = _context.TGiaoDichMuonTras
                 .Count(g => g.NgayMuon.Date == DateTime.Now.Date && g.NgayTra == null);

            // 2. Quản lý Tài liệu/Kho
            ViewBag.TotalCopies = _context.TBanSaos.Count();
            ViewBag.AvailableCopies = _context.TBanSaos.Count(bs => bs.TinhTrang == "Sẵn sàng");

            // 3. Quản lý Bạn đọc
            ViewBag.TotalReaders = _context.TBanDocs.Count();
            ViewBag.LockedReaders = _context.TTheBanDocs.Count(t => t.TrangThai == "Bị khóa");


            // =======================================================
            // KHỐI C: DANH SÁCH CHI TIẾT (LISTS)
            // =======================================================

            // 1. Danh sách Quá hạn lâu nhất (Top 5)
            ViewBag.OverdueList = _context.TGiaoDichMuonTras
                .Where(g => g.NgayHenTra.Date < DateTime.Now.Date && g.NgayTra == null)
                .OrderBy(g => g.NgayHenTra).Take(5).Select(g => new {
                    TenSach = g.TGiaoDichBanSaos.FirstOrDefault() != null ?
                              g.TGiaoDichBanSaos.First().MaBsNavigation.MaTlNavigation.TenTl : "N/A",
                    DocGia = g.MaTbdNavigation.MaBdNavigation.HoDem + " " + g.MaTbdNavigation.MaBdNavigation.Ten,
                    NgayHenTra = g.NgayHenTra.ToString("dd/MM/yyyy")
                }).ToList<dynamic>();

            // 2. Danh sách Tồn kho thấp (Ngưỡng <= 2 bản, Top 5)
            ViewBag.LowStockList = _context.TTaiLieus
                .Select(tl => new {
                    MaTl = tl.MaTl,
                    TenTl = tl.TenTl,
                    SoBanSanSang = tl.TBanSaos.Count(bs => bs.TinhTrang == "Sẵn sàng")
                }).Where(x => x.SoBanSanSang <= 2).OrderBy(x => x.SoBanSanSang).Take(5).ToList<dynamic>();

            // 3. Danh sách Giao dịch gần nhất (Top 5)
            ViewBag.RecentTransactions = _context.TGiaoDichMuonTras
                .OrderByDescending(g => g.NgayMuon).Take(5).Select(g => new {
                    MaGd = g.MaGd,
                    Loai = g.NgayTra == null ? "Mượn" : "Trả",
                    DocGia = g.MaTbdNavigation.MaBdNavigation.Ten,
                    ThoiGian = g.NgayTra != null ? g.NgayTra.Value.ToString("HH:mm") : g.NgayMuon.ToString("HH:mm")
                }).ToList<dynamic>();

            // 4. Danh sách Bạn đọc mới (Top 5)
            ViewBag.NewReadersList = _context.TBanDocs
                .OrderByDescending(bd => bd.MaBd)
                .Take(5)
                .Select(bd => new {
                    MaBd = bd.MaBd,
                    Ten = bd.HoDem + " " + bd.Ten,
                    NgayTao = bd.TTheBanDocs.FirstOrDefault() != null ?
                              (DateTime?)bd.TTheBanDocs.First().NgayCap.ToDateTime(TimeOnly.MinValue) : (DateTime?)null
                }).ToList<dynamic>();


            // 5. Thêm số lượng đầu sách tồn kho thấp (đếm số đầu sách)
            ViewBag.LowStockCount = ((List<dynamic>)ViewBag.LowStockList).Count;


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