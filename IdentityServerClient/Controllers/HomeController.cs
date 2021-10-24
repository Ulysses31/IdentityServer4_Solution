using IdentityServerClient.Models;
using IdentityServerClient.ViewModels;
using IdentityServerModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IdentityServerClient.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<HomeController> _logger;

		public HomeController(
			IHttpClientFactory httpClientFactory,
			ILogger<HomeController> logger
		)
		{
			_httpClientFactory = httpClientFactory ??
					throw new ArgumentNullException(nameof(httpClientFactory));
			_logger = logger;
		}

		//[Authorize(Policy = "RestrictedAccessPolicy")]
		public async Task<IActionResult> Index()
		{
			await WriteOutIdentityInformation();

			return View();
		}

		public async Task<IActionResult> GetImages()
		{
			await WriteOutIdentityInformation();

			var httpClient = _httpClientFactory.CreateClient("APIClient");

			var request = new HttpRequestMessage(
					HttpMethod.Get,
					"/api/images/"
			);

			var response = await httpClient.SendAsync(
					request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

			if (response.IsSuccessStatusCode)
			{
				var responseStream = await response.Content.ReadAsStringAsync();

				var serializeOptions = new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};
				return View(new GalleryIndexViewModel(
					 JsonSerializer.Deserialize<List<Image>>(responseStream, serializeOptions)
				 ));
			}
			else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
							response.StatusCode == System.Net.HttpStatusCode.Forbidden)
			{
				return RedirectToAction("AccessDenied", "Authorization");
			}

			throw new Exception("Problem accessing the API");
		}

		[Authorize(Policy = "RestrictedAccessPolicy")]
		public IActionResult Privacy()
		{
			return View();
		}

		[Authorize(Policy = "FullAccessPolicy")]
		public IActionResult AddImage()
		{
			return View();
		}

		public async Task Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public async Task WriteOutIdentityInformation()
		{
			// get the saved token
			var identityToken = await HttpContext
				.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

			// write it out
			Debug.WriteLine($"Identity Token: {identityToken}");

			// write out the user claims
			foreach (var claim in User.Claims)
			{
				Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
			}
		}
	}
}