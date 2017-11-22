using System;
using System.IO;
using System.Reflection;
using log4net;
using LiteDB;

namespace Sage50Connector.API
{
    public class LocalDbApi
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(LocalDbApi));

        protected string DbPath
        {
            get
            {
                var assemblyFullPath = Assembly.GetEntryAssembly().Location;
                var serviceExecutionDirectory = new FileInfo(assemblyFullPath).Directory?.FullName;

                if (serviceExecutionDirectory == null)
                    throw new SystemException("Crytical error. Can not get execution path.");

                var databaseDirectory = Path.Combine(serviceExecutionDirectory, "DB");

                Directory.CreateDirectory(databaseDirectory);

                return Path.Combine(databaseDirectory, "local.db");
            }
        }

        public void StoreCustomerId(string externalId, string sageCustomerId)
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var customerIdsMapping = db.GetCollection<CustomerIdMapping>();

                customerIdsMapping.Insert(new CustomerIdMapping
                {
                    ExternalId = externalId,
                    SageCustomerId = sageCustomerId
                });

                customerIdsMapping.EnsureIndex(x => x.SageCustomerId);
                customerIdsMapping.EnsureIndex(x => x.ExternalId);

                Log.Info($"LOCALDB: Stored for ExternalId: '{externalId}' map to CustomerId: '{sageCustomerId}'");
            }
        }

        public string GetCustomerIdByExternalId(string externalId)
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var customerIdsMapping = db.GetCollection<CustomerIdMapping>();
                var map = customerIdsMapping.FindOne(x => x.ExternalId.Equals(externalId));

                Log.Info($"LOCALDB: Found for ExternalId: '{externalId}' map to CustomerId: '{map?.SageCustomerId ?? "null"}'");

                return map?.SageCustomerId;
            }
        }

        public class CustomerIdMapping
        {
            public CustomerIdMapping()
            {
                _id = ObjectId.NewObjectId();
            }

            public ObjectId _id { get; set; }
            public string ExternalId { get; set; }
            public string SageCustomerId { get; set; }
        }
    }
}
