using BotDetect.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;
using System.Web.Mvc;
using uStora.Common;
using uStora.Model.Models;
using uStora.Service;
using uStora.Web.Infrastructure.NganLuongAPI.Card;
using uStora.Web.Models;

namespace uStora.Web.Controllers
{
    [Authorize]
    public class CoinController : Controller
    {
        private readonly string _merchantId = ConfigHelper.GetByKey("MerchantId");
        private readonly string _merchantPassword = ConfigHelper.GetByKey("MerchantPassword");
        private readonly string _merchantEmail = ConfigHelper.GetByKey("MerchantEmail");
        private readonly IApplicationUserService _applicationUserService;
        private readonly ITransactionHistoryService transactionHistoryService;

        public CoinController(IApplicationUserService applicationUserService, ITransactionHistoryService transactionHistoryService)
        {
            _applicationUserService = applicationUserService;
            this.transactionHistoryService = transactionHistoryService;
        }

        public ActionResult Deposit()
        {
            var viewModel = new CardsViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CaptchaValidation("CaptchaCodeCoin", "cardCaptcha", "Mã xác nhận không đúng")]
        public ActionResult Deposit(CardsViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var info = new RequestInfo
            {
                //Merchant site id
                Merchant_id = _merchantId,
                //Email tài khoản nhận tiền khi nạp
                Merchant_acount = _merchantEmail,
                //Mật khẩu giao tiếp khi đăng ký merchant site
                Merchant_password = _merchantPassword,

                //Nhà mạng
                CardType = viewModel.CardType,
                Pincard = viewModel.PinCard,

                //Mã đơn hàng
                Refcode = (new Random().Next(0, 10000)).ToString(),
                SerialCard = viewModel.SerialCard
            };

            var result = NLCardLib.CardChage(info);
            string html = "";
            var applyResult = new CardsViewModel();
            if (result.Errorcode.Equals("00"))
            {
                html += "<div>" + result.Message + "</div>";
                html += "<div>Số tiền nạp : <b>" + result.Card_amount + "</b> đ</div>";
                html += "<div>Mã giao dịch : <b>" + result.TransactionID + "</b></div>";
                html += "<div>Mã đơn hàng : <b>" + result.Refcode + "</b></div>";

                applyResult.Result.Message = html;
                applyResult.Result.Result = true;

                var coin = int.Parse(result.Card_amount);
                coin = (coin / 1000 * 8) / 10;
                _applicationUserService.UpdateCoin(User.Identity.GetUserId(), coin);
                _applicationUserService.SaveChanges();

                var transactionHistory = new TransactionHistory
                {
                    CardOwner = User.Identity.GetUserId(),
                    Description = $"Mã thẻ: {info.Pincard} | Seri thẻ: { info.SerialCard }",
                    Money = Decimal.Parse(result.Card_amount),
                    MoneyInSite = coin
                };

                this.transactionHistoryService.AddTransactionHistory(transactionHistory);
                _applicationUserService.SaveChanges();

                return View("Deposit", applyResult);
            }

            html += "<div>Nạp thẻ không thành công</div>";
            html += "<div>" + result.Message + "</div>";
            applyResult.Result.Message = html;
            applyResult.Result.Result = false;
            return View("Deposit", applyResult);

        }

    }

}