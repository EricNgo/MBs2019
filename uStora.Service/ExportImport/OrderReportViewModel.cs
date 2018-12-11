using System;

namespace uStora.Service.ExportImport
{
    public class OrderReportViewModel
    {
        public int ID { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAddress { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerMobile { get; set; }

        public string CustomerMessage { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string PaymentMethod { get; set; }

        public int PaymentStatus { get; set; }

        public bool Status { get; set; }

        public bool IsCancel { get; set; } = false;

        public string BankCode { get; set; }
    }
}
