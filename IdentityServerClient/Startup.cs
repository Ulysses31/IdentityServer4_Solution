using IdentityModel;
using IdentityServerClient.HttpHandlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityServerClient
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews()
				.AddJsonOptions(opts =>
					opts.JsonSerializerOptions.PropertyNamingPolicy = null
				);

			services.AddHttpContextAccessor();

			services.AddTransient<BearerTokenHandler>();

			// TODO: USER POLICIES

			services.AddAuthorization(options =>
			{
				// Full Access
				options.AddPolicy(
					"FullAccessPolicy",
					policy =>
					{
						policy.RequireAuthenticatedUser();
						policy.RequireClaim("read", "true");
						policy.RequireClaim("edit", "true");
						policy.RequireClaim("write", "true");
						policy.RequireClaim("delete", "true");
					});
				// Restricted Access
				options.AddPolicy(
					"RestrictedAccessPolicy",
					policy =>
					{
						policy.RequireAuthenticatedUser();
						policy.RequireClaim("read", "true");
					});
			});

			// create an HttpClient used for accessing the API
			services.AddHttpClient("APIClient", client =>
			{
				client.BaseAddress = new Uri("https://localhost:44366/");
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
			}).AddHttpMessageHandler<BearerTokenHandler>();

			// create an HttpClient used for accessing the IDP
			services.AddHttpClient("IDPClient", client =>
			{
				client.BaseAddress = new Uri("https://localhost:5001/");
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
			});

			services.AddAuthentication(options =>
			{
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
			})
				.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
				{
					options.AccessDeniedPath = "/Authorization/AccessDenied";
				})
				.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
				{
					options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					options.Authority = "https://localhost:5001/";
					options.ClientId = "imagegalleryclient";
					options.ClientSecret = "secret";
					options.ResponseType = "code";
					options.UsePkce = true;
					options.Scope.Add("openid");
					options.Scope.Add("profile");
					options.Scope.Add("email");
					options.Scope.Add("address");
					options.Scope.Add("offline_access");
					options.Scope.Add("roles");
					options.Scope.Add("read");
					options.Scope.Add("edit");
					options.Scope.Add("write");
					options.Scope.Add("delete");

					// API
					options.Scope.Add("imagegalleryapi");

					// remove a claim from claim identity
					//options.ClaimActions.Remove("nbf");
					options.ClaimActions.DeleteClaim("sid");
					options.ClaimActions.DeleteClaim("idp");
					options.ClaimActions.DeleteClaim("s_hash");
					options.ClaimActions.DeleteClaim("auth_time");
					options.ClaimActions.MapUniqueJsonKey("role", "role");
					options.ClaimActions.MapUniqueJsonKey("read", "read");
					options.ClaimActions.MapUniqueJsonKey("edit", "edit");
					options.ClaimActions.MapUniqueJsonKey("write", "write");
					options.ClaimActions.MapUniqueJsonKey("delete", "delete");

					options.SaveTokens = true;
					options.GetClaimsFromUserInfoEndpoint = true;
					// options.CallbackPath = new PathString("...");

					// this is for role authorization
					options.TokenValidationParameters = new TokenValidationParameters()
					{
						NameClaimType = JwtClaimTypes.GivenName,
						RoleClaimType = JwtClaimTypes.Role
					};
				});

		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
									name: "default",
									pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}