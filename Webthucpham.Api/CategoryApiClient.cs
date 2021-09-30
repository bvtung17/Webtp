using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Api;
using Webthucpham.Utilities.Constants;
using Webthucpham.ViewModels.Catalog.Categories;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Api
{
    public class CategoryApiClient : BaseApiClient, ICategoryApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CategoryApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> Create(CategoryCreateRequest request)
        {
            var sessions = _httpContextAccessor
               .HttpContext
               .Session
               .GetString(SystemConstants.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/categories", httpContent);

            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<int>(result);
            }

            return JsonConvert.DeserializeObject<int>(result);

        }
 
        public async Task<bool> Edit(CategoryUpdateRequest request)
        {
            var sessions = _httpContextAccessor
           .HttpContext
           .Session
           .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/categories", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> Delete(int id)
        {
            var sessions = _httpContextAccessor
             .HttpContext
             .Session
             .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_config[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.DeleteAsync($"/api/categories/{id}");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<List<CategoryViewModel>> GetAll()
        {
            var url = "/api/categories";
            return await GetAsync<List<CategoryViewModel>>(url);
        }

        public async Task<CategoryViewModel> GetById( int id)
        {
            var url = $"/api/categories/{id}";
            return await GetAsync<CategoryViewModel>(url);
        }

        public async Task<List<HomeCategoryViewModel>> GetHomeProductCategories()
        {
            var url = $"/api/categories/products";
            var productCategories = await GetAsync<List<HomeCategoryViewModel>>(url);
            return productCategories;
        }

        public async Task<PageResponse<ProductViewModel>> GetProductInCategory(PaginateRequest request, int categoryId)
        {
            var url = $"/api/categories/productincategory?pageIndex=" +
            $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}&categoryId={categoryId}";
            var products = await GetAsync<PageResponse<ProductViewModel>>(url);
            return products;
        }

        public async Task<PageResponse<CategoryViewModel>> GetAllPaging(PaginateRequest request, string status)
        {
            var url = $"/api/categories/paging?pageIndex=" +
              $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}&Status={status}";
            var categories = await GetAsync<PageResponse<CategoryViewModel>>(url);
            return categories;
        }

        public async Task<PageResponse<ProductViewModel>> Search(PaginateRequest request, string categoryId)
        {
            var url = $"/api/categories/search?keyword={request.Keyword}&categoryId={categoryId}&pageIndex=" +
             $"{request.PageIndex}&pageSize={request.PageSize}";
            var products = await GetAsync<PageResponse<ProductViewModel>>(url);
            return products;
        }
    }
}
