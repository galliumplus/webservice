﻿using GalliumPlus.WebApi.Core.Exceptions;

namespace GalliumPlus.WebApi.Core.Users;

public class UserTest
{
    private readonly Role profRole = new Role(0, "Prof", Permissions.NONE);
    private readonly PasswordInformation password = PasswordInformation.FromPassword("motdepasse123");

    private string[] validIdList = new string[] { "am200927", "amdzznzs", "AM200927", "AMDzzNZs", "am200NDs" };
    private string[] invalidIdList = new string[] { "@me", "am_", "$Am", "_am200927" };

    [Fact]
    public void ConstructorWithoutPasswordInformation()
    {
            User user = new User("mmansouri", new UserIdentity("Mehdi", "Mansouri", "mehdi.mansouri@iut-dijon.u-bourgogne.fr", "PROF"), this.profRole, 21.30m, false);

            Assert.Equal("mmansouri", user.Id);
            Assert.Equal("Mehdi", user.Identity.FirstName);
            Assert.Equal("Mansouri", user.Identity.LastName);
            Assert.Equal(this.profRole, user.Role);
            Assert.Equal("PROF", user.Identity.Year);
            Assert.Equal(21.30m, user.Deposit);
            Assert.False(user.IsMember);
        }

    [Fact]
    public void ConstructorWithPasswordInformation()
    {
            User user = new User("mmansouri", new UserIdentity("Mehdi", "Mansouri", "mehdi.mansouri@iut-dijon.u-bourgogne.fr", "PROF"), this.profRole, 21.30m, false, this.password);

            Assert.Equal("mmansouri", user.Id);
            Assert.Equal("Mehdi", user.Identity.FirstName);
            Assert.Equal("Mansouri", user.Identity.LastName);
            Assert.Equal(this.profRole, user.Role);
            Assert.Equal("PROF", user.Identity.Year);
            Assert.Equal(21.30m, user.Deposit);
            Assert.False(user.IsMember);
            Assert.Equal(this.password, user.Password);
        }

    [Fact]
    public void InvalidItemExceptionThrownOnInvalidUserIdInConstructor()
    {
            Action userCreationWithPassword = () => new User("@mansouri", new UserIdentity("Mehdi", "Mansouri", "mehdi.mansouri@iut-dijon.u-bourgogne.fr", "PROF"), this.profRole, 21.30m, false, this.password);
            InvalidItemException exception = Assert.Throws<InvalidItemException>(userCreationWithPassword);

            Action userCreationWithoutPassword = () => new User("@mansouri", new UserIdentity("Mehdi", "Mansouri", "mehdi.mansouri@iut-dijon.u-bourgogne.fr", "PROF"), this.profRole, 21.30m, false);
            exception = Assert.Throws<InvalidItemException>(userCreationWithoutPassword);
        }

    [Fact]
    public void InvalidItemExceptionThrownOnInvalidUserIdInSetter()
    {
            User user = new(
                "mansouri",
                new UserIdentity("Mehdi", "Mansouri", "mehdi.mansouri@iut-dijon.u-bourgogne.fr", "PROF"),
                this.profRole, 21.30m, false,
                this.password
            );

            foreach (string id in this.validIdList)
            {
                try
                {
                    user.Id = id;
                }
                catch (InvalidItemException)
                {
                    Assert.Fail($"{id} est censé etre accepté par le setter de User.Id");
                }
            }

            foreach (string id in this.invalidIdList)
                Assert.Throws<InvalidItemException>(() => user.Id = id);

        }

}