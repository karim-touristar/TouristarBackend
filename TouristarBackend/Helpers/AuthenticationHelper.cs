using System.Security.Claims;

namespace TouristarBackend.Helpers;

public class AuthenticationHelper
{
    public static long GetUserId(HttpContext context)
    {
        var userContext = context.User;
        var id = userContext.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return int.Parse(id);
    }
}