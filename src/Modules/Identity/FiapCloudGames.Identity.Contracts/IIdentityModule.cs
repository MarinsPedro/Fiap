namespace FiapCloudGames.Identity.Contracts;

public interface IIdentityModule
{
    Task<UserSnapshot?> GetUserAsync(
        GetUserQuery query,
        CancellationToken cancellationToken);
}
