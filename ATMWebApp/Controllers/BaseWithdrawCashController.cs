using ATMWebApp.Data;
using ATMWebApp.Models;
using ATMWebApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ATMWebApp.Controllers
{
    public abstract class BaseWithdrawCashController : Controller, IWithdrawCash, IFastWithdrawCash
    {
        protected readonly ApplicationDbContext _context;

        public BaseWithdrawCashController(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual IActionResult Withdraw()
        {
            return View();
        }

        [HttpPost]
        public virtual IActionResult Withdraw(decimal amount)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // User not logged in, redirect to login page
                return RedirectToAction("Login", "ATM");
            }

            var d_user = _context.Users.Include(u => u.Transactions).FirstOrDefault(u => u.ID == userId);

            if (d_user == null)
            {
                // User not found, redirect to login page
                return RedirectToAction("Login", "ATM");
            }

            var cashAvailability = _context.Users.FirstOrDefault(u => u.ID == userId)?.Balance ?? 0;

            if (cashAvailability == 0 || amount > d_user.Balance)
            {
                // Insufficient cash availability, show error message
                ModelState.AddModelError("Amount", "Insufficient cash availability");
                return View(amount);
            }

            if (d_user?.Transactions?.Count(t => t.Date.Date == DateTime.Today) >= 10)
            {
                // Transaction limit exceeded, show error message
                ModelState.AddModelError("Amount", "Transaction limit exceeded");
                return View(amount);
            }

            if (d_user != null)
            {
                // Update user balance and create transaction
                d_user.Balance -= amount;
                d_user?.Transactions?.Add(new Transaction { UserId = d_user.ID, Date = DateTime.Now, Amount = amount });

                _context.SaveChanges();

                return RedirectToAction("Options", "ATM");
            }

            throw new Exception("Error");
        }

        public virtual IActionResult FastWithdraw()
        {
            return View();
        }

        [HttpPost]
        public virtual IActionResult FastWithdraw(decimal amount)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // User not logged in, redirect to login page
                return RedirectToAction("Login", "ATM");
            }

            var d_user = _context.Users.Include(u => u.Transactions).FirstOrDefault(u => u.ID == userId);

            if (d_user == null)
            {
                // User not found, redirect to login page
                return RedirectToAction("Login", "ATM");
            }

            var cashAvailability = _context.Users.FirstOrDefault(u => u.ID == userId)?.Balance ?? 0;

            if (cashAvailability == 0 || amount > d_user.Balance)
            {
                // Insufficient cash availability, show error message
                ModelState.AddModelError("Amount", "Insufficient cash availability");
                return View(amount);
            }

            if (d_user?.Transactions?.Count(t => t.Date.Date == DateTime.Today) >= 10)
            {
                // Transaction limit exceeded, show error message
                ModelState.AddModelError("Amount", "Transaction limit exceeded");
                return View(amount);
            }

            if (d_user != null)
            {
                // Update user balance and create transaction
                d_user.Balance -= amount;
                d_user?.Transactions?.Add(new Transaction { UserId = d_user.ID, Date = DateTime.Now, Amount = amount });

                _context.SaveChanges();

                return RedirectToAction("Options", "ATM");
            }

            throw new Exception("Error");
        }
    }
}
