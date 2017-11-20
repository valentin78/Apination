namespace Sage50Connector.Models.Payloads
{
    public class PhoneNumber
    {
        public string Number { get; set; }
        // TODO replace SAGE enums
        /// <summary>
        /// PhoneNumberKind enum
        /// </summary>
        public string Key { get; }
    }
}