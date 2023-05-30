﻿using DineView.Application.infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;

namespace DineView.Webapp.Services
{
    public class AuthService
    {
        private readonly bool _isDevelopment;
        private readonly DineContext _db;
        private readonly ICryptService _cryptService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(DineContext db, ICryptService cryptService, IHttpContextAccessor httpContextAccessor, bool isDevelopment)
        {
            _db = db;
            _cryptService = cryptService;
            _httpContextAccessor = httpContextAccessor;
            _isDevelopment = isDevelopment;
        }

        public HttpContext HttpContext => _httpContextAccessor.HttpContext
            ?? throw new NotSupportedException();

        public async Task<(bool success, string message)> TryLoginAsync(string username, string password)
        {
            var dbUser = _db.Users.FirstOrDefault(u => u.Username == username);
            if (dbUser is null)
            {
                return (false, "Unknown username or wrong password.");
            }

            var passwordHash = _cryptService.GenerateHash(dbUser.Salt, password);
            if (passwordHash != dbUser.PasswordHash)
            {
                return (false, "Unknown username or wrong password.");
            }

            var role = dbUser.Usertype.ToString();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                //new Claim("Userdata", JsonSerializer.Serialize(currentUser)),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);


            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3),
            };

            await HttpContext.SignInAsync(
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            return (true, string.Empty);
        }

        public bool IsAuthenticated => HttpContext.User.Identity?.Name != null;
        public string? Username => HttpContext.User.Identity?.Name;
        public bool HasRole(string role) => HttpContext.User.IsInRole(role);
        public bool IsAdmin => HttpContext.User.IsInRole(Application.models.Usertype.Admin.ToString());
        public Task LogoutAsync() => HttpContext.SignOutAsync();
    }
}
