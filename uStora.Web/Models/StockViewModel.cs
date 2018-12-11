namespace uStora.Web.Models
{
    public class StockViewModel
    {
        public long ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public int AdjustedQuantity { get; set; } = 0;

        public int StockStatus { get; set; }
    }
}