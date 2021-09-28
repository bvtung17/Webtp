using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Data.EF;
using Webthucpham.Data.Entities;
using Webthucpham.ViewModels.Catalog.Carts;
using Webthucpham.ViewModels.Common;

namespace Webthucpham.Application.Catalog.Carts
{
    public class CartService : ICartService
    {
        private readonly WebthucphamDbContext _context;
        public CartService(WebthucphamDbContext context)
        {
            _context = context;
        }
        public async Task<ClientCartViewModel> AddToCart(ClientCartViewModel request)
        {
            var cart = await _context.Carts.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (cart == null)
            {
                cart = new Cart()
                {
                    DateCreated = DateTime.Now,
                    Price = request.Products.Sum(x => x.ProductPrice * x.Quantity),
                    Quantity = request.Products.Count,
                    ClientId = request.ClientId
                };
                _context.Carts.Add(cart);
                foreach (var item in request.Products)
                {
                    var productIncart = new ProductInCart()
                    {
                        CartId = cart.Id,
                        ProductId = item.Id,
                        ProductName = item.ProductName,
                        ProductPrice = item.ProductPrice,
                        Quantity = item.Quantity
                    };
                    _context.ProductInCarts.Add(productIncart);
                }
            }
            else
            {
                cart.Quantity = request.Products.Count;
                cart.Price = request.Products.Sum(x => x.ProductPrice * x.Quantity);
                cart.ClientId = request.ClientId;
                foreach (var product in request.Products)
                {
                    var pic = await _context.ProductInCarts.Where(x => x.CartId == cart.Id && x.ProductId == product.Id).FirstOrDefaultAsync();
                    if (pic == null)
                    {
                        var newPic = new ProductInCart()
                        {
                            CartId = cart.Id,
                            ProductId = product.Id,
                            ProductPrice = product.ProductPrice,
                            ProductName = product.ProductName,
                            Quantity = product.Quantity
                        };
                        _context.ProductInCarts.Add(newPic);
                    }
                    else
                    {
                        pic.Quantity = product.Quantity;
                        _context.ProductInCarts.Attach(pic);
                    }
                }
                _context.Carts.Attach(cart);
            }
            await _context.SaveChangesAsync();
            request.Id = cart.Id;
            return request;

        }

        public async Task<ApiResult<ClientCartViewModel>> GetClientCart(Guid id)
        {
            var cart = await _context.Carts.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (cart == null)
            {
                return new ApiErrorResult<ClientCartViewModel>("Giỏ hàng không tồn tại");
            }
            var clientCartViewModel = new ClientCartViewModel()
            {
                Id = cart.Id,
                CartPrice = cart.Price,
                ClientId = cart.ClientId,
            };
            var productsInCart = await _context.ProductInCarts.Where(x => x.CartId == id).Select(p => new ProductInCartViewModel()
            {
                Id = p.ProductId,
                ProductName = p.ProductName,
                Quantity = p.Quantity,
                ProductPrice = p.ProductPrice,
            }).ToListAsync();
            foreach (var product in productsInCart)
            {
                var image = await _context.ProductImages.Where(x => x.ProductId == product.Id && x.IsDefault).FirstOrDefaultAsync();
                if (image != null)
                {
                    product.ProductImage = image.ImagePath;
                }
            }
            clientCartViewModel.Products = productsInCart;

            return new ApiSuccessResult<ClientCartViewModel>(clientCartViewModel);

        }

        public async Task<ApiResult<ClientCartViewModel>> RemoveProductInCart(DeleteProductInCartRequest request)
        {
            var requestCart = request.Cart;
            var product = requestCart.Products.Where(x => x.Id == request.ProductId).FirstOrDefault();
            var productInCart = await _context.ProductInCarts.Where(x => x.CartId == request.Cart.Id && x.ProductId == request.ProductId).FirstOrDefaultAsync();
            // handle remove
            _context.ProductInCarts.Remove(productInCart);
            requestCart.Products.Remove(product);
            // handle update cart
            var cart = await _context.Carts.Where(x => x.Id == requestCart.Id).FirstOrDefaultAsync();
            cart.Quantity = requestCart.Products.Count;
            cart.Price = requestCart.Products.Sum(x => x.Quantity * x.ProductPrice);
            _context.Carts.Attach(cart);
            await _context.SaveChangesAsync();

            requestCart.CartPrice = cart.Price;
            return new ApiSuccessResult<ClientCartViewModel>(requestCart);

        }

        public Task<ClientCartViewModel> UpdateCart(ClientCartViewModel request)
        {
            throw new NotImplementedException();
        }
    }
}
