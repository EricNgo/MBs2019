using System.Web;

namespace uStora.Web.Infrastructure.Extensions
{
    public class StockStatusHelper
    {
        public enum StockStatus
        {
            ConHang,
            SapHetHang,
            HetHang
        }

        public static IHtmlString DisplayStatus(int status)
        {
            switch (status)
            {
                case 0:
                    string conhangHtml = $"<label class='text-success'> Còn hàng</label>";
                    return new HtmlString(conhangHtml);
                case 1:
                    string sapHetHangHtml = $"<label class='text-warning'> Sắp hết hàng</label>";
                    return new HtmlString(sapHetHangHtml); 
                case 2:
                    string hetHangHtml = $"<label class='text-danger'> Hết hàng</label>";
                    return new HtmlString(hetHangHtml);
            }
            return new HtmlString("Lỗi chuyển đổi trạng thái tồn kho!");
        }
    }
}