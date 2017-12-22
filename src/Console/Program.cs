using System;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Sage.Peachtree.API;
using Sage50Connector.Models.Data;
using Sage50Connector.Processing.Actions.ActionHandlers;
using Sage50Connector.Processing.Actions.SageActions;
using Customer = Sage50Connector.Models.Data.Customer;

namespace Console
{
    /// <summary>
    /// Console Application for testing SDK possibilities
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var api = new API();

            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schemaCust = generator.Generate(typeof(Customer));
            JSchema schemaInv = generator.Generate(typeof(SalesInvoiceLine));
            JSchema schemaReceipt = generator.Generate(typeof(Sage50Connector.Models.Data.Receipt));

            Sage.Peachtree.API.Vendor v;

            var companies = api.CompaniesList();
            //var companyId = companies.SingleOrDefault(c => c.CompanyName == "Chase Ridge Holdings");
            api.OpenCompany(companies[1]);
            var list = api.PaymentList();
            list.Load();
            var r_list = api.ReceiptList();
            r_list.Load();
            var i_list = api.InvoicesList();
            i_list.Load();

            foreach (var item in list)
            {
                System.Console.WriteLine("At: {0}", item.LastSavedAt);
            }

            System.Console.WriteLine("Count: {0}", list.Count);

            list.ListChanged += (sender, eventArgs) =>
            {
                Debugger.Launch();
            };

            dynamic actionsJson = JsonConvert.DeserializeObject("[{\"_id\": \"5a3b9a673f04681b7b3e9eb9\",\"source\": \"Qualer\",\"type\": \"UpsertInvoice\",\"payload\": {\"invoice\": {\"grandTotal\": 330.5,\"customer\": {\"name\": \"Southeastern Freight Lines (Tpa)\",\"externalId\": \"4274\",\"phoneNumbers\": null,\"shipToContact\": {\"email\": \"\",\"address\": {\"zip\": 28804,\"country\": \"United States\",\"state\": \"NC\",\"address1\": \"275 Aiken Road\",\"city\": \"Asheville\",\"address2\": \"\"},\"lastName\": \"\",\"firstName\": \"\",\"phoneNumbers\": [{\"key\": \"PhoneNumber1\",\"number\": \"828-658-2711\"}],\"companyName\": \"Southeastern Freight Lines (Tpa)\"},\"billToContact\": {\"email\": \"\",\"address\": {\"zip\": 28804,\"country\": \"United States\",\"state\": \"NC\",\"address1\": \"275 Aiken Road\",\"city\": \"Asheville\",\"address2\": \"\"},\"lastName\": \"\",\"firstName\": \"\",\"phoneNumbers\": [{\"key\": \"PhoneNumber1\",\"number\": \"828-658-2711\"}],\"companyName\": \"Southeastern Freight Lines (Tpa)\"}},\"date\": \"2017-10-06T12:53:47.67\",\"referenceNumber\": \"173929\",\"dateDue\": \"2017-10-24T00:00:00\",\"shipToAddress\": {\"address\": {\"zip\": 28804,\"country\": \"United States\",\"state\": \"NC\",\"address1\": \"275 Aiken Road\",\"city\": \"Asheville\",\"address2\": null}},\"shippingFee\": null,\"salesLines\": [{\"description\": \"The new scale was calibrated and tested.\nAll test are in tolerance.\nNew Scale, No As Found Data 10-9-17\",\"amount\": 330,\"unitPrice\": 330,\"quantity\": 1,\"salesTaxType\": 1}]},\"CompanyName\": \"Southeastern Freight Lines (Tpa)\"},\"mainLogId\": \"5a3b99cc38e9a4000fab2e8b\",\"userId\": 1281,\"triggerId\": \"5a2911e7ceb87200047c046a\",\"clientId\": \"greenville-scale-sage-1\",\"createdAt\": \"2017-12-21T11:20:10.363Z\",\"processed\": false}]");
            var strings = (actionsJson.Root as Newtonsoft.Json.Linq.JArray)?.Select(i => i.ToString()).ToArray()[0];

            var action =
                new Sage50Connector.Processing.Actions.SageActions.Factory.SageActionFromJsonFactory().Create(strings);

            new UpsertInvoiceSageActionHandler().Handle((UpsertInvoiceSageAction)action);
        }
    }

    class API : IDisposable
    {
        private PeachtreeSession m_Session = null;
        /// <summary>
        /// the active Sage 50 Company
        /// </summary>
        public Company m_Company = null;

        public void Dispose()
        {
            CloseCurrentCompany();

            if (m_Session != null)
            {
                m_Session.Dispose();
            }
        }

        protected PeachtreeSession CurrentSession
        {
            get
            {
                // if the session has not been initialized
                if (m_Session == null || !m_Session.SessionActive)
                {
                    // dispose of the inactive session
                    if (m_Session != null)
                    {
                        m_Session.Dispose();
                    }

                    // create a new session instance
                    m_Session = new PeachtreeSession();

                    // start the session.  
                    // with no application ID, you can only open Sample companies
                    m_Session.Begin("7OzMf+fDer9SYGJE0DXFcdM1o4Ss1DUhUp7crFgiEk4WWMK4Pjpa5Q==wMV81b1OF03+5H+wZBXZOKU5WTbh08tFyOwPsUGMPWV3Qc15BvfXSx83ttLWASvUasK3lROTT24LPIrct9hum7RM3GfEv55CF5hb7jIckIbdHAwyMekjjE5rxBPIdF0H9JjrJ+YkEGJ/7CUolX0bOUsK/1Z1FHDbDVycvVz5t6KvtV/vRLUsWPp7iIJh2dLeP6eszs8MWPdnauddjzitbUQ6qLEKZSUanHfuhGg57fo=");
                }
                // return the current session
                return m_Session;
            }
        }

        private void CloseCurrentCompany()
        {
            if (m_Company != null)
            {
                m_Company.Close();
                m_Company = null;
            }
        }

        public void OpenCompany(CompanyIdentifier CompanyID)
        {
            CloseCurrentCompany();

            if (CompanyID == null) return;

            // Ask the Sage 50 application if this application has
            // been granted access to the company.
            var AuthResult = CurrentSession.VerifyAccess(CompanyID);

            // if the app has never asked for authorization before, We need to ask now
            if (AuthResult == AuthorizationResult.NoCredentials)
            {
                AuthResult = CurrentSession.RequestAccess(CompanyID);
            }

            // handle the authorization result
            switch (AuthResult)
            {
                case AuthorizationResult.CompanyLocked:
                case AuthorizationResult.LoginRestricted:
                case AuthorizationResult.Denied:
                case AuthorizationResult.CorruptedOrTampered:
                case AuthorizationResult.Pending:
                    // show the error messages for these situations
                    System.Console.WriteLine($"Error OpenCompany_{AuthResult.ToString()}");
                    break;

                case AuthorizationResult.Granted:
                    // open the company
                    m_Company = CurrentSession.Open(CompanyID);
                    break;
                default:
                    Debug.Fail(AuthResult.ToString());
                    break;
            }

        }

        public CompanyIdentifierList CompaniesList()
        {
            return CurrentSession.CompanyList();
        }

        public PaymentList PaymentList()
        {
            return m_Company.Factories.PaymentFactory.List();
        }
        public SalesInvoiceList InvoicesList()
        {
            return m_Company.Factories.SalesInvoiceFactory.List();
        }
        public ReceiptList ReceiptList()
        {
            return m_Company.Factories.ReceiptFactory.List();
        }

        public CustomerList CustomersList()
        {
            return m_Company.Factories.CustomerFactory.List();
        }
    }
}
