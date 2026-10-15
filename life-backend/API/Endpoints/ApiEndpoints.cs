namespace API.Endpoints;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/auth").WithTags("Auth");
        app.MapGroup("/me").WithTags("Me");
        app.MapGroup("/households").WithTags("Households");

        return app;
    }
}
