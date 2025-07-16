using TastyGo.Helpers;
using TastyGo.Interfaces.Services;

internal sealed class UserContextService(IHttpContextAccessor httpContextAccessor)
    : IUserContextService
{
    public Guid UserId =>
        httpContextAccessor
            .HttpContext?
            .User
            .GetLoggedInUserId() ??
        throw new ApplicationException("User context is unavailable");

    public bool IsAuthenticated =>
        httpContextAccessor
            .HttpContext?
            .User
            .Identity?
            .IsAuthenticated ??
        throw new ApplicationException("User context is unavailable");
}