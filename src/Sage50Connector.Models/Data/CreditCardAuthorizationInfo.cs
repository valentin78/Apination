using System;

namespace Sage50Connector.Models.Data
{
    public class CreditCardAuthorizationInfo
    {
        public NameAndAddress Address { get; set; }
        public string AuthorizationCode { get; set; }
        public string LastFourDigits { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Note { get; set; }
        public decimal AmountAuthorized { get; set; }
    }
}