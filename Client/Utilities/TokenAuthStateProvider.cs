using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
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

        public async Task<string> GetTokenRawAsync()
        {
            string token = await GetTokenAsync();
            if (token != null && token != string.Empty)
            {
                JsonDocument jsonDocument = JsonDocument.Parse(token);
                JsonElement root = jsonDocument.RootElement;

                if (root.TryGetProperty("token", out JsonElement tokenElement))
                {
                    string rawToken = tokenElement.GetString();
                    return rawToken;
                }
            }
            return token;
        }

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
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            if (!string.IsNullOrEmpty(token))
            {
                IEnumerable<Claim> claims = ParseClaimsFromJwt(token);

                // Check if token is expired
                Claim expClaim = claims.FirstOrDefault(c => c.Type.Equals("exp"));
                if (expClaim != null)
                {
                    DateTime expDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToDouble(expClaim.Value));
                    if (expDate < DateTime.UtcNow)
                    {
                        await SetTokenAsync(string.Empty);
                    }
                    else
                    {
                        // Token is not expired
                        claimsIdentity = new ClaimsIdentity(claims, "jwt");
                    }
                }
                else
                {
                    // Expired
                    await SetTokenAsync(string.Empty);
                }
            }
            return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
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
