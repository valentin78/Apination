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

                Sage50CronSchedule = "0 0/5 * * * ?",
                ApinationCronSchedule = "0 0/5 * * * ?",

                ApinationActionEndpointUrl = "api/sage50DTO",
            };
        }

        [HttpGet]
        [Route("api/actions")]
        public dynamic Actions()
        {
            return new dynamic[]
            {
                new {
                    type = "UpsertCustomer",
                    id = "1",
                    payload = new UpsertCustomerPayload
                    {
                        companyName = "Chase Ridge Holdings",
                        customer = new Customer
                        {
                            ExternalId = "CST-05",
                            Name = "Customer 1",
                            Email = "emal",
                            AccountNumber = "123",
                            BillToContact = new Contact()
                            {
                                FirstName = "Name",
                                LastName = "Lastname"
                            }
                        }
                    }
                },
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
    }


    public class PatchAction
    {
        // ReSharper disable once InconsistentNaming
        public string id { get; set; }
        // ReSharper disable once InconsistentNaming
        public bool processed { get; set; }
    }
}
