using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
namespace SMART.Utilities
{
    public class OracleDatabaseUtility:IDatabaseUtility
    {
        OracleConnection connObject;
        string connString;

        public OracleDatabaseUtility(string connectionString)
        {
            connString = connectionString;
        }

        /// <summary>
        /// Get Single cell value From DB
        /// </summary>
        /// <param name="query">eg: Select productid from product where productname='Intel Core i5'</param>
        /// <returns></returns>
        public string GetSingleAttributeValue(string query)
        {
            string result = string.Empty;
            try
            {
                using (OracleConnection connObject = new OracleConnection(connString))
                {
                    connObject.Open();
                    OracleCommand command = new OracleCommand(query, connObject);
                    result = command.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught at GetSingleDataFromDB!!!" + ex.StackTrace);
                Console.WriteLine("Exception Message - " + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Get Single Record/Row From DB
        /// </summary>
        /// <param name="query">eg: select top 1 * from product</param>
        /// <returns></returns>
        public List<string> GetSingleRecord(string query)
        {
            List<string> result = new List<string>();
            try
            {
                using (OracleConnection connObject = new OracleConnection(connString))
                {
                    connObject.Open();
                    OracleCommand command = new OracleCommand(query, connObject);
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i <= reader.FieldCount; i++)
                                {
                                    result.Add(reader[i].ToString());
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("No rows found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught at GetSingleRowFromDB!!!" + ex.StackTrace);
                Console.WriteLine("Exception Message - " + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Get Multiple Records/Rows From DB
        /// </summary>
        /// <param name="query">eg: Select * from product</param>
        /// <returns></returns>
        public List<List<string>> GetMultipleRecords(string query)
        {
            List<List<string>> result = new List<List<string>>();
            try
            {
                using (OracleConnection connObject = new OracleConnection(connString))
                {
                    connObject.Open();
                    OracleCommand command = new OracleCommand(query, connObject);
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                List<string> singleRowData = new List<string>();
                                for (int i = 0; i <= reader.FieldCount; i++)
                                {
                                    singleRowData.Add(reader[i].ToString());
                                }

                                result.Add(singleRowData);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No rows found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught at GetMultipleRowsFromDB!!!" + ex.StackTrace);
                Console.WriteLine("Exception Message - " + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Insert Or Update Table In DB
        /// </summary>
        /// <param name="query">eg: Update product set productname='Intel core i10' where productid='344544'</param>    
        public void InsertOrUpdateTable(string query)
        {
            try
            {
                using (OracleConnection connObject = new OracleConnection(connString))
                {
                    connObject.Open();
                    OracleCommand command = new OracleCommand(query, connObject);
                    int result = command.ExecuteNonQuery();
                    Console.WriteLine("No of Rows effected " + result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught at InsertOrUpdateTableInDB!!!" + ex.StackTrace);
                Console.WriteLine("Exception Message - " + ex.Message);
            }
        }
    }
}
