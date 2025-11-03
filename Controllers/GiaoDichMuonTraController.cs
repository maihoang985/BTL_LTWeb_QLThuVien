using Library_Manager.Filters;
using Library_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_Manager.Controllers
{
    [Authorization("QTV,QLB,QLT,QLM")]
    public class GiaoDichMuonTraController : Controller
    {
        private readonly QlthuVienContext _context;

        public GiaoDichMuonTraController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: GiaoDichMuonTra
        public IActionResult Index(int? page, string searchString, string returnUrl)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            // 1. Giữ ở dạng IQueryable, include các navigation property
            IQueryable<TGiaoDichMuonTra> giaoDiches = _context.TGiaoDichMuonTras
                .Include(t => t.MaTbdNavigation)    
                    .ThenInclude(TheBanDocController => TheBanDocController.MaBdNavigation)
                .Include(t => t.MaTkNavigation);

            // 2. Nếu có tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();

                giaoDiches = giaoDiches.Where(gd =>
                    gd.MaTbd.ToLower().Contains(searchLower) ||
                    gd.MaGd.ToLower().Contains(searchLower) ||
                    (gd.TrangThai != null && gd.TrangThai.ToLower().Contains(searchLower)) ||
                    EF.Functions.Like(gd.NgayMuon.Year.ToString(), $"%{searchString}%") ||
                    (
                        gd.MaTbdNavigation != null &&
                        gd.MaTbdNavigation.MaBdNavigation != null &&
                        (
                            gd.MaTbdNavigation.MaBdNavigation.Ten.ToLower().Contains(searchLower) ||
                            gd.MaTbdNavigation.MaBdNavigation.HoDem.ToLower().Contains(searchLower) ||
                            gd.MaTbdNavigation.MaBdNavigation.MaBd.ToLower().Contains(searchLower)
                        )
                    )
                );
            }

            // 3. Sắp xếp (ví dụ theo Mã giao dịch)
            giaoDiches = giaoDiches.OrderBy(gd => gd.MaTbd);

            // 4. Tạo paged list
            var pagedGiaoDiches = new PagedList<TGiaoDichMuonTra>(giaoDiches, pageNumber, pageSize);

            // 5. Truyền lại giá trị tìm kiếm để hiển thị trong View
            ViewBag.CurrentFilter = searchString;

            ViewBag.ReturnUrl = returnUrl;

            return View(pagedGiaoDiches);
        }

        
        // GET: GiaoDichMuonTra/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tGiaoDichMuonTra = await _context.TGiaoDichMuonTras
                .Include(t => t.MaTbdNavigation)
                    .ThenInclude(BanDocController => BanDocController.MaBdNavigation)
                .Include(t => t.MaTkNavigation)
                    .ThenInclude(NhanVienController => NhanVienController.MaNvNavigation)   
                .Include(t => t.TGiaoDichBanSaos)
                .FirstOrDefaultAsync(m => m.MaGd == id);
            if (tGiaoDichMuonTra == null)
            {
                return NotFound();
            }
            

            return View(tGiaoDichMuonTra);
        }

        
        // GET: GiaoDichMuonTra/Create
        public IActionResult Create(string returnUrl)
        {
            ViewData["MaTbd"] = new SelectList(_context.TTheBanDocs, "MaTbd", "MaTbd");
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoans, "MaTk", "MaTk");
            ViewBag.ReturnUrl = returnUrl;  
            return View();
        }

        // POST: GiaoDichMuonTra/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGd,MaTbd,MaTk,NgayMuon,NgayHenTra,NgayTra,TrangThai")] TGiaoDichMuonTra tGiaoDichMuonTra)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tGiaoDichMuonTra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTbd"] = new SelectList(_context.TTheBanDocs, "MaTbd", "MaTbd", tGiaoDichMuonTra.MaTbd);
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoans, "MaTk", "MaTk", tGiaoDichMuonTra.MaTk);
            return View(tGiaoDichMuonTra);
        }


        // GET: GiaoDichMuonTra/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tGiaoDichMuonTra = await _context.TGiaoDichMuonTras.FindAsync(id);
            if (tGiaoDichMuonTra == null)
            {
                return NotFound();
            }
            ViewData["MaTbd"] = new SelectList(_context.TTheBanDocs, "MaTbd", "MaTbd", tGiaoDichMuonTra.MaTbd);
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoans, "MaTk", "MaTk", tGiaoDichMuonTra.MaTk);
            return View(tGiaoDichMuonTra);
        }

        // POST: GiaoDichMuonTra/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaGd,MaTbd,MaTk,NgayMuon,NgayHenTra,NgayTra,TrangThai")] TGiaoDichMuonTra tGiaoDichMuonTra)
        {
            if (id != tGiaoDichMuonTra.MaGd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tGiaoDichMuonTra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TGiaoDichMuonTraExists(tGiaoDichMuonTra.MaGd))
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
            ViewData["MaTbd"] = new SelectList(_context.TTheBanDocs, "MaTbd", "MaTbd", tGiaoDichMuonTra.MaTbd);
            ViewData["MaTk"] = new SelectList(_context.TTaiKhoans, "MaTk", "MaTk", tGiaoDichMuonTra.MaTk);
            return View(tGiaoDichMuonTra);
        }

        // GET: GiaoDichMuonTra/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tGiaoDichMuonTra = await _context.TGiaoDichMuonTras
                .Include(t => t.MaTbdNavigation)
                .Include(t => t.MaTkNavigation)
                .FirstOrDefaultAsync(m => m.MaGd == id);
            if (tGiaoDichMuonTra == null)
            {
                return NotFound();
            }

            return View(tGiaoDichMuonTra);
        }

        // POST: GiaoDichMuonTra/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tGiaoDichMuonTra = await _context.TGiaoDichMuonTras.FindAsync(id);
            if (tGiaoDichMuonTra != null)
            {
                _context.TGiaoDichMuonTras.Remove(tGiaoDichMuonTra);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TGiaoDichMuonTraExists(string id)
        {
            return _context.TGiaoDichMuonTras.Any(e => e.MaGd == id);
        }
    }
}
