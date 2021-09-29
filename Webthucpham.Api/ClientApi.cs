using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Utilities.Constants;
using Webthucpham.ViewModels.Catalog.Orders;
using Webthucpham.ViewModels.Common;
using Webthucpham.ViewModels.System.Clients;

namespace Webthucpham.Api
{
    public class ClientApi : BaseApiClient, IClientApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ClientApi(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ClientViewModel> GetByClientId(Guid clientid)
        {
            var url = $"/api/clients/{clientid}/details";
            var data = await GetAsync<ClientViewModel>(url);
            return data;
        }

        public async Task<ApiResult<PageResponse<ClientViewModel>>> GetClientPaging(GetClientPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);

            var bearerToken = _httpContextAccessor.HttpContext.Session.GetString("Token");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            
            var Url = $"/api/clients/paging?pageIndex=" +
              $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}";
            var response = await client.GetAsync(Url);

            var body = await response.Content.ReadAsStringAsync();

            var users = JsonConvert.DeserializeObject<ApiSuccessResult<PageResponse<ClientViewModel>>>(body);
            users.ResultObj.PageSize = request.PageSize;
            users.ResultObj.PageIndex = request.PageIndex;

            return users;

        }

        public async Task<ApiResult<ClientUpdateViewModel>> GetDetail(Guid userId)
        {
            var url = $"api/clients/{userId}";
            var result = await GetAsync<ApiResult<ClientUpdateViewModel>>(url);
            return result;
        }

        public async Task<List<OrderViewModel>> GetOrderByClientId(Guid id)
        {
            var url = $"/api/clients/{id}/details/orders";
            var data = await GetAsync<List<OrderViewModel>>(url);
            return data;
        }

        public async Task<ApiResult<string>> Login(ClientLoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/clients/login", httpContent);
            var Obj = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(Obj);

            }
            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(Obj);
        }

        public async Task<ApiResult<string>> Register(ClientRegisterRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/clients/register", httpContent);
            var Obj = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ApiSuccessResult<string>>(Obj);

            }
            return JsonConvert.DeserializeObject<ApiErrorResult<string>>(Obj);
        }

        public async Task<ApiResult<ClientUpdateViewModel>> Update(ClientUpdateViewModel request)
        {
            var sessions = _httpContextAccessor
                  .HttpContext
                  .Session
                  .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var requestContent = new MultipartFormDataContent();
            if (request.NewAvatar != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.NewAvatar.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.NewAvatar.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "newAvatar", request.NewAvatar.FileName);
            }

            requestContent.Add(new StringContent(request.Name, Encoding.UTF8), "name");
            requestContent.Add(new StringContent(request.Id.ToString()), "id");
            requestContent.Add(new StringContent(request.Dob.ToString()), "dob");
            requestContent.Add(new StringContent(request.Address, Encoding.UTF8), "address");
            requestContent.Add(new StringContent(request.PhoneNumber), "phoneNumber");
            requestContent.Add(new StringContent(request.NewPassword ?? ""), "newPassword");
            requestContent.Add(new StringContent(request.RepeatPassword ?? ""), "repeatPassword");
            requestContent.Add(new StringContent(request.OldPassword ?? ""), "oldPassword");

            var response = await client.PutAsync($"/api/clients/{request.Id}", requestContent);
            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResult<ClientUpdateViewModel>>(result);
        }
    }
}
