using Microsoft.AspNetCore.Mvc;

namespace ATMWebApp.Repositories
{
    public interface IFastWithdrawCash
    {
        IActionResult FastWithdraw();
        IActionResult FastWithdraw(decimal amount);
    }
}
