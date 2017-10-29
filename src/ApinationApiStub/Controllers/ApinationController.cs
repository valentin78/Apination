using System;
using System.Collections.Generic;
using System.Linq;
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
        public Config Get()
        {
            return new Config
            {
                HeartBeatCronSchedule = "0 0/1 * * * ?",
                DefaultCronSchedule = "0 0/1 * * * ?",
                CompaniesList = new[]
                {
                    new Company
                    {
                        CompanyName = "Demo Company",
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

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
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
