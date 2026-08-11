namespace FiapCloudGames.Application.Common.Exceptions;

public enum AppErrorCategory
{
    Validation = 1,
    Authentication,
    Forbidden,
    NotFound,
    Conflict,
    BusinessRule
}
