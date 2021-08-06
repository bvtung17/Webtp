using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.Application.Catalog.Products.Dtos;
using Webthucpham.Application.Dtos;
using Webthucpham.Data.EF;
using Webthucpham.Data.Entities;

namespace Webthucpham.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService // kế thừa
    {

        private readonly WebthucphamDbContext _context; //đọc
        public ManageProductService(WebthucphamDbContext context)
        {
            _context = context; //gán 1 lần
        }
        public async Task<int> Create(ProductCreateRequest request)
        {
            var product = new Product()
            {
                Price = request.Price,
            };
            _context.Products.Add(product);
            return await _context.SaveChangesAsync(); // giúp giảm thời gian chờ
        }

        public Task<int> Delete(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductViewModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<PagedViewModel<ProductViewModel>> GetAllPaging(string keyword, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<int> Update(ProductEditRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
