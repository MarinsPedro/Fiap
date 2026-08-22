namespace FiapCloudGames.Application.Common.Authentication;

public interface ICurrentUserContext
{
    Guid? UserId { get; }
}
