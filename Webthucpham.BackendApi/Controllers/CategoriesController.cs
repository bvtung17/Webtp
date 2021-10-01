using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webthucpham.Application.Catalog.Categories;
using Webthucpham.ViewModels.Catalog.Categories;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(
            ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAll();
            return Ok(categories);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] PaginateRequest request, string status)
        {
            var categories = await _categoryService.GetAllPaging(request, status);
            return Ok(categories);
        }

        [HttpGet("productincategory")]
        public async Task<IActionResult> GetProductInCategory([FromQuery] PaginateRequest request, int categoryId)
        {
            var categories = await _categoryService.GetProductInCategory(request, categoryId);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.GetById(id);
            if (category == null)
            {
                return BadRequest($"Cannot find category with id: {id}");
            }
            return Ok(category);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CategoryCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var categoryId = await _categoryService.Create(request);
            if (categoryId == 0)
            {
                return BadRequest();
            }

            var product = await _categoryService.GetById(categoryId);

            return CreatedAtAction(nameof(GetById), new { Id = categoryId }, categoryId);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Edit([FromBody] CategoryUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _categoryService.Edit(request);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {

            var result = await _categoryService.Delete(id);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpGet("products")]
        public async Task<IActionResult> GetProductCategories()
        {
            var productCategories = await _categoryService.GetProductCategories();
            return Ok(productCategories);
        }
    }
}
