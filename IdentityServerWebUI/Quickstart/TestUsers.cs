// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel;
using IdentityServer4;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace IdentityServerHost.Quickstart.UI
{
	public class TestUsers
	{
		public static List<TestUser> Users
		{
			get
			{
				var address = new
				{
					street_address = "One Hacker Way",
					locality = "Heidelberg",
					postal_code = 69118,
					country = "Germany"
				};

				return new List<TestUser>
								{
										new TestUser
										{
												SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
												Username = "alice",
												Password = "alice",
												Claims = new List<Claim>()
												{
														new Claim(JwtClaimTypes.Role, "Administrator"),
														new Claim(JwtClaimTypes.Name, "Alice Smith"),
														new Claim(JwtClaimTypes.GivenName, "Alice"),
														new Claim(JwtClaimTypes.FamilyName, "Smith"),
														new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
														new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
														new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
														new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json),
														new Claim("read", "true"),
														new Claim("edit", "true"),
														new Claim("write", "true"),
														new Claim("delete", "true")
												}
										},
										new TestUser
										{
												SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
												Username = "bob",
												Password = "bob",
												Claims = new List<Claim>()
												{
														new Claim(JwtClaimTypes.Role, "User"),
														new Claim(JwtClaimTypes.Name, "Bob Smith"),
														new Claim(JwtClaimTypes.GivenName, "Bob"),
														new Claim(JwtClaimTypes.FamilyName, "Smith"),
														new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
														new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
														new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
														new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json),
														new Claim("read", "true"),
														new Claim("edit", "false"),
														new Claim("write", "false"),
														new Claim("delete", "false")
												}
										}
								};
			}
		}
	}
}