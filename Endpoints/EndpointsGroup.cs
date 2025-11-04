namespace UsersService.Endpoints;

public static class EndpointsGroup
{
    public static void RegisterUsersEndpoints(this WebApplication app)
    {
        var UsersGroup = app.MapGroup("api/v1/users");

        UsersGroup.MapPost("/register", UserRegister.Handler);

        UsersGroup.MapPost("/login", UserLogin.Handler);

        UsersGroup.MapGet("/validate", (Delegate)UserValidate.Handler);

        UsersGroup.MapPost("/refreshtoken", UserRefreshToken.Handler);

        app.MapFallback(async (context) =>
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("404 - Page Not Found");
        });

    }

}

