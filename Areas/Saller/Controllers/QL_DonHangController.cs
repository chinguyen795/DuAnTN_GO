using Microsoft.AspNetCore.Mvc;

namespace DuAnTN.Areas.Seller.Controllers
{
    [Area("Saller")]
    public class QL_DonHangController : Controller
    {
        private readonly OrderService _orderService;

        public QL_DonHangController(OrderService orderService)
        {
            _orderService = orderService;
        }
        public async Task<IActionResult> Index()
        {
            var orderCounts = await _orderService.GetOrderCountsAsync();

            ViewBag.PendingOrders = orderCounts.Pending;
            ViewBag.ProcessedOrders = orderCounts.Processed;
            ViewBag.CanceledOrders = orderCounts.Canceled;

            return View();
        }

        /*public async Task<IActionResult> Index()
        {
            var orderCounts = await _orderService.GetOrderCountsAsync();
            var statistics = await _orderService.GetOrderStatisticsAsync();

            ViewBag.PendingOrders = orderCounts.Pending;
            ViewBag.ProcessedOrders = orderCounts.Processed;
            ViewBag.CanceledOrders = orderCounts.Canceled;
            ViewBag.Revenue = statistics.Revenue;
            ViewBag.TotalVisits = statistics.TotalVisits;
            ViewBag.TotalOrders = statistics.TotalOrders;

            return View();
        }*/
    }
}
