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
        private static readonly ILog Log = LogManager.GetLogger(typeof(LocalDbApi));

        private string DbPath
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
        /// <param name="key"></param>
        /// <param name="sageCustomerId"></param>
        public void StoreCustomerId(string key, string sageCustomerId)
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var customerIdsMapping = db.GetCollection<CustomerIdMapping>();

                customerIdsMapping.Insert(new CustomerIdMapping
                {
                    ApinationKey = key,
                    SageCustomerId = sageCustomerId
                });

                customerIdsMapping.EnsureIndex(x => x.SageCustomerId);
                customerIdsMapping.EnsureIndex(x => x.ApinationKey);

                Log.Info($"LOCALDB: Stored for Key: '{key}' map to CustomerId: '{sageCustomerId}'");
            }
        }

        /// <summary>
        /// Get Sage50 Customer Id by Key (cross-system)
        /// </summary>
        /// <param name="key">Composite unique key</param>
        /// <returns>Sage50 Customer Id or null if key is not found</returns>
        public string GetCustomerIdByKey(string key)
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var customerIdsMapping = db.GetCollection<CustomerIdMapping>();
                var map = customerIdsMapping.FindOne(x => x.ApinationKey.Equals(key));

                Log.Info($"LOCALDB: Found for key: '{key}' map to CustomerId: '{map?.SageCustomerId ?? "null"}'");

                return map?.SageCustomerId;
            }
        }

        /// <summary>
        /// Saves new id pair in LocalDB
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sageVendorId"></param>
        public void StoreVendorrId(string key, string sageVendorId)
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var customerIdsMapping = db.GetCollection<VendorIdMapping>();

                customerIdsMapping.Insert(new VendorIdMapping
                {
                    ApinationKey = key,
                    SageVendorId = sageVendorId
                });

                customerIdsMapping.EnsureIndex(x => x.SageVendorId);
                customerIdsMapping.EnsureIndex(x => x.ApinationKey);

                Log.Info($"LOCALDB: Stored for Key: '{key}' map to VendorId: '{sageVendorId}'");
            }
        }

        /// <summary>
        /// Get Sage50 Vendor Id by Key (cross-system)
        /// </summary>
        /// <param name="key">Composite unique key</param>
        /// <returns>Sage50 Customer Id or null if key is not found</returns>
        public string GetVendorIdByKey(string key)
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var customerIdsMapping = db.GetCollection<VendorIdMapping>();
                var map = customerIdsMapping.FindOne(x => x.ApinationKey.Equals(key));

                Log.Info($"LOCALDB: Found for key: '{key}' map to VendorId: '{map?.SageVendorId?? "null"}'");

                return map?.SageVendorId;
            }
        }

        private class CustomerIdMapping
        {
            public CustomerIdMapping()
            {
                Id = ObjectId.NewObjectId();
            }

            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public ObjectId Id { get; }
            public string ApinationKey { get; set; }
            public string SageCustomerId { get; set; }
        }

        private class VendorIdMapping
        {
            public VendorIdMapping()
            {
                Id = ObjectId.NewObjectId();
            }

            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            // ReSharper disable once MemberCanBePrivate.Global
            // ReSharper disable once MemberCanBePrivate.Local
            public ObjectId Id { get; }
            public string ApinationKey { get; set; }
            public string SageVendorId { get; set; }
        }
    }
}
