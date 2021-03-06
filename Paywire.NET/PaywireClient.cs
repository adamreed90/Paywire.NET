using System.Security.Cryptography.X509Certificates;
using Paywire.NET.Models.Base;
using Paywire.NET.Models.BatchInquiry;
using Paywire.NET.Models.GetAuthToken;
using Paywire.NET.Models.GetConsumerFee;
using Paywire.NET.Models.Sale;
using Paywire.NET.Models.SearchTransactions;
using Paywire.NET.Models.Verification;
using RestSharp;

namespace Paywire.NET
{
    public class PaywireClient
    {
        private readonly PaywireClientOptions _paywireClientOptions;
        private readonly RestClient _restClient;

        public PaywireClient(PaywireClientOptions options)
        {
            _paywireClientOptions = options;
            var endpointUrl = GetEndpointUrl(options.Endpoint);
            var restClientOptions = new RestClientOptions(endpointUrl)
            {
                ThrowOnAnyError = true,
                ThrowOnDeserializationError = true,
                Timeout = 20000,
            };
            _restClient = new RestClient(restClientOptions);
        }

        public async Task<T?> SendRequest<T>(BasePaywireRequest request) where T : BasePaywireResponse
        {
            request.TransactionHeader ??= new TransactionHeader();

            request.TransactionHeader.PWCLIENTID = _paywireClientOptions.AuthenticationClientId;
            request.TransactionHeader.PWKEY = _paywireClientOptions.AuthenticationKey;
            request.TransactionHeader.PWUSER = _paywireClientOptions.AuthenticationUsername;
            request.TransactionHeader.PWPASS = _paywireClientOptions.AuthenticationPassword;

            request.TransactionHeader.PWTRANSACTIONTYPE = request switch
            {
                GetAuthTokenRequest => PaywireTransactionType.GetAuthToken,
                VerificationRequest => PaywireTransactionType.Verification,
                GetConsumerFeeRequest => PaywireTransactionType.GetConsumerFee,
                SaleRequest => PaywireTransactionType.Sale,
                BatchInquiryRequest => PaywireTransactionType.BatchInquiry,
                SearchTransactionsRequest => PaywireTransactionType.SearchTransactions,
                _ => request.TransactionHeader.PWTRANSACTIONTYPE
            };

            var restRequest = new RestRequest("/API/pwapi", Method.Post);

            restRequest.AddXmlBody(request);

            var response = await _restClient.PostAsync<T>(restRequest);

            return response;
        }

        /// <summary>
        /// Translates the selected endpoint enumeration into the proper url.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns>Endpoint URL</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid Endpoint Selection Provided</exception>
        private static string GetEndpointUrl(PaywireEndpoint endpoint)
        {
            return endpoint switch
            {
                PaywireEndpoint.Staging => 
                    //"https://f7cc-72-180-100-82.ngrok.io",
                    "https://dbstage1.paywire.com",
                PaywireEndpoint.Production => "https://dbtranz.paywire.com",
                _ => throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, null)
            };
        }
    }
}