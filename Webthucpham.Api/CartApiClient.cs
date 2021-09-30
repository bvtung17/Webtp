using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Utilities.Constants;
using Webthucpham.ViewModels.Catalog.Carts;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Api
{
    public class CartApiClient : BaseApiClient, ICartApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) :
            base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ClientCartViewModel> AddToCart(ClientCartViewModel request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/Carts/AddToCart", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var cart = JsonConvert.DeserializeObject<ClientCartViewModel>(result);
            return cart;
        }

        public async Task<ApiResult<ClientCartViewModel>> GetCart(Guid id)
        {
            var url = $"/api/carts/{id}";
            var result = await GetAsync<ApiResult<ClientCartViewModel>>(url);
            return result;
        }

        public async Task<ApiResult<ClientCartViewModel>> RemoveProduct(DeleteProductInCartRequest request)
        {
            var url = $"api/carts/delete";
            var result = await PostAsync<ApiResult<ClientCartViewModel>, DeleteProductInCartRequest>(url, request);
            return result;
        }

        public async Task<ClientCartViewModel> UpdateCart(ClientCartViewModel request)
        {
            throw new NotImplementedException();
        }
    }
}
