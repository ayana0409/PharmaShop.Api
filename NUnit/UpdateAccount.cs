using NUnitTest.Configuration;
using PharmaShop.Application.Models.Request.Account;

namespace NUnitTest
{
    public class AccountServiceTest
    {
        private AccountServiceSetup _setup;

        [SetUp]
        public async Task SetUp()
        {
            _setup = new AccountServiceSetup();
            await _setup.InitializeAsync();
        }

        // Test case 1: Update thành công
        [Test]
        public async Task UpdateAsync_Should_Update_Account_Successfully()
        {
            var accountRequest = new AccountRequest
            {
                Id = "TestAccount1",
                Name = "New Test Account 1 Name",
                Email = "newemail@example.com",
                Phone = "123456789",
                Username = "newUsername"
            };

            await _setup.AccountService.UpdateAsync(accountRequest);

            var updatedAccount = await _setup.Context.Users.FindAsync(accountRequest.Id);
            Assert.That(updatedAccount?.UserName, Is.EqualTo(accountRequest.Username));
            Assert.That(updatedAccount.FullName, Is.EqualTo(accountRequest.Name));
            Assert.That(updatedAccount.Email, Is.EqualTo(accountRequest.Email));
            Assert.That(updatedAccount.PhoneNumber, Is.EqualTo(accountRequest.Phone));
        }

        // Test case 2: Ném lỗi không tìm thấy tài khoản
        [Test]
        public void UpdateAsync_ShouldThrowKeyNotFoundException_WhenAccountNotFound()
        {
            var accountRequest = new AccountRequest
            {
                Id = "nonexistentUser",
                Username = "newUser",
                NewPassword = "NewPass@123",
                Name = "Updated Name"
            };

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _setup.AccountService.UpdateAsync(accountRequest));
            Assert.That(ex.Message, Is.EqualTo($"Account with ID '{accountRequest.Id}' not found."));
        }

        // Test case 3: Ném lỗi thông tin update không hợp lệ
        [Test]
        public async Task UpdateAsync_ShouldThrowInvalidOperationException_WhenUpdateFails()
        {
            var accountRequest = new AccountRequest
            {
                Id = "TestAccount1",
                //Username = "newUser",
                NewPassword = "NewPass@123",
                Name = "Updated Name",
                Email = "new@example.com",
                Phone = "123456789",
                Roles = ["User"]
            };

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _setup.AccountService.UpdateAsync(accountRequest));
            Assert.That(ex.Message, Is.EqualTo("Failed to update account information."));
        }

        // Test case 4: Ném lỗi Role không tồn tại
        [Test]
        public async Task UpdateAsync_Should_Throw_InvalidOperationException_When_Role_Does_Not_Exist()
        {
            string roleName = "NONEXISTENTROLEe";
            var accountRequest = new AccountRequest
            {
                Id = "TestAccount1",
                Name = "New Name",
                Email = "newemail@example.com",
                Phone = "123456789",
                Username = "newUsername",
                Roles = [roleName]
            };

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _setup.AccountService.UpdateAsync(accountRequest));
            Assert.That(ex.Message, Is.EqualTo($"Role {roleName.ToUpper()} does not exist."));
        }

        [TearDown]
        public void TearDown()
        {
            _setup.Cleanup();
            _setup.Dispose();
        }
    }
}