using Microsoft.AspNetCore.Mvc;

namespace ATMWebApp.Repositories
{
    public interface IWithdrawCash
    {
        IActionResult Withdraw();
        IActionResult Withdraw(decimal amount);
    }
}
