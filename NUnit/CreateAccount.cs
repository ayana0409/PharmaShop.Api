using NUnitTest.Configuration;
using PharmaShop.Application.Models.Request.Account;
using Microsoft.EntityFrameworkCore;

namespace NUnitTest
{
    [TestFixture]
    public class CreateAccount
    {
        private AccountServiceSetup _setup;

        [SetUp]
        public async Task SetUp()
        {
            _setup = new AccountServiceSetup();
            await _setup.InitializeAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _setup.Cleanup();
            _setup.Dispose();
        }

        // Test case 1: Tạo tài khoản thành công
        [Test]
        public async Task CreateAsync_Should_Create_Account_Successfully()
        {
            var accountRequest = new AccountRequest
            {
                Username = "newUser",
                Name = "New User",
                Email = "newuser@example.com",
                Phone = "123456789",
                NewPassword = "Test@1234",
                Roles = ["User"]
            };

            await _setup.AccountService.CreateAsync(accountRequest);

            var createdAccount = await _setup.Context.Users
                .SingleAsync(p => p.UserName == "newUser");
            Assert.That(createdAccount, Is.Not.Null);
            Assert.That(createdAccount.UserName, Is.EqualTo(accountRequest.Username));
            Assert.That(createdAccount.Email, Is.EqualTo(accountRequest.Email));
            Assert.That(createdAccount.PhoneNumber, Is.EqualTo(accountRequest.Phone));
        }

        // Test case 2: Ném lỗi khi tên người dùng đã tồn tại
        [Test]
        public void CreateAsync_ShouldThrowInvalidOperationException_WhenUserNameExists()
        {
            var accountRequest = new AccountRequest
            {
                Username = "TestUser",
                Name = "Existing User",
                Email = "existinguser@example.com",
                Phone = "987654321",
                NewPassword = "Test@1234"
            };

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _setup.AccountService.CreateAsync(accountRequest));
            Assert.That(ex.Message, Is.EqualTo($"Username '{accountRequest.Username}' is already taken."));
        }

        // Test case 3: Ném lỗi khi mật khẩu không hợp lệ
        [Test]
        [TestCase("", "Passwords must be at least 6 characters.")]
        [TestCase("1@aA", "Passwords must be at least 6 characters.")]
        [TestCase("1234567", "Passwords must have at least one non alphanumeric character.")]
        [TestCase("1234567@", "Passwords must have at least one lowercase ('a'-'z').")]
        [TestCase("1234567@a", "Passwords must have at least one uppercase ('A'-'Z').")]
        [TestCase("@#$%^&*", "Passwords must have at least one digit ('0'-'9').")]
        public void CreateAsync_ShouldThrowArgumentException_WhenPasswordIsInvalid(string password, string errorMessage)
        {
            var accountRequest = new AccountRequest
            {
                Username = "newUser2",
                Name = "New User 2",
                Email = "newuser2@example.com",
                Phone = "1122334455",
                NewPassword = password,
                Roles = new List<string> { "User" }
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _setup.AccountService.CreateAsync(accountRequest));
            Assert.That(ex.Message, Is.EqualTo(errorMessage));
        }

        // Test case 4: Ném lỗi khi không thể thêm vai trò
        [Test]
        public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenCannotAddRoles()
        {
            string roleName = "NonExistingRole";
            var accountRequest = new AccountRequest
            {
                Username = "newUser3",
                Name = "New User 3",
                Email = "newuser3@example.com",
                Phone = "123456789",
                NewPassword = "Test@1234",
                Roles = [roleName]
            };

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _setup.AccountService.CreateAsync(accountRequest));
            Assert.That(ex.Message, Is.EqualTo($"Role {roleName.ToUpper()} does not exist."));
        }

        // Test case 5: Ném lỗi khi email không hợp lệ
        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("email.example")]
        [TestCase("invalid email")]
        public async Task CreateAsync_ShouldThrowArgumentException_WhenEmailIsInvalid(string email)
        {
            var accountRequest = new AccountRequest
            {
                Username = "newUser4",
                Name = "New User",
                Email = email,
                Phone = "123456789",
                NewPassword = "Test@1234",
                Roles = ["User"]
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _setup.AccountService.CreateAsync(accountRequest));
            Assert.That(ex.Message, Is.EqualTo("Invalid email format."));
        }

        // Test case 6: Ném lỗi khi username không hợp lệ
        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("user name")]
        [TestCase("user!name")]
        [TestCase("username#")]
        public async Task CreateAsync_ShouldThrowArgumentException_WhenUsernameIsInvalid(string username)
        {
            var accountRequest = new AccountRequest
            {
                Username = username,
                Name = "New User",
                Email = "validemail@example.com",
                Phone = "123456789",
                NewPassword = "Test@1234",
                Roles = ["User"]
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _setup.AccountService.CreateAsync(accountRequest));

            Assert.That(ex.Message, Is.EqualTo($"Username '{username}' is invalid, can only contain letters or digits."));
        }
    }
}
