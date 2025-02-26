using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Mvc;

namespace DuAnTN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CatelogyController : Controller
    {
        private readonly CategoryService _categoryService;

        public CatelogyController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? id, string search, int page = 1)
        {
            int pageSize = 10; // Số danh mục trên mỗi trang
            var categories = await _categoryService.GetCategoriesAsync();

            // Lọc theo tìm kiếm nếu có
            if (!string.IsNullOrEmpty(search))
            {
                categories = categories.Where(c => c.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Phân trang
            int totalItems = categories.Count();
            var pagedCategories = categories.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Nếu có id, lấy danh mục cần chỉnh sửa
            if (id.HasValue)
            {
                ViewBag.CategoryToEdit = await _categoryService.GetCategoryByIdAsync(id.Value);
            }

            // Truyền dữ liệu vào ViewBag
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(pagedCategories);
        }



        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return View(category);
        }

        [HttpGet]
        public IActionResult Create() { return View(); }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                // Gọi service để lưu danh mục vào cơ sở dữ liệu
                await _categoryService.CreateCategoryAsync(category);

                // Sau khi lưu thành công, chuyển hướng về trang danh sách danh mục
                return RedirectToAction(nameof(Index));
            }
            return View(category); // Nếu có lỗi, trả lại view và hiển thị thông báo lỗi
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category); // Trả về View có chứa Modal
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(category.Id, category);
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index), new { id = category.Id }); // Nếu có lỗi, giữ lại modal
        }



        [HttpGet]
        [Route("Category/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await _categoryService.DeleteCategoryAsync(id);

            TempData["SuccessMessage"] = $"Danh mục '{category.CategoryName}' đã được xóa thành công!";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category != null)
            {
                await _categoryService.DeleteCategoryAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetFoodsByCategory(int categoryId)
        {
            var foods = await _categoryService.GetFoodsByCategoryAsync(categoryId);
            return View(foods);
        }
    }
}
