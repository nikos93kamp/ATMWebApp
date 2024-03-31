using ATMWebApp.Data;
using ATMWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ATMWebApp.Components
{
    [ViewComponent(Name = "GridView")]
    public class GridViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<User> cardNumbers)
        {
            return View(cardNumbers);
        }

    }
}
