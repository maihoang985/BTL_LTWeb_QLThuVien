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

        // Hiển thị trang thống kê (View chính)
        public IActionResult Index()
        {
            int nam = DateTime.Now.Year;
            ViewBag.Nam = nam;
            return View();
        }

        // API: Lấy dữ liệu thống kê lượt mượn theo năm
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

        // API MỚI: Lấy thống kê tài liệu và giáo trình
        [HttpGet]
        public IActionResult GetThongKeTaiLieu()
        {
            try
            {
                // Lấy danh sách MaBS đang được mượn (chưa trả)
                var banSaoDangMuon = _context.TGiaoDichMuonTras
                    .Where(g => g.NgayTra == null) // Giao dịch chưa trả
                    .Join(_context.TGiaoDichBanSaos,
                        gd => gd.MaGd,
                        gdbs => gdbs.MaGd,
                        (gd, gdbs) => gdbs.MaBs)
                    .Distinct()
                    .ToList();

                // Đếm tổng số giáo trình (MaThL = "GT")
                var tongGiaoTrinh = _context.TBanSaos
                    .Join(_context.TTaiLieus,
                        bs => bs.MaTl,
                        tl => tl.MaTl,
                        (bs, tl) => tl)
                    .Count(tl => tl.MaThL == "GT");

                // Đếm tổng số tài liệu khác (không phải GT)
                var tongTaiLieu = _context.TBanSaos
                    .Join(_context.TTaiLieus,
                        bs => bs.MaTl,
                        tl => tl.MaTl,
                        (bs, tl) => tl)
                    .Count(tl => tl.MaThL != "GT" && tl.MaThL != null);

                // Đếm giáo trình có sẵn (GT và không đang mượn)
                var giaoTrinhCoSan = _context.TBanSaos
                    .Join(_context.TTaiLieus,
                        bs => bs.MaTl,
                        tl => tl.MaTl,
                        (bs, tl) => new { bs.MaBs, tl.MaThL })
                    .Count(x => x.MaThL == "GT" && !banSaoDangMuon.Contains(x.MaBs));

                // Đếm tài liệu có sẵn (không phải GT và không đang mượn)
                var taiLieuCoSan = _context.TBanSaos
                    .Join(_context.TTaiLieus,
                        bs => bs.MaTl,
                        tl => tl.MaTl,
                        (bs, tl) => new { bs.MaBs, tl.MaThL })
                    .Count(x => x.MaThL != "GT" && x.MaThL != null && !banSaoDangMuon.Contains(x.MaBs));

                // Thống kê chi tiết theo thể loại - GỘP theo MaThL
                var chiTietTheoDanhMuc = _context.TBanSaos
                    .Join(_context.TTaiLieus,
                        bs => bs.MaTl,
                        tl => tl.MaTl,
                        (bs, tl) => new { bs.MaBs, tl.MaThL })
                    .Join(_context.TTheLoais,
                        x => x.MaThL,
                        thl => thl.MaThL,
                        (x, thl) => new { x.MaBs, x.MaThL, thl.TenThL })
                    .GroupBy(x => new { x.MaThL, x.TenThL })
                    .Select(g => new
                    {
                        TenDanhMuc = g.Key.TenThL,
                        TongSoLuong = g.Count(),
                        SoLuongCoSan = g.Count(x => !banSaoDangMuon.Contains(x.MaBs))
                    })
                    .Where(x => x.TongSoLuong > 0)
                    .OrderByDescending(x => x.TongSoLuong)
                    .ToList();

                var result = new
                {
                    tongGiaoTrinh = tongGiaoTrinh,
                    tongTaiLieu = tongTaiLieu,
                    giaoTrinhCoSan = giaoTrinhCoSan,
                    taiLieuCoSan = taiLieuCoSan,
                    chiTietTheoDanhMuc = chiTietTheoDanhMuc
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new 
                { 
                    error = ex.Message,
                    tongGiaoTrinh = 0,
                    tongTaiLieu = 0,
                    giaoTrinhCoSan = 0,
                    taiLieuCoSan = 0,
                    chiTietTheoDanhMuc = new object[] { }
                });
            }
        }
    }
}