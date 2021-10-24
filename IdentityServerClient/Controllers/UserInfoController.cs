using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityServerClient.Controllers
{
	[Authorize(Policy = "RestrictedAccessPolicy")]
	public class UserInfoController : Controller
	{
		private IHttpClientFactory _httpClientFactory;

		public UserInfoController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory ??
					 throw new ArgumentNullException(nameof(httpClientFactory));
		}

		public async Task<IActionResult> Index()
		{
			ViewData["Title"] = "User Info";

			var idpClient = _httpClientFactory.CreateClient("IDPClient");

			var metaDataResource = await idpClient.GetDiscoveryDocumentAsync();

			if (metaDataResource.IsError)
			{
				throw new Exception(
					"Problem accessing the discovery endpoint.",
					metaDataResource.Exception
				);
			};

			var accessToken = await HttpContext.GetTokenAsync(
				OpenIdConnectParameterNames.AccessToken
			);

			var userInfoResponse = await idpClient.GetUserInfoAsync(
				new UserInfoRequest()
				{
					Address = metaDataResource.UserInfoEndpoint,
					Token = accessToken
				}
			);

			if (userInfoResponse.IsError)
			{
				throw new Exception(
					"Problem accessing the UserInfo endpoint.",
					userInfoResponse.Exception
				);
			};

			ViewData["userInfoResponceClaims"] = userInfoResponse.Claims;

			return View();
		}
	}
}