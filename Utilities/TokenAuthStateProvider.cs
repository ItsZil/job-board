using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace job_board.Utilities
{
    public class TokenAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;

        public TokenAuthStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string> GetTokenAsync()
             => await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

        public async Task SetTokenAsync(string token, bool notifyStateChanged = true)
        {
            if (token == null || token == string.Empty)
            {
                await _jsRuntime.InvokeAsync<object>("localStorage.removeItem", "authToken");
            }
            else
            {
                await _jsRuntime.InvokeAsync<object>("localStorage.setItem", "authToken", token);
            }
            if (notifyStateChanged)
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await GetTokenAsync();
            var identity = string.IsNullOrEmpty(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
