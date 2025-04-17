using GalliumPlus.Core.Exceptions;

namespace GalliumPlus.Core.Security;

public class OneTimeSecretTest
{
    [Fact]
    public void NeverGenerated()
    {
        var ots = new OneTimeSecret();

        Assert.Throws<BlankSecretException>(() => ots.Hash);
        Assert.Throws<BlankSecretException>(() => ots.Salt);
        Assert.Throws<BlankSecretException>(() => ots.Match(""));
    }

    [Fact]
    public void Match()
    {
        var ots = new OneTimeSecret();
        
        string secret = ots.Regenerate();
        
        Assert.True(ots.Match(secret));
        Assert.False(ots.Match("prout"));
    }
}