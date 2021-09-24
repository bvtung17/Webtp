using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Data.EF;
using Webthucpham.Data.Entities;
using Webthucpham.Utilities.Exceptions;
using Webthucpham.ViewModels.Common;
using Webthucpham.ViewModels.Sales;

namespace Webthucpham.Application.Catalog.Orders
{
    public class OrderService : IOrderService
    {
        private readonly WebthucphamDbContext _context;

        public OrderService(WebthucphamDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(OrderCreateRequest request)
        {
            var order = new Order()
            {
                ShipAddress = request.ShipAddress,
                OrderDate = request.OrderDate,
                ShipEmail = request.ShipEmail,
                ShipName = request.ShipName,
                ShipPhoneNumber = request.ShipPhoneNumber,
                Status = request.Status,
                UserId = request.UserId,
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            //dung async tan dang thread chuong trinh chay ngon hon
            var orderDetails = new List<OrderDetail>();

            int maxId = _context.Orders.OrderByDescending(x => x.Id).Select(x => x.Id).ToList()[0];
            foreach (var id in request.ProductDetails)
            {
                var OrderDetails = new OrderDetail()
                {
                    ProductId = id.Key,
                    Price = request.Price,
                    Quantity = id.Value,
                    OrderId = maxId
                };

                orderDetails.Add(OrderDetails);
            }

            _context.OrderDetails.AddRange(orderDetails);
            await _context.SaveChangesAsync();
            return order.Id;
        }

        public async Task<int> Delete(int orderId)
        {
            var rs = await _context.Orders.FindAsync(orderId);
            if (rs == null)
            {
                throw new WebthucphamException("Không tìm thấy đơn hàng theo id");
            }
            _context.Orders.Remove(rs);
            var orderDetails = _context.OrderDetails.Where(x => x.OrderId == orderId);
            if (orderDetails == null)
            {
                throw new WebthucphamException("Không tìm thấy đơn hàng theo id");
            }
            _context.OrderDetails.RemoveRange(orderDetails);
            return await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<OrderViewModel>> GetAllPaging(OrderPagingRequest request)
        {
            var query = from o in _context.Orders
                        join od in _context.OrderDetails on o.Id equals od.OrderId into odd
                        from od in odd.DefaultIfEmpty()
                        join p in _context.ProductTranslations on od.ProductId equals p.ProductId into pp
                        from p in pp.DefaultIfEmpty()
                        where p.LanguageId == request.LanguageId
                        select new { o, od, p };

            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.p.Name.Contains(request.Keyword) || x.o.AppUser.UserName.Contains(request.Keyword));
            if (request.Status != 0)
            {
                query = query.Where(x => x.o.Status == request.Status);
            }
            //3. Paging
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).
                Select(x => new OrderViewModel()
                {
                    Id = x.o.Id,
                    Product = x.p.Name,
                    OrderDate = x.o.OrderDate,
                    Name = x.o.AppUser.UserName,
                    ShipName = x.o.ShipName,
                    ShipAddress = x.o.ShipAddress,
                    ShipEmail = x.o.ShipEmail,
                    ShipPhoneNumber = x.o.ShipPhoneNumber,
                    Status = x.o.Status,
                    Price = x.od.Price,
                    Quantity = x.od.Quantity
                }).ToListAsync();

            //4. Select and projection
            var pagedResult = new PagedResult<OrderViewModel>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public async Task<PagedResult<OrderViewModel>> GetChart(OrderPagingRequest request)
        {
            var query = from o in _context.Orders
                        join od in _context.OrderDetails on o.Id equals od.OrderId into odd
                        from od in odd.DefaultIfEmpty()
                        join p in _context.ProductTranslations on od.ProductId equals p.ProductId into pp
                        from p in pp.DefaultIfEmpty()
                        select new { o, od, p };

            //3. Paging
            int totalRow = await query.CountAsync();
            var data = await query.OrderByDescending(x => x.o.Id).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).
                Select(x => new OrderViewModel()
                {
                    Id = x.o.Id,
                    Product = x.p.Name,
                    OrderDate = x.o.OrderDate,
                    Name = x.o.AppUser.UserName,
                    ShipName = x.o.ShipName,
                    ShipAddress = x.o.ShipAddress,
                    ShipEmail = x.o.ShipEmail,
                    ShipPhoneNumber = x.o.ShipPhoneNumber,
                    Status = x.o.Status,
                    Price = x.od.Price,
                    Quantity = x.od.Quantity
                }).ToListAsync();

            //4. Select and projection
            var pagedResult = new PagedResult<OrderViewModel>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return pagedResult;
        }

        public OrderViewModel GetById(OrderViewModel request)
        {
            return request;
        }

        public async Task<bool> UpdateStatus(int orderId, int status)
        {
            var order = _context.Orders.FirstOrDefault(x => x.Id == orderId);
            order.Status = status;
            var rs = await _context.SaveChangesAsync();
            if (rs != 0)
            {
                return false;
            }
            return true;
        }
    }
}
