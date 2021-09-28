using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Application.Common;
using Webthucpham.Data.EF;
using Webthucpham.Data.Entities;
using Webthucpham.ViewModels.Catalog.Orders;
using Webthucpham.ViewModels.Common;
using Webthucpham.ViewModels.System.Clients;

namespace Webthucpham.Application.Catalog.Clients
{
    public class ClientService : IClientService
    {
        private readonly WebthucphamDbContext _context;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;

        public ClientService(WebthucphamDbContext context, IConfiguration configuration, IStorageService storageService)
        {
            _context = context;
            _config = configuration;
            _storageService = storageService;
        }

        public async Task<ClientViewModel> GetClientById(Guid id)
        {

            var client = await _context.Clients.Where(x => x.Id == id).FirstOrDefaultAsync();

            var clientViewModel = new ClientViewModel()
            {
                Id = id,
                Name = client.Name,
                Email = client.Email,
                Address = client.Address,
                Dob = client.Dob,
                PhoneNumber = client.PhoneNumber
            };
            return clientViewModel;
        }

        public async Task<List<OrderViewModel>> GetOrderByClient(Guid id)
        {
            var query = from c in _context.Clients
                        where c.Id == id
                        join o in _context.Orders on c.Id equals o.ClientId
                        select new { o };

            query = query.OrderByDescending(x => x.o.Id);
            var orders = await query.Select(x => new OrderViewModel()
            {
                Id = x.o.Id,
                OrderDate = x.o.OrderDate,
                Price = x.o.Price,
                ShipEmail = x.o.ShipEmail,
                ShipAddress = x.o.ShipAddress,
                ShipName = x.o.ShipName,
                ShipPhoneNumber = x.o.ShipPhoneNumber,
                Status = x.o.Status,
            }).ToListAsync();

            return orders;
        }
        public async Task<ApiResult<PageResponse<ClientViewModel>>> GetClientPaging(GetClientPagingRequest request)
        {
            var query = from c in _context.Clients select c;

            if (request.Keyword != null)
            {
                query = query.Where(x => x.Name.Contains(request.Keyword));
            }

            var pageIndex = request.PageIndex;
            var pageSize = request.PageSize;
            var count = await query.CountAsync();

            var clients = await query.Select(x => new ClientViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Address = x.Address,
                Dob = x.Dob,
                PhoneNumber = x.PhoneNumber
            }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            foreach (var item in clients)
            {
                var orders = from c in clients
                             join o in _context.Orders on c.Id equals o.ClientId
                             where c.Id == item.Id
                             select new { c, o };
                var QuantityOfOrders = orders.Select(x => x.o).ToList();
                item.OrderQuanttity = QuantityOfOrders.Count();
            }

            var pageResponse = new PageResponse<ClientViewModel>()
            {
                TotalRecords = count,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = clients
            };
            var apiResult = new ApiSuccessResult<PageResponse<ClientViewModel>>(pageResponse);


            return apiResult;
        }

        public async Task<ApiResult<ClientUpdateViewModel>> GetDetail(Guid clientId)
        {
            var client = await _context.Clients.Where(x => x.Id == clientId).FirstOrDefaultAsync();
            if (client == null)
            {
                return new ApiErrorResult<ClientUpdateViewModel>("Không tìm thấy tài khoản");
            }
            var model = new ClientUpdateViewModel()
            {
                Id = clientId,
                Address = client.Address,
                Dob = client.Dob,
                Email = client.Email,
                Name = client.Name,
                PhoneNumber = client.PhoneNumber,
                Avatar = client.Avatar
            };
            return new ApiSuccessResult<ClientUpdateViewModel>(model);
        }



        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }

