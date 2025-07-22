namespace IdentityTestConsole.Services;

public interface IApiTestService
{
    Task CallProtectedEndpointAsync(string accessToken);
}
