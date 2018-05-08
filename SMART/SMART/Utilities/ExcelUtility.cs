using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace SMART.Utilities
{
    class ExcelUtility
    {
        /// <summary>
        /// Get ConnectionString
        /// </summary>
        /// <param name="filePath">filePath of excel</param>
        /// <returns></returns>
        public static string GetConnectionString(string filePath)
        {
            string returnString = string.Empty;
            try
            {
                Dictionary<string, string> props = new Dictionary<string, string>();
                /******************XLSX - Excel 2007, 2010, 2012, 2013*********************/
                props["Provider"] = "Microsoft.ACE.OLEDB.12.0;";
                props["Extended Properties"] = "Excel 12.0;";
                props["Data Source"] = filePath;
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> prop in props)
                {
                    sb.Append(prop.Key);
                    sb.Append('=');
                    sb.Append(prop.Value);
                    sb.Append(';');
                }
                returnString = sb.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception while getting connection string." + e.StackTrace);
            }
            return returnString;
        }

        /// <summary>
        /// Gets single cell value from excel sheet
        /// </summary>
        /// <param name="filePath">filePath of excel sheet - Excel 2007/2010/2012/2013</param>
        /// <param name="dataSheetName">Name of excel sheet from which we want to retrieve data</param>
        /// <param name="searchColumnName">The name of column which will act as search element(Column Header). Eg : TestcaseID</param>
        /// <param name="searchColumnValue">The value of key for which data needs to be retrieved. Eg- TC_142</param>
        /// <param name="expectedColumnName">Column name whose data needs to be retrieved. Eg - ExecutionStatus</param>
        /// <returns></returns>
        public static string GetSingleCellValue(string filePath, string dataSheetName, string searchColumnName, string searchColumnValue, string expectedColumnName)
        {
            string value = string.Empty;
            try
            {
                string connectionString = GetConnectionString(filePath);

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    /****************************Get all Sheets in Excel File*********************************/
                    DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    /*************************Loop through all Sheets to get data******************************/
                    foreach (DataRow dr in dtSheet.Rows)
                    {
                        string sheetName = dr["TABLE_NAME"].ToString();
                        if (sheetName.EndsWith("$") && sheetName == dataSheetName + "$")
                        {
                            string cmdText = "SELECT " + expectedColumnName + " FROM [" + sheetName + "] where " + searchColumnName + " = '" + searchColumnValue + "';";
                            DataTable dt = new DataTable();
                            OleDbDataAdapter da = new OleDbDataAdapter(cmdText, conn);
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                value = dt.Rows[0][0].ToString();
                            }

                            break;
                        }
                    }

                    conn.Close();
                }

                Console.WriteLine("Value from excel is " + value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught!!!" + ex.StackTrace);
            }

            return value;
        }

        /// <summary>
        /// Gets single cell value from excel sheet
        /// </summary>
        /// <param name="filePath">filePath of excel sheet - Excel 2007/2010/2012/2013</param>
        /// <param name="dataSheetName">Name of excel sheet from which we want to retrieve data</param>
        /// <param name="expectedColumnValue">Column name whose data needs to be retrieved. Eg - ExecutionStatus</param>
        /// <param name="filters">Key Value pair of filters which needs to be applied on the data to retrieve the Result column</param>
        /// <returns></returns>
        public static string GetSingleCellValueBasedOnFilters(string filePath, string dataSheetName, string expectedColumnValue, Dictionary<string, string> filters)
        {
            string value = string.Empty;
            try
            {
                string connectionString = GetConnectionString(filePath);

                string strFilters = "";

                /* Populate Filters*/
                foreach (KeyValuePair<string, string> filter in filters)
                {
                    strFilters += filter.Key + " = '" + filter.Value + "' AND ";
                }

                // Removed the 'AND' added in the last
                strFilters = strFilters.Substring(0, strFilters.Length - 4);

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    /****************************Get all Sheets in Excel File*********************************/
                    DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    /*************************Loop through all Sheets to get data******************************/
                    foreach (DataRow dr in dtSheet.Rows)
                    {
                        string sheetName = dr["TABLE_NAME"].ToString();
                        if (sheetName.EndsWith("$") && sheetName == dataSheetName + "$")
                        {
                            string cmdText = "SELECT " + expectedColumnValue + " FROM [" + sheetName + "] where " + strFilters + ";";
                            DataTable dt = new DataTable();
                            OleDbDataAdapter da = new OleDbDataAdapter(cmdText, conn);
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                value = dt.Rows[0][0].ToString();
                            }

                            break;
                        }
                    }

                    conn.Close();
                }

                Console.WriteLine("Value from excel is " + value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught!!!" + ex.StackTrace);
            }

            return value;
        }

        /// <summary>
        /// Updates Single Cell Value of excel sheet
        /// </summary>
        /// <param name="filePath">filePath of excel sheet</param>
        /// <param name="dataSheetName">dataSheetName from which we wnat to retrieve data</param>
        /// <param name="searchColumnName">ColumnName of search element</param>
        /// <param name="searchColumnValue">ColumnValue of search element</param>
        /// <param name="updateColumnName">ColumnName of update element</param>
        /// <param name="value">value to update</param>
        public static void UpdateSingleCellValue(string filePath, string dataSheetName, string searchColumnName, string searchColumnValue, string updateColumnName, string value)
        {
            try
            {
                string connectionString = GetConnectionString(filePath);
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    /****************************Get all Sheets in Excel File*********************************/
                    DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    /*************************Loop through all Sheets to get data******************************/
                    foreach (DataRow dr in dtSheet.Rows)
                    {
                        string sheetName = dr["TABLE_NAME"].ToString();
                        if (sheetName.EndsWith("$") && sheetName == dataSheetName + "$")
                        {
                            string strCommandText = "UPDATE [" + sheetName + "] SET " + updateColumnName + " = '" + value + "' WHERE " + searchColumnName + " = '" + searchColumnValue + "';";
                            OleDbCommand cmd = new OleDbCommand(strCommandText, conn);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            break;
                        }
                    }
                }

                Console.WriteLine("Updated new value " + value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught!!!" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Select TestCases Which Are Set To Yes in Excel Sheet.Excel
        /// </summary>
        /// <param name="filePath">filePath of excel sheet</param>
        /// <param name="dataSheetName">dataSheetName from which we wnat to retrieve data</param>
        /// <param name="testcasesColumnName">testcases ColumnName</param>
        /// <returns></returns>
        public static List<string> SelectTestCasesWhichAreSetToYes(string filePath, string dataSheetName, string testcasesColumnName)
        {
            List<string> testcasesList = new List<string>();
            try
            {
                string connectionString = GetConnectionString(filePath);
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    string cmdText = "Select " + testcasesColumnName + " FROM [" + dataSheetName + "$] where ExecutionStatus = 'Yes';";
                    DataTable dt = new DataTable();
                    OleDbDataAdapter da = new OleDbDataAdapter(cmdText, conn);
                    conn.Open();
                    da.Fill(dt);
                    conn.Close();
                    testcasesList = dt.AsEnumerable().Select(r => r[0].ToString()).ToList();
                }

                Console.WriteLine(testcasesList.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught!!!" + ex.StackTrace);
            }

            return testcasesList;
        }

        /// <summary>
        /// Get entire Row From ExcelSheet based on 'where' condition which is passed as parameter
        /// If 'where' condition is empty, return first row from the sheet name which is passed as parameter
        /// </summary>
        /// <param name="dataFile"></param>
        /// <param name="dSheetName"></param>
        /// <param name="strWhereCondition"></param>
        /// <returns></returns>
        public static DataRow GetDataRowFromExcelSheet(string dataFile, string dSheetName, string strWhereCondition)
        {
            DataRow dataRow = null;
            string query = strWhereCondition == string.Empty ? "Select * FROM [" + dSheetName + "$]" : "Select * FROM [" + dSheetName + "$] WHERE " + strWhereCondition + ";";
            Console.WriteLine("Query to retrieve data from excel sheet >>> " + query);

            try
            {
                DataTable dataTable = new DataTable("MyTable");
                using (OleDbConnection conn = new OleDbConnection(GetConnectionString(dataFile)))
                {
                    conn.Open();
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, conn))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
                dataRow = dataTable.Rows[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught!!!" + ex.StackTrace);
            }

            return dataRow;
        }

        /// <summary>
        /// Get entire data from excel sheet in the DataTable format 
        /// based on sheet name which is passed as parameter
        /// </summary>
        /// <param name="dataFile"></param>
        /// <param name="dSheetName"></param>
        /// <returns></returns>
        public static DataTable GetDataTableFromExcelSheet(string dataFile, string dSheetName)
        {
            DataTable dataTable = null;
            try
            {
                string query = "Select * FROM [" + dSheetName + "$]";
                dataTable = new DataTable("MyTable");
                using (OleDbConnection conn = new OleDbConnection(GetConnectionString(dataFile)))
                {
                    conn.Open();
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, conn))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught!!!" + ex.StackTrace);
            }

            return dataTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="searchColumnName"></param>
        /// <param name="searchColumnValue"></param>
        /// <returns></returns>
        public static DataRow GetDataRowFromDataTable(DataTable dataTable, string searchColumnName, string searchColumnValue)
        {
            DataRow dataRow = null;
            try
            {
                if (dataTable != null)
                {
                    dataRow = dataTable.Rows
                                      .Cast<DataRow>()
                                      .Where(x => x[searchColumnName].ToString() == searchColumnValue).FirstOrDefault();
                }
                else
                {
                    Console.WriteLine("Table is null or empty");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught!!!" + ex.StackTrace);
            }

            return dataRow;
        }

        /// <summary>
        /// Converts DataRow to Dictionary
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ConvertDataRowToDict(DataRow row)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (DataColumn col in row.Table.Columns)
            {
                string colStr = col.ToString();
                dict.Add(colStr, row[colStr].ToString());
            }

            return dict;
        }

    }
}
