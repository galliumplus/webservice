namespace CoreTest.Users
{
    public class UserTest
    {
        private readonly Role profRole = new Role(0, "Prof", Permissions.NONE);
        private readonly PasswordInformation password = PasswordInformation.FromPassword("motdepasse123");

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
    }
}
