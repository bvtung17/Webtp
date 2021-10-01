using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Data.EF;
using Webthucpham.Data.Entities;
using Webthucpham.Data.Enums;
using Webthucpham.Utilities.Exceptions;
using Webthucpham.ViewModels.Catalog.Orders;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Application.Catalog.Orders
{
    public class OrderService : IOrderService
    {
        private readonly WebthucphamDbContext _context;

        public OrderService(WebthucphamDbContext context)
        {
            _context = context;
        }
        private IQueryable<Order> GetQueryOrders(SelectListItem item, IQueryable<Order> query, string keyWord)
        {
            switch (item.Value)
            {
                case "id":
                    return query.Where(x => x.Id.ToString() == keyWord);
                case "phone":
                    return query.Where(x => x.ShipPhoneNumber.Contains(keyWord));
                case "shipname":
                    return query.Where(x => x.ShipName.Contains(keyWord));
                default:
                    return query;
            }
        }
        public async Task<PageResponse<OrderViewModel>> GetAll(GetOrderRequest request, string status)
        {
            var query = from o in _context.Orders
                        select o;
            var category = OrderCategorySearch.Categories.Where(x => x.Value == request.Type).FirstOrDefault();
            if (!String.IsNullOrEmpty(request.KeyWord) && category==null)
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
            
            var orders = await query.Select(x=> new OrderViewModel()
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

        public async Task<PageResponse<ClientOrderHistoryViewMode>> ClientGetOrderHistory(Guid clientId, GetOrderRequest request, string status)
        {

            var query = from o in _context.Orders select o;
            query = query.Where(x => x.ClientId == clientId);
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

            var orders = await query.Select(x => new ClientOrderHistoryViewMode()
            {
                Id = x.Id,
                ClientId = clientId,
                OrderDate = x.OrderDate,
                Total = x.Price,
                Status = x.Status
            }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            var pageResponse = new PageResponse<ClientOrderHistoryViewMode>()
            {
                TotalRecords = count,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = orders
            };


            return pageResponse;
        }
        public async Task<OrderViewModel> GetclientOrderDetails(Guid clientId, int id)
        {
            var query = from o in _context.Orders
                        join c in _context.Clients on o.ClientId equals clientId
                        where o.Id == id
                        select o;
            var order = await query.FirstOrDefaultAsync();
            if (order == null) return null;
            var productQuery = from od in _context.OrderDetails
                               where od.OrderId == id
                               join p in _context.Products on od.ProductId equals p.Id
                               select p;
            var user = await _context.Clients.Where(x => x.Id == clientId).FirstOrDefaultAsync();

            var orderViewModel = new OrderViewModel()
            {
                Id = id,
                OrderDate = order.OrderDate,
                Price = order.Price,
                ShipName = order.ShipName,
                ShipAddress = order.ShipAddress,
                ShipEmail = order.ShipEmail,
                Status = order.Status,
                UserNameOrder = user?.Name ?? order.ShipName,
                ShipPhoneNumber = order.ShipPhoneNumber,
                ProductQuantity = await productQuery.CountAsync(),
                CancelReason = order.CancelReason
            };
            return orderViewModel;
        }
        public async Task<OrderViewModel> GetById(int id)
        {
            var query = from o in _context.Orders where o.Id == id select o;
            var order = await query.FirstOrDefaultAsync();
            if (order == null) return null;
            var productQuery = from od in _context.OrderDetails
                               where od.OrderId == id
                               join p in _context.Products on od.ProductId equals p.Id
                               select p;
            var user = await _context.Clients.Where(x => x.Id == order.ClientId).FirstOrDefaultAsync();

            var orderViewModel = new OrderViewModel()
            {
                Id = id,
                OrderDate = order.OrderDate,
                Price = order.Price,
                ShipName = order.ShipName,
                ShipAddress = order.ShipAddress,
                ShipEmail = order.ShipEmail,
                Status = order.Status,
                UserNameOrder = user?.Name ?? order.ShipName,
                ShipPhoneNumber = order.ShipPhoneNumber,
                ProductQuantity = await productQuery.CountAsync(),
                CancelReason = order.CancelReason
            };
            return orderViewModel;
        }
        public async Task<List<OrderProductViewModel>> GetOrderProducts(int orderid)
        {
            var query = from od in _context.OrderDetails
                        where od.OrderId == orderid
                        join p in _context.Products on od.ProductId equals p.Id
                        join pi in _context.ProductImages on p.Id equals pi.ProductId
                        where pi.IsDefault
                        select new { p, od, pi };

            var products = await query.Select(x => new OrderProductViewModel()
            {
                Id = x.od.ProductId,
                ImagePath = x.pi.ImagePath,
                Name = x.p.Name,
                Price = x.od.Price,
                Quantity = x.od.Quantity
            }).ToListAsync();

            return products;
        }

        public async Task<bool> UpdateStatus(OrderViewModel request)
        {
            var order = await _context.Orders.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (order == null) return false;

            order.Status = request.Status;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<int> ClientCreateOrder(ClientCreateOrderViewModel request)
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

        public async Task<ApiResult<ClientOrderViewModel>> ClientGetOrder(Guid cartId, int orderId)
        {
            var order = await _context.Orders.Where(x => x.CartId == cartId && x.Id == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                return new ApiErrorResult<ClientOrderViewModel>("Không tồn tại");
            }
            var clientOrder = new ClientOrderViewModel()
            {
                Id = order.Id,
                Note = order.Note,
                ShipAddress = order.ShipAddress,
                ShipEmail = order.ShipEmail,
                ShipName = order.ShipName,
                ShipPhoneNumber = order.ShipPhoneNumber,
                Status = order.Status,
                TotalPrice = order.Price
            };
            return new ApiSuccessResult<ClientOrderViewModel>(clientOrder);
        }

        /*public async Task<PageResponse<ClientOrderHistoryViewMode>> ClientGetOrderHistory(Guid clientId)
        {
            List<ClientOrderHistoryViewMode> orders = new List<ClientOrderHistoryViewMode>();

            var clientOrders = await _context.Orders.Where(x => x.ClientId == clientId).OrderByDescending(x => x.Id).ToListAsync();
            foreach (var item in clientOrders)
            {
                var order = new ClientOrderHistoryViewMode()
                {
                    Id = item.Id,
                    ClientId = clientId,
                    OrderDate = item.OrderDate,
                    Status = item.Status,
                    Total = item.Price
                };
                orders.Add(order);
            }
            return orders;
        }*/

        public async Task<ApiResult<bool>> ClientCancelOrder(int orderId, string reason)
        {
            var order = await _context.Orders.Where(x => x.Id == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                return new ApiErrorResult<bool>("Không tìm thấy order");
            }

            order.CancelReason = reason;
            order.Status = OrderStatus.Canceled;
            _context.Orders.Attach(order);

            await _context.SaveChangesAsync();

            return new ApiSuccessResult<bool>()
            {
                Message = "Hủy đơn hàng thành công!"
            };
        }


    }
}
