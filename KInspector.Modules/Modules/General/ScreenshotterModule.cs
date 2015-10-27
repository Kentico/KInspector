using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using JSErrorCollector;
using Kentico.KInspector.Core;
using OpenQA.Selenium.Firefox;

namespace Kentico.KInspector.Modules
{
    public class ScreenshotterModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            {
                Name = "Screenshotter",
                SupportedVersions = new[] { 
                    new Version("5.5"),
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2")
                },
                Comment = @"Screenshotting started. It will be saved into your Desktop folder. You can see actual progress in a console window.",
            };
        }


        public ModuleResults GetResults(InstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var urls = dbService.ExecuteAndGetTableFromFile("ScreenshotterModule.sql");

            // Start process in separate thread to make website responsive.
            Thread t = new Thread(StartScreenshotting);
            t.Start(new object[] { instanceInfo, urls });

            return new ModuleResults
            {
                Result = urls
            };
        }


        public void StartScreenshotting(object parameters)
        {
            var parameter = parameters as object[];
            InstanceInfo instanceInfo = parameter[0] as InstanceInfo;
            DataTable urls = parameter[1] as DataTable;

            Log("Starting firefox...");
            FirefoxProfile ffProfile = new FirefoxProfile();
            JavaScriptError.AddExtension(ffProfile);
            
            using (var browser = new FirefoxDriver(ffProfile))
            {
                string targetDirectory = CreateTargetDirectory(instanceInfo);
                List<JavaScriptError> jsErrors = new List<JavaScriptError>();

                for (int i = 0; i < urls.Rows.Count; i++)
                {
                    try
                    {
                        Guid nodeGuid = (Guid)urls.Rows[i]["NodeGUID"];
                        Uri url = new Uri(instanceInfo.Uri, "getdoc/" + nodeGuid);

                        Log("Screenshotting [{0}/{1}]: {2}", i, urls.Rows.Count, nodeGuid);

                        browser.Navigate().GoToUrl(url);
                        string fileName = GetFileName(targetDirectory, browser.Url);
                        browser.GetScreenshot()
                            .SaveAsFile(fileName, ImageFormat.Jpeg);

                        jsErrors.AddRange(JavaScriptError.ReadErrors(browser));
                    }
                    catch (Exception e)
                    {
                        Log("Exception: {0}", e.Message);
                    }
                }
                
                SaveJavaScriptErrorsToFile(jsErrors, targetDirectory);
                Log("Screenshotting finished.");
                browser.Close();
            }
        }


        private static string CreateTargetDirectory(InstanceInfo config)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string targetDirectory = desktopPath + "\\" + SanitizeFileName(config.Uri.ToString()) + "\\";

            Log("Creating target directory: {0}", targetDirectory);
            Directory.CreateDirectory(targetDirectory);

            return targetDirectory;
        }


        private static string GetFileName(string targetDirectory, string urlPath)
        {
            const string EXTENSION = ".jpg";
            string filePath = targetDirectory + "\\" + SanitizeFileName(urlPath) + EXTENSION;

            const int MAX_WINDOWS_FILENAME_LENGTH = 248;
            if (filePath.Length > MAX_WINDOWS_FILENAME_LENGTH)
            {
                filePath = filePath.Substring(0, MAX_WINDOWS_FILENAME_LENGTH - EXTENSION.Length);
                filePath += EXTENSION;
            }

            return filePath;
        }


        private static string SanitizeFileName(string fileName)
        {
            var invalids = Path.GetInvalidFileNameChars();
            return String.Join("_", fileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }


        private static void Log(string message)
        {
            Console.WriteLine("[{0}]: {1}", DateTime.Now.ToString(), message);
        }


        private static void Log(string message, params object[] args)
        {
            Console.WriteLine("[{0}]: {1}", DateTime.Now.ToString(), String.Format(message, args));
        }


        private void SaveJavaScriptErrorsToFile(IList<JavaScriptError> jsErrors, string targetDirectory)
        {
            if (jsErrors.Count <= 0)
            {
                return;
            }

            var errors = jsErrors.Select(javaScriptError => javaScriptError.ToString()).ToList();
            File.AppendAllLines(targetDirectory + "JsErrors.txt", errors);
        }
    }
}
