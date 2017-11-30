using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sage50Connector.Models;
using Sage50Connector.Models.Data;
using Sage50Connector.Models.Payloads;

namespace ApinationApiStub.Controllers
{
    public class ApinationController : Controller
    {
        // GET api/apination
        [HttpGet]
        [Route("api/config")]
        public Config Get()
        {
            return new Config
            {
                HeartBeatCronSchedule = "0 0/5 * * * ?",

                ApinationCronSchedule = "0 0/5 * * * ?",

                ApinationActionEndpointUrl = "api/sage50DTO",
            };
        }

        [HttpGet]
        [Route("api/actions")]
        public dynamic Actions()
        {
            //return actionJSON;
            return new dynamic[]
            {
                //new {
                //    type = "CreatePayment",
                //    source = "Qualer",
                //    triggerId = "1",
                //    payload = new PaymentPayload
                //    {
                //        companyName = "Chase Ridge Holdings",
                //        payments = new []
                //        {
                //            new Payment
                //            {
                //                Vendor = new Vendor
                //                {
                //                    Name = "Vendor 1",
                //                    ExternalId = "VND01", 
                //                    Form1099Type = "None", 
                //                    CashAccount = new Account
                //                    {
                //                        Id = "AC01", 
                //                        Classification = "Cash"
                //                    }
                //                },
                //                PaymentMethod = "MASTER CARD"
                //            }
                //        },
                //        invoice = new SalesInvoice
                //        {
                //            ReferenceNumber = "3",
                //            FreightAmount = 0,
                //            DiscountAmount = 0,
                //            CustomerNote = "Note3",
                //            Date = DateTime.Parse("2019-3-15"),
                //            DateDue = DateTime.Parse("2019-4-14"),
                //            SalesLines = new List<SalesInvoiceLine>
                //            {
                //                new SalesInvoiceLine
                //                {
                //                    Amount = 2,
                //                    Description = "zzz3",
                //                    SalesTaxType = 1,
                //                    Account = new Account
                //                    {
                //                        Id = "1",
                //                        Classification = "Cash"
                //                    }
                //                }
                //            },
                //            ShipToAddress = new NameAndAddress()
                //            {
                //                Name = "Name 1",
                //                Address = new Address
                //                {
                //                    Address1 = "Addr1",
                //                    Address2 = "Addr3",
                //                }
                //            },
                //            Customer = new Customer
                //            {
                //                ExternalId = "CST-07",
                //                Name = "Customer 7",
                //                AccountNumber = "123",
                //                BillToContact = new Contact()
                //                {
                //                    FirstName = "Name",
                //                    LastName = "Lastname"
                //                }
                //            }
                //        }
                //    }
                //},
                new {
                    type = "UpdateInvoice",
                    id = "2",
                    payload = new SalesInvoicePayload
                    {
                        companyName = "Chase Ridge Holdings",
                        invoice = new SalesInvoice
                        {
                            ReferenceNumber = "3",
                            FreightAmount = 0,
                            DiscountAmount = 0,
                            CustomerNote = "Note3",
                            Date = DateTime.Parse("2019-3-15"),
                            DateDue = DateTime.Parse("2019-4-14"),
                            SalesLines = new List<SalesInvoiceLine>
                            {
                                new SalesInvoiceLine
                                {
                                    Amount = 2,
                                    Description = "zzz3",
                                    SalesTaxType = 1,
                                    Account = new Account
                                    {
                                        Id = "1",
                                        Classification = "Cash"
                                    }
                                }
                            },
                            ShipToAddress = new NameAndAddress()
                            {
                                Name = "Name 1",
                                Address = new Address
                                {
                                    Address1 = "Addr1",
                                    Address2 = "Addr3",
                                }
                            },
                            Customer = new Customer
                            {
                                ExternalId = "CST-07",
                                Name = "Customer 7",
                                AccountNumber = "123",
                                BillToContact = new Contact()
                                {
                                    FirstName = "Name",
                                    LastName = "Lastname"
                                }
                            }
                        }
                    }
                }
            };
        }

        [HttpPatch]
        [Route("api/actions")]
        public void ActionsPatch([FromBody]PatchAction[] list)
        {

        }

        // GET api/heartbeat
        [HttpGet]
        [Route("api/heartbeat")]
        public void Heartbeat()
        {
        }

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

        private string actionJSON = @"
                [
                    {
                        ""type"": ""UpdateInvoice"",
                        ""mainLogId"": 1,
                        ""source"": ""Qualer"",
                        ""userId"": 1,
                        ""workflowId"": 1,
                        ""triggerId"": 1,
                        ""payload"": {
                            ""CompanyName"": ""Chase Ridge Holdings"",
                            ""Invoice"": {
                                ""referenceNumber"": ""171983"",
                                ""date"": ""2019-03-15T12:18:56.467"",
                                ""dateDue"": ""2019-04-14T00:00:00"",
                                ""salesLines"": [
                                    {
                                        ""amount"": 1,
                                        ""SalesTaxType"":1,
                                        ""unitPrice"": 448
                                    }
                                ],
                                ""shipToAddress"": {
                                    ""address"": {
                                        ""address1"": ""1866 Old Grove Rd."",
                                        ""address2"": null,
                                        ""city"": ""Piedmont"",
                                        ""country"": ""United States"",
                                        ""state"": ""SC"",
                                        ""zip"": 29673
                                    }
                                },
                                ""Customer"": {
                                    ""externalId"": ""3916"",
                                    ""name"": ""JTEKT Automotive SC, Inc."",
                                    ""phoneNumbers"": null,
                                    ""billToContact"": {
                                        ""lastName"": """",
                                        ""firstName"": """",
                                        ""email"": """",
                                        ""companyName"": ""JTEKT Automotive SC, Inc."",
                                        ""address"": {
                                            ""address1"": ""Attn: Cleo Anderson"",
                                            ""address2"": """",
                                            ""city"": ""Piedmont"",
                                            ""country"": ""United States"",
                                            ""state"": ""SC"",
                                            ""zip"": 29673
                                        },
                                        ""phoneNumbers"": [
                                            {
                                                ""number"": ""864-277-0400"",
                                                ""key"": ""PhoneNumber1""
                                            }
                                        ]
                                    },
                                    ""shipToContact"": {
                                        ""lastName"": """",
                                        ""firstName"": """",
                                        ""email"": """",
                                        ""companyName"": ""JTEKT Automotive SC, Inc."",
                                        ""address"": {
                                            ""address1"": ""1866 Old Grove Rd."",
                                            ""address2"": """",
                                            ""city"": ""Piedmont"",
                                            ""country"": ""United States"",
                                            ""state"": ""SC"",
                                            ""zip"": 29673
                                        },
                                        ""phoneNumbers"": [
                                            {
                                                ""number"": ""864-277-0400"",
                                                ""key"": ""PhoneNumber1""
                                            }
                                        ]
                                    }
                                }
                            }
                        }
                    },
                   
                ]
                ";
    }


    public class PatchAction
    {
        // ReSharper disable once InconsistentNaming
        public string id { get; set; }
        // ReSharper disable once InconsistentNaming
        public bool processed { get; set; }
    }
}
