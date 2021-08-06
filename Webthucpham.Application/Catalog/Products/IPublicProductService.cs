﻿using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Application.Catalog.Products.Dtos;
using Webthucpham.Application.Dtos;

namespace Webthucpham.Application.Catalog.Products
{
    public interface IPublicProductService //Công khai
    {
        PagedViewModel<ProductViewModel> GetAllByCategory(int categoryId,int pageIndex, int pageSize);
    }
}