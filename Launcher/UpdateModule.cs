using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml.Linq;
using DRM;
using Ionic.Zip;
using UpdateHelper;

namespace Launcher
{
    public static class UpdateModule
    {
        /// <summary>
        /// Check game updates.
        /// </summary>
        public static void CheckUpdates()
        {
            // create configs folder if it doesnt exist
            if (!Directory.Exists("configs"))
            {
                Directory.CreateDirectory("configs");
            }

            // TRY: download updatelist
            try
            {
                using (WebClient Client = new WebClient())
                {
                    Client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    Client.DownloadFile(FileNames.ServerUpdateList, FileNames.UpdateList);
                }
            }
            catch
            {
                MsgBoxF.Show("Unable to check updates.", "Please check your Internet connection. If the problem persists, please contact 3LT Support.");
                return;
            }

            // TRY: read updatelist as xml, and parse the file
            XDocument updatelist;
            string[] updateStrings;
            try
            {
                updatelist = XDocument.Load(FileNames.UpdateList);
                updateStrings = ParseUpdateList(updatelist);
            }
            catch
            {
                MsgBoxF.Show("Unable to check updates.", "An error occurred when parsing the update list.");
                return;
            }

            // TRY: find game version and compare.
            try
            {
                // read the current game version
                var versionInfo = FileVersionInfo.GetVersionInfo(FileNames.GameExe);
                string version = versionInfo.ProductVersion;

                // check if it is the latest
                if (updateStrings[0] == version)
                {
                    MsgBoxF.Show("No updates available.", "You game is currently up to date.");
                }
                else
                {
                    MsgBoxF.Show(string.Format("New version available: {0}", updateStrings[0]), string.Format("Your current game version is {0}. Do you want to update your game now?\nPlease note: Do NOT interrupt the update process!", version), "UPDATE", "CANCEL", new EventHandler(UpdateConfirmClick));
                }
            }
            catch
            {
                // usually exceptions when game exe is not found
                MsgBoxF.Show("Unable to check updates.", "Failed to acquire current version number.");
                return;
            }

        }

        /// <summary>
        /// Check message of the day.
        /// </summary>
        public static void CheckFeed()
        {
            // create configs folder if it doesnt exist
            if (!Directory.Exists("configs"))
            {
                Directory.CreateDirectory("configs");
            }

            // TRY: download feedlist
            try
            {
                using (WebClient Client = new WebClient())
                {
                    Client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    Client.DownloadFile(FileNames.ServerFeedList, FileNames.FeedList);
                }
            }
            catch
            {
                MsgBoxF.Show("Unable to fetch 3LT Feeds.", "Please check your Internet connection. If the problem persists, please contact 3LT Support.", true);
                return;
            }

            // TRY: read feedlist as xml, and parse the xml
            XDocument feedlist;
            string[] feedStrings;
            try
            {
                feedlist = XDocument.Load(FileNames.FeedList);
                feedStrings = ParseFeedList(feedlist);
            }
            catch
            {
                MsgBoxF.Show("Unable to fetch 3LT Feeds.", "An error occurred when parsing the feed list.", true);
                return;
            }

            // check if title and detail is read
            if (feedStrings[0] == "" && feedStrings[1] == "")
            {
                MsgBoxF.Show("Unable to fetch 3LT Feeds.", "An error occurred when parsing the feed list.", true);
                return;
            }

            // display message
            MsgBoxF.Show(feedStrings[0], feedStrings[1], feedStrings[2], feedStrings[3], feedStrings[4], true);
        }

        /// <summary>
        /// When YES is pressed when prompted to update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UpdateConfirmClick(object sender, EventArgs e)
        {
            // dispose the window asking for update
            ((Label)sender).Parent.Dispose();

            // start the loading screen when updating
            using (FormLoading loadingForm = new FormLoading(ThreadProc_UpdateGame))
            {
                loadingForm.ShowDialog();
            }
        }

