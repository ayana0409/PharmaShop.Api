using NUnitTest.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTest
{
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
    }
}
