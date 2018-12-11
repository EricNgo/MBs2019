using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace uStora.Web.Models
{
    public class CoinViewModel
    {
        public int Coin { get; set; }
    }

    public class CardType
    {
        public string Text { get; set; }

        public string Value { get; set; }
    }
    public class CardsViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn nhà mạng")]
        public string CardType { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã thẻ cào")]
        public string PinCard { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã seri")]
        public string SerialCard { get; set; }

        public SelectList CardTypeList { get; set; }

        public OrderResultViewModel Result { get; set; }

        public CardsViewModel()
        {
            Result = new OrderResultViewModel();
            var CardTypeListLocal = new List<CardType>
            {
                new CardType { Text="Viettel", Value="VIETTEL" },
                new CardType { Text="Vinaphone", Value="VNP"},
                new CardType { Text="Mobiphone", Value="VMS" },
                new CardType { Text="Vcoin", Value="VCOIN" },
                new CardType { Text="Gate", Value="GATE" },
            };

            CardTypeList = new SelectList(CardTypeListLocal, "Value", "Text");
        }
    }
}