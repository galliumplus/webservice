using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Random;

namespace GalliumPlus.Core.Security;

public class OneTimeSecretTest
{
    [Fact]
    public void NeverGenerated()
    {
        var ots = new OneTimeSecret();

        Assert.Throws<NotBuildException>(() => ots.Hash);
        Assert.Throws<NotBuildException>(() => ots.Salt);
        Assert.Throws<NotBuildException>(() => ots.Match(""));

        string secret = ots.Regenerate();
        Assert.True(ots.Match(secret));
    }
}