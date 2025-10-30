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
    public class TacGiaController : Controller
    {
        private readonly QlthuVienContext _context;

        public TacGiaController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: TacGia
        public IActionResult Index(int? page, string searchString)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;
            IQueryable<TTacGia> tacGias = _context.TTacGia;
            if (!string.IsNullOrEmpty(searchString))
            {
                tacGias = tacGias.Where(tg =>
                    tg.Ten.ToLower().Contains(searchString.ToLower()) ||
                    tg.HoDem.ToLower().Contains(searchString.ToLower()) ||
                    tg.MaTg.Contains(searchString));
            }
            tacGias = tacGias.OrderBy(tg => tg.MaTg);
            var pagedTacGias = new PagedList<TTacGia>(tacGias, pageNumber, pageSize);
            ViewBag.CurrentFilter = searchString;
            return View(pagedTacGias);
        }

        // GET: TacGia/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) { return NotFound(); }
            var tTacGium = await _context.TTacGia
                .FirstOrDefaultAsync(m => m.MaTg == id);
            if (tTacGium == null) { return NotFound(); }
            return View(tTacGium);
        }

        // GET: TacGia/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TacGia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTg,HoDem,Ten")] TTacGia tTacGium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tTacGium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tTacGium);
        }

        // GET: TacGia/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) { return NotFound(); }
            var tTacGium = await _context.TTacGia
                .FirstOrDefaultAsync(m => m.MaTg == id);
            if (tTacGium == null) { return NotFound(); }
            return View(tTacGium);
        }

        // POST: TacGia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tTacGium = await _context.TTacGia.FindAsync(id);
            if (tTacGium != null)
            {
                _context.TTacGia.Remove(tTacGium);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TTacGiumExists(string id)
        {
            return _context.TTacGia.Any(e => e.MaTg == id);
        }

        // GET: TacGia/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tTacGium = await _context.TTacGia.FindAsync(id);
            if (tTacGium == null)
            {
                return NotFound();
            }
            return View(tTacGium);
        }

        // POST: TacGia/Edit/5
        // (Đã được tùy biến theo logic của TaiLieuController)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaTg,HoDem,Ten")] TTacGia tTacGium)
        {
            if (id != tTacGium.MaTg)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tTacGium);
                    await _context.SaveChangesAsync();

                    // THÀNH CÔNG: Set TempData
                    TempData["StatusMessage"] = "success";
                    TempData["Message"] = "Thông tin Tác giả đã được cập nhật thành công.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TTacGiumExists(tTacGium.MaTg))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // LỖI XUNG ĐỘT: Set TempData
                        TempData["StatusMessage"] = "danger";
                        TempData["Message"] = "Lỗi xung đột dữ liệu. Vui lòng thử lại.";
                    }
                }
                catch (Exception ex)
                {
                    // LỖI HỆ THỐNG: Set TempData
                    TempData["StatusMessage"] = "danger";
                    TempData["Message"] = "Lỗi hệ thống khi lưu: " + ex.Message;
                }
            }
            else
            {
                // LỖI VALIDATION: Set TempData
                TempData["StatusMessage"] = "danger";
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                   .Select(x => $"{x.Key}: {string.Join("; ", x.Value.Errors.Select(e => e.ErrorMessage))}");
                TempData["Message"] = $"Dữ liệu không hợp lệ. Vui lòng kiểm tra: <ul><li>{string.Join("</li><li>", errors)}</li></ul>";
            }

            // LUÔN LUÔN: Return View để hiển thị thông báo
            return View(tTacGium);
        }
    }
}