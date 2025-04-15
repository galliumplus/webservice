using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Random;

namespace GalliumPlus.Core.Security;

public class OneTimeSecretTest
{
    [Fact]
    public void NeverGenerated()
    {
        var ots = new OneTimeSecret();

        Assert.Throws<BadOrEmptyCredentials>(() => ots.Hash);
        Assert.Throws<BadOrEmptyCredentials>(() => ots.Salt);
        Assert.Throws<BadOrEmptyCredentials>(() => ots.Match(""));

        string secret = ots.Regenerate();
        Assert.True(ots.Match(secret));
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