using System;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using System.Configuration;
using System.Windows.Forms;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace SMART.Smart
{
    public class SmartLog
    {
        public static IsmartLogger Log;

        static SmartLog()
        {
            SmartLog objSmart = new SmartLog();
            bool flag;

            bool result = Boolean.TryParse(objSmart.GetSettingsFromConfigFile(), out flag);

            if (result)
            {
                if (flag)
                {
                    Log = SmartCustomLog.GetSmartCustomLogger(true);
                }
                else
                {
                    Log = SmartGenericLog.GetSmartGenericLogger(true);
                }
            }
        }

        /// <summary>
        /// Function to read credentials or url from a config file. Currently app.config is used.
        /// </summary>
        /// <param name="key">Send the key whose value needs to be retrieved. This should match name in config file.</param>
        /// <returns>The value for the key passed</returns>
        public string GetSettingsFromConfigFile(string key = "LogCustomType")
        {
            string returnValue = "";
            try
            {
                var currentAssembly = Assembly.GetExecutingAssembly();
                var callerAssemblies = new StackTrace().GetFrames()
                        .Select(x => x.GetMethod().ReflectedType.Assembly).Distinct()
                        .Where(x => x.GetReferencedAssemblies().Any(y => y.FullName == currentAssembly.FullName));
                var initialAssembly = callerAssemblies.Last();

                Configuration config = ConfigurationManager.OpenExeConfiguration(initialAssembly.Location);
                // Get the appSettings section
                AppSettingsSection logConfigSettings = (AppSettingsSection)config.GetSection("appSettings");
                // return the desired field 
                returnValue = logConfigSettings.Settings[key].Value;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception while reading value from config file. Please check config file");
                Console.WriteLine("Exception stacktrace:" + e.StackTrace);
            }
            return returnValue;
        }

        /// <summary>
        /// Function to create Instance for SmartGenericLog or SmartCustomLog
        /// </summary>
        /// <returns></returns>
        public static IsmartLogger GetLogTypeInstance()
        {
            SmartLog objSmart = new SmartLog();
            bool flag;

            bool result = bool.TryParse(objSmart.GetSettingsFromConfigFile(), out flag);

            if (result)
            {
                if (flag)
                {
                    Log = SmartCustomLog.GetSmartCustomLogger(true);
                    return Log;
                }
            }
            Log = SmartGenericLog.GetSmartGenericLogger(true);
            return Log;
        }


    }
    /// <summary>
    /// Used for Creating Generic Custom Log.
    /// </summary>
    public class SmartGenericLog : IsmartLogger
    {
        protected static log4net.ILog log;

        #region SmartLogger



        private static SmartGenericLog smartLoggerInstance;

        public static IsmartLogger GetSmartGenericLogger(bool invalidate)
        {

            if (smartLoggerInstance == null || invalidate)
            {
                SmartGenericLog.log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                smartLoggerInstance = new SmartGenericLog();
            }
            return smartLoggerInstance;

        }


        /// <summary>
        /// Invalidate object.
        /// </summary>
        public static void Invalidate()
        {
            smartLoggerInstance = null;
        }

        /// <summary>
        /// TestCase Started Common Log
        /// </summary>
        public virtual void TestCaseStarted()
        {
            string testCaseId = new StackTrace().GetFrame(1).GetMethod().Name;
            Info("********************************************\n");
            Info("Executing TestCaseId : " + testCaseId);
        }


        /// <summary>
        /// TestCase Completed Common Log
        /// </summary>
        public void TestCaseEnded()
        {
            string testCaseId = new StackTrace().GetFrame(1).GetMethod().Name;
            Info("Execution Completed for  TestCaseId : " + testCaseId);
            Info("********************************************\n");
        }

        /// <summary>
        /// TestCase Completed Common Log
        /// </summary>
        /// <param name="testCaseStatus"></param>
        public void TestCaseCompleted(bool testCaseStatus)
        {
            string testCaseId = new StackTrace().GetFrame(1).GetMethod().Name;

            if (testCaseStatus)
            {
                Pass(testCaseId + " PASSED!!!");
            }
            else
            {
                Fail(testCaseId + "FAILED !!!");
                Assert.Fail();
            }

            Info("TestCase Execution completed!!! TestCaseID : " + testCaseId);
            Info("********************************************\n");
        }

        /// <summary>
        /// Method Started Common Log
        /// </summary>
        /// <param name="comment"></param>
        public void MethodStarted(string comment = "")
        {
            string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
            string text = comment == "" ? " Entering " + methodName : "Entering Method :" + methodName + ":==>" + comment;
            Info(text);
        }

        /// <summary>
        /// Method Completed Common Log
        /// </summary>
        /// <param name="status"></param>
        public void MethodCompleted(bool? status = null)
        {
            string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
            string text = status == null ? " Method " + methodName + " Exited" : " Method " + methodName + " Exited" + " : Status - " + status;
            Info(text);
        }


        /// <summary>
        /// Method Exception Common Log
        /// </summary>
        /// <param name="ex"></param>
        public virtual void MethodExceptionMessage(Exception ex)
        {
            string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
            Error("--- " + methodName + " ended with exception!!! Exception message is " + ex.Message);
            Error("Exception stacktrace is " + ex.StackTrace + "! Screenshot at " + TakeScreenshot(methodName + "Error"));
        }
        /// <summary>
        /// Logs generic information to the Log File.
        /// </summary>
        /// <param name="InfoMessage"></param>
        public virtual void Info(string InfoMessage)
        {
            InfoMessage = "" + InfoMessage;
            log.Info(InfoMessage);
            Console.WriteLine(InfoMessage);
        }
        /// <summary>
        /// Logs Debug information to the Log File.
        /// </summary>
        /// <param name="InfoMessage"></param>
        public void Debug(string InfoMessage)
        {
            InfoMessage = "" + InfoMessage;
            log.Debug(InfoMessage);
        }
        /// <summary>
        /// Logs Warning information to the Log File.
        /// </summary>
        /// <param name="InfoMessage"></param>
        public void Warning(string WarningMessage)
        {
            WarningMessage = "" + WarningMessage;
            log.Warn(WarningMessage);

        }
        /// <summary>
        /// Logs Error information to the Log File with Exception.
        /// </summary>
        /// <param name="ErrorMessage"></param>
        /// <param name="ex"></param>
        public virtual void Error(string ErrorMessage, Exception ex)
        {
            log.Error(ErrorMessage, ex);

        }
        /// <summary>
        /// Logs Error information to the Log File without Exception.
        /// </summary>
        /// <param name="ErrorMessage"></param>
        public virtual void Error(string ErrorMessage)
        {
            log.Error(ErrorMessage);

        }
        /// <summary>
        /// Logs Pass status to the Log File 
        /// </summary>
        /// <param name="text"></param>
        public void Pass(string text)
        {
            text = "Passed -> " + text;
            /***Write the statement to log***/
            Info(text);
            Console.WriteLine(text);
        }
        /// <summary>
        /// Log Fail Status along Exception  with Screenshot captured.
        /// </summary>
        /// <param name="ErrorMessage"></param>
        /// <param name="ex"></param>

        public void Fail(string ErrorMessage, Exception ex)
        {
            ErrorMessage = "Failed -> " + ErrorMessage;
            Console.WriteLine(ErrorMessage);
            Error(ErrorMessage, ex);
            Error("Screenshot captured and available at " + TakeScreenshot(ErrorMessage));

        }
        /// <summary>
        ///  Log Fail Status without exception Screenshot captured.
        /// </summary>
        /// <param name="ErrorMessage"></param>
        public void Fail(string ErrorMessage)
        {

            ErrorMessage = "Failed_" + ErrorMessage;
            Console.WriteLine(ErrorMessage);
            Error(ErrorMessage);
            Error("Screenshot captured and available at " + TakeScreenshot(ErrorMessage));

        }
        /// <summary>
        /// Get Time Stamp
        /// </summary>
        /// <returns></returns>
        internal string GetTimestamp()
        {
            return DateTime.Now.ToString();
        }
        /// <summary>
        /// Screenshot capture.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal virtual string TakeScreenshot(string message)
        {

            string execpath = Path.GetDirectoryName(Path.GetDirectoryName(
                                                        System.IO.Path.GetDirectoryName(
                                                         System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase))).Remove(0, "file:\\".Length) + "\\";

            var fileName = "";
            var AssemblyPath = Assembly.GetExecutingAssembly();
            var screenshotfolder = System.IO.Path.GetDirectoryName(AssemblyPath.ToString());
            var LogsPath = System.IO.Path.Combine(execpath, "Results\\Screenshots");

            if (!Directory.Exists(LogsPath))
            {
                try
                {
                    Info("Create Log Folder named Screenshots");
                    Directory.CreateDirectory(LogsPath);
                }
                catch (Exception) { }
            }
            try
            {

                var bitMap = new Bitmap(SystemInformation.VirtualScreen.Width,
                               SystemInformation.VirtualScreen.Height,
                               PixelFormat.Format32bppArgb);

                Graphics screenGraph = Graphics.FromImage(bitMap);
                screenGraph.CopyFromScreen(SystemInformation.VirtualScreen.X,
                           SystemInformation.VirtualScreen.Y,
                           0,
                           0,
                           SystemInformation.VirtualScreen.Size,
                           CopyPixelOperation.SourceCopy);
                var tfileName = message + "_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss_ffff");
                fileName = System.IO.Path.ChangeExtension(System.IO.Path.Combine(LogsPath, tfileName), ".jpeg");
                bitMap.Save(fileName, ImageFormat.Jpeg);

            }
            catch (Exception e)
            {
                Error("Exception while capturing screenshot" + e.StackTrace);
                Error("Exception message " + e.Message);
            }
            return fileName;
        }

        /// <summary>
        /// Function to take screenshot and put in a custom folder.
        /// </summary>
        /// <param name="folder">Complete path of folder where screenshot should be stored</param>
        /// <param name="message">Message for screenshot.Will be appended in screenshot</param>
        /// <param name="Manager"></param>
        /// <returns></returns>
        internal string TakeScreenshotusingCustomPath(string folder, string message)
        {
            string returnValue = null;
            try
            {
                string filename = message + "_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss_ffff") + ".jpeg";
                string fullPath = System.IO.Path.Combine(folder, filename);
                var bitMap = new Bitmap(SystemInformation.VirtualScreen.Width,
                            SystemInformation.VirtualScreen.Height,
                            PixelFormat.Format32bppArgb);

                Graphics screenGraph = Graphics.FromImage(bitMap);
                screenGraph.CopyFromScreen(SystemInformation.VirtualScreen.X,
                           SystemInformation.VirtualScreen.Y,
                           0,
                           0,
                           SystemInformation.VirtualScreen.Size,
                           CopyPixelOperation.SourceCopy);
                bitMap.Save(fullPath, ImageFormat.Jpeg);
                returnValue = fullPath;

            }
            catch (Exception e)
            {
                Error("Exception while capturing screenshot" + e.StackTrace);
                Error("Exception message " + e.Message);
            }

            return returnValue;
        }

        #endregion

    }
    /// <summary>
    /// Used for Creating Customised version of Log
    /// </summary>
    public class SmartCustomLog : SmartGenericLog
    {

        #region SmartLogger
        private string logFolder;
        public SmartCustomLog()
        {
            string strDirPath = Path.GetDirectoryName(Path.GetDirectoryName(
                                                        System.IO.Path.GetDirectoryName(
                                                         System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase))).Remove(0, "file:\\".Length);

            string execpath = strDirPath + "\\";

            // string sTestResultFolder = "Results\\" + TestContext.CurrentContext.Test.ClassName.Replace("PVP_UIAutotest.", "");
            string[] arrClassPath = TestContext.CurrentContext.Test.ClassName.Split('.');
            string[] arrExecPath = strDirPath.Replace("\\", "/").Split('/');
            strDateTime = System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm").ToString();
            string sTestResultFolder = "Results\\" + arrExecPath[arrExecPath.Length - 1] + "_" + strDateTime + "\\" + arrClassPath[arrClassPath.Length - 1];
            CreateFolder(execpath, sTestResultFolder);

            logFolder = execpath + sTestResultFolder;
        }

        private static SmartCustomLog smartLoggerCustomInstance;

        public static IsmartLogger GetSmartCustomLogger(bool invalidate)
        {
            if (smartLoggerCustomInstance == null || invalidate)
                smartLoggerCustomInstance = new SmartCustomLog();
            return smartLoggerCustomInstance;

        }

        /// <summary>
        /// Invalidate object.
        /// </summary>
        public static void Invalidate()
        {
            smartLoggerCustomInstance = null;
        }

        #endregion

        private string _strDateTime;
        private string strlogFolderFullPath;

        public string strDateTime
        {
            get { return _strDateTime; }
            set
            {
                if (string.IsNullOrEmpty(_strDateTime))
                {
                    _strDateTime = value;

                }
            }
        }

        /// <summary>
        /// Logs Error information to the Log File with Exception.
        /// </summary>
        /// <param name="ErrorMessage"></param>
        /// <param name="ex"></param>
        public override void Error(string ErrorMessage, Exception ex)
        {
            this.Error(ErrorMessage + "Exception:" + ex.Message);
        }

        /// <summary>
        /// Method Exception Common Log
        /// </summary>
        /// <param name="ex"></param>
        public override void MethodExceptionMessage(Exception ex)
        {
            string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
            Error("--- " + methodName + " ended with exception!!! Exception message is " + ex.Message);
            Error("Exception stacktrace is " + ex.StackTrace + "! Screenshot at " + TakeScreenshot(methodName + "Error"));
        }

        /// <summary>
        /// Logs Error information to the Log File without Exception.
        /// </summary>
        /// <param name="ErrorMessage"></param>
        public override void Error(string ErrorMessage)
        {
            try
            {
                createSubFolder(TestContext.CurrentContext.Test.Name);
                File.AppendAllText(strlogFolderFullPath + @"\Logs\SmartLogger.txt", System.DateTime.Now.ToString() + " Error:" + ErrorMessage + Environment.NewLine);
                Console.WriteLine(ErrorMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }

        //    /// <summary>
        //    /// Creates sub folder
        //    /// </summary>
        //    /// <returns></returns>
        public void createSubFolder(string strTCName)
        {
            string strSubFldrPath = logFolder + "\\" + strTCName;

            CreateFolder(strSubFldrPath, "Logs");
            CreateFolder(strSubFldrPath, "Screenshots");

            strlogFolderFullPath = strSubFldrPath;
        }

        //    /// <summary>
        //    /// Creates test results folders.
        //    /// </summary>
        //    /// <returns></returns>
        public string CreateTestResultLogFolder()
        {
            string execpath = Path.GetDirectoryName(Path.GetDirectoryName(
                                                         System.IO.Path.GetDirectoryName(
                                                          System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase))).Remove(0, "file:\\".Length) + "\\";

            // string sTestResultFolder = "Results\\" + TestContext.CurrentContext.Test.ClassName.Replace("PVP_UIAutotest.", "");
            string[] arrClassPath = TestContext.CurrentContext.Test.ClassName.Split('.');
            strDateTime = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm").ToString();
            string sTestResultFolder = "Results\\" + arrClassPath[0] + strDateTime + "\\" + arrClassPath[1];
            CreateFolder(execpath, sTestResultFolder);

            logFolder = execpath + sTestResultFolder;
            return execpath + sTestResultFolder;
        }

        /// <summary>
        ///  Function to take screenshot and put in a custom folder.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal override string TakeScreenshot(string message)
        {

            var fileName = "";
            try
            {

                var bitMap = new Bitmap(SystemInformation.VirtualScreen.Width,
                               SystemInformation.VirtualScreen.Height,
                               PixelFormat.Format32bppArgb);

                Graphics screenGraph = Graphics.FromImage(bitMap);
                screenGraph.CopyFromScreen(SystemInformation.VirtualScreen.X,
                           SystemInformation.VirtualScreen.Y,
                           0,
                           0,
                           SystemInformation.VirtualScreen.Size,
                           CopyPixelOperation.SourceCopy);
                var tfileName = message + "_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss_ffff");

                char[] ch = System.IO.Path.GetInvalidFileNameChars();
                fileName = System.IO.Path.ChangeExtension(System.IO.Path.Combine(strlogFolderFullPath + @"\Screenshots", tfileName.Replace("->", " ")), ".jpeg");
                bitMap.Save(fileName, ImageFormat.Jpeg);

            }
            catch (Exception e)
            {
                Error("Exception while capturing screenshot" + e.StackTrace);
                Error("Exception message " + e.Message);
            }
            return fileName;
        }

        /// <summary>
        /// TestCase Started Common Log
        /// </summary>
        public override void TestCaseStarted()
        {
            string testCaseId = new StackTrace().GetFrame(1).GetMethod().Name;
            //createSubFolder(TestContext.CurrentContext.Test.Name);
            Info("********************************************\n");
            Info("Executing TestCaseId : " + TestContext.CurrentContext.Test.Name);
        }

        /// <summary>
        /// Function to report the information  
        /// </summary>
        /// <param name="text">stores text.User logged in to the application</param>
        /// <param name="Manager">stores Manager Object</param>

        public override void Info(string text)
        {

            try
            {
                createSubFolder(TestContext.CurrentContext.Test.Name);
                File.AppendAllText(strlogFolderFullPath + @"\Logs\SmartLogger.txt", System.DateTime.Now.ToString() + " Info:" + text + Environment.NewLine);
                Console.WriteLine(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }



        /// <summary>
        /// Creates folder based on Assebly location
        /// </summary>
        /// <param name="parentFolderPath"></param>
        /// <param name="folderToBeCreated"></param>
        public void CreateFolder(string parentFolderPath, string folderToBeCreated)
        {
            string path = Path.Combine(parentFolderPath, folderToBeCreated);
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Not able to create a new directory, under the path:" + path);
                Console.WriteLine(e.Message);
            }
        }


    }
    public interface IsmartLogger
    {


        /// <summary>
        /// TestCase Started Common Log
        /// </summary>
        void TestCaseStarted();
        /// <summary>
        /// TestCase Completed Common Log
        /// </summary>
        void TestCaseEnded();
        /// <summary>
        ///  TestCase Completed Common Log with Testcase status
        /// </summary>
        /// <param name="testCaseStatus"></param>
        void TestCaseCompleted(bool testCaseStatus);

        /// <summary>
        /// Method Started Common Log
        /// </summary>
        /// <param name="comment"></param>
        void MethodStarted(string comment = "");
        /// <summary>
        ///  Method Completed Common Log
        /// </summary>
        /// <param name="ex"></param>      
        void MethodCompleted(bool? status = null);
        /// /// <summary>
        /// Method Exception Common Log
        /// </summary>
        /// <param name="comment"></param>
        void MethodExceptionMessage(Exception ex);

        void Info(string InfoMessage);

        void Debug(string InfoMessage);

        void Warning(string WarningMessage);

        void Error(string ErrorMessage, Exception ex);

        void Error(string ErrorMessage);

        void Pass(string text);

        void Fail(string ErrorMessage, Exception ex);

        void Fail(string ErrorMessage);

    }
}
