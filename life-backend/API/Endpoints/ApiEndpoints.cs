namespace API.Endpoints;

/// <summary>
/// Estructura de rutas del MVP. Cada grupo se llena en el ticket de su funcionalidad
/// (auth, perfil, hogares) sin tocar Program.cs.
/// </summary>
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
