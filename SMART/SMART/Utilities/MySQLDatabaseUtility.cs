using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
namespace SMART.Utilities
{
    public class MySQLDatabaseUtility : IDatabaseUtility
    {
        MySqlConnection connection = null;
        MySqlDataReader dataReader;
        private string sConnectionString = null;

        public MySQLDatabaseUtility(string sServerIP, string sPortNumber, string sDatabase, string sUserName, string sPassword)
        {
            sConnectionString = "Server=" + sServerIP + ";database=" + sDatabase + ";user=" + sUserName + ";password=" + sPassword + ";port=" + sPortNumber;
        }


        /// <summary>
        /// To create MySQL connection to DB
        /// </summary>
        /// <returns></returns>
        public MySqlConnection OpenConnection()
        {
            Console.WriteLine("Attempting to connect to DB with connection string:" + sConnectionString);
            try
            {
                connection = new MySqlConnection(sConnectionString);
                connection.Open();
                Console.WriteLine("DB Connection successful");
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Not able to connect to Database : " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns connection object
        /// </summary>
        /// <returns></returns>
        public MySqlConnection GetConnection()
        {
            return connection;
        }

        /// <summary>
        /// To close the existing connection
        /// </summary>
        public void CloseConnection()
        {
            connection.Close();
        }

        /// <summary>
        /// Get Multiple Records/Rows From DB
        /// </summary>
        /// <param name="query">select * from temp</param>
        /// <returns></returns>
        public List<List<string>> GetMultipleRecords(string query)
        {
            validateDBConnection();

            MySqlCommand cmd = new MySqlCommand(query, GetConnection());
            dataReader = cmd.ExecuteReader();
            List<List<String>> resultSet = convertDataReaderToList();
            if (resultSet == null)
            {
                Console.WriteLine("No records returned from DB for the Query:" + query);
                return null;
            }
            else
            {
                return resultSet;
            }
        }

        /// <summary>
        /// To Get single value from the table
        /// </summary>
        /// <param name="query">select count(*) from temp</param>
        /// <returns></returns>
        public string GetSingleAttributeValue(string query)
        {
            validateDBConnection();

            MySqlCommand cmd = new MySqlCommand(query, GetConnection());
            dataReader = cmd.ExecuteReader();
            if (dataReader != null)
            {
                dataReader.Read();
                return dataReader[0].ToString();
            }
            else
            {
                Console.WriteLine("No records returned from DB for the Query:" + query);
                return null;
            }
        }

        /// <summary>
        /// To Get Single Record of data from the table       
        /// </summary>
        /// <param name="query">select * from temp where row_id=1</param>
        /// <returns></returns>
        public List<string> GetSingleRecord(string query)
        {
            validateDBConnection();

            MySqlCommand cmd = new MySqlCommand(query, GetConnection());
            dataReader = cmd.ExecuteReader();
            if (dataReader.HasRows)
            {
                List<string> singleRowData = new List<string>();
                while (dataReader.Read())
                {
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        singleRowData.Add(dataReader[i].ToString());
                    }
                    return singleRowData;
                }
                return null;
            }
            else
            {
                Console.WriteLine("No records returned from DB for the Query:" + query);
                return null;
            }
        }

        /// <summary>
        /// To insert a new record or update an existing record in the table
        /// </summary>
        /// <param name="query"></param>
        public void InsertOrUpdateTable(string query)
        {
            validateDBConnection();

            MySqlCommand cmd = new MySqlCommand(query, GetConnection());
            int rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected < 1)
            {
                Console.WriteLine("No rows affected in updation/insertion");
            }
            else
            {
                Console.WriteLine("Total no of rows successfully inserted/updated : " + rowsAffected);
            }
        }

        /// <summary>
        /// Helper function to verify DataReader is Opened or closed
        /// </summary>
        /// <returns></returns>
        private bool IsDataReaderClosed()
        {
            if (dataReader == null)
            {
                return true;
            }

            return dataReader.IsClosed;
        }

        /// <summary>
        /// Helper function to convert dataReader object to List of values
        /// Note: This picks the dataReader which has been declared globally in this class
        /// </summary>
        /// <returns></returns>
        private List<List<String>> convertDataReaderToList()
        {
            List<List<string>> result = new List<List<string>>();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    List<string> singleRowData = new List<string>();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        singleRowData.Add(dataReader[i].ToString());
                    }
                    result.Add(singleRowData);
                }
                return result;
            }
            return null;
        }

        /// <summary>
        /// Helper function to verify there is an Open DB connection and DataReader is Closed to execute new query
        /// </summary>
        private void validateDBConnection()
        {
            //Validate DB connection is existing else will create a new connection

            if (connection.State == System.Data.ConnectionState.Closed)
            {
                Console.WriteLine("There is no Open connection to execute the query, creating new connection...");
                this.OpenConnection();
            }

            //Close dataReader if one exists 

            if (!IsDataReaderClosed())
                dataReader.Close();
        }
    }
}
