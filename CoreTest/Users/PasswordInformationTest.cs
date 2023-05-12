using GalliumPlus.WebApi.Core.Users;
using Xunit;

namespace CoreTest.Users
{
    public class PasswordInformationTest
    {
        [Fact]
        public void ConstructorTest()
        {
            var password = new PasswordInformation(
                new byte[5] {0x12, 0x34, 0x56, 0x78, 0x90},
                "abcdefghijklmnopqrstuvwxyz"
            );

            Assert.Equal(5, password.Hash.Length);
            Assert.Equal(0x12, password.Hash[0]);
            Assert.Equal(0x34, password.Hash[1]);
            Assert.Equal(0x56, password.Hash[2]);
            Assert.Equal(0x78, password.Hash[3]);
            Assert.Equal(0x90, password.Hash[4]);
            Assert.Equal("abcdefghijklmnopqrstuvwxyz", password.Salt);
        }

        [Fact]
        public void FromPasswordTest()
        {
            var password1 = PasswordInformation.FromPassword("eticlubdufoot");
            var password2 = PasswordInformation.FromPassword("eticlubdufoot");

            Assert.NotEqual(password1.Salt, password2.Salt);

            // Les deux hashs ne sont pas identiques car ils sont salés
            Assert.NotEqual(password1.Hash, password2.Hash);
        }

        [Fact]
        public void MatchTest()
        {
            var password = PasswordInformation.FromPassword("eticlubdufoot");

            Assert.True(password.Match("eticlubdufoot"));
            Assert.False(password.Match("eticlubdefoot"));
        }
    }
}
