using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Drawing.Imaging;
using OpenQA.Selenium.Interactions;
using System.Threading;
using SeleniumWrapper;
using SMART.Smart;
using System.Drawing;

namespace SMART
{
    public static class SeleniumWrapper
    {
        public static IWebDriver driver;
        public static string currentWindow = "";

        #region [Navigate To Url]
        /// <summary>
        /// This function will navigate to specific url on browser
        /// </summary>
        /// <param name="url">Appilication url </param>  
        public static void NavigateUrl(string url)
        {
            try
            {
                driver.Navigate().GoToUrl(url);
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

            }
            catch (WebDriverException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region [Close Browser]
        /// <summary>
        /// This function will close active browser
        /// </summary>        
        public static void CloseBrowser()
        {
            try
            {
                driver.Close();
                driver.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region [Click Object]
        /// <summary>
        /// This function will perform click action on webobjects
        /// </summary>
        /// <param name="uiObject">elment from UI </param>        
        public static void ClickObject(string uiObject)
        {
            try
            {
                driver.SwitchTo().Window(driver.CurrentWindowHandle);
                IWebElement uiWebElement = FindWebElement(uiObject);
                if (uiWebElement != null && uiWebElement.Enabled && uiWebElement.Displayed)
                {
                    uiWebElement.Click();
                }
                else
                {
                    Console.WriteLine("{0}, object is not present in UI", uiObject);
                }
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
        #endregion

        #region [Implicit Wait]
        /// <summary>
        /// This function will make dirver object to wait for specified time
        /// </summary>
        /// <param name="waitinSeconds">Wait time in seconds </param>  
        public static void ImplicitWait(int waitinSeconds)
        {
            try
            {
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(waitinSeconds));
            }
            catch (WebDriverException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
        #endregion

        #region [Explicit Wait]
        /// <summary>
        /// This function will allow to wait till perticular element is visible
        /// </summary>
        /// <param name="waitinSeconds">Wait time in seconds </param>  
        /// <param name="uiObject">Element from UI </param>  
        public static void ExplicitWait(int waitinSeconds, string uiObject)
        {
            try
            {

                int index = uiObject.IndexOf(':');
                string elementType = uiObject.Substring(0, index);
                string objectData = uiObject.Substring(index + 1);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(waitinSeconds));

                switch (elementType.ToLower())
                {
                    case "id":
                        wait.Until(ExpectedConditions.ElementIsVisible(By.Id(objectData)));
                        break;
                    case "xpath":
                        wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(objectData)));
                        break;
                    case "name":
                        wait.Until(ExpectedConditions.ElementIsVisible(By.Name(objectData)));
                        break;
                    case "linktext":
                        wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText(objectData)));
                        break;
                    case "tagname":
                        wait.Until(ExpectedConditions.ElementIsVisible(By.TagName(objectData)));
                        break;
                    case "classname":
                        wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(objectData)));
                        break;
                    case "css":
                        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(objectData)));
                        break;
                }
            }
            catch (WebDriverException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
        #endregion

        #region [Get PageTitle]
        /// <summary>
        /// This function will get title of the page from UI
        /// </summary>
        /// <returns>Returns string </returns>
        public static string GetPageTitle()
        {
            string pageTitle = "";
            try
            {
                pageTitle = driver.Title;
            }
            catch (WebDriverException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return pageTitle;
        }

        #endregion

        #region [Verify Element Exist]
        /// <summary>
        /// This function is to verify whether element is present on UI
        /// </summary>         
        /// <param name="uiObject">Element from UI </param>  
        /// <returns>Returns true if element present,if not false</returns>

        public static bool VerifyElementExist(string uiObject)
        {
            bool isElementExist = false;
            try
            {
                if (FindWebElement(uiObject) != null)
                {
                    isElementExist = true;
                }
                else
                {
                    Console.WriteLine("{0}, object is not present in UI", uiObject);
                }
            }
            catch (WebDriverException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return isElementExist;
        }
        #endregion

        #region [Get Text From Tags]
        /// <summary>
        /// This function will get the text of the web element/tag
        /// </summary>         
        /// <param name="uiObject">Element from UI </param>  
        /// <returns>Returns text as a string</returns>
        public static string GetTextBetweenTags(string uiObject)
        {
            string textFromUI = "";
            try
            {
                IWebElement uiWebElement = FindWebElement(uiObject);
                if (uiWebElement != null)
                {
                    textFromUI = uiWebElement.Text;
                }
                else
                {
                    Console.WriteLine("{0}, object is not present in UI", uiObject);
                }
            }
            catch (WebDriverException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return textFromUI;

        }
        #endregion

        #region [Set Value In Textbox]
        /// <summary>
        /// This function will type value in textbox
        /// </summary>         
        /// <param name="uiObject">Element from UI </param> 
        public static void SetValueForTextbox(string uiObject, string text)
        {
            try
            {
                IWebElement uiWebElement = FindWebElement(uiObject);
                if (uiWebElement != null && uiWebElement.Displayed)
                {
                    uiWebElement.Click();
                    uiWebElement.Clear();
                    uiWebElement.SendKeys(text);
                }
                else
                {
                    Console.WriteLine("{0}, object is not displayed", uiObject);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

        }
        #endregion

        #region [Get text from Textbox]
        /// <summary>
        /// This function will retrieve text present in the Textbox
        /// </summary>         
        /// <param name="uiObject">Element from UI </param> 
        /// <returns>Returns text as a string</returns>
        public static string GetTextFromTextbox(string uiObject, string inputValue = "value")
        {
            string valueFromTextbox = "";
            try
            {
                IWebElement uiWebElement = FindWebElement(uiObject);
                if (uiWebElement != null)
                {
                    valueFromTextbox = uiWebElement.GetAttribute(inputValue);
                }
                else
                {
                    Console.WriteLine("{0}, object is not present on UI", uiObject);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return valueFromTextbox;
        }
        #endregion

        #region [Select Option From Dropdown]
        /// <summary>
        /// This function will Select option from Dropdown box
        /// </summary>         
        /// <param name="uiObject">Element from UI </param> 
        /// <param name="indexOrText">Index or visible text needs to be selected from Dropdown</param> 
        /// <param name="isIndex">If the selection is based on index then true or else false</param> 
        public static void SelectOptionFromDropdown(string uiObject, string indexOrText, bool isIndex)
        {
            try
            {
                IWebElement uiWebElement = FindWebElement(uiObject);
                if (uiWebElement != null && uiWebElement.Displayed)
                {
                    int num;
                    SelectElement selectElement = new SelectElement(uiWebElement);
                    if (isIndex)
                    {
                        num = int.Parse(indexOrText);
                        selectElement.SelectByIndex(num);
                    }
                    else
                    {
                        selectElement.SelectByText(indexOrText);
                    }
                }
                else
                    Console.WriteLine("{0}, object is not displayed", uiObject);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
        #endregion

        #region [Capture Screenshot from UI]
        /// <summary>
        /// This function will capture screenshot
        /// </summary>         
        /// <param name="methodName">Testcase name,used as screenshot file name</param> 
        /// <param name="indexOrText">Index or visible text needs to be selected from Dropdown</param> 
        public static void CaptureScreenshot(string methodName)
        {
            try
            {
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "\\Screenshots");
                String fp = System.IO.Directory.GetCurrentDirectory() + "\\Screenshots\\" + methodName + "_" + DateTime.Now.ToString("ddMMMyyyy hh_mm_tt") + ".png";
                screenshot.SaveAsFile(fp, ImageFormat.Png);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

        }
        #endregion

        #region [Double click object]
        /// <summary>
        /// This function will double click on UI element
        /// </summary>         
        /// <param name="uiObject">Element from UI</param>         
        public static void DoubleClickObject(string uiObject)
        {
            try
            {
                driver.SwitchTo().Window(driver.CurrentWindowHandle);
                IWebElement elementUI = FindWebElement(uiObject);
                if (elementUI != null && elementUI.Enabled && elementUI.Displayed)
                {
                    Actions action = new Actions(driver);
                    action.DoubleClick(elementUI).Build().Perform();
                }
                else
                {
                    Console.WriteLine("{0}, object is not present in UI", uiObject);
                }
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine(ex.Message);

            }
        }
        #endregion

        #region [EnterActionOnWebElement]
        /// <summary>
        /// This function will hit enter key on UI element
        /// </summary>         
        /// <param name="uiObject">Element from UI</param> 
        public static void EnterActionOnWebElement(string uiObject)
        {
            try
            {
                driver.SwitchTo().Window(driver.CurrentWindowHandle);
                IWebElement uiWebElement = FindWebElement(uiObject);

                if (uiWebElement != null && uiWebElement.Enabled && uiWebElement.Displayed)
                {
                    uiWebElement.SendKeys(Keys.Return);
                    System.Threading.Thread.Sleep(2000);
                }
                else
                {
                    Console.WriteLine("{0}, object is not present in UI", uiObject);
                }
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine(ex.Message);

            }

        }
        #endregion

        #region [Pres enter key]
        /// <summary>
        /// This function will simulate enter key press
        /// </summary>         

        public static void PressEnter()
        {
            try
            {
                Actions action = new Actions(driver);
                action.SendKeys(Keys.Return).Perform();
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region [Accept Alert]
        /// <summary>
        /// This function will accept alert if it is present.
        /// </summary>  
        public static void AcceptAlert()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                alert.Accept();
            }
            catch (NoAlertPresentException ex)
            {
                Console.WriteLine("Alert is not present on UI or unable to accept" + ex.StackTrace);
            }

        }
        #endregion

        #region [Dismiss Alert]
        /// <summary>
        /// This function will accept alert if it is present.
        /// </summary>  
        public static void DismissAlert()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                alert.Dismiss();
            }
            catch (NoAlertPresentException ex)
            {
                Console.WriteLine("Alert is not present on UI or unable to dismiss" + ex.StackTrace);
            }

        }
        #endregion

        #region [Verify Alert Present]
        /// <summary>
        /// This function will return true if alert is prest or else false
        /// </summary> 
        /// <returns>Returns boolean</returns>
        public static bool VerifyAlertPresent()
        {
            bool isAlertPresent = false;
            try
            {
                driver.SwitchTo().Alert();
                isAlertPresent = true;
            }
            catch (NoAlertPresentException ex)
            {
                Console.WriteLine("Alert is not present on UI" + ex.StackTrace);
            }
            return isAlertPresent;
        }
        #endregion

        #region [Page refresh]
        /// <summary>
        /// This function will refresh webpage
        /// </summary>  
        public static void PageRefresh()
        {
            try
            {
                driver.Navigate().Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Webpage refresh is not happend" + ex.StackTrace);

            }

        }
        #endregion

        #region [Switch To New Window]
        /// <summary>
        /// This function will switch driver focus to new window
        /// </summary>         
        /// <param name="pageTitle">Page title of the new window</param>
        public static void SwitchToNewWindow(string pageTitle)
        {
            try
            {
                currentWindow = driver.CurrentWindowHandle;
                List<string> availableWindows = new List<string>(driver.WindowHandles);
                foreach (string handle in availableWindows)
                {
                    if (handle != currentWindow)
                    {
                        driver.SwitchTo().Window(handle).Title.Contains(pageTitle);
                        break;
                    }
                }
            }
            catch (NoSuchWindowException ex)
            {
                Console.WriteLine("driver focus is not switched to new window" + ex.StackTrace);
            }
        }
        #endregion

        #region [Switch To Parent Window]
        /// <summary>
        /// This function will switch driver focus to Parent window
        /// </summary>         

        public static void SwitchToParentWindow()
        {
            try
            {
                driver.SwitchTo().Window(currentWindow);
            }
            catch (NoSuchWindowException ex)
            {
                Console.WriteLine("driver focus is not switched to parent window" + ex.StackTrace);
            }
        }
        #endregion

        #region [Find web element]
        /// <summary>
        /// This function will find element on webpage,if present element is returned else null
        /// </summary>         
        /// <param name="uiObject">Pass Webelement of UI,like id,name,xpath,linktext,partiallinktext,tagname,classname,css
        ///  And value for the parameter should be colon seperated with type eg:"Xpath://*[]","ID:id of the element"</param>
        /// <returns>Returns weblement</returns>
        public static IWebElement FindWebElement(string uiObject)
        {
            IWebElement webElement = null;
            try
            {
                int index = uiObject.IndexOf(':');
                string elementType = uiObject.Substring(0, index);
                string objectData = uiObject.Substring(index + 1);
                if (elementType.Equals("Xpath", StringComparison.InvariantCultureIgnoreCase))
                {
                    webElement = driver.FindElement(By.XPath(objectData));
                }
                else if (elementType.Equals("Id", StringComparison.InvariantCultureIgnoreCase))
                {
                    webElement = driver.FindElement(By.Id(objectData));
                }
                else if (elementType.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
                {
                    webElement = driver.FindElement(By.Name(objectData));
                }
                else if (elementType.Equals("LinkText", StringComparison.InvariantCultureIgnoreCase))
                {
                    webElement = driver.FindElement(By.LinkText(objectData));
                }
                else if (elementType.Equals("PartialLinkText", StringComparison.InvariantCultureIgnoreCase))
                {
                    webElement = driver.FindElement(By.PartialLinkText(objectData));
                }
                else if (elementType.Equals("TagName", StringComparison.InvariantCultureIgnoreCase))
                {
                    webElement = driver.FindElement(By.TagName(objectData));
                }
                else if (elementType.Equals("ClassName", StringComparison.InvariantCultureIgnoreCase))
                {
                    webElement = driver.FindElement(By.ClassName(objectData));
                }
                else if (elementType.Equals("Css", StringComparison.InvariantCultureIgnoreCase))
                {
                    webElement = driver.FindElement(By.CssSelector(objectData));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return webElement;
        }
        #endregion

        #region [Find list of web elements]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uiObject"></param>
        /// <returns></returns>
        public static IList<IWebElement> FindListOfWebElements(string uiObject)
        {
            IList<IWebElement> elements = new List<IWebElement>();

            try
            {
                int index = uiObject.IndexOf(':');
                string elementType = uiObject.Substring(0, index);
                string objectData = uiObject.Substring(index + 1);

                switch (elementType.ToLower())
                {
                    case "id":
                        elements = driver.FindElements(By.Id(objectData));
                        break;
                    case "xpath":
                        elements = driver.FindElements(By.XPath(objectData));
                        break;
                    case "name":
                        elements = driver.FindElements(By.Name(objectData));
                        break;
                    case "linktext":
                        elements = driver.FindElements(By.LinkText(objectData));
                        break;
                    case "partiallinktext":
                        elements = driver.FindElements(By.PartialLinkText(objectData));
                        break;
                    case "tagname":
                        elements = driver.FindElements(By.TagName(objectData));
                        break;
                    case "classname":
                        elements = driver.FindElements(By.ClassName(objectData));
                        break;
                    case "css":
                        elements = driver.FindElements(By.CssSelector(objectData));
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return elements;
        }
        #endregion

        #region [Send Arrow Down Keys]

        /// <summary>
        /// This function will simulate arrow down keys
        /// </summary>
        /// <param name="uiObject"></param>
        public static void SendArrowDownKeys(string uiObject)
        {
            try
            {
                IWebElement uiWebElement = FindWebElement(uiObject);
                if (uiWebElement != null && uiWebElement.Displayed)
                {
                    Actions action = new Actions(driver);
                    action.KeyDown(uiWebElement, Keys.ArrowDown);
                }
                else
                {
                    Console.WriteLine("{0}, object is not displayed", uiObject);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [is Element Clickable]

        /// <summary>
        /// This function will return true if the element is clickable and false if it isn't
        /// </summary>
        /// <param name="uiObject"></param>
        /// <returns></returns>
        public static Boolean isClickable(string uiObject)
        {
            try
            {
                int index = uiObject.IndexOf(':');
                string elementType = uiObject.Substring(0, index);
                string objectData = uiObject.Substring(index + 1);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                switch (elementType.ToLower())
                {
                    case "id":
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.Id(objectData)));
                        break;
                    case "xpath":
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(objectData)));
                        break;
                    case "name":
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.Name(objectData)));
                        break;
                    case "linktext":
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(objectData)));
                        break;
                    case "tagname":
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.TagName(objectData)));
                        break;
                    case "classname":
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName(objectData)));
                        break;
                    case "css":
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(objectData)));
                        break;
                }
                return true;
            }
            catch (WebDriverException e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }
        #endregion

        #region [Is Element Present]

        /// <summary>
        /// This function will return true if the element is displayed and enabled and false if it isn't
        /// </summary>
        /// <param name="uiObject"></param>
        /// <returns></returns>
        public static Boolean isElementPresentByLocator(String uiObject)
        {
            Boolean status = false;
            IWebElement elementUI = null;
            try
            {
                int index = uiObject.IndexOf(':');
                string elementType = uiObject.Substring(0, index);
                string objectData = uiObject.Substring(index + 1);

                switch (elementType.ToLower())
                {
                    case "id":
                        elementUI = driver.FindElement(By.Id(objectData));
                        break;
                    case "xpath":
                        elementUI = driver.FindElement(By.XPath(objectData));
                        break;
                    case "name":
                        elementUI = driver.FindElement(By.Name(objectData));
                        break;
                    case "linktext":
                        elementUI = driver.FindElement(By.LinkText(objectData));
                        break;
                    case "tagname":
                        elementUI = driver.FindElement(By.TagName(objectData));
                        break;
                    case "classname":
                        elementUI = driver.FindElement(By.ClassName(objectData));
                        break;
                    case "css":
                        elementUI = driver.FindElement(By.CssSelector(objectData));
                        break;
                }
                if (elementUI != null && elementUI.Enabled || elementUI.Displayed)
                    status = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
            return status;
        }
        #endregion

        #region [Check Presence Of Horizontal Scroll]

        /// <summary>
        /// This function is to check presence of horizontal scroll on the page
        /// </summary>
        public static Boolean checkPresenceOfHorizontalScroll()
        {
            try
            {
                IJavaScriptExecutor javascript = (IJavaScriptExecutor)driver;
                Boolean horzscrollStatus = (Boolean)javascript.ExecuteScript("return document.documentElement.scrollWidth>document.documentElement.clientWidth;");
                return horzscrollStatus;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                return false;
            }
        }
        #endregion

        #region [Check Presence Of Vertical Scroll]

        /// <summary>
        /// This function is to check presence of vertical scroll on the page
        /// </summary>
        /// <returns></returns>
        public static Boolean checkPresenceOfVerticalScroll()
        {
            try
            {
                IJavaScriptExecutor javascript = (IJavaScriptExecutor)driver;
                Boolean VertscrollStatus = (Boolean)javascript.ExecuteScript("return document.documentElement.scrollHeight>document.documentElement.clientHeight;");

                return VertscrollStatus;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                return false;
            }
        }
        #endregion

        #region [Scroll Down Window]

        /// <summary>
        /// This function will perform scroll down action
        /// </summary>
        public static void scrollDown()
        {
            try
            {
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript("window.scrollBy(0,400)", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region [Scroll To Element]

        /// <summary>
        /// This function will perform scroll to the element on the page
        /// </summary>
        public static void scrollingToElementofAPage(IWebElement element)
        {
            try
            {
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript("arguments[0].scrollIntoView();", element);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
            }
        }

        #endregion

        #region [Move To Element, Wait Until Visible And Then Click]

        /// <summary>
        /// This function will move the cursor to element, wait until its visible and then click on it
        /// </summary>
        /// <param name="uiObject"></param>
        public static void moveToElementAndClick(String uiObject, int waitinSeconds)
        {
            try
            {
                int index = uiObject.IndexOf(':');
                string elementType = uiObject.Substring(0, index);
                string locator = uiObject.Substring(index + 1);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(waitinSeconds));
                Actions actions = new Actions(driver);

                switch (elementType.ToLower())
                {
                    case "id":
                        actions.MoveToElement(wait.Until(ExpectedConditions.ElementIsVisible(By.Id(locator)))).Click().Build().Perform();
                        break;
                    case "xpath":
                        actions.MoveToElement(wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(locator)))).Click().Build().Perform();
                        break;
                    case "name":
                        actions.MoveToElement(wait.Until(ExpectedConditions.ElementIsVisible(By.Name(locator)))).Click().Build().Perform();
                        break;
                    case "linktext":
                        actions.MoveToElement(wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText(locator)))).Click().Build().Perform();
                        break;
                    case "partiallinktext":
                        actions.MoveToElement(wait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText(locator)))).Click().Build().Perform();
                        break;
                    case "tagname":
                        actions.MoveToElement(wait.Until(ExpectedConditions.ElementIsVisible(By.TagName(locator)))).Click().Build().Perform();
                        break;
                    case "classname":
                        actions.MoveToElement(wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(locator)))).Click().Build().Perform();
                        break;
                    case "css":
                        actions.MoveToElement(wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(locator)))).Click().Build().Perform();
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
            }

        }
        #endregion

        #region [Click Element Using JS Executor]

        /// <summary>
        /// This function will perform click action on element using java script executor
        /// </summary>
        public static void clickElementUsingJSExecutor(string uiObject)
        {
            try
            {
                IWebElement uiWebElement = FindWebElement(uiObject);
                IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                jse.ExecuteScript("arguments[0].click();", uiWebElement);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
            }
        }

        #endregion

        #region [Launch a browser and returns driver instance]
        /// <summary>
        /// Launches the specified browser and returns the driver instance
        /// Note: Supported Browsers [Chrome=0, FireFox=2, IE=1]
        /// </summary>
        /// <param name="iBrowserName"></param>
        /// <returns></returns>
        public static IWebDriver OpenBrowser(BROWSER iBrowserName)
        {
            try
            {
                string driverPath = System.IO.Directory.GetCurrentDirectory() + "\\Dependencies\\";
                switch ((int)iBrowserName)
                {
                    case 0:
                        Console.WriteLine("Launching Chrome browser");
                        ChromeOptions chromeOptions = new ChromeOptions();
                        chromeOptions.AddArgument("--start-maximized");
                        driver = new ChromeDriver(driverPath, chromeOptions);
                        break;
                    case 1:
                        Console.WriteLine("Launching Internet Explorer browser");
                        InternetExplorerOptions ieOptions = new InternetExplorerOptions();
                        ieOptions.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
                        ieOptions.EnableNativeEvents = false;
                        ieOptions.EnablePersistentHover = false;
                        ieOptions.RequireWindowFocus = true;
                        ieOptions.IgnoreZoomLevel = true;
                        driver = new InternetExplorerDriver(driverPath, ieOptions);
                        break;
                    case 2:
                        Console.WriteLine("Launching Firefox browser");
                        driver = new FirefoxDriver();
                        break;
                    default:
                        Console.WriteLine("Invalid browser option selected");
                        return null;
                }
                driver.Manage().Window.Maximize();
                return driver;
            }
            catch (WebDriverException ex)
            {
                Console.WriteLine("Driver initialization failed.");
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }
        #endregion

        #region [Returns webdriver instance]
        /// <summary>
        /// Returns Webdriver instance created using OpenBrowser function
        /// </summary>
        /// <returns></returns>
        public static IWebDriver GetDriver()
        {
            return driver;
        }
        #endregion

        #region [Click (IWebElement)]
        /// <summary>
        /// Click an object which accepts IWebElement as an argument
        /// </summary>
        /// <param name="element"></param>
        public static void Click(IWebElement element)
        {
            try
            {
                //driver.SwitchTo().Window(driver.CurrentWindowHandle);
                if (element != null && element.Enabled && element.Displayed)
                {
                    element.Click();
                }
                else
                {
                    Console.WriteLine("{0}, object is not present in UI", element);
                }
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
        #endregion

        #region [MouseOver]
        /// <summary>
        /// This function will mouseover the element
        /// </summary>
        /// <param name="uiObject">elment from UI </param>   
        public static void MouseOver(IWebElement element)
        {
            try
            {
                Actions actions = new Actions(SeleniumWrapper.driver);
                IWebElement iwebElement = element;
                actions.ClickAndHold(iwebElement).Perform();
                int num1 = 3;
                int num2 = 3;
                actions.MoveByOffset(num1, num2).Perform();
                actions.Release().Perform();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mouse over element operation failed, Exception:" + ex.Message);
            }
        }
        #endregion

        #region [SetValueForTextbox ]
        /// <summary>
        /// This function will set value for textbox accepting IWebElement as an argument
        /// </summary>
        /// <param name="uiObject">elment from UI </param>   
        public static void SetValueForTextbox(IWebElement uiObject, string text)
        {
            try
            {
                if (!CheckElementDisplayed(uiObject))
                    return;
                uiObject.Click();
                uiObject.Clear();
                uiObject.SendKeys(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw ex;
            }
        }
        #endregion

        #region [CheckElementDisplayed]
        /// <summary>
        /// This function will check if an element is displayed
        /// </summary>
        /// <param name="uiObject">elment from UI </param>   
        public static bool CheckElementDisplayed(IWebElement element)
        {
            bool status = false;
            try
            {
                if (element.Displayed)
                {
                    status = true;
                }
            }
            catch (ElementNotVisibleException ex)
            {
                Console.WriteLine("Element is not visible on the screen!");
                throw ex;
            }
            return status;
        }
        #endregion

        #region [Select]
        /// <summary>
        /// This function will perform select items from dropdown
        /// </summary>
        /// <param name="uiObject">elment from UI </param>   
        public static void Select(IWebElement element, SELECT_BY selectByValue, string sValue)
        {
            try
            {
                if (!SeleniumWrapper.CheckElementDisplayed(element))
                    return;
                Console.WriteLine("Selecting element:" + sValue);
                SelectElement selectElement = new SelectElement(element);
                switch (selectByValue)
                {
                    case SELECT_BY.VALUE:
                        selectElement.SelectByValue(sValue);
                        break;
                    case SELECT_BY.TEXT:
                        selectElement.SelectByText(sValue);
                        break;
                    case SELECT_BY.INDEX:
                        selectElement.SelectByIndex(int.Parse(sValue));
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region [CheckElementEnabled]
        /// <summary>
        /// This function will check if an element is enabled
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool CheckElementEnabled(IWebElement element)
        {
            bool status = false;
            try
            {
                if (element.Enabled)
                {
                    status = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Element is not enabled!");
                throw ex;
            }
            return status;
        }
        #endregion

        #region[IsCheckBoxSelected]
        /// <summary>
        /// Verify whether Checkbox is selected 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool IsCheckBoxSelected(IWebElement element)
        {
            bool value = false;
            try
            {
                value = element.Selected;
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }
            return value;
        }
        #endregion

        #region [SelectCheckBox]
        /// <summary>
        /// Selects the CheckBox if the checkbox is not already selected and viceversa
        /// </summary>
        /// <param name="element"></param>
        /// <param name="sValue"></param>
        public static void SelectCheckBox(IWebElement element, bool sValue)
        {
            bool isSelectedAlready = IsCheckBoxSelected(element);

            try
            {
                if (isSelectedAlready != sValue)
                {
                    element.Click();
                }
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }
        }
        #endregion

        #region [ClickByLinkText]
        /// <summary>
        /// Click on the element by using locator "LinkText"
        /// </summary>
        /// <param name="sText"></param>
        public static void ClickByLinkText(string sText)
        {
            try
            {
                driver.FindElement(By.LinkText(sText)).Click();
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }
        }
        #endregion

        #region [GetAttributeValue]
        /// <summary>
        /// Gets the attribute value from the element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string GetAttributeValue(IWebElement element, string attribute)
        {
            string value = null;
            try
            {
                value = element.GetAttribute(attribute);
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }
            return value;
        }
        #endregion

        #region [CheckIfValuePresentInDropDownUsingText]
        /// <summary>
        /// Checks whether option value is present in the dropdown using 'text' attribute
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool CheckIfValuePresentInDropDownUsingText(IWebElement element, string value)
        {
            bool present = false;
            try
            {
                SelectElement select = new SelectElement(element);
                IList<IWebElement> options = select.Options;


                for (int i = 0; i < options.Count(); i++)
                {
                    string textValue = options[i].GetAttribute("text");
                    if (textValue.ToLower() == value.ToLower())
                    {
                        present = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }
            return present;
        }
        #endregion

        #region [WaitUntilInvisibilityofElementLocated]
        /// <summary>
        /// Waits till the webelement becomes invisible
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="timeinseconds"></param>
        /// <returns></returns>
        public static bool WaitUntilInvisibilityofElementLocated(By XPath, int timeinseconds)
        {
            bool status = false;

            try
            {
                WebDriverWait wait = new WebDriverWait(SeleniumWrapper.driver, TimeSpan.FromSeconds(timeinseconds));
                status = wait.Until(ExpectedConditions.InvisibilityOfElementLocated(XPath));
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }
            return status;
        }
        #endregion

        #region [ClearText using IWebElement]
        /// <summary>
        /// Method to clear text
        /// </summary>
        /// <param name="element"></param>
        public static void ClearText(IWebElement element)
        {
            try
            {
                element.Clear();
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }
        }
        #endregion

        #region [MaximizeWindow]
        /// <summary>
        /// Method to maximize the application window
        /// </summary>
        public static void MaximizeWindow()
        {
            try
            {
                driver.Manage().Window.Maximize();
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }
        }
        #endregion

        #region [GetPageSource]
        /// <summary>
        /// Method to return page source
        /// </summary>
        /// <returns></returns>
        public static string GetPageSource()
        {
            string pageSource = string.Empty;

            try
            {
                pageSource = driver.PageSource;
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }

            return pageSource;
        }
        #endregion

        #region [GetUrl]
        /// <summary>
        /// Method to return current url
        /// </summary>
        /// <returns></returns>
        public static string GetUrl()
        {
            string url = string.Empty;

            try
            {
                url = driver.Url;
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }

            return url;
        }
        #endregion

        #region [ScrollUp]
        /// <summary>
        /// Method to scroll up the window
        /// </summary>
        public static void ScrollUp()
        {
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("scroll(0, -400);");
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }
        }
        #endregion

        #region [SwitchToFrame (IWebElement)]
        /// <summary>
        /// Method to switch to iframe 
        /// </summary>
        /// <param name="xpath"></param>
        public static void SwitchToFrame(IWebElement element)
        {
            try
            {
                driver.SwitchTo().Frame(element);
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }
        }
        #endregion

        #region [SwitchToFrame (string)]
        /// <summary>
        /// Method to switch to iframe by xpath
        /// </summary>
        /// <param name="xpath"></param>
        public static void SwitchToFrame(string xpath)
        {
            try
            {
                IWebElement element = driver.FindElement(By.XPath(xpath));
                driver.SwitchTo().Frame(element);
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }
        }
        #endregion

        #region [Wait]
        /// <summary>
        /// Method to wait for some time idle
        /// </summary>
        /// <param name="seconds"></param>
        public static void Wait(int seconds)
        {
            System.Threading.Thread.Sleep(seconds * 1000);
        }
        #endregion

        #region [SwitchToDefault]
        /// <summary>
        /// Method to switch to default frame
        /// </summary>
        public static void SwitchToDefault()
        {
            try
            {
                driver.SwitchTo().DefaultContent();
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }
        }
        #endregion

        #region [WaitForElement]
        /// <summary>
        /// Method to wait for an element for specific time
        /// </summary>
        /// <param name="element"></param>		
        /// <returns></returns>
        public static bool WaitForElement(IWebElement element, int waitinSeconds)
        {
            try
            {
                while (!CheckElementDisplayed(element) && (waitinSeconds > 0))
                {
                    Thread.Sleep(1000);
                    waitinSeconds--;
                }
                return CheckElementDisplayed(element);
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                return false;
            }
        }
        #endregion

        #region GetSelectedValueFromDropDown
        /// <summary>
        /// Method to return the selected value from dropdown list                                                                          
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetSelectedValueFromDropDown(IWebElement element)
        {
            string selectedValue = string.Empty;

            try
            {
                if (element.Enabled)
                {
                    SelectElement selectElement = new SelectElement(element);
                    selectedValue = selectElement.SelectedOption.Text;
                }
                else
                {
                    Console.WriteLine("Element is not found");
                }
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
                throw ex;
            }

            return selectedValue;
        }
        #endregion

        #region GetText
        /// <summary>
        /// Method to get text from a webelement
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string GetText(IWebElement element)
        {
            return element.Text;
        }
        #endregion

        #region GetAllValuesFromDropDown
        /// <summary>
        /// Method to return all the value from dropdown
        /// </summary>
        /// <param name="element"></param>        
        /// <returns></returns>
        public static string[] GetAllValuesFromDropDown(IWebElement element)
        {
            string[] values = null;

            try
            {
                if (element.Enabled)
                {
                    SelectElement selectElement = new SelectElement(element);
                    IList<IWebElement> options = selectElement.Options;

                    values = new string[options.Count];
                    for (int i = 0; i < options.Count; i++)
                    {
                        values[i] = options[i].Text;
                    }
                }
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
            }

            return values;
        }
        #endregion

        #region DoubleClick
        /// <summary>
        /// This function will double click on web element
        /// </summary>         
        /// <param name="element">Element from UI</param>         
        public static void DoubleClick(IWebElement element)
        {
            try
            {
                driver.SwitchTo().Window(driver.CurrentWindowHandle);

                if (element != null && element.Enabled && element.Displayed)
                {
                    Actions action = new Actions(driver);
                    action.DoubleClick(element).Build().Perform();
                }
                else
                {
                    SmartLog.Log.Info("Element is not found");
                }
            }
            catch (Exception ex)
            {
                SmartLog.Log.MethodExceptionMessage(ex);
            }

            #endregion
        }
    }

}
