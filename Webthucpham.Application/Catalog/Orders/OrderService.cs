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
            var newOrder = new Order()
            {
                OrderDate = DateTime.Now,
                Price = request.TotalPrice,
                ShipAddress = request.ShipAddress,
                ShipPhoneNumber = request.ShipPhone,
                ShipEmail = request.Email,
                Note = request.ClientNote,
                ShipName = request.ClientName,
                CartId = request.ClientCart.Id,
                Status = OrderStatus.InProgress,
            };
            if (request.ClientID != null)
            {
                newOrder.ClientId = request.ClientID;
            }
            else
            {
                newOrder.ClientId = null;
            }
            _context.Orders.Add(newOrder);


            var productsInCart = request.ClientCart.Products;
            newOrder.OrderDetails = new List<OrderDetail>();
            foreach (var product in productsInCart)
            {
                var newOrderDetails = new OrderDetail()
                {
                    OrderId = newOrder.Id,
                    Price = product.ProductPrice,
                    Quantity = product.Quantity,
                    ProductId = product.Id
                };

                newOrder.OrderDetails.Add(newOrderDetails);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var test = e;
                throw;
            }
            return newOrder.Id;
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

        public async Task<PageResponse<OrderViewModel>> GetAll(GetOrderRequest request, string status)
        {

            var query = from o in _context.Orders select o;

            var category = OrderCategorySearch.Categories.Where(x => x.Value == request.Type).FirstOrDefault();
            if (!String.IsNullOrEmpty(request.KeyWord) && category == null)
            {
                query = query.Where(x => x.Id.ToString() == request.KeyWord ||
                x.Id.ToString() == request.KeyWord ||
                x.ShipPhoneNumber.Contains(request.KeyWord) || x.ShipName.Contains(request.KeyWord));
            }

            if (category != null && request.KeyWord != null)
            {
                query = GetQueryOrders(category, query, request.KeyWord);
            }

            if (!String.IsNullOrEmpty(request.DateStart) && !String.IsNullOrEmpty(request.DateEnd))
            {
                query = query.Where(x => x.OrderDate >= Convert.ToDateTime(request.DateStart) && x.OrderDate <= Convert.ToDateTime(request.DateEnd));
            }


            switch (status)
            {
                case "Success":
                    query = query.Where(x => x.Status == OrderStatus.Success);
                    break;
                case "Canceled":
                    query = query.Where(x => x.Status == OrderStatus.Canceled);
                    break;
                case "InProgess":
                    query = query.Where(x => x.Status == OrderStatus.InProgress);
                    break;
                case "Shipping":
                    query = query.Where(x => x.Status == OrderStatus.Shipping);
                    break;
                default:
                    break;
            }
            query = query.OrderByDescending(x => x.Id);

            var pageIndex = request.PageIndex;
            var pageSize = request.PageSize;
            var count = await query.CountAsync();

            var orders = await query.Select(x => new OrderViewModel()
            {
                Id = x.Id,
                Price = x.Price,
                OrderDate = x.OrderDate,
                ShipAddress = x.ShipAddress,
                ShipEmail = x.ShipEmail ?? "",
                ShipName = x.ShipName,
                Status = x.Status,
                UserId = x.ClientId,
                ShipPhoneNumber = x.ShipPhoneNumber,
                CancelReason = x.CancelReason,
            }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();


            foreach (var item in orders)
            {
                var products = from o in orders
                               join od in _context.OrderDetails on o.Id equals od.OrderId
                               join p in _context.Products on od.ProductId equals p.Id
                               where o.Id == item.Id
                               select new { p, od };
                var productInOrders = products.Select(x => x.p).ToList();
                item.ProductQuantity = productInOrders.Count();
            }

            var pageResponse = new PageResponse<OrderViewModel>()
            {
                TotalRecords = count,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = orders
            };


            return pageResponse;
        }

        public async Task<PageResponse<OrderViewModel>> GetChart(OrderPagingRequest request)
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
            var pagedResult = new PageResponse<OrderViewModel>()
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
