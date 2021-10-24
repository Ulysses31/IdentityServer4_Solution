using Microsoft.AspNetCore.Mvc;

namespace IdentityServerClient.Controllers
{
	public class AuthorizationController : Controller
	{
		public IActionResult AccessDenied()
		{
			return View();
		}
	}
}