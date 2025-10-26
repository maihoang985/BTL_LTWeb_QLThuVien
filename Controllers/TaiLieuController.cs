using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library_Manager.Models;

namespace Library_Manager.Controllers
{
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
        public IActionResult Create()
        {
            ViewData["MaDd"] = new SelectList(_context.TDinhDangs, "MaDd", "MaDd");
            ViewData["MaNn"] = new SelectList(_context.TNgonNgus, "MaNn", "MaNn");
            ViewData["MaNxb"] = new SelectList(_context.TNhaXuatBans, "MaNxb", "MaNxb");
            ViewData["MaThL"] = new SelectList(_context.TTheLoais, "MaThL", "MaThL");
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoans, "MaTk", "MaTk");
            return View();
        }

        // POST: TaiLieu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTl,MaNxb,MaNn,MaThL,MaDd,TenTl,LanXuatBan,NamXuatBan,SoTrang,KhoCo,MaTk")] TTaiLieu tTaiLieu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tTaiLieu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDd"] = new SelectList(_context.TDinhDangs, "MaDd", "MaDd", tTaiLieu.MaDd);
            ViewData["MaNn"] = new SelectList(_context.TNgonNgus, "MaNn", "MaNn", tTaiLieu.MaNn);
            ViewData["MaNxb"] = new SelectList(_context.TNhaXuatBans, "MaNxb", "MaNxb", tTaiLieu.MaNxb);
            ViewData["MaThL"] = new SelectList(_context.TTheLoais, "MaThL", "MaThL", tTaiLieu.MaThL);
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoans, "MaTk", "MaTk", tTaiLieu.MaTk);
            return View(tTaiLieu);
        }

        // GET: TaiLieu/Edit/5
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
            ViewData["MaDd"] = new SelectList(_context.TDinhDangs, "MaDd", "MaDd", tTaiLieu.MaDd);
            ViewData["MaNn"] = new SelectList(_context.TNgonNgus, "MaNn", "MaNn", tTaiLieu.MaNn);
            ViewData["MaNxb"] = new SelectList(_context.TNhaXuatBans, "MaNxb", "MaNxb", tTaiLieu.MaNxb);
            ViewData["MaThL"] = new SelectList(_context.TTheLoais, "MaThL", "MaThL", tTaiLieu.MaThL);
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoans, "MaTk", "MaTk", tTaiLieu.MaTk);
            return View(tTaiLieu);
        }

        // POST: TaiLieu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            ViewData["MaDd"] = new SelectList(_context.TDinhDangs, "MaDd", "MaDd", tTaiLieu.MaDd);
            ViewData["MaNn"] = new SelectList(_context.TNgonNgus, "MaNn", "MaNn", tTaiLieu.MaNn);
            ViewData["MaNxb"] = new SelectList(_context.TNhaXuatBans, "MaNxb", "MaNxb", tTaiLieu.MaNxb);
            ViewData["MaThL"] = new SelectList(_context.TTheLoais, "MaThL", "MaThL", tTaiLieu.MaThL);
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoans, "MaTk", "MaTk", tTaiLieu.MaTk);
            return View(tTaiLieu);
        }

        // GET: TaiLieu/Delete/5
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
