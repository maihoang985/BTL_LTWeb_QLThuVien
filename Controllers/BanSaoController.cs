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
    public class BanSaoController : Controller
    {
        private readonly QlthuVienContext _context;

        public BanSaoController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: BanSao
        public IActionResult Index(string id, int? page, string searchString)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            // Ban đầu lấy tất cả bản sao
            IQueryable<TBanSao> banSaos = _context.TBanSao.Include(t => t.MaTlNavigation);

            // Nếu có mã tài liệu truyền vào thì lọc theo mã đó
            if (!string.IsNullOrEmpty(id))
            {
                banSaos = banSaos.Where(bs => bs.MaTl == id);
                ViewBag.TenTaiLieu = _context.TTaiLieu
                    .Where(t => t.MaTl == id)
                    .Select(t => t.TenTl)
                    .FirstOrDefault();

                ViewBag.MaTl = id;
            }



            // Nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                banSaos = banSaos.Where(bs =>
                    bs.MaTlNavigation.TenTl.ToLower().Contains(searchString.ToLower()) ||
                    bs.MaTl.Contains(searchString));
            }

            banSaos = banSaos.OrderBy(bs => bs.MaBs);

            var pagedBanSaos = new PagedList<TBanSao>(banSaos, pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.MaTl = id; // Lưu mã tài liệu để sử dụng trong View
            }


            return View(pagedBanSaos);
        }


        // GET: BanSao/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tBanSao = await _context.TBanSao
                .Include(t => t.MaTlNavigation)
                .FirstOrDefaultAsync(m => m.MaBs == id);
            if (tBanSao == null)
            {
                return NotFound();
            }

            return View(tBanSao);
        }

        // GET: BanSao/Create
        public IActionResult Create()
        {
            ViewData["MaTl"] = new SelectList(_context.TTaiLieu, "MaTl", "MaTl");
            return View();
        }

        // POST: BanSao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaBs,MaTl,TinhTrang")] TBanSao tBanSao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tBanSao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTl"] = new SelectList(_context.TTaiLieu, "MaTl", "MaTl", tBanSao.MaTl);
            return View(tBanSao);
        }

        // GET: BanSao/Edit/5
        public async Task<IActionResult> Edit(string id, string returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tBanSao = await _context.TBanSao.FindAsync(id);
            if (tBanSao == null)
            {
                return NotFound();
            }
            ViewData["MaTl"] = new SelectList(_context.TTaiLieu, "MaTl", "MaTl", tBanSao.MaTl);
            ViewBag.ReturnUrl = returnUrl;
            return View(tBanSao);
        }

        // POST: BanSao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaBs,MaTl,TinhTrang")] TBanSao tBanSao)
        {
            if (id != tBanSao.MaBs)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tBanSao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TBanSaoExists(tBanSao.MaBs))
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
            ViewData["MaTl"] = new SelectList(_context.TTaiLieu, "MaTl", "MaTl", tBanSao.MaTl);
            return View(tBanSao);
        }

        // GET: BanSao/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tBanSao = await _context.TBanSao
                .Include(t => t.MaTlNavigation)
                .FirstOrDefaultAsync(m => m.MaBs == id);
            if (tBanSao == null)
            {
                return NotFound();
            }

            return View(tBanSao);
        }

        // POST: BanSao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tBanSao = await _context.TBanSao.FindAsync(id);
            if (tBanSao != null)
            {
                _context.TBanSao.Remove(tBanSao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TBanSaoExists(string id)
        {
            return _context.TBanSao.Any(e => e.MaBs == id);
        }
    }
}
