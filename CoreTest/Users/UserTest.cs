﻿using System;
using GalliumPlus.WebApi.Core.Exceptions;

namespace CoreTest.Users
{
    public class UserTest
    {
        private readonly Role profRole = new Role(0, "Prof", Permissions.NONE);
        private readonly PasswordInformation password = PasswordInformation.FromPassword("motdepasse123");

        private string[] validIdList = new string[] { "am200927", "amdzznzs", "AM200927", "AMDzzNZs", "am200NDs" };
        private string[] unvalidIdList = new string[] { "@me", "am_", "$Am", "_am200927" };

        [Fact]
        public void ConstructorWithoutPasswordInformation()
        {
            User user = new User("mmansouri", "Mehdi Mansouri", profRole, "Prof", 21.30, false);

            Assert.Equal("mmansouri", user.Id);
            Assert.Equal("Mehdi Mansouri", user.Name);
            Assert.Equal(profRole, user.Role);
            Assert.Equal("Prof", user.Year);
            Assert.Equal(21.30, user.Deposit);
            Assert.False(user.IsMember);
        }

        [Fact]
        public void ConstructorWithPasswordInformation()
        {
            User user = new User("mmansouri", "Mehdi Mansouri", profRole, "Prof", 21.30, false, password);

            Assert.Equal("mmansouri", user.Id);
            Assert.Equal("Mehdi Mansouri", user.Name);
            Assert.Equal(profRole, user.Role);
            Assert.Equal("Prof", user.Year);
            Assert.Equal(21.30, user.Deposit);
            Assert.False(user.IsMember);
            Assert.Equal(password, user.Password);
        }

        [Fact]
        public void InvalidItemExceptionThrownOnInvalidUserIdInConstructor()
        {
            Action userCreationWithPassword = () => new User("@mansouri", "Mehdi Mansouri", profRole, "Prof", 21.30, false, password);
            InvalidItemException exception = Assert.Throws<InvalidItemException>(userCreationWithPassword);

            Action userCreationWithoutPassword = () => new User("@mansouri", "Mehdi Mansouri", profRole, "Prof", 21.30, false);
            exception = Assert.Throws<InvalidItemException>(userCreationWithoutPassword);
        }

        [Fact]
        public void InvalidItemExceptionThrownOnInvalidUserIdInSetter()
        {
            User user = new("mansouri", "Mehdi Mansouri", profRole, "Prof", 21.30, false, password);

            foreach (string id in validIdList)
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

            foreach (string id in unvalidIdList)
                Assert.Throws<InvalidItemException>(() => user.Id = id);

        }

    }
}
