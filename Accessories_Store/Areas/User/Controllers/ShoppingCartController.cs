using Accessories_Store.Areas.Admin.Services;
using Accessories_Store.Data_Access;
using Accessories_Store.Helpers;
using Accessories_Store.Models.Entities;
using Accessories_Store.Services;
using Accessories_Store.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Hangfire.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Word;
using System.Security.Claims;

namespace Accessories_Store.Areas.User.Controllers
{
	[Area("User")]
	
	public class ShoppingCartController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IProductVariantRepo _productVariantContext;
		private readonly IOrderRepo _orderContext;
		private readonly IProductRepo _productContext;
        private readonly IVoucherRepo _voucherContext;

        private readonly UserManager<ApplicationUser> _userManager;
		private readonly IVnPayService _vnPayService;
		public INotyfService _notifyService { get; }
		public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepo productContext,
			IProductVariantRepo productVariantContext, IOrderRepo orderContext,INotyfService notifyService, IVnPayService vnPayService, IVoucherRepo voucherContext)
		{
			_context = context;
			_productContext = productContext;
			_userManager = userManager;
			_productVariantContext = productVariantContext;
			_orderContext = orderContext;
			_voucherContext = voucherContext;
			_notifyService = notifyService;
			_vnPayService = vnPayService;
		}

		
		[Route("/cart/add")]
		public IActionResult AddToCart(int id, int quantity)
		{
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
			var productVariant =_productVariantContext.GetById(id);

			// 3. Find existing cart item for this product (if any)
			var existingCartItem = cart.Items.FirstOrDefault(item => item.ProductId == productVariant.Product.Id && item.ProductSize == productVariant.ProductSize);
			// 4. Handle different scenarios:
			if (existingCartItem == null)
			{
				// Product not in cart yet: Create a new CartItem
				int sumItem = cart.Items.Count();
				var cartItem = new CartItemVM
				{
					Id = sumItem++,
					Thumb = productVariant.Product.Thumb,
					ProductId = productVariant.Product.Id,
					Name = productVariant.Product.Name,
					Price = productVariant.Price,
					Quantity = quantity,
					TotalPrice = (productVariant.Price*quantity),
					ProductSize = productVariant.ProductSize,

				};
				cart.AddItem(cartItem);

			}
			else
			{
				existingCartItem.Quantity += quantity;
				// Check for potential overflow (quantity might become negative)
				if (existingCartItem.Quantity < 1)
				{
					existingCartItem.Quantity = 1; // Set minimum quantity to 1
				}
				cart.UpdateItem(existingCartItem);
			}
			HttpContext.Session.SetObjectAsJson("Cart", cart);
			_notifyService.Success("Thêm sản phẩm vào giỏ hàng thành công!");
			return Json(new { success = true });
		}

		[Route("/shopping-cart")]
		public IActionResult Index()
		{
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

			return View(cart);
		}
		// Các actions khác...
		
		[Route("/cart/remove")]
		public IActionResult RemoveFromCart(int id)
		{
			var cart =
		   HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
			if (cart is not null)
			{
				cart.RemoveItem(id);// Lưu lại giỏ hàng vào Session sau khi đã xóa mục
				HttpContext.Session.SetObjectAsJson("Cart", cart);
			}
			return Json(new { success = true });
		}

		[Route("/cart/remove-all")]
		public IActionResult RemoveAllFromCart()
		{
			var cart =
		   HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
			if (cart is not null)
			{
				cart.ClearCart();// Lưu lại giỏ hàng vào Session sau khi đã xóa mục
				HttpContext.Session.SetObjectAsJson("Cart", cart);
			}
			return Json(new { success = true });
		}
		[Route("/cart/update-cart-item")]
		public IActionResult UpdateCartItem(int id, int currQuantity)
		{
			var cart =
		   HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
			if (cart is not null)
			{
				var cartItem = cart.Items.FirstOrDefault(x => x.Id == id);
				cartItem.Quantity = currQuantity;
				cart.UpdateItem(cartItem);// Lưu lại giỏ hàng vào Session sau khi đã cập nhật
				HttpContext.Session.SetObjectAsJson("Cart", cart);
			}
			return Json(new { success = true });
		}
		[HttpGet]
		[Route("/cart/get-all-items")]
		public IActionResult GetAllItems()
		{
			// Lấy giỏ hàng từ Session
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

			// Kiểm tra xem giỏ hàng có tồn tại không
			if (cart != null)
			{
				// Trả về tất cả các mục trong giỏ hàng dưới dạng JSON
				return Json(new { success = true, items = cart.Items });
			}

			// Trả về một phản hồi JSON rỗng nếu không có giỏ hàng
			return Json(new { success = false });
		}


		[Route("/cart/check-out")]
		public IActionResult Checkout()
		{
			var user = _context.ApplicationUsers.FirstOrDefault(x=> x.Id == _userManager.GetUserId(User));
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
			if (cart != null)
			{
				// Access and process list items from cart.Items
				List<CartItemVM> cartItems = cart.Items;

				// Example: Iterate through cart items and display or process data
				foreach (var item in cartItems)
				{
					string? productName = item.Name;
					int? quantity = item.Quantity;
					double? price = item.Price;
					string thumb = item.Thumb;
					int? productSize = item.ProductSize;
					double? totalPrice = item.Price * item.Quantity;

					// ... (Process or display item data)
				}

				ViewBag.CartOrder = cartItems;
				double? discountValue = 0;
				discountValue = cart.DiscountValue;
				ViewBag.DiscountValue = discountValue;
			}
			
			return View(user);
		}

		[HttpPost]
		public async Task<IActionResult> CheckoutConfirmed(CheckOutVM model, string paymentMethod )
		{
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
			HttpContext.Session.SetObjectAsJson("CheckoutModel", model);
			try
			{
				var user = await _userManager.GetUserAsync(User);

				if(paymentMethod == PaymentType.VNPAY)
				{
					var vnPayModel = new VnPaymentRequestModel
					{
						Amount = (double)(cart.Items.Sum(x => x.TotalPrice) - cart.DiscountValue),
						CreatedDate = DateTime.Now,
						Description = $"{model.Name} {model.Phone}",
						FullName = model.Name,
					};
					return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
				}
				else if(paymentMethod == PaymentType.COD)
				{
                    var coupon = await _voucherContext.GetByVoucherAsync(cart.CouponCode);
                    coupon.DiscountUnit = coupon.DiscountUnit - 1;
                    // Tạo đơn hàng mới
                    Order order = new Order();
					order.ApplicationUser = user;
					order.Name = model.Name;
					order.Phone = model.Phone;
					order.Address = model.Address;

					order.CreatedAt = DateTime.Now;
					order.TotalPrice = cart.Items.Sum(i => i.TotalPrice);
					order.TotalQuantity = cart.Items.Sum(i => i.Quantity);
					order.OrderStatusPayment = PaymentType.StatusCOD;
					order.TotalDiscount = cart.DiscountValue;
					order.Status = Status.StatusNotConfirmed;

					foreach (var i in cart.Items)
					{
						OrderDetail orderDetail = new OrderDetail()
						{

							ProductId = i.ProductId,
							Quantity = i.Quantity,
							Price = i.Price,
							ProductSize = i.ProductSize,
							OrderId = order.Id,
						};
						order.OrderDetails.Add(orderDetail);

					}

					_context.Orders.Update(order);
					await _context.SaveChangesAsync();

					// Xóa giỏ hàng từ Session
					HttpContext.Session.Remove("Cart");
					HttpContext.Session.Remove("CheckoutModel");

					_notifyService.Success("Đặt hàng thành công!");
					return RedirectToAction("OrderCompleted", new { orderId = order.Id });

				}


			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi lưu dữ liệu: {ex.Message}");
			}
			return View(model);
		}

		[Route("/order/completed")]
		public IActionResult OrderCompleted(int orderId)
		{
			return View(orderId);
		}

		[Route("/vouchers/check-coupon")]
		public IActionResult CheckCoupon(string couponCode)
		{
			var coupon = _context.PaymentCoupons.Where(x => x.CouponCode == couponCode).FirstOrDefault();
			// Kiểm tra xem giỏ hàng có tồn tại không
			
			if (coupon != null && coupon.DiscountUnit >0)
			{
				// Lấy giỏ hàng từ Session
				var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

				if(cart != null)
				{
					if (cart.Items.Count() < coupon.MinimumOrder)
					{
						return Json(new { success = false, message = "Mã này không áp dụng cho đơn hàng của bạn!" });
					}
					// Tính toán subTotalPrice
					double? subTotalPrice = cart.Items.Sum(x => x.Price * x.Quantity);

					// Tính toán giá trị giảm giá dựa trên coupon
					double? discountValue;
					if ((coupon.DiscountValue * subTotalPrice) / 100 >= coupon.MaximumDiscount)
					{
						discountValue = coupon.MaximumDiscount;
					}
					else
					{
						discountValue = (coupon.DiscountValue * subTotalPrice) / 100;
					}

					// Lưu giá trị discountValue vào thuộc tính của giỏ hàng
					cart.DiscountValue = discountValue;
					cart.CouponCode = couponCode;
					// Lưu giỏ hàng được cập nhật vào Session
					HttpContext.Session.SetObjectAsJson("Cart", cart);

					// Trả về dữ liệu JSON kèm theo giá trị discountValue và subTotalPrice
					return Json(new { success = true, discountValue = discountValue, subTotalPrice = subTotalPrice });
                }
				return Json(new { success = false, message = "Chưa có sản phẩm nào trong giỏ hàng của bạn!" });
			}
			return Json(new { success = false, message = "Mã này không tồn tại!" });

		}

		public IActionResult PaymentFail()
		{
			return View();
		}


		[Route("/order/completed/payment-callback")]
		public async Task<IActionResult> PaymentCallBack()
		{
			var response = _vnPayService.PaymentExecute(Request.Query);
			if(response == null || response.VnPayResponseCode!= "00")
			{
				TempData["Message"] = $"Lỗi thanh toán VnPay:{response.VnPayResponseCode}";
				return RedirectToAction("PaymentFail");
			}
			var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
			var model = HttpContext.Session.GetObjectFromJson<CheckOutVM>("CheckoutModel");

			var coupon = await _voucherContext.GetByVoucherAsync(cart.CouponCode);
			coupon.DiscountUnit = coupon.DiscountUnit - 1;
            // Lưu đơn hàng vô db
            var user = await _userManager.GetUserAsync(User);
			var newOrder = new Order();

			newOrder.UserId = user?.Id; // Get user ID
			newOrder.Name = model.Name;
			newOrder.Phone = model.Phone;
			newOrder.Address = model.Address;
			newOrder.TotalQuantity = cart.Items.Sum(x => x.Quantity);
			newOrder.TotalPrice = cart.Items.Sum(x => x.TotalPrice);
			newOrder.TotalDiscount = cart.DiscountValue;
			newOrder.CreatedAt = DateTime.Now;
			newOrder.OrderStatusPayment = PaymentType.StatusVNPAY;
			newOrder.Status = Status.StatusNotConfirmed;
			
			await _orderContext.AddAsync(newOrder);

			foreach (var i in cart.Items)
			{
				OrderDetail orderDetail = new OrderDetail()
				{
					ProductId = i.ProductId,
					Quantity = i.Quantity,
					Price = i.Price,
					ProductSize = i.ProductSize,
					OrderId = newOrder.Id,
				};
				newOrder.OrderDetails.Add(orderDetail);

			}
			await _orderContext.UpdateAsync(newOrder);

			// Xóa giỏ hàng từ Session
			HttpContext.Session.Remove("Cart");
			HttpContext.Session.Remove("CheckoutModel");

			TempData["Message"] = $"Thanh toán VN Pay thành công";
			return RedirectToAction("OrderCompleted", new { orderId = newOrder.Id });
		}
	}
}
