using System.Reflection;
using FiapCloudGames.Api.IntegrationTests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FiapCloudGames.Api.IntegrationTests.Contracts;

public sealed class ControllerContractTests :
    IClassFixture<FiapCloudGamesApiFactory>
{
    private readonly IReadOnlyList<ControllerActionDescriptor> _actions;

    public ControllerContractTests(FiapCloudGamesApiFactory factory)
    {
        _actions = factory.Services
            .GetRequiredService<IActionDescriptorCollectionProvider>()
            .ActionDescriptors.Items
            .OfType<ControllerActionDescriptor>()
            .Where(action =>
                action.ControllerTypeInfo.Assembly.GetName().Name?.EndsWith(
                    ".Presentation",
                    StringComparison.Ordinal) == true)
            .ToArray();
    }

    [Fact]
    public void AllControllerActions_ShouldDeclareInternalServerError()
    {
        Assert.NotEmpty(_actions);

        foreach (var action in _actions)
        {
            AssertDeclaresStatus(
                action,
                StatusCodes.Status500InternalServerError);
        }
    }

    [Fact]
    public void ProtectedControllerActions_ShouldDeclareUnauthorized()
    {
        var protectedActions = _actions.Where(IsProtected).ToArray();
        Assert.NotEmpty(protectedActions);

        foreach (var action in protectedActions)
        {
            AssertDeclaresStatus(action, StatusCodes.Status401Unauthorized);
        }
    }

    [Fact]
    public void AdministratorControllerActions_ShouldDeclareForbidden()
    {
        var administratorActions = _actions
            .Where(RequiresAdministrator)
            .ToArray();
        Assert.NotEmpty(administratorActions);

        foreach (var action in administratorActions)
        {
            AssertDeclaresStatus(action, StatusCodes.Status403Forbidden);
        }
    }

    private static bool IsProtected(ControllerActionDescriptor action) =>
        !GetAttributes<AllowAnonymousAttribute>(action).Any() &&
        GetAttributes<AuthorizeAttribute>(action).Any();

    private static bool RequiresAdministrator(ControllerActionDescriptor action) =>
        GetAttributes<AuthorizeAttribute>(action)
            .SelectMany(attribute =>
                (attribute.Roles ?? string.Empty).Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries))
            .Contains("Administrator", StringComparer.Ordinal);

    private static IEnumerable<TAttribute> GetAttributes<TAttribute>(
        ControllerActionDescriptor action)
        where TAttribute : Attribute =>
        action.ControllerTypeInfo
            .GetCustomAttributes<TAttribute>(inherit: true)
            .Concat(action.MethodInfo.GetCustomAttributes<TAttribute>(
                inherit: true));

    private static void AssertDeclaresStatus(
        ControllerActionDescriptor action,
        int expectedStatus)
    {
        var declaredStatuses = action.MethodInfo
            .GetCustomAttributes<ProducesResponseTypeAttribute>(inherit: true)
            .Select(attribute => attribute.StatusCode)
            .ToArray();

        Assert.Contains(
            expectedStatus,
            declaredStatuses);
    }
}
