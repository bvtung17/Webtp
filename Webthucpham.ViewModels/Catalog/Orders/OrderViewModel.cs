using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Webthucpham.Data.Enums;


namespace Webthucpham.ViewModels.Catalog.Orders
{
    public class OrderViewModel
    {
        public int Id { set; get; }
        [DisplayName("Ngày tạo")]
        public DateTime OrderDate { set; get; }
        [DisplayName("SL sản phẩm")]
        public int ProductQuantity { get; set; }
        [DisplayName("Giá trị")]
        public decimal Price { get; set; }
        public List<OrderProductViewModel> OrderProducts { get; set; }
        public Guid? UserId { set; get; }
        [DisplayName("Người đặt")]
        public string UserNameOrder { get; set; }
        [DisplayName("Người nhận")]
        public string ShipName { set; get; }
        [DisplayName("Địa chỉ nhận")]
        public string ShipAddress { set; get; }
        [DisplayName("Email")]
        public string ShipEmail { set; get; }
        [DisplayName("Số điện thoại")]
        public string ShipPhoneNumber { set; get; }
        public string CancelReason { set; get; }

        [DisplayName("Trạng thái")]
        public OrderStatus Status { set; get; }
    }
}
