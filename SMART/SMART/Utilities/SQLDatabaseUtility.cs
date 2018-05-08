using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace SMART.Utilities
{
    public class SQLDatabaseUtility:IDatabaseUtility
    {
        SqlConnection connObject;
        private readonly string connString;

        public SQLDatabaseUtility(string connectionString)
        {
            connString = connectionString;
        }

        ///<inheritdoc/>
        public string GetSingleAttributeValue(string query)
        {
            string result = string.Empty;
            try
            {
                using (SqlConnection connObject = new SqlConnection(connString))
                {
                    connObject.Open();
                    SqlCommand command = new SqlCommand(query, connObject);
                    result = command.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught at GetSingleDataFromDB!!!" + ex.StackTrace);
                Console.WriteLine("Exception Message - " + ex.Message);
                throw ex;
            }

            return result;
        }

        ///<inheritdoc/>
        public List<string> GetSingleRecord(string query)
        {
            List<string> result = new List<string>();
            try
            {
                using (SqlConnection connObject = new SqlConnection(connString))
                {
                    connObject.Open();
                    SqlCommand command = new SqlCommand(query, connObject);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
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
                throw ex;
            }

            return result;
        }

        ///<inheritdoc/>
        public List<List<string>> GetMultipleRecords(string query)
        {
            List<List<string>> result = new List<List<string>>();
            try
            {
                using (SqlConnection connObject = new SqlConnection(connString))
                {
                    connObject.Open();
                    SqlCommand command = new SqlCommand(query, connObject);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                List<string> singleRowData = new List<string>();
                                for (int i = 0; i < reader.FieldCount; i++)
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
                throw ex;
            }

            return result;
        }

        ///<inheritdoc/>
        public void InsertOrUpdateTable(string query)
        {
            try
            {
                using (SqlConnection connObject = new SqlConnection(connString))
                {
                    connObject.Open();
                    SqlCommand command = new SqlCommand(query, connObject);
                    int result = command.ExecuteNonQuery();
                    Console.WriteLine("No of Rows effected " + result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught at InsertOrUpdateTableInDB!!!" + ex.StackTrace);
                Console.WriteLine("Exception Message - " + ex.Message);
                throw ex;
            }
        }
    }
}
