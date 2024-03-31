using Microsoft.AspNetCore.Mvc;
using ATMWebApp.Data;
using ATMWebApp.Models;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;


namespace ATMWebApp.Controllers
{
    public class ATMController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IMemoryCache _cache;

        public ATMController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(User user)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(user.CardNumber))
            {
                errors.Add("Please enter a CardNumber");
                return Json(new { success = errors.Count == 0, errors });
            }

            else if (string.IsNullOrEmpty(user.PIN))
            {
                errors.Add("Please enter a PIN");
                return Json(new { success = errors.Count == 0, errors });
            }

            if (!Regex.IsMatch(user.PIN, @"^[0-9]+$"))
            {
                errors.Add("PIN must be an Integer");
                return Json(new { success = errors.Count == 0, errors });
            }

            if (user.CardNumber.Length != 16 && user.PIN.Length != 4)
            {
                errors.Add("Invalid Card Number!");
                errors.Add("Invalid PIN!");
                return Json(new { success = errors.Count == 0, errors });
            }

            if (user.CardNumber.Length != 16)
            {
                errors.Add("Card Number must be a 16-digit number.");
                return Json(new { success = errors.Count == 0, errors });
            }

            if (user.PIN.Length != 4)
            {
                errors.Add("PIN must be a 4-digit number");
                return Json(new { success = errors.Count == 0, errors });
            }

            bool cardNumberExists = _context.Users.Any(u => u.CardNumber == user.CardNumber);
            bool pinExists = _context.Users.Any(u => Convert.ToInt32(u.PIN) == Convert.ToInt32(user.PIN));

            var logged_user = _context.Users.FirstOrDefault(u => u.CardNumber == user.CardNumber && u.PIN == user.PIN);

            if (logged_user == null && cardNumberExists && pinExists)
            {
                return Json(new { logged_user, cardNumberExists, pinExists });
            }

            if (!cardNumberExists && !pinExists)
            {
                return Json(new { cardNumberExists, pinExists });
            }

            if (!cardNumberExists)
            {
                return Json(new { cardNumberExists, pinExists });
            }
            if (!pinExists)
            {
                return Json(new { cardNumberExists, pinExists });
            }

            // Positive verification, store the user ID in session or authentication context
            // For example, using session:
            HttpContext.Session.SetInt32("UserId", logged_user.Id);

            return Json(new { success = true });
        }


        public IActionResult Options()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // User not logged in, redirect to login page
                return RedirectToAction("Login");
            }

            var user = _context.Users.Find(userId);

            return View(user);
        }

        public IActionResult CheckCashAvailability()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var cashAvailability = _context.Users.FirstOrDefault(u => u.Id == userId)?.Balance ?? 0;

            return View("CheckCashAvailability", cashAvailability);
        }

        public IActionResult TransactionHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // User not logged in, redirect to login page
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                // Retrieve the user's transaction history from the database
                ICollection<Transaction> transactions = _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

                return View("ShowTransactionHistory", transactions);
            }
            else
            {
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }


        public IActionResult ForgotPIN(int page = 1, int pageSize = 10)
        {
            List<User> cardNumbers = _context.Users.Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                CardNumber = u.CardNumber
            }).ToList();

            int totalItems = cardNumbers.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            cardNumbers = cardNumbers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CardNumbers = cardNumbers;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View();
        }

        //public IActionResult Search(string searchTerm)
        //{
        //    List<User> cardNumbers = _context.Users.Select(u => new User
        //    {
        //        Id = u.Id,
        //        Name = u.Name,
        //        CardNumber = u.CardNumber
        //    }).ToList(); // Retrieve data
        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        List<User> filteredUsers = cardNumbers.Where(u => u.Id.ToString().Contains(searchTerm) ||
        //    u.Name.Contains(searchTerm) || u.CardNumber.Contains(searchTerm)).ToList();
        //        ViewBag.CardNumbers = filteredUsers;

        //    }

        //    return View("ForgotPIN");
        //}


        public IActionResult ShowMessage(bool useCustomMessage = false)
        {
            string message = useCustomMessage ? "This pin has already been used before." : "This pin is already in use.";
            return View("Message", message);
        }


        [HttpPost]
        public IActionResult ForgotPIN(int? userId, string pin)
        {
            var errors = new List<string>();
            bool pinExists = _context.Users.Any(c => c.Id == userId && c.PIN == pin);

            if (string.IsNullOrEmpty(pin) && userId != null)
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("PIN", "You must enter a pin");

                    var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                            .Skip(1) // Skip the first error
                                            .Select(e => e.ErrorMessage)
                                            .ToList();

                    return Json(new { errors = errorMessages });
                }
            }

            if (!Regex.IsMatch(pin, @"^[0-9]+$"))
            {
                errors.Add("PIN is not integer");
                return Json(new { isValid = errors.Count == 0, errors });
            }
            
            if (pin.Length != 4 )
            {
                errors.Add("PIN must be a 4-digit number");
                return Json(new { isValid = errors.Count == 0, errors });
            }

            var card = _context.Users.FirstOrDefault(c => c.Id == userId);

            if (pinExists)
            {
                if (card.PIN == pin)
                {
                    bool isValid = (card != null && card.PIN == pin);
                    return Json(new { isValid });
                }
            }

            string cacheKey = $"PIN_{card.Id}";

            if (!_cache.TryGetValue(cacheKey, out string cachedPin))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Set expiration time

                _cache.Set(cacheKey, pin, cacheEntryOptions);

                if (card != null)
                {
                    card.PIN = pin;
                    _context.Update(card);
                    _context.SaveChanges();
                    bool isSuccess = (card != null && card.PIN == pin);
                    return Json(new { isSuccess });
                }
            }
            else
            {
                // Handle the scenario where PIN is already cached
                bool isInValid = (card != null || card.PIN == pin);
                return Json(new { isInValid });
            }

            //using (var transaction = _context.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        _cache.Set(cacheKey, pin, cacheEntryOptions);

            //        if (card != null && card.PIN != pin)
            //        {
            //            card.PIN = pin;
            //            _context.Update(card);
            //            _context.SaveChanges();

            //            transaction.Commit();
            //            return Json(new { success = true });
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        transaction.Rollback();
            //        return Json(new { error = ex.Message });
            //    }
            //}

            return Json(new { errors });
        }

        //[ActionName("ForgotPINWithoutUser")]
        [HttpPost]
        public IActionResult CheckPIN(string enteredPin)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(enteredPin))
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("PIN", "You must select a CardNumber");

                    var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                            //.Skip(1) // Skip the first error
                                            .Select(e => e.ErrorMessage)
                                            .ToList();

                    return Json(new { errors = errorMessages });
                }               
            }
            else if (ModelState.IsValid)
            {
                errors.Add("You must select a CardNumber");
                return Json(new { isInValid = true });
            }
            else
            {
                return Json(new { errors = true });
            }

            return Json(new { isValid = true });
        }
    }
}
