using System;

namespace uStora.Common.ViewModels
{
    public class RevenueStatisticViewModel
    {
        public DateTime Date { set; get; }
        public decimal Revenues { set; get; }
        public decimal Benefit { set; get; }

        public int Quarter { set; get; }

        public int Year { set; get; }
        public long ProductID { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public int ToTal { get; set; }
    }
}