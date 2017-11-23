using System;

namespace Sage50Connector.Core
{
    class TypeUtil
    {
        /// <summary>
        /// Convert datetime to UTC string format
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateToUTC(DateTime value)
        {
            return value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK");
        }

        /// <summary>
        /// Convert datetime to ODBC string format
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateToODBC(DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
