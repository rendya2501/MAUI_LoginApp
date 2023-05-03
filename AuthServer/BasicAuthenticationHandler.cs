using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;

namespace BasicAuthServer;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return await Task.FromResult(AuthenticateResult.NoResult());
    }

    public async Task<(bool, string)> AuthenticateUser(string authHeader)
    {
        if (AuthenticationHeaderValue.TryParse(authHeader, out var headerValue))
        {
            var credentialBytes = Convert.FromBase64String(headerValue.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            var username = credentials[0];
            var password = credentials[1];

            // ユーザー名とパスワードの確認
            if (username == "test" && password == "test")
            {
                return (true, username);
            }
        }

        return (false, null);
    }
}
