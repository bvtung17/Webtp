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
        public async Task<bool> Create(OrderCreateRequest request)
        {
            var sessions = _httpContextAccessor
                .HttpContext
                .Session
                .GetString(SystemConstants.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/Orders/Create", httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(int id)
        {
            return await Delete($"/api/Orders/Delete?id=" + id);
        }

        public async Task<OrderViewModel> GetById(OrderViewModel request)
        {
            string url = $"/api/Orders/GetById?id={request.Id}&oderDate={request.OrderDate}" +
                $"&name={request.Name}&shipName={request.ShipName}&shipAddress={request.ShipAddress}$shipEmail={request.ShipEmail}" +
                $"&status={request.Status}&product={request.Product}&quantity={request.Quantity}&price={request.Price}";

            var data = await GetAsync<OrderViewModel>(url);
            return data;
        }

        public async Task<PagedResult<OrderViewModel>> GetChart(OrderPagingRequest request)
        {
            string url = $"/api/Orders/GetChart?pageIndex={request.PageIndex}&pageSize={request.PageSize}&keyWord={request.Keyword}";
            var data = await GetAsync<PagedResult<OrderViewModel>>(url);
            return data;
        }

        public async Task<PagedResult<OrderViewModel>> GetPagings(OrderPagingRequest request)
        {
            string url = $"/api/Orders/GetAllPaging?pageIndex={request.PageIndex}&pageSize={request.PageSize}&keyWord={request.Keyword}&languageId={request.LanguageId}&status={request.Status}";
            var data = await GetAsync<PagedResult<OrderViewModel>>(url);
            return data;
        }

        public async Task<bool> UpdateStatus(int orderId, int status)
        {
            var sessions = _httpContextAccessor
              .HttpContext
              .Session
              .GetString(SystemConstants.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var requestContent = new MultipartFormDataContent();

            requestContent.Add(new StringContent(orderId.ToString()), "orderId");
            requestContent.Add(new StringContent(status.ToString()), "status");
            var response = await client.PutAsync($"/api/Orders/UpdateStatus", requestContent);
            return response.IsSuccessStatusCode;
        }
    }
}