        public async Task<ApiResult<string>> Register(ClientRegisterRequest request)
        {
            var client = await _context.Clients.Where(x => x.Email == request.Email).FirstOrDefaultAsync();
            var user = await _context.Users.Where(x => x.Email == request.Email).FirstOrDefaultAsync();
            if (client != null || user != null)
            {
                return new ApiErrorResult<string>("Email đã tồn tại");
            }
            var hashser = new PasswordHasher<Client>();
            var newClient = new Client()
            {
                Name = request.FullName,
                Email = request.Email,
                Address = request.Address,
                Dob = request.Dob,
                PhoneNumber = request.PhoneNumber,
                Password = hashser.HashPassword(null, request.Password),
            };
            _context.Clients.Add(newClient);
            await _context.SaveChangesAsync();

            try
            {
                var claims = new[]
                {
                new Claim(ClaimTypes.Email,newClient.Email),
                new Claim(ClaimTypes.Name,newClient.Name),
                new Claim("Id",newClient.Id.ToString())
            };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                    _config["Tokens:Issuer"],
                    claims,
                    expires: DateTime.Now.AddHours(24),
                    signingCredentials: creds);

                var apiResult = new ApiResult<string>()
                {
                    IsSuccessed = true,
                    ResultObj = new JwtSecurityTokenHandler().WriteToken(token)
                };
                return apiResult;
            }
            catch (Exception)
            {

                return new ApiErrorResult<string>("Liên hệ quản lý");
            }
        }

        public async Task<ApiResult<string>> Login(ClientLoginRequest request)
        {
            var client = await _context.Clients.Where(x => x.Email == request.Email).FirstOrDefaultAsync();
            if (client == null)
            {
                return new ApiErrorResult<string>("Tài khoản không tồn tại");
            }
            var hasher = new PasswordHasher<Client>();
            var isCorrectPassword = hasher.VerifyHashedPassword(client, client.Password, request.Password);
            if (isCorrectPassword != PasswordVerificationResult.Success)
            {
                return new ApiErrorResult<string>("Tài khoản hoặc mật khẩu khoogn chính xác");
            }
            try
            {
                var claims = new[]
                {
                    new Claim("PhoneNumber",client.PhoneNumber),
                    new Claim("Dob",client.Dob.ToString()),
                    new Claim("Avatar",client.Avatar ?? ""),
                    new Claim("Email",request.Email),
                    new Claim("Name",client.Name),
                    new Claim("Id",client.Id.ToString())
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                    _config["Tokens:Issuer"],
                    claims,
                    expires: DateTime.Now.AddHours(24),
                    signingCredentials: creds);
                return new ApiSuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
            }
            catch (Exception)
            {
                return new ApiErrorResult<string>("Lỗi đăng nhập");
            }
        }

        public async Task<ApiResult<ClientUpdateViewModel>> Update(ClientUpdateViewModel request)
        {
            var client = await _context.Clients.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (client == null)
            {
                return new ApiErrorResult<ClientUpdateViewModel>("Không tìm thấy tài khoản");
            }
            if (request.NewAvatar != null)
            {
                var newAvatarUrl = await SaveFile(request.NewAvatar);
                client.Avatar = newAvatarUrl;
            }
            client.Name = request.Name;
            client.PhoneNumber = request.PhoneNumber;
            client.Address = request.Address;
            client.Dob = request.Dob;
            var hasher = new PasswordHasher<Client>();
            var isHaveData = !String.IsNullOrEmpty(request.OldPassword) || !String.IsNullOrEmpty(request.NewPassword) || !String.IsNullOrEmpty(request.RepeatPassword);
            if (isHaveData)
            {
                var isEmpty = String.IsNullOrEmpty(request.OldPassword) || String.IsNullOrEmpty(request.NewPassword) || String.IsNullOrEmpty(request.RepeatPassword);
                if (isEmpty)
                {
                    return new ApiErrorResult<ClientUpdateViewModel>("Phải điền đầy đủ 3 trường để cập nhật mật khẩu");
                }
                if (request.NewPassword != request.RepeatPassword)
                {
                    return new ApiErrorResult<ClientUpdateViewModel>("2 mật khẩu không trùng nhau");
                }

                var isCorrectPassword = hasher.VerifyHashedPassword(client, client.Password, request.OldPassword) == PasswordVerificationResult.Success;

                if (!isCorrectPassword)
                {
                    return new ApiErrorResult<ClientUpdateViewModel>("Mật khẩu cũ không đúng");
                }
                var isSamePassword = hasher.VerifyHashedPassword(client, client.Password, request.NewPassword) == PasswordVerificationResult.Success;
                if (isSamePassword)
                {
                    return new ApiErrorResult<ClientUpdateViewModel>("Mật khẩu mới không được trùng với mật khẩu cũ");
                }
                client.Password = hasher.HashPassword(client, request.NewPassword);
            }


            _context.Clients.Attach(client);

            await _context.SaveChangesAsync();
            request.Avatar = client.Avatar;
            request.NewPassword = null;
            request.OldPassword = null;
            request.RepeatPassword = null;
            request.NewAvatar = null;

            return new ApiSuccessResult<ClientUpdateViewModel>(request);
        }

    }
}
