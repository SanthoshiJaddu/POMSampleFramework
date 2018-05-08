using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace SMART
{
    public class TestEngine
    {
        #region Public variables and properties

        public static string executionEnvironment = string.Empty;
        public static string executionBrowser = string.Empty;

        public static class EnvironmentDetails
        {
            public static string Url { get; set; }
            public static string Username { get; set; }
            public static string Password { get; set; }
            public static string DatabaseString { get; set; }
            public static Dictionary<string, string> CustomParametersList { get; set; }
        }

        #endregion Public variables and properties

        #region Public static methods
       /// <summary>
       /// method to initialize by getting execution browser env,credetails etc...
       /// </summary>
       /// <param name="xmlPath">Pass on th exml path of the project config file</param>
        public static void Initialize(string xmlPath)
        {
            //get Env details
            executionEnvironment = GetEnvironment(xmlPath);
            //get browser details
            executionBrowser = GetExecutionBrowser(xmlPath);
            //get the details of selected env

            GetEnvironmentDetails(xmlPath, executionEnvironment);

        }
        #endregion Public static methods


        #region PrivateMethods
        /// <summary>
        /// Function to decrypt a encrypted string to a plain text string.
        /// </summary>
        /// <param name="encryptedString">Send the encrypted string to be decrypted to normal string.</param>
        /// <param name="key">Returns a plain text string</param>
        /// <returns></returns>
        public static string GetDecryptedString(string encryptedString)
        {
            string key = "AutomationTest";
            try
            {
                encryptedString = encryptedString.Replace(" ", "+");
                byte[] bytesBuff = Convert.FromBase64String(encryptedString);
                using (Aes objAES = Aes.Create())
                {
                    Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(key, new byte[] { 0x1d, 0x68, 0x6b, 0x5a, 0x20, 0x4d, 0x65, 0x5d });
                    objAES.Key = crypto.GetBytes(32);
                    objAES.IV = crypto.GetBytes(16);
                    using (MemoryStream mStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(mStream, objAES.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(bytesBuff, 0, bytesBuff.Length);
                            cryptoStream.Close();
                        }
                        encryptedString = Encoding.Unicode.GetString(mStream.ToArray());
                    }
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Cryptographic exception occured. Details :" + e.StackTrace);
                Console.WriteLine("Cryptographic exception occured. Messsage :" + e.Message);
            }
            return encryptedString;
        }
        public static string GetEnvironment(string xmlPath)
        {
            string environment = "";
            try
            {
                XDocument xmlDoc = XDocument.Load(xmlPath);
                var envDetails = from elements in xmlDoc.Descendants("Configuration")
                                 select new
                                 {
                                     Environment = elements.Element("ExecutionEnvironment").Value,
                                 };

                environment = envDetails.FirstOrDefault().Environment;

            }
            catch(NullReferenceException ex)
            {
                Console.WriteLine("Null reference exception while returning environment details.check xml");
            }
            return environment;
        }

        public static string GetExecutionBrowser(string xmlPath)
        {
            string executionBrowser = "";
            try
            {
                XDocument xDoc = XDocument.Load(xmlPath);
                var browserDeatils = from elements in xDoc.Descendants("Configuration")
                                     select new
                                     {
                                         BrowserDetails = elements.Element("ExecutionBrowser").Value,
                                    };
                if (browserDeatils.Count() > 0)
                {
                    executionBrowser = browserDeatils.FirstOrDefault().BrowserDetails;
                }
                else
                {
                    executionBrowser = "Chrome"; //Default value
                }

            }
            catch(NullReferenceException ex)
            {
                Console.WriteLine("error while retreiveing browser details:" + ex.StackTrace);
            }
            return executionBrowser;
        }

        public static string GetEnvironmentDetails(string xmlPath, string environment)
        {
            Console.WriteLine("Environment selected is:" + environment);
            EnvironmentDetails.CustomParametersList = new Dictionary<string, string>();
            bool customerVariablePresentFlag = false;
            XDocument xmlDoc = XDocument.Load(xmlPath);

            //extract environment details like url,credentials
            try
            {
                Console.WriteLine("environment is:" + environment);
                var config = from elements in xmlDoc.Descendants("Environments")
                             select new
                             {
                                 url = elements.Element(environment).Element("url").Value,
                                 userName = elements.Element(environment).Element("Credentials").Element("UserName").Value,
                                 password = elements.Element(environment).Element("Credentials").Element("Password").Value,
                                 databasestring = elements.Element(environment).Element("DataBaseString").Value,

                             };
                if (config.Count() > 0) {
                    EnvironmentDetails.Url = config.FirstOrDefault().url;
                    EnvironmentDetails.Username = config.FirstOrDefault().userName;
                    //EnvironmentDetails.Password = GetDecryptedString(config.FirstOrDefault().password);
                    EnvironmentDetails.Password = config.FirstOrDefault().password;
                    EnvironmentDetails.DatabaseString = config.FirstOrDefault().databasestring;
                }
            }
            catch(NullReferenceException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            // custom parameters section

            try
            {
                //check if custom parameters exists and atleaset one child exist for custom element 
                customerVariablePresentFlag = (xmlDoc.Descendants("Environments").Descendants(environment).Descendants("Custom").Any()) && (xmlDoc.Descendants("Environments").Descendants(environment).Descendants("Custom").DescendantNodes().Count() > 0);
                Console.WriteLine("custom parameters exist?" + customerVariablePresentFlag);
                if(customerVariablePresentFlag)
                {
                    var customfeilds = from elements in xmlDoc.Descendants(environment).Descendants("Custom").Elements("parameters")
                                       select new
                                       {
                                           key = elements.Attribute("name").Value,
                                           value = elements.Attribute("value").Value,
                                       };
                    foreach(var item in customfeilds)
                    {
                        EnvironmentDetails.CustomParametersList.Add(item.key, item.value);
                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }
        #endregion Privatemethods
    }
}
