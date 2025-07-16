using System;

namespace TastyGo.Interfaces.Services
{

    public interface IUserContextService
    {
        bool IsAuthenticated { get; }

        Guid UserId { get; }
    }
}