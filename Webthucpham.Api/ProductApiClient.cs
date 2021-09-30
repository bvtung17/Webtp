using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Api;
using Webthucpham.Utilities.Constants;
using Webthucpham.ViewModels.Catalog.ProductImages;
using Webthucpham.ViewModels.Catalog.Products;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Api
{
    public class ProductApiClient : BaseApiClient, IProductApiClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public ProductApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        //CREATE PRODUCT
        public async Task<ProductUpdateRequest> Create(ProductCreateRequest request)
        {
            var sessions = _httpContextAccessor
                .HttpContext
                .Session
                .GetString(SystemConstants.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "thumbnailImage", request.ThumbnailImage.FileName);
            }

            var contentJs = new StringContent(request.Price.ToString());
            requestContent.Add(contentJs, "price");
            var originJs = new StringContent(request.OriginalPrice.ToString());
            requestContent.Add(originJs, "originalPrice");
            var stockJs = new StringContent(request.Stock.ToString());
            requestContent.Add(stockJs, "stock");
            var nameJs = new StringContent(request.Name.ToString());
            requestContent.Add(nameJs, "name");
            var descriptionJs = new StringContent((request.Description ?? "").ToString());
            requestContent.Add(descriptionJs, "description");
            var detailsJs = new StringContent((request.Details ?? "").ToString());
            requestContent.Add(detailsJs, "details");
            var originalCountryJs = new StringContent((request.OriginalCountry ?? "").ToString());
            requestContent.Add(originalCountryJs, "originalCountry");
            var originNalPriceJs = new StringContent(request.OriginalPrice.ToString());
            requestContent.Add(originNalPriceJs, "originalPrice");
            
            var response = await client.PostAsync($"/api/products", requestContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var product = JsonConvert.DeserializeObject<ProductUpdateRequest>(result);
                return product;
            }

            return null;
        }
        public async Task<PageResponse<ProductViewModel>> GetPagings(GetProductRequest request, string status)
        {
            var url = $"/api/products/paging?pageIndex=" +
              $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}&categoryId={request.CategoryId}&Status={status}";

            var data = await GetAsync<PageResponse<ProductViewModel>>(url);

            return data;
        }
        //UPDATE
        public async Task<bool> UpdateProduct(ProductUpdateRequest request)
        {
            var sessions = _httpContextAccessor
                .HttpContext
                .Session
                .GetString(SystemConstants.AppSettings.Token);  
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            if (request.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "thumbnailImage", request.ThumbnailImage.FileName);
            }

            requestContent.Add(new StringContent((request.Name ?? "").ToString()), "name");

            requestContent.Add(new StringContent((request.Description ?? "").ToString()), "description");

            requestContent.Add(new StringContent((request.Details ?? "").ToString()), "details");

            requestContent.Add(new StringContent((request.OriginalCountry ?? "").ToString()), "originalCountry");
 
            requestContent.Add(new StringContent(request.Id.ToString()), "id");

            requestContent.Add(new StringContent(request.OriginalPrice.ToString()), "originalPrice");

            requestContent.Add(new StringContent(request.Price.ToString()), "Price");

            requestContent.Add(new StringContent(request.Stock.ToString()), "Stock");
            requestContent.Add(new StringContent(request.SelectedId.ToString()), "SelectedId");

            var response = await client.PutAsync($"/api/products/{request.Id}", requestContent);

            return response.IsSuccessStatusCode;
        }


        //CATEGORY
        public async Task<ApiResult<bool>> CategoryAssign(CategoryAssignRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/products/{request.Id}/categories", httpContent);

            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }
        public async Task<bool> Delete(int product_id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.DeleteAsync($"/api/products/{product_id}");

            if (response.IsSuccessStatusCode)
                return true;

            return false;
        }

        public async Task<PageResponse<ProductViewModel>> GetPaging(GetProductRequest request, string status)
        {
            var url = $"/api/products/paging?pageIndex=" +
                $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}&categoryId={request.CategoryId}&Status={status}";

            var data = await GetAsync<PageResponse<ProductViewModel>>(url);
            return data;
        }

        public async Task<bool> Update(ProductUpdateRequest request)
        {
            var sessions = _httpContextAccessor
                  .HttpContext
                  .Session
                  .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();
            if (request.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "thumbnailImage", request.ThumbnailImage.FileName);
            }

            requestContent.Add(new StringContent((request.Name ?? "").ToString()), "name");

            requestContent.Add(new StringContent((request.Description ?? "").ToString()), "description");

            requestContent.Add(new StringContent((request.Details ?? "").ToString()), "details");

            requestContent.Add(new StringContent((request.OriginalCountry ?? "").ToString()), "originalCountry");
       
            requestContent.Add(new StringContent(request.Id.ToString()), "id");

            requestContent.Add(new StringContent(request.OriginalPrice.ToString()), "originalPrice");

            requestContent.Add(new StringContent(request.Price.ToString()), "Price");

            requestContent.Add(new StringContent(request.Stock.ToString()), "Stock");
            requestContent.Add(new StringContent(request.SelectedId.ToString()), "SelectedId");

            var response = await client.PutAsync($"/api/products/{request.Id}", requestContent);

            return response.IsSuccessStatusCode;
        }

       

        public async Task<PageResponse<ProductImageViewModel>> GetProductImages(int productId, PagingRequestBase request)
        {

            var url = $"/api/products/{productId}/images?pageSize={request.PageSize}&pageIndex={request.PageIndex}";
            var data = await GetAsync<PageResponse<ProductImageViewModel>>(url);
            return data;
        }

        public async Task<bool> AddImage(ProductImageCreateRequest request)
        {
            var sessions = _httpContextAccessor
             .HttpContext
             .Session
             .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();
            if (request.ImageFile != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ImageFile.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ImageFile.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "imageFile", request.ImageFile.FileName);
            }

            var contentJs = new StringContent((request.Caption ?? "").ToString());
            requestContent.Add(contentJs, "caption");


            var response = await client.PostAsync($"/api/products/{request.ProductId}/images", requestContent);
            if (response.IsSuccessStatusCode)
                return true;

            return false;
        }

        public async Task<bool> ChangeThumbnail(int imageId, int productId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.PutAsync($"/api/products/{productId}/images/{imageId}/thumbnail", null);

            if (response.IsSuccessStatusCode)
                return true;

            return false;
        }

        public async Task<bool> UpdateProductImage(ProductImageUpdateRequest request)
        {
            var sessions = _httpContextAccessor
              .HttpContext
              .Session
              .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();
            if (request.ImageFile != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ImageFile.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ImageFile.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "imageFile", request.ImageFile.FileName);
            }

            var contentJs = new StringContent((request.Caption ?? "").ToString());
            requestContent.Add(contentJs, "caption");


            var response = await client.PutAsync($"/api/products/images/{request.Id}", requestContent);
            if (response.IsSuccessStatusCode)
                return true;

            return false;
        }

        public async Task<ProductImageViewModel> GetImageById(int productId, int imageId)
        {
            var url = $"/api/products/{productId}/images/{imageId}";

            var image = await GetAsync<ProductImageViewModel>(url);

            return image;
        }

        public async Task<ApiResult<bool>> DeleteImage(int productId, int imageId)
        {
            var sessions = _httpContextAccessor
               .HttpContext
               .Session
               .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.DeleteAsync($"/api/products/{productId}/images/{imageId}");
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result); ;
            }
            return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);
        }

        public async Task<PageResponse<ProductViewModel>> SearchProduct(GetProductRequest request)
        {
            var url = $"/api/products/Search?pageIndex=" +
               $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}&categoryId={request.CategoryId}&PriceStart={request.PriceStart}&PriceEnd={request.PriceEnd}&SortPrice={request.SortPrice}";

            var data = await GetAsync<PageResponse<ProductViewModel>>(url);

            return data;
        }

        public async Task<ProductUpdateRequest> GetById(int id)
        {
             var url = $"/api/products/{id}";

            var response = await GetAsync<ProductUpdateRequest>(url);

            return response;
        }
    }
}
