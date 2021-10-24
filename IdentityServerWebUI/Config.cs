// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServerWebUI
{
	public static class Config
	{
		public static IEnumerable<IdentityResource> IdentityResources =>
				new IdentityResource[]
				{
					new IdentityResources.OpenId(),
					new IdentityResources.Profile(),
					new IdentityResources.Email(),
					new IdentityResources.Address(),
					new IdentityResource(
						"roles",
						"Your role(s)",
						new List<string>() { "role" }
					),
					new IdentityResource(
						"read",
						"Read Access Policy",
						new List<string>() { "read" }
					),
					new IdentityResource(
						"edit",
						"Edit Access Policy",
						new List<string>() { "edit" }
					),
					new IdentityResource(
						"write",
						"Write Access Policy",
						new List<string>() { "write" }
					),
					new IdentityResource(
						"delete",
						"Delete Access Policy",
						new List<string>() { "delete" }
					)
				};

		public static IEnumerable<ApiScope> ApiScopes =>
				new ApiScope[]
				{
					new ApiScope("imagegalleryapi", "Image Gallery API Score")
				};

		public static IEnumerable<ApiResource> Apis =>
				new ApiResource[]
				{
					new ApiResource(
						"imagegalleryapi",
						"Image Gallery API",
						new List<string>() { "role" }
					)
					{
						ApiSecrets = { new Secret("apisecret".Sha256()) },
						Scopes = { "imagegalleryapi" }
					}
				};

		public static IEnumerable<Client> Clients =>
				new Client[]
				{
						// m2m client credentials flow client
						new Client
						{
								ClientId = "m2m.imagegalleryclient",
								ClientName = "Image Gallery API",
								AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

								ClientSecrets = {
									new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256())
								},

								// refresh token
								AllowOfflineAccess = true,

								AllowedScopes = {
									IdentityServerConstants.StandardScopes.OpenId,
									IdentityServerConstants.StandardScopes.Profile,
									IdentityServerConstants.StandardScopes.Email,
									IdentityServerConstants.StandardScopes.Address,
									"imagegalleryapi",
									"roles",
									"read",
									"edit",
									"write",
									"delete"
								},

								IncludeJwtId = false,
								AlwaysIncludeUserClaimsInIdToken = false,
								AlwaysSendClientClaims = false,
								UpdateAccessTokenClaimsOnRefresh = true,
								//IdentityTokenLifetime = 300,
								//AuthorizationCodeLifetime = 300,
								RefreshTokenExpiration = TokenExpiration.Sliding
								//SlidingRefreshTokenLifetime = 120,
								//AccessTokenLifetime = 120,
								//AccessTokenType = AccessTokenType.Reference
						},

						// interactive client using code flow + pkce
						new Client
						{
								ClientName = "Image Gallery",
								ClientId = "imagegalleryclient",
								ClientSecrets = { new Secret("secret".Sha256()) },
								AllowedGrantTypes = GrantTypes.Code,

								RequirePkce = true,
								RequireConsent = true,

								RedirectUris = { "https://localhost:44327/signin-oidc" },
								// FrontChannelLogoutUri = "https://localhost:44327/signout-oidc",
								PostLogoutRedirectUris = { "https://localhost:44327/signout-callback-oidc" },

								// refresh token
								AllowOfflineAccess = true,

								AllowedScopes = {
									IdentityServerConstants.StandardScopes.OpenId,
									IdentityServerConstants.StandardScopes.Profile,
									IdentityServerConstants.StandardScopes.Email,
									IdentityServerConstants.StandardScopes.Address,
									"imagegalleryapi",
									"roles",
									"read",
									"edit",
									"write",
									"delete"
								},

								UpdateAccessTokenClaimsOnRefresh = true,
								//IdentityTokenLifetime = 300,
								//AuthorizationCodeLifetime = 300,
								RefreshTokenExpiration = TokenExpiration.Sliding,
								//SlidingRefreshTokenLifetime = 120,
								//AccessTokenLifetime = 120,
								AccessTokenType = AccessTokenType.Reference
						},
				};
	}
}