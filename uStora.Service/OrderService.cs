using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using uStora.Common.Services.Int32;
using uStora.Common.ViewModels;
using uStora.Data.Infrastructure;
using uStora.Data.Repositories;
using uStora.Model.Models;
using uStora.Service.ExportImport;

namespace uStora.Service
{
    public interface IOrderService : ICrudService<Order>, IGetDataService<Order>
    {
        Order Add(ref Order order, List<OrderDetail> orderDetails);
        Order FindWithRelationData(int id, bool related = false);
        IEnumerable<OrderClientViewModel> GetListOrders(string userId);
        IEnumerable<Order> GetUnCompletedOrder();
        void UpdateStatus(int orderId);
        Task GenerateXls(List<OrderReportViewModel> datasource, string filePath);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork,
            IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderDetailRepository = orderDetailRepository;
        }

        public Task GenerateXls(List<OrderReportViewModel> datasource, string filePath)
        {
            return Task.Run(() =>
            {
                using (ExcelPackage pck = new ExcelPackage(new FileInfo(filePath)))
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Order");
                    ws.Cells["A1"].LoadFromCollection(datasource, true, TableStyles.Medium18);
                    ws.Cells.AutoFitColumns();
                    var workSheet = ws.Workbook.Worksheets[1];
                    BindingFormatForExcel(workSheet, datasource);
                    pck.Save();
                }
            });
        }

        private void BindingFormatForExcel(ExcelWorksheet worksheet, List<OrderReportViewModel> listItems)
        {
            // Set default width cho tất cả column
            worksheet.DefaultColWidth = 10;
            // Tự động xuống hàng khi text quá dài
            worksheet.Cells.Style.WrapText = true;
            // Tạo header
            worksheet.Cells[1, 1].Value = "Mã đơn hàng";
            worksheet.Cells[1, 2].Value = "Khách hàng";
            worksheet.Cells[1, 3].Value = "Email";
            worksheet.Cells[1, 4].Value = "Địa chỉ";
            worksheet.Cells[1, 5].Value = "Điện thoại";
            worksheet.Cells[1, 6].Value = "Tài khoản đặt hàng";
            worksheet.Cells[1, 7].Value = "Phương thức thanh toán";
            worksheet.Cells[1, 8].Value = "Trạng thái đơn hàng";
            worksheet.Cells[1, 9].Value = "Ngày giao hàng";
            worksheet.Cells[1, 10].Value = "Lời nhắn";
            worksheet.Cells[1, 11].Value = "Mã Ngân hàng";
            worksheet.Cells[1, 12].Value = "Đơn hàng đã hủy";
            worksheet.Cells[1, 13].Value = "Trạng thái";

            using (var range = worksheet.Cells["A1:J1"])
            {
                // Canh giữa cho các text
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                // Set Font cho text  trong Range hiện tại
                range.Style.Font.SetFromFont(new Font("Arial", 10));
            }

            // Đỗ dữ liệu từ list vào 
            for (int i = 0; i < listItems.Count; i++)
            {
                var item = listItems[i];
                worksheet.Cells[i + 2, 1].Value = item.ID;
                worksheet.Cells[i + 2, 2].Value = item.CustomerName;
                worksheet.Cells[i + 2, 3].Value = item.CustomerEmail;
                worksheet.Cells[i + 2, 4].Value = item.CustomerAddress;
                worksheet.Cells[i + 2, 5].Value = item.CustomerMobile;
                worksheet.Cells[i + 2, 6].Value = item.CreatedBy;
                worksheet.Cells[i + 2, 7].Value = item.PaymentMethod;

                worksheet.Cells[i + 2, 8].Value = item.PaymentStatus;

                if (item.IsCancel)
                {
                    worksheet.Cells[i + 2, 8].Value = "Đã hủy";
                }
                else
                {
                    switch (item.PaymentStatus)
                    {
                        case 0: worksheet.Cells[i + 2, 8].Value = "Đang chờ duyệt"; break;
                        case 1: worksheet.Cells[i + 2, 8].Value = "Đang chuyển hàng"; break;
                        case 2: worksheet.Cells[i + 2, 8].Value = "Thành công"; break;
                    }
                }
                worksheet.Cells[i + 2, 9].Value = item.CreatedDate;
                worksheet.Cells[i + 2, 9].Style.Numberformat.Format = @"dd MMM yyyy hh:mm";
                worksheet.Cells[i + 2, 10].Value = item.CustomerMessage;
                worksheet.Cells[i + 2, 11].Value = item.BankCode;
                worksheet.Cells[i + 2, 12].Value = item.IsCancel ? "Đã hủy" : "Chưa hủy";

                worksheet.Cells[i + 2, 13].Value = item.Status ? "Hoạt động" : "Ngừng hoạt động";

            }
            worksheet.Cells["A3:D3"].Merge = true;
            worksheet.Cells["A3:D3"].Value = "Sản phẩm đã mua";
            worksheet.Cells["A3:D3"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells["A3:D3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A3:D3"].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
            // Tạo header
            worksheet.Cells[4, 1].Value = "Tên sản phẩm";
            worksheet.Cells[4, 2].Value = "Số lượng mua";
            worksheet.Cells[4, 3].Value = "Giá";
            worksheet.Cells[4, 4].Value = "Thành tiền";

            worksheet.Cells["A4:D4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells["A4:D4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            using (var range = worksheet.Cells["A4:D4"])
            {
                range.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
            }

            var currencyFormat = "#.###\" đ\"";
            var orderFromList = listItems.First();
            var order = _orderRepository.GetSingleByCondition(x=>x.ID == orderFromList.ID, new string[] { "OrderDetails" } );
            var orderDetail = order.OrderDetails.ToList();
            for (int i = 0; i < orderDetail.Count; i++)
            {
                var item = orderDetail[i];
                worksheet.Cells[i + 5, 1].Value = item.Product.Name;
                worksheet.Cells[i + 5, 2].Value = item.Quantity;
                worksheet.Cells[i + 5, 3].Value = item.Product.Price;
                worksheet.Cells[i + 5, 4].Value = item.Product.Price * item.Quantity;
                worksheet.Cells[i + 5, 4].Style.Numberformat.Format = currencyFormat;
                worksheet.Cells[i + 5, 3].Style.Numberformat.Format = currencyFormat;
            }

            // Thực hiện tính theo formula trong excel
            // Hàm Sum 
            worksheet.Cells[orderDetail.Count + 5, 3].Value = "Tổng tiền :";
            worksheet.Cells[orderDetail.Count + 5, 4].Formula = "SUM(D5:D" + (orderDetail.Count + 4) + ")";
            worksheet.Cells[orderDetail.Count + 5, 4].Style.Numberformat.Format = currencyFormat;

        }

        public Order Add(ref Order order, List<OrderDetail> orderDetails)
        {
            try
            {
                _orderRepository.Add(order);
                _unitOfWork.Commit();
                foreach (var orderDetail in orderDetails)
                {
                    orderDetail.OrderID = order.ID;
                    _orderDetailRepository.Add(orderDetail);
                }
                return order;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine(@"Entity of type ""{0}"" in state ""{1}"" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine(@"- Property: ""{0}"", Error: ""{1}""",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public IEnumerable<OrderClientViewModel> GetListOrders(string userId)
        {
            return _orderRepository.GetListOrder(userId).OrderBy(x => x.PaymentStatus);
        }

        public Order FindById(int id)
        {
            return _orderRepository.GetSingleById(id);
        }

        public IEnumerable<Order> GetAll(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return _orderRepository.GetAll();
            return _orderRepository.GetMulti(x => x.CustomerName.Contains(keyword));
        }

        public IEnumerable<Order> GetUnCompletedOrder()
        {
            return _orderRepository.GetMulti(x => x.PaymentStatus == 0);
        }

        public void Update(Order order)
        {
            _orderRepository.Update(order);
        }

        public Order Add(Order order)
        {
            return _orderRepository.Add(order);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public void Delete(int id)
        {
            _orderRepository.Delete(id);
        }
        public void UpdateStatus(int orderId)
        {
            var order = _orderRepository.GetSingleById(orderId);
            order.Status = true;
            _orderRepository.Update(order);
        }

        public Order FindWithRelationData(int id, bool related = false)
        {
            if (related) return _orderRepository
                    .GetSingleByCondition(x => x.ID == id,
                    new[] { "OrderDetails" });

            return _orderRepository.GetSingleById(id);
        }
    }
}