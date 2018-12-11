using System;
using AutoMapper;
using Microsoft.AspNet.Identity;
using MvcPaging;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using uStora.Common;
using uStora.Model.Models;
using uStora.Service;
using uStora.Web.App_Start;
using uStora.Web.Infrastructure.Extensions;
using uStora.Web.Infrastructure.NganLuongAPI;
using uStora.Web.Models;
using static uStora.Web.Infrastructure.Extensions.StockStatusHelper;

namespace uStora.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IApplicationUserService _userManager;

        private readonly string _merchantId = ConfigHelper.GetByKey("MerchantId");
        private readonly string _merchantPassword = ConfigHelper.GetByKey("MerchantPassword");
        private readonly string _merchantEmail = ConfigHelper.GetByKey("MerchantEmail");


        public ShoppingCartController(IProductService productService,
            IApplicationUserService userManager, IOrderService orderService)
        {
            _productService = productService;
            _userManager = userManager;
            _orderService = orderService;
        }

        // GET: ShoppingCart
        public ActionResult Index(string searchString, int? page)
        {
            if (Session[CommonConstants.ShoppingCartSession] == null)
            {
                Session[CommonConstants.ShoppingCartSession] = new List<ShoppingCartViewModel>();
            }
            var cart = new ShoppingCartViewModel();
            int defaultPageSize = int.Parse(ConfigHelper.GetByKey("pageSizeAjax"));
            var common = new CommonController(_productService);
            int currentPageIndex = page ?? 1;
            cart.ListProducts = common.ProductListAjax(page, searchString).ToPagedList(currentPageIndex, defaultPageSize);
            if (Request.IsAjaxRequest())
                return PartialView("_AjaxProductList", cart.ListProducts);
            return View(cart);
        }
        [Authorize]
        public JsonResult GetUserInfo()
        {
            if (!Request.IsAuthenticated)
                return Json(new
                {
                    status = false,
                    message = "Bạn cần đăng nhập để sử dụng tính năng này!!!"
                });

            var userId = User.Identity.GetUserId();
            var user = _userManager.GetUserById(userId);
            return Json(new
            {
                data = user,
                status = true
            });
        }

        public JsonResult GetAll()
        {
            if (Session[CommonConstants.ShoppingCartSession] == null)
            {
                Session[CommonConstants.ShoppingCartSession] = new List<ShoppingCartViewModel>();
            }
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.ShoppingCartSession];

            return Json(new
            {
                status = true,
                data = cart
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Add(long productId)
        {
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.ShoppingCartSession];
            var product = _productService.FindById(productId);
            if (cart == null)
            {
                cart = new List<ShoppingCartViewModel>();
            }

            if (product.Quantity == 0 || product.StockStatus == (int)StockStatus.HetHang)
            {
                return Json(new
                {
                    status = false,
                    message = "Sản phẩm này hiện tại đang hết hàng."
                });
            }
            if (cart.Any(x => x.ProductId == productId))
            {
                foreach (var item in cart)
                {
                    if (item.ProductId == productId)
                        item.Quantity += 1;
                }
            }
            else
            {
                var newItem = new ShoppingCartViewModel
                {
                    ProductId = productId,
                    Product = Mapper.Map<Product, ProductViewModel>(product),
                    Quantity = 1
                };
                cart.Add(newItem);
            }
            Session[CommonConstants.ShoppingCartSession] = cart;
            Session[CommonConstants.SendCartSession] = cart;
            Session[CommonConstants.SelledProducts] = cart;
            return Json(new
            {
                status = true
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult Update(string cartData)
        {
            var cartViewModel = new JavaScriptSerializer().Deserialize<List<ShoppingCartViewModel>>(cartData);
            var cartSession = (List<ShoppingCartViewModel>)Session[CommonConstants.ShoppingCartSession];
            foreach (var item in cartSession)
            {
                foreach (var jitem in cartViewModel)
                {
                    if (item.ProductId == jitem.ProductId)
                    {
                        item.Quantity = jitem.Quantity;
                    }
                }
            }

            Session[CommonConstants.ShoppingCartSession] = cartSession;
            Session[CommonConstants.SendCartSession] = cartSession;

            return Json(new
            {
                status = true
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult DeleteItem(int productId)
        {
            var cartSession = (List<ShoppingCartViewModel>)Session[CommonConstants.ShoppingCartSession];
            if (cartSession == null)
                return Json(new
                {
                    status = false
                });
            cartSession.RemoveAll(x => x.ProductId == productId);
            return Json(new
            {
                status = true
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult DeleteAll()
        {
            DeleteCart();
            return Json(new
            {
                status = true
            });
        }
        
        [HttpPost]
        public ActionResult CreateOrder(string orderViewModel)
        {
            if (!Request.IsAuthenticated)
            {
                TempData["UnAuthenticated"] = "Bạn phải đăng nhập để thanh toán";
                return Json(new { status = false });
            }
            var order = new JavaScriptSerializer().Deserialize<OrderViewModel>(orderViewModel);
            var orderNew = new Order();
            bool isEnough = true;
            orderNew.UpdateOrder(order);
            if (Request.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                orderNew.CustomerId = userId;
                orderNew.CreatedBy = User.Identity.GetUserName();
            }
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.ShoppingCartSession];
            var orderDetails = new List<OrderDetail>();
            foreach (var item in cart)
            {
                if(_productService.CanPurchase(item.ProductId, item.Quantity))
                {
                    var detail = new OrderDetail
                    {
                        ProductID = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Product.Price
                    };
                    orderDetails.Add(detail);
                    isEnough = _productService.SellingProduct(item.ProductId, item.Quantity);
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        message = "Sản phẩm bạn chọn mua không đủ trong kho."
                    });
                }

            }
            if (!isEnough)
                return Json(new
                {
                    status = false,
                    message = "Sản phẩm này hiện tại đang hết hàng."
                });

            var orderReturn = _orderService.Add(ref orderNew, orderDetails);
            _orderService.SaveChanges();

            var totalAmount = orderDetails.Sum(x => x.Quantity * x.Price).ToString();
            Session["totalAmount"] = totalAmount;

            if (order.PaymentMethod == "CASH")
            {
                ApplySendHtmlOrder();
                return Json(new
                {
                    status = true
                });
            }


            var currentLink = ConfigHelper.GetByKey("CurrentLink");
            var info = new RequestInfo
            {
                Merchant_id = _merchantId,
                Merchant_password = _merchantPassword,
                Receiver_email = _merchantEmail,
                cur_code = "vnd",
                bank_code = order.BankCode,
                Order_code = orderReturn.ID.ToString(),
                Total_amount = totalAmount,
                fee_shipping = "0",
                Discount_amount = "0",
                order_description = "Thanh toán đơn hàng tại uStora shop",
                return_url = currentLink + "/xac-nhan-don-hang.htm",
                cancel_url = currentLink + "/huy-don-hang.htm",
                Buyer_fullname = order.CustomerName,
                Buyer_email = order.CustomerEmail,
                Buyer_mobile = order.CustomerMobile
            };

            Session["OrderId"] = orderReturn.ID;

            var objNlChecout = new APICheckout();
            var result = objNlChecout.GetUrlCheckout(info, order.PaymentMethod);
            if (result.Error_code == "00")
            {
                return Json(new
                {
                    status = true,
                    urlCheckout = result.Checkout_url,
                    message = result.Description
                });
            }
            return Json(new
            {
                status = false,
                message = result.Description
            });
        }

        [Authorize]
        public ActionResult CheckOutSuccess()
        {
            var orders = _orderService.GetListOrders(User.Identity.GetUserId());
            ViewBag.isNull = false;
            if (!orders.Any())
                ViewBag.isNull = true;
            return View(orders);
        }

        [Authorize]
        public JsonResult GetListOrder()
        {
            var userId = User.Identity.GetUserId();
            var orders = _orderService.GetListOrders(userId);

            return Json(new
            {
                data = orders,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        private void ApplySendHtmlOrder()
        {
            var items = (List<ShoppingCartViewModel>)Session[CommonConstants.SendCartSession];
            var htmlItems = "";

            foreach (var cart in items)
            {

                string url = "http://localhost:7493/";
                htmlItems += "<div style=\"width:100%; float: left\">" +
                             $"<img style=\"float: left\" src=\"{url + cart.Product.Image}\"/>" +
                             "<div style=\"float: left\">" +
                             $"<p><a href=\"{ url + "product/" + cart.Product.Alias + "-" + cart.Product.ID + ".htm" }\"> { cart.Product.Name }</a></p>" +
                             $"<p> Giá bán: { cart.Product.Price }</p>" +
                             $"<p> Số lượng: { cart.Quantity }</p>" +
                             "</div>" +
                             "</div>";
            }

            var totalAmount = Session["totalAmount"];
            decimal totalAmountCasted = decimal.Parse(totalAmount.ToString());
            string htmlContent = System.IO.File.ReadAllText(Server.MapPath("/Assets/client/templates/orderTemplate.html"));
            htmlContent = htmlContent.Replace("{{contentHtml}}", htmlItems);
            htmlContent = htmlContent.Replace("{{orderDate}}", DateTime.Today.ToShortDateString());
            htmlContent = htmlContent.Replace("{{total}}", totalAmountCasted.ToString("c0"));
            MailHelper.SendMail(_userManager.GetUserById(User.Identity.GetUserId()).Email, "Đơn hàng từ uStora.", htmlContent);
        }

        public ActionResult ConfirmOrder()
        {

            var token = Request["token"];
            var info = new RequestCheckOrder
            {
                Merchant_id = _merchantId,
                Merchant_password = _merchantPassword,
                Token = token
            };
            var objNlCheckout = new APICheckout();
            var result = objNlCheckout.GetTransactionDetail(info);
            if (result.errorCode == "00")
            {
                _orderService.UpdateStatus(int.Parse(result.order_code));
                ApplySendHtmlOrder();
                return RedirectToAction("CompleteOrder", new OrderResultViewModel { Result = true });
            }

            return RedirectToAction("CompleteOrder", new OrderResultViewModel { Result = false });
        }

        public ActionResult CompleteOrder(OrderResultViewModel viewModel)
        {
            Session["GoToQROrder"] = null;
            return View("CompleteOrder", viewModel);
        }

        public ActionResult CancelOrder()
        {
            var orderId = Session["OrderId"];
            var order = _orderService.FindById((int)orderId);
            Session["OrderId"] = null;
            order.IsCancel = true;
            _orderService.SaveChanges();
            Session["GoToQROrder"] = null;
            return View();
        }

        [Authorize]
        public ActionResult QrScanner()
        {
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.ShoppingCartSession];
            if (cart == null || cart.Count == 0)
                return RedirectToAction("Shop", "Product");
            Session["GoToQROrder"] = Guid.NewGuid().ToString();

            var orderDetails = new List<OrderDetail>();
            foreach (var item in cart)
            {
                var detail = new OrderDetail
                {
                    ProductID = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                };
                orderDetails.Add(detail);
            }
            var totalAmountSession = orderDetails.Sum(x => x.Quantity * x.Price);
           
            var totalAmountInt = decimal.ToInt32(totalAmountSession);
            var viewModel = new QrViewModel { QrCode = _userManager.GetUserById(User.Identity.GetUserId()).QrCode, IsEnoughCoin = totalAmountInt < _userManager.GetUserById(User.Identity.GetUserId()).Coin ? true : false };
            return View("QrScanner", viewModel);
        }

        public ActionResult QrOrder()
        {
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.ShoppingCartSession];
            var orderDetails = new List<OrderDetail>();

            if (Session["GoToQROrder"] == null || cart.Count == 0)
                return RedirectToAction("QrScanner");
            Session["GoToQROrder"] = null;
            var user = _userManager.GetUserById(User.Identity.GetUserId());

            var orderViewModel = new OrderViewModel
            {
                CustomerAddress = user.Address,
                CustomerEmail = user.Email,
                CustomerMobile = user.PhoneNumber,
                CustomerId = user.Id,
                CustomerMessage = "Thanh toán bằng QR Code",
                PaymentMethod = "QR",
                CustomerName = user.FullName,
                CreatedDate = DateTime.Now,
                CreatedBy = user.UserName,
                PaymentStatus = 0
            };
            if (orderViewModel.CustomerAddress == null) orderViewModel.CustomerAddress = "Địa chỉ";
            var order = new Order();
            bool isEnough = true;
            order.UpdateOrder(orderViewModel);

            foreach (var item in cart)
            {
                var detail = new OrderDetail
                {
                    ProductID = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                };
                orderDetails.Add(detail);
                isEnough = _productService.SellingProduct(item.ProductId, item.Quantity);
            }
            if (!isEnough)
                return RedirectToAction("CompleteOrder", new OrderResultViewModel { Result = false, Message = "Sản phẩm đã hết hàng" });

            var totalAmountSession = orderDetails.Sum(x => x.Quantity * x.Price);

            var totalAmountInt = decimal.ToInt32(totalAmountSession);

            var myAccount = _userManager.GetUserById(User.Identity.GetUserId());

            myAccount.Coin -= totalAmountInt;

            _orderService.Add(ref order, orderDetails);
            _orderService.SaveChanges();

            Session["totalAmount"] = orderDetails.Sum(x => x.Quantity * x.Price).ToString();

            ApplySendHtmlOrder();
            DeleteCart();
            return RedirectToAction("CompleteOrder", new OrderResultViewModel { Result = true });
        }

        private void DeleteCart() {
            Session[CommonConstants.ShoppingCartSession] = new List<ShoppingCartViewModel>();
        }
    }
}