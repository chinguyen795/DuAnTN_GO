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
            int pageSize = 10;
            var categories = await _categoryService.GetCategoriesAsync();

            if (!string.IsNullOrEmpty(search))
            {
                categories = categories.Where(c => c.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            int totalItems = categories.Count();
            var pagedCategories = categories.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (id.HasValue)
            {
                ViewBag.CategoryToEdit = await _categoryService.GetCategoryByIdAsync(id.Value);
            }

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
                await _categoryService.CreateCategoryAsync(category);

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(category.Id, category);
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index), new { id = category.Id });
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
