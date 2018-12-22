 using System;

namespace uStora.Common.ViewModels
{
    public class RevenueStatistiByQuaterlyViewModel
    {
        public int Quarter { set; get; }
        public int Year { set; get; }
        public decimal Revenues { set; get; }
        public decimal Benefit { set; get; }
    }
}