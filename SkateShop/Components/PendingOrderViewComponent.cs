using Microsoft.AspNetCore.Mvc;
using SkateShop.Services;
using Microsoft.EntityFrameworkCore;


public class PendingOrderViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public PendingOrderViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        bool hasPending = await _context.Orders.AnyAsync(o => o.OrderStatus == "Pending");
        return View(hasPending);
    }
}
