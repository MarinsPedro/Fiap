namespace FiapCloudGames.Application.Common.Exceptions;

public sealed record AppError(
    string Message,
    string? Field = null);
