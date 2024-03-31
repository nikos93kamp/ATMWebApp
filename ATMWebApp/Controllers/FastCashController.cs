using ATMWebApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace ATMWebApp.Controllers
{
    public class FastCashController : BaseWithdrawCashController
    {
        public FastCashController(ApplicationDbContext context) : base(context) { }

        public override IActionResult FastWithdraw()
        {
            return View();
        }

        [HttpPost]
        public override IActionResult FastWithdraw(decimal amount)
        {
            return base.FastWithdraw(amount);
        }
    }
}
