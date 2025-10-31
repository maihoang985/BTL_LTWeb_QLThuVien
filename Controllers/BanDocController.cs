using Library_Manager.Filters;
using Library_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core; // Thêm dòng này
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library_Manager.Models;
using PagedList.Core;

namespace Library_Manager.Controllers
{
    [Authorization("QTV,QLB,QLT,QLM")]
    public class BanDocController : Controller
    {
        private readonly QlthuVienContext _context;

        public BanDocController(QlthuVienContext context)
        {
            _context = context;
        }

        // GET: TBanDoc
        
        public IActionResult Index(int? page, string searchString) // Bỏ async và await
        {
            var pageNumber = page ?? 1;
            var pageSize = 6;

            // 1. Giữ ở dạng IQueryable (Không dùng ToList() hoặc ToListAsync())
            IQueryable<TBanDoc> banDocs = _context.TBanDocs;

            if (!string.IsNullOrEmpty(searchString))
            {
                banDocs = banDocs.Where(t =>
                t.Ten.ToLower().Contains(searchString) ||
                t.HoDem.ToLower().Contains(searchString) ||
                t.MaBd.ToLower().Contains(searchString) ||
                t.Email.ToLower().Contains(searchString) ||
                (t.GioiTinh == "M" && "nam".Contains(searchString)) ||
                (t.GioiTinh == "F" && "nữ".Contains(searchString)) ||
                (t.GioiTinh != "M" && t.GioiTinh != "F" && "khác".Contains(searchString)) ||
                EF.Functions.Like(t.NgaySinh.Year.ToString(), $"%{searchString}%"));
            }

            // Sắp xếp
            banDocs = banDocs.OrderBy(bd => bd.MaBd);

            // 2. ToPagedList() sẽ tự xử lý việc thực thi truy vấn phân trang
            var pagedBanDocs = new PagedList<TBanDoc>(banDocs, pageNumber, pageSize);

            // Hoặc sử dụng ToPagedList() nếu bạn đã cài đặt package PagedList.Core.Mvc
            // var pagedBanDocs = banDocs.ToPagedList(pageNumber, pageSize);

            // Truyền lại giá trị tìm kiếm để hiển thị lại trong View
            ViewBag.CurrentFilter = searchString;

            return View(pagedBanDocs);
        }

        //[Authorization("QLB")]
        // GET: TBanDoc/Details/5
        public async Task<IActionResult> Details(string id, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tBanDoc = await _context.TBanDocs
                .FirstOrDefaultAsync(m => m.MaBd == id);
            if (tBanDoc == null)
            {
                return NotFound();
            }

            // Dùng ViewBag hoặc ViewData để truyền returnUrl sang View
            ViewBag.ReturnUrl = returnUrl;

            return View(tBanDoc);
        }

        //[Authorization("QLB")]
        // GET: TBanDoc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TBanDoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorization("QLB")]
        public async Task<IActionResult> Create([Bind("MaBd,HoDem,Ten,NgaySinh,GioiTinh,DiaChi,Sdt,Email")] TBanDoc tBanDoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tBanDoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tBanDoc);
        }

        //[Authorization("QLB")]
        // GET: TBanDoc/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tBanDoc = await _context.TBanDocs.FindAsync(id);
            if (tBanDoc == null)
            {
                return NotFound();
            }
            return View(tBanDoc);
        }

        // POST: TBanDoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorization("QLB")]
        public async Task<IActionResult> Edit(string id, [Bind("MaBd,HoDem,Ten,NgaySinh,GioiTinh,DiaChi,Sdt,Email")] TBanDoc tBanDoc)
        {
            if (id != tBanDoc.MaBd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tBanDoc);
                    await _context.SaveChangesAsync();

                    // THAY ĐỔI: Sử dụng TempData và return View
                    TempData["StatusMessage"] = "success";
                    TempData["Message"] = "Thông tin Bạn đọc đã được lưu thành công.";

                    return View(tBanDoc);
                    // return RedirectToAction(nameof(Index)); // Bỏ dòng này
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TBanDocExists(tBanDoc.MaBd))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // THAY ĐỔI: Thêm TempData cho lỗi xung đột
                        TempData["StatusMessage"] = "danger";
                        TempData["Message"] = "Lỗi xung đột dữ liệu. Vui lòng tải lại trang và thử lại.";
                        // throw; // Bỏ dòng này
                    }
                }
                // THAY ĐỔI: Thêm catch tổng quát
                catch (Exception ex)
                {
                    TempData["StatusMessage"] = "danger";
                    TempData["Message"] = "Lỗi hệ thống khi lưu dữ liệu: " + ex.Message;
                }
            }
            // THAY ĐỔI: Thêm TempData cho lỗi validation
            else
            {
                TempData["StatusMessage"] = "danger";
                var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => $"{x.Key}: {string.Join("; ", x.Value.Errors.Select(e => e.ErrorMessage))}");
                TempData["Message"] = $"Dữ liệu không hợp lệ. Vui lòng kiểm tra: <ul><li>{string.Join("</li><li>", errors)}</li></ul>";
            }
            return View(tBanDoc);
        }

        //[Authorization("QLB")]
        // GET: TBanDoc/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tBanDoc = await _context.TBanDocs
                .FirstOrDefaultAsync(m => m.MaBd == id);
            if (tBanDoc == null)
            {
                return NotFound();
            }

            return View(tBanDoc);
        }

        // POST: TBanDoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorization("QLB")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tBanDoc = await _context.TBanDocs.FindAsync(id);
            if (tBanDoc != null)
            {
                _context.TBanDocs.Remove(tBanDoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TBanDocExists(string id)
        {
            return _context.TBanDocs.Any(e => e.MaBd == id);
        }
    }
}
