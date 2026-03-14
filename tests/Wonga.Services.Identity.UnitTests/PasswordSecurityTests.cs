using Wonga.Services.Identity.Application;

namespace Wonga.Services.Identity.UnitTests;

public sealed class PasswordSecurityTests
{
    [Fact]
    public void HashPasswordAndVerifyPassword_ShouldReturnTrueForTheOriginalPassword()
    {
        var (hash, salt) = PasswordSecurity.HashPassword("Passw0rd!");

        var isValid = PasswordSecurity.VerifyPassword("Passw0rd!", hash, salt);

        Assert.True(isValid);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalseForDifferentPasswords()
    {
        var (hash, salt) = PasswordSecurity.HashPassword("Passw0rd!");

        var isValid = PasswordSecurity.VerifyPassword("DifferentPassw0rd!", hash, salt);

        Assert.False(isValid);
    }
}