        /// <summary>
        /// Process of updating the game.
        /// </summary>
        /// <param name="loadingScreen">The loading screen associated with the process.</param>
        private static void ThreadProc_UpdateGame(Form loadingScreen)
        {
            // TRY: parse the update list to get update package url, and useHelper
            string fileURL;
            bool useHelper;

            try
            {
                // read updatelist to get the update file url
                XDocument document = XDocument.Load(FileNames.UpdateList);
                fileURL = ParseUpdateList(document)[1];
                useHelper = bool.Parse(ParseUpdateList(document)[2]);
            }
            catch
            {
                loadingScreen.Hide();
                MsgBoxF.Show("Unable to download updates.", "An error occurred when parsing the update list.", true);
                return;
            }

            // TRY: download update
            try
            {
                using (WebClient Client = new WebClient())
                {
                    Client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    Client.DownloadFile(fileURL, FileNames.UpdatePackage);
                }
            }
            catch
            {
                loadingScreen.Hide();
                MsgBoxF.Show("Unable to download updates.", "Please check your Internet connection. If the problem persists, please contact 3LT Support.", true);
                return;
            }

            // TRY: unzip the package
            try
            {
                using (ZipFile updateFile = new ZipFile(FileNames.UpdatePackage))
                {
                    updateFile.ExtractAll("./", ExtractExistingFileAction.OverwriteSilently);
                }
                // delete zip file
                if (File.Exists(FileNames.UpdatePackage)) File.Delete(FileNames.UpdatePackage);
            }
            catch
            {
                loadingScreen.Hide();
                MsgBoxF.Show("Unable to apply updates.", "Failed to unarchive the update package.", true);
                return;
            }

            // When its done...
            if (useHelper)
            {
                loadingScreen.Hide();
                //MsgBoxF.Show("Almost done...", "Launcher needs to restart to apply this update.", "", "OK", "", true);
                try
                {
                    Process.Start("UpdateHelper.exe", "0");
                }
                catch
                {
                    MsgBoxF.Show("Unable to apply updates.", "Update helper is not found.");
                }
                Application.Exit();
            }
            else
            {
                loadingScreen.Hide();
                MsgBoxF.Show("Update downloaded.", "The update has been successfully downloaded and applied.", true);
            }
        }

        /// <summary>
        /// Parse the update list.
        /// </summary>
        /// <param name="document">Updatelist XML</param>
        /// <returns>LatestVersion, PackageURL.</returns>
        public static string[] ParseUpdateList(XDocument document)
        {
            string latestStr = string.Empty, urlStr = string.Empty, useHelper = string.Empty;

            var selectedApp = from r in document.Descendants("app").Where
               (r => (string)r.Attribute("id") == Licenser.AppIDConst1)
                              select new
                              {
                                  Latest = r.Element("latest").Value,
                                  EncURL = r.Element("url").Value,
                                  UseHelper = r.Element("helper").Value,
                              };

            foreach (var r in selectedApp)
            {
                latestStr = r.Latest;
                urlStr = ListEnc.Decrypt(r.EncURL, SKeys.UpdateListPass);
                useHelper = r.UseHelper;
            }

            string[] updateStrings = new string[] { latestStr, urlStr, useHelper };

            return updateStrings;
        }

        /// <summary>
        /// Parse the feed list.
        /// </summary>
        /// <param name="document">Feedlist XML</param>
        /// <returns>Title, Detail, Button1, Button2, URL.</returns>
        public static string[] ParseFeedList(XDocument document)
        {
            string titleStr = string.Empty, detailStr = string.Empty, button1Str = string.Empty, button2Str = string.Empty, urlStr = string.Empty;

            var selectedApp = from r in document.Descendants("app").Where
               (r => (string)r.Attribute("id") == Licenser.AppIDConst1)
                              select new
                              {
                                  Title = r.Element("title").Value,
                                  Detail = r.Element("detail").Value,
                                  Button1 = r.Element("button1").Value,
                                  Button2 = r.Element("button2").Value,
                                  EncURL = r.Element("url").Value,
                              };
            foreach (var r in selectedApp)
            {
                titleStr = r.Title;
                detailStr = r.Detail;
                button1Str = r.Button1;
                button2Str = r.Button2;
                urlStr = ListEnc.Decrypt(r.EncURL, SKeys.FeedListPass);
            }

            string[] feedStrings = new string[] { titleStr, detailStr, button1Str, button2Str, urlStr };

            return feedStrings;
        }
    }
}
