using ATMWebApp.Data;
using ATMWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ATMWebApp.Controllers
{
    public class CashController : BaseWithdrawCashController
    {
        public CashController(ApplicationDbContext context) : base(context) { }

        public override IActionResult Withdraw()
        {
            return View();
        }

        [HttpPost]
        public override IActionResult Withdraw(decimal amount)
        {
            return base.Withdraw(amount);
        }
    }
}
