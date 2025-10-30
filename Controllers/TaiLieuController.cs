using Library_Manager.Filters;
using Library_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_Manager.Controllers
{
    [Authorization("QTV,QLB,QLT,QLM")]
    public class TaiLieuController : Controller
    {
        private readonly QlthuVienContext _context;

        public TaiLieuController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: TaiLieu
        public IActionResult Index(int? page, string searchString)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            // 1. Giữ ở dạng IQueryable để có thể lọc và phân trang hiệu quả
            IQueryable<TTaiLieu> taiLieus = _context.TTaiLieus
                .Include(t => t.MaDdNavigation)
                .Include(t => t.MaNnNavigation)
                .Include(t => t.MaNxbNavigation)
                .Include(t => t.MaThLNavigation)
                .Include(t => t.MaTkNavigation);

            // 2. Tìm kiếm theo tên tài liệu hoặc các trường liên quan
            if (!string.IsNullOrEmpty(searchString))
            {
                taiLieus = taiLieus.Where(tl =>
                    tl.TenTl.ToLower().Contains(searchString.ToLower()) ||
                    tl.MaTl.ToLower().Contains(searchString.ToLower()) ||
                    (tl.MaNxbNavigation != null && tl.MaNxbNavigation.TenNxb.ToLower().Contains(searchString.ToLower())) ||
                    (tl.MaThLNavigation != null && tl.MaThLNavigation.TenThL.ToLower().Contains(searchString.ToLower())));
            }

            // 3. Sắp xếp theo mã tài liệu
            taiLieus = taiLieus.OrderBy(tl => tl.MaTl);

            // 4. Tạo danh sách phân trang
            var pagedTaiLieus = new PagedList.Core.PagedList<TTaiLieu>(taiLieus, pageNumber, pageSize);

            // 5. Lưu lại chuỗi tìm kiếm để hiển thị lại trên View
            ViewBag.CurrentFilter = searchString;

            // 6. Trả về View
            return View(pagedTaiLieus);
        }

        // GET: TaiLieu/Details/5
        [Authorization("QLT")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTaiLieu = await _context.TTaiLieus
                .Include(t => t.MaDdNavigation)
                .Include(t => t.MaNnNavigation)
                .Include(t => t.MaNxbNavigation)
                .Include(t => t.MaThLNavigation)
                .Include(t => t.MaTkNavigation)
                .FirstOrDefaultAsync(m => m.MaTl == id);
            if (tTaiLieu == null)
            {
                return NotFound();
            }

            return View(tTaiLieu);
        }

        // GET: TaiLieu/Create
        [Authorization("QLT")]
        public IActionResult Create()
        {
            // ĐIỀU CHỈNH: Hiện thị Tên thay vì Mã
            ViewData["MaDd"] = new SelectList(_context.TDinhDangs, "MaDd", "TenDd");
            ViewData["MaNn"] = new SelectList(_context.TNgonNgus, "MaNn", "TenNn");
            ViewData["MaNxb"] = new SelectList(_context.TNhaXuatBans, "MaNxb", "TenNxb");
            ViewData["MaThL"] = new SelectList(_context.TTheLoais, "MaThL", "TenThL");
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoans, "MaTk", "MaTk"); // Giữ MaTk nếu không có tên hiển thị

            return View();
        }

        // POST: TaiLieu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("QLT")]
        public async Task<IActionResult> Create([Bind("MaTl,MaNxb,MaNn,MaThL,MaDd,TenTl,LanXuatBan,NamXuatBan,SoTrang,KhoCo,MaTk")] TTaiLieu tTaiLieu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tTaiLieu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // ĐIỀU CHỈNH: Hiện thị Tên thay vì Mã khi Validation thất bại
            ViewData["MaDd"] = new SelectList(_context.TDinhDangs, "MaDd", "TenDd", tTaiLieu.MaDd);
            ViewData["MaNn"] = new SelectList(_context.TNgonNgus, "MaNn", "TenNn", tTaiLieu.MaNn);
            ViewData["MaNxb"] = new SelectList(_context.TNhaXuatBans, "MaNxb", "TenNxb", tTaiLieu.MaNxb);
            ViewData["MaThL"] = new SelectList(_context.TTheLoais, "MaThL", "TenThL", tTaiLieu.MaThL);
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoans, "MaTk", "MaTk", tTaiLieu.MaTk); // Giữ MaTk nếu không có tên hiển thị
            return View(tTaiLieu);
        }

        // GET: TaiLieu/Edit/5
        [Authorization("QLT")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTaiLieu = await _context.TTaiLieus.FindAsync(id);
            if (tTaiLieu == null)
            {
                return NotFound();
            }
            // ĐIỀU CHỈNH: Hiện thị Tên thay vì Mã
            ViewData["MaDd"] = new SelectList(_context.TDinhDangs, "MaDd", "TenDd", tTaiLieu.MaDd);
            ViewData["MaNn"] = new SelectList(_context.TNgonNgus, "MaNn", "TenNn", tTaiLieu.MaNn);
            ViewData["MaNxb"] = new SelectList(_context.TNhaXuatBans, "MaNxb", "TenNxb", tTaiLieu.MaNxb);
            ViewData["MaThL"] = new SelectList(_context.TTheLoais, "MaThL", "TenThL", tTaiLieu.MaThL);
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoans, "MaTk", "MaTk", tTaiLieu.MaTk); // Giữ MaTk nếu không có tên hiển thị
            return View(tTaiLieu);
        }

        // POST: TaiLieu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("QLT")]
        public async Task<IActionResult> Edit(string id, [Bind("MaTl,MaNxb,MaNn,MaThL,MaDd,TenTl,LanXuatBan,NamXuatBan,SoTrang,KhoCo,MaTk")] TTaiLieu tTaiLieu)
        {
            if (id != tTaiLieu.MaTl)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tTaiLieu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TTaiLieuExists(tTaiLieu.MaTl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // ĐIỀU CHỈNH: Hiện thị Tên thay vì Mã khi Validation thất bại
            ViewData["MaDd"] = new SelectList(_context.TDinhDangs, "MaDd", "TenDd", tTaiLieu.MaDd);
            ViewData["MaNn"] = new SelectList(_context.TNgonNgus, "MaNn", "TenNn", tTaiLieu.MaNn);
            ViewData["MaNxb"] = new SelectList(_context.TNhaXuatBans, "MaNxb", "TenNxb", tTaiLieu.MaNxb);
            ViewData["MaThL"] = new SelectList(_context.TTheLoais, "MaThL", "TenThL", tTaiLieu.MaThL);
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoans, "MaTk", "MaTk", tTaiLieu.MaTk); // Giữ MaTk nếu không có tên hiển thị
            return View(tTaiLieu);
        }

        // GET: TaiLieu/Delete/5
        [Authorization("QLT")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTaiLieu = await _context.TTaiLieus
                .Include(t => t.MaDdNavigation)
                .Include(t => t.MaNnNavigation)
                .Include(t => t.MaNxbNavigation)
                .Include(t => t.MaThLNavigation)
                .Include(t => t.MaTkNavigation)
                .FirstOrDefaultAsync(m => m.MaTl == id);
            if (tTaiLieu == null)
            {
                return NotFound();
            }

            return View(tTaiLieu);
        }

        // POST: TaiLieu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorization("QLT")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tTaiLieu = await _context.TTaiLieus.FindAsync(id);
            if (tTaiLieu != null)
            {
                _context.TTaiLieus.Remove(tTaiLieu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TTaiLieuExists(string id)
        {
            return _context.TTaiLieus.Any(e => e.MaTl == id);
        }
    }
}
