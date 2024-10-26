using GalliumPlus.Core.Users;

namespace GalliumPlus.Core.Applications;

public class SameSignOnTest
{
    [Fact]
    public void ConstructorExisting()
    {
        SameSignOn sso = new(
            id: 123,
            secret: "sso-secret",
            signatureMethod: SignatureMethod.HS256,
            scope: SameSignOnScope.Email,
            displayName: null,
            redirectUrl: "https://etiq-dijon.fr/",
            logoUrl: "https://etiq-dijon.fr/assets/images/logo.png"
        );

        Assert.Equal(123, sso.Id);
        Assert.Equal("sso-secret", sso.Secret);
        Assert.Equal(SignatureMethod.HS256, sso.SignatureMethod);
        Assert.Equal(SameSignOnScope.Email, sso.Scope);
        Assert.Null(sso.DisplayName);
        Assert.Equal("https://etiq-dijon.fr/", sso.RedirectUrl);
        Assert.Equal("https://etiq-dijon.fr/assets/images/logo.png", sso.LogoUrl);
        Assert.False(sso.RequiresApiKey);
    }

    [Fact]
    public void ConstructorNew()
    {
        SameSignOn sso = new(
            scope: SameSignOnScope.Email,
            redirectUrl: "https://etiq-dijon.fr/"
        );

        Assert.Equal("", sso.Secret);
        Assert.Equal(SignatureMethod.HS256, sso.SignatureMethod);
        Assert.Equal(SameSignOnScope.Email, sso.Scope);
        Assert.Null(sso.DisplayName);
        Assert.Equal("https://etiq-dijon.fr/", sso.RedirectUrl);
        Assert.Null(sso.LogoUrl);
        Assert.False(sso.RequiresApiKey);
    }

    [Fact]
    public void RegenerateSecret()
    {
        SameSignOn sso = new(
            scope: SameSignOnScope.Email,
            redirectUrl: "https://etiq-dijon.fr/"
        );
        
        // vide à la création
        Assert.Equal("", sso.Secret);
        
        // première génération (HS256)
        sso.GenerateNewSecret(SignatureMethod.HS256);
        Assert.Matches(ClientTest.ReSecret, sso.Secret);
        Assert.Equal(SignatureMethod.HS256, sso.SignatureMethod);

        // seconde génération (HS256)
        string previousSecret = sso.Secret;
        sso.GenerateNewSecret(SignatureMethod.HS256);
        Assert.NotEqual(previousSecret, sso.Secret);
    }
}