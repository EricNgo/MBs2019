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
    }
}