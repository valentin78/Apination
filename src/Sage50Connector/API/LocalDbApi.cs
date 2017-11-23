using System;
using System.IO;
using System.Reflection;
using log4net;
using LiteDB;

namespace Sage50Connector.API
{
    /// <summary>
    /// Local DB Interface wrapper
    /// Hides DB-dependent implementation
    /// </summary>
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

        /// <summary>
        /// Saves new id pair in LocalDB
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="sageCustomerId"></param>
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

        /// <summary>
        /// Get Sage50 Customer Id by ExternalId
        /// </summary>
        /// <param name="externalId">Composite unique key</param>
        /// <returns>Sage50 Customer Id or null if external Id is not found</returns>
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
                Id = ObjectId.NewObjectId();
            }

            public ObjectId Id { get; set; }
            public string ExternalId { get; set; }
            public string SageCustomerId { get; set; }
        }
    }
}
