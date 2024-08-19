using Microsoft.Extensions.Configuration;
using PayPal.Api;
using PharmaShop.Application.Abtract;
using PharmaShop.Application.Models.Request;

namespace PharmaShop.Application.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly IConfiguration _configuration;

        public PayPalService(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        private APIContext GetAPIContext()
        {
            string mode = _configuration["Paypal:Mode"] ?? throw new ApplicationException("Invalid PayPal mode");
            string clientId = _configuration["Paypal:ClientID"] ?? throw new ApplicationException("Invalid PayPal ClientID");
            string clientSecret = _configuration["Paypal:SecretKey"] ?? throw new ApplicationException("Invalid PayPal SecretKey");
            var config = new Dictionary<string, string>
            {
                { "mode", mode },
                { "clientId", clientId },
                { "clientSecret", clientSecret }
            };

            var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
            return new APIContext(accessToken);
        }

        public bool ProcessPayment(OrderRequest request)
        {
            var apiContext = GetAPIContext();

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer
                {
                    payment_method = "paypal"
                },
                transactions = new List<Transaction>
            {
                new Transaction
                {
                    amount = new Amount
                    {
                        currency = "USD",
                        total = request.TotalPrice.ToString("F2")
                    },
                    description = "Order description"
                }
            },
                redirect_urls = new RedirectUrls
                {
                    cancel_url = "https://yourdomain.com/cancel",
                    return_url = "https://yourdomain.com/return"
                }
            };

            var createdPayment = payment.Create(apiContext);
            var approvalUrl = createdPayment.links.FirstOrDefault(link => link.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;

            if (approvalUrl != null)
            {
                return true;
            }

            return false;
        }
    }
}
