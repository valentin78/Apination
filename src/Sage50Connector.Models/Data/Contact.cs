using System.Collections.Generic;

namespace Sage50Connector.Models.Data
{
    public class Contact
    {
        public Contact()
        {
            Address = new Address();
            PhoneNumbers = new List<PhoneNumber>(2);
            Gender = "NotSpecified";
            NameToUseOnForms = "ContactName";
        }
        //readonly public bool IsPrimaryContact { get; set; }
        public string Suffix { get; set; }
        public string Prefix { get; set; }
        public string Notes { get; set; }
        /// <summary>
        /// NameToUseOnForms
        /// </summary>
        public string NameToUseOnForms { get; set; }
        //readonly public string Name { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public Address Address { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        /// <summary>
        /// Gender enum
        /// </summary>
        public string Gender { get; set; }
    }
}