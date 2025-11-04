using Microsoft.AspNetCore.Http.HttpResults;

namespace UsersService.Endpoints;

public class UserValidate
{
    public static Results<Ok, UnauthorizedHttpResult> Handler(HttpContext context)
    {
        int? UserId = (int?)context.Items["UserId"];

        if ((UserId is not null) && (UserId > 0))
        {
            Console.WriteLine("Validated UserId: " + UserId);
            return TypedResults.Ok();
        }
        else
        {
            Console.WriteLine("No valid UserId present in request or invalid format/value");
            return TypedResults.Unauthorized();

        }

    }
}
