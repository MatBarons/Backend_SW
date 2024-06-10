using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Academy.Api.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Academy.Api.Web.Handlers;

/// <summary>
/// Implements HTTP Basic Authentication
/// </summary>
public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
    private ILogger<BasicAuthenticationHandler> logger;

    public BasicAuthenticationHandler(
        IOptionsMonitor<BasicAuthenticationOptions> options,
        ILoggerFactory logger,
        ILogger<BasicAuthenticationHandler> l,
        UrlEncoder encoder,
        ISystemClock clock
        ) : base(options, logger, encoder, clock)
    {
        this.logger = l;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            if (!Request.Headers.ContainsKey(Constants.HeaderAuthorizationKey))
            {
                logger.LogDebug("No authentication header found");
                return AuthenticateResult.Fail("Unauthorized - credentials must be specified in " + Constants.HeaderAuthorizationKey + " header");
            }

            string credentials = Request.Headers[Constants.HeaderAuthorizationKey].First();
            if (string.IsNullOrWhiteSpace(credentials))
            {
                logger.LogDebug(
                    "Empty authentication header, passthrough to the controller for authorization requirements");
                return AuthenticateResult.NoResult();
            }
            else
            {
                string decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(credentials));
                string username,password;
                if (decodedAuthenticationToken.Contains(':'))
                {
                    //Convert the string into an string array
                    string[] usernamePasswordArray = decodedAuthenticationToken.Split(':');
                    //First element of the array is the username
                    username = usernamePasswordArray[0];
                    //Second element of the array is the password
                    password = usernamePasswordArray[1];
                }
                else
                {
                    return AuthenticateResult.Fail("Bad formatted " + Constants.HeaderAuthorizationKey + " header for HTTP Basic Authentication");
                }
                
                //TODO validate username and password with project specific logic and remove the following Fail!
                AuthenticateResult.Fail("Basic Authentication is missing validation logic");
                
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    //new Claim(ClaimTypes.GivenName, userSessionDto.FirstName),
                    //new Claim(ClaimTypes.Surname, userSessionDto.LastName),
                    //new Claim(ClaimTypes.Email, userSessionDto.Email),
                    //new Claim(ClaimTypes.Role, userSessionDto.CanAccessAdminArea ? "Admin" : "User")
                };

                var identity = new ClaimsIdentity(claims, "Basic");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                logger.LogDebug("User {0} successfully authenticated", credentials);
                return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during authentication");
            return AuthenticateResult.Fail("Error during authentication: " + ex.Message);
        }
    }
}
