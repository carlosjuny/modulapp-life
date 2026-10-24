using Infrastructure.Auth;

namespace Application.Tests.Auth;

public class AspNetPasswordHasherTests
{
    [Fact]
    public void Hash_ConUnaContrasena_NoGuardaElTextoPlano()
    {
        var hasher = new AspNetPasswordHasher();

        var hash = hasher.Hash("clave1234");

        Assert.DoesNotContain("clave1234", hash);
        Assert.True(hasher.Verify(hash, "clave1234"));
    }

    [Fact]
    public void Verify_ConContrasenaIncorrecta_DevuelveFalso()
    {
        var hasher = new AspNetPasswordHasher();
        var hash = hasher.Hash("clave1234");

        var result = hasher.Verify(hash, "incorrecta1");

        Assert.False(result);
    }
}
