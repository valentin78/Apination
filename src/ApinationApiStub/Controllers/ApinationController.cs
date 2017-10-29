using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sage50Connector.Models;

namespace ApinationApiStub.Controllers
{
    //[Route("api/[controller]")]
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
                DefaultCronSchedule = "0 0/1 * * * ?",
                CompaniesList = new[]
                {
                    new Company
                    {
                        CompanyName = name, //"Demo Company",
                        Processes = new [] {
                            new SyncProcess
                            {
                                ProcessID = "CBD51F9F-4B8D-40A2-B086-1F849894EB96",
                                JobData = new Dictionary<string, object>
                                {
                                    {"p1", "p1Value"},
                                    {"p2", "p2Value"}
                                },
                                AutoStart = true
                            },
                            new SyncProcess
                            {
                                CronSchedule = "0 0/2 * * * ?",
                                ProcessID = "CBD51F9F-4B8D-40A2-B086-1F849894EB96",
                                JobData = new Dictionary<string, object>
                                {
                                    {"p1", "Hello"},
                                    {"p2", "world"}
                                },
                                AutoStart = false
                            }
                        }
                    }
                }
            };
        }

        [HttpPost]
        [Route("api/heartbeat")]
        public string Post(string value)
        {
            return value+ "!";
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
}
