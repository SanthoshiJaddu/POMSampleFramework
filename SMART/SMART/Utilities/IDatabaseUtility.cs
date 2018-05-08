using System.Collections.Generic;

namespace SMART.Utilities
{
    public interface IDatabaseUtility
    {
        /// <summary>
        /// Get Single cell value From DB
        /// </summary>
        /// <param name="query">eg: Select productid from product where productname='Intel Core i5'</param>
        /// <returns></returns>
        string GetSingleAttributeValue(string query);

        /// <summary>
        /// Get Single Record/Row From DB
        /// </summary>
        /// <param name="query">eg: select top 1 * from product</param>
        /// <returns></returns>
        List<string> GetSingleRecord(string query);

        /// <summary>
        /// Get Multiple Records/Rows From DB
        /// </summary>
        /// <param name="query">eg: Select * from product</param>
        /// <returns></returns>
        List<List<string>> GetMultipleRecords(string query);

        /// <summary>
        /// Insert Or Update Table In DB
        /// </summary>
        /// <param name="query">eg: Update product set productname='Intel core i10' where productid='344544'</param>
        void InsertOrUpdateTable(string query);
    }
}
