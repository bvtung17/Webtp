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

        public CategoriesController (ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAll();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById( int id)
        {
            var category = await _categoryService.GetById(id);
            return Ok(category);
        }
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] PaginateRequest request, string status)
        {
            var categories = await _categoryService.GetAllPaging(request, status);
            return Ok(categories);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CategoryCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var categoryId = await _categoryService.Create(request);
            if (categoryId ==0)
            {
                return BadRequest(ModelState);
            }
            var product = await _categoryService.GetById(categoryId);
            return CreatedAtAction(nameof(GetById), new { Id = categoryId }, categoryId);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit([FromBody] CategoryUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _categoryService.Edit(request);
            if (!result)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }
    }
}
