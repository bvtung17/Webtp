using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Utilities.Constants;
using Webthucpham.ViewModels.Catalog.Orders;
using Webthucpham.ViewModels.Common;
using Webthucpham.ViewModels.Sales;

namespace Webthucpham.Api
{
    public class OrderApiClient : BaseApiClient, IOrderApiClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public OrderApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration,
           IHttpContextAccessor httpContextAccessor) :
           base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<PageResponse<OrderViewModel>> GetAll(GetOrderRequest request, string status)
        {
            var url = $"/api/orders/paging?pageIndex=" +
               $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.KeyWord}&type={request.Type}&Status={status}&DateStart={request.DateStart}&DateEnd={request.DateEnd}";
            var data = await GetAsync<PageResponse<OrderViewModel>>(url);
            return data;
        }

        public async Task<OrderViewModel> GetById(int id)
        {
            var url = $"/api/orders/{id}";
            var data = await GetAsync<OrderViewModel>(url);
            return data;
        }
        public async Task<List<OrderProductViewModel>> GetProducts(int id)
        {
            var url = $"/api/orders/{id}/products";
            var data = await GetAsync<List<OrderProductViewModel>>(url);
            return data;
        }
        public async Task<bool> UpdateStatus(OrderViewModel request)
        {
            var url = $"/api/orders/status";
            var sessions = _httpContextAccessor
               .HttpContext
               .Session
               .GetString(SystemConstants.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(url, httpContent);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
    }
}
