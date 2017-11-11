﻿using Microsoft.AspNetCore.Mvc;
using Sage50Connector.Models;

namespace ApinationApiStub.Controllers
{
    public class ApinationController : Controller
    {
        // GET api/apination
        [HttpGet]
        [Route("api/config")]
        public Config Get(string name)
        {
            return new Config
            {
                HeartBeatCronSchedule = "0 0/1 * * * ?",
                
                Sage50CronSchedule = "0 0/1 * * * ?",
                ApinationCronSchedule = "0 0/1 * * * ?",

                CompaniesList = new[]
                {
                    new Company
                    {
                        CompanyName = "Chase Ridge Holdings"
                    }
                }, 
                
                ApinationDTOToSage50Url = "api/sage50DTO",

                TriggersConfig = new []
                {
                    new Sage50TriggersConfig
                    {
                        TriggerBindingType = EventBindingTypes.CreatedCustomers,
                        Sage50DTOToApinationUrl = "api/sage50/createdCustomer"
                    }, 
                }
            };
        }

        //[HttpPost]
        //[Route("api/heartbeat")]
        //public string Post(string value)
        //{
        //    return value+ "!";
        //}

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
}
