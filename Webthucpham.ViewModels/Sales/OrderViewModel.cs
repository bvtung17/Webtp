using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Webthucpham.ViewModels.Sales
{
    public class OrderViewModel
    {
        [Display(Name = "Mã hóa đơn")]
        public int Id { set; get; }

        [Display(Name = "Ngày tạo")]
        public DateTime OrderDate { set; get; }

        [Display(Name = "Tên khách")]
        public string Name { get; set; }

        [Display(Name = "Đơn vị vận chuyển")]
        public string ShipName { set; get; }

        [Display(Name = "Địa chỉ")]
        public string ShipAddress { set; get; }

        [Display(Name = "Hòm thư")]
        public string ShipEmail { set; get; }

        [Display(Name = "Số điện thoại")]
        public string ShipPhoneNumber { set; get; }

        [Display(Name = "Trạng thái")]
        public int Status { set; get; }

        [Display(Name = "Tên sản phẩm")]
        public string Product { get; set; }

        [Display(Name = "Số lượng")]
        public int? Quantity { set; get; }

        [Display(Name = "Giá")]
        public decimal? Price { set; get; }
    }
}
