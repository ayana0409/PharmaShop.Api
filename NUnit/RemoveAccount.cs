using Microsoft.EntityFrameworkCore;
using NUnitTest.Configuration;
using PharmaShop.Application.Models.Request.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTest
{
    public class RemoveAccount
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

        [Test]
        public async Task RemoveAsync_Should_Successfully_Remove_Account()
        {
            var accountRequest = new AccountRequest
            {
                Username = "removeUser",
                Name = "Remove User",
                Email = "removeuser@example.com",
                Phone = "123456789",
                NewPassword = "Test@1234",
                Roles = new List<string> { "User" }
            };

            await _setup.AccountService.CreateAsync(accountRequest);
            var accountToRemove = 
                await _setup.Context.Users.SingleOrDefaultAsync(p => p.UserName == "removeUser");

            await _setup.AccountService.RemoveAsync(accountToRemove.Id);

            var removedAccount = 
                await _setup.Context.Users.SingleOrDefaultAsync(p => p.UserName == "removeUser");
            Assert.That(removedAccount.IsActive, Is.False);
        }

        [Test]
        public void RemoveAsync_Should_Throw_KeyNotFoundException_When_Account_Not_Found()
        {
            string nonExistentAccountId = "nonExistentUser";

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _setup.AccountService.RemoveAsync(nonExistentAccountId));

            Assert.That(ex.Message, Is.EqualTo($"Account with ID '{nonExistentAccountId}' not found."));
        }


    }
}
