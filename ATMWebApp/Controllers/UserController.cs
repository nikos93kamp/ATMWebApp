using ATMWebApp.Data;
using ATMWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace ATMWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: User/Index
        public async Task<IActionResult> Index()
        {
            return _context.Users != null ?
                        View(await _context.Users.ToListAsync()) :
                        Problem("Entity set 'UserContext.Users'  is null.");
            //return View();
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,CardNumber,PIN,Balance,Transactions")] User user)
        {
            //ModelState["Transactions"]!.ValidationState = ModelValidationState.Valid;

            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.CardNumber == user.CardNumber) && _context.Users.Any(u => u.PIN == user.PIN))
                {
                    ModelState.AddModelError("CardNumber", "CardNumber already exists.");
                    ModelState.AddModelError("PIN", "PIN already exists.");
                    return View(user);
                }
                if (_context.Users.Any(u => u.CardNumber == user.CardNumber))
                {
                    ModelState.AddModelError("CardNumber", "CardNumber already exists.");
                    return View(user);
                }
                if (_context.Users.Any(u => u.PIN == user.PIN))
                {
                    ModelState.AddModelError("PIN", "PIN already exists.");
                    return View(user);
                }

                if (user.PIN != null)
                {
                    if (!Regex.IsMatch(user.PIN, @"^[0-9]+$"))
                    {
                        ModelState.AddModelError("PIN", "PIN must be an Integer");
                        return View(user);
                    }

                    if (user?.CardNumber?.Length != 16 && user?.PIN?.Length != 4)
                    {
                        ModelState.AddModelError("CardNumber", "Invalid Card Number!");
                        ModelState.AddModelError("PIN", "Invalid PIN!");
                        return View(user);
                    }

                    if (user?.CardNumber?.Length != 16)
                    {
                        ModelState.AddModelError("CardNumber", "Card Number must be a 16-digit number.");
                        return View(user);
                    }

                    if (user.PIN.Length != 4)
                    {
                        ModelState.AddModelError("PIN", "PIN must be a 4-digit number");
                        return View(user);
                    }

                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }                
  
            }
            //var errors = ModelState.Values.SelectMany(v => v.Errors);

            return View();
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //int userId = _context.Users.FirstOrDefault()?.Id ?? 0;

            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,CardNumber,PIN,Balance,Transactions")] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            //ModelState["Transactions"]!.ValidationState = ModelValidationState.Valid;


            if (ModelState.IsValid)
            {
                try
                {
                    if (user.PIN != null)
                    {
                        if (!Regex.IsMatch(user.PIN, @"^[0-9]+$"))
                        {
                            ModelState.AddModelError("PIN", "PIN must be an Integer");
                            return View(user);
                        }

                        if (user?.CardNumber?.Length != 16 && user?.PIN.Length != 4)
                        {
                            ModelState.AddModelError("CardNumber", "Invalid Card Number!");
                            ModelState.AddModelError("PIN", "Invalid PIN!");
                            return View(user);
                        }

                        if (user?.CardNumber?.Length != 16)
                        {
                            ModelState.AddModelError("CardNumber", "Card Number must be a 16-digit number.");
                            return View(user);
                        }

                        if (user.PIN.Length != 4)
                        {
                            ModelState.AddModelError("PIN", "PIN must be a 4-digit number");
                            return View(user);
                        }

                        _context.Update(user);
                        await _context.SaveChangesAsync();
                    }                  
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'UserContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
