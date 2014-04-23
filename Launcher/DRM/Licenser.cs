using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DRM
{
    public static class Licenser
    {
        public const string AppIDConst1 = "0001";
        private const string AppIDConst2 = "0001";

        /// <summary>
        /// Checks if license key is valid
        /// </summary>
        /// <param name="license">Product key</param>
        /// <returns>True if valid.</returns>
        public static bool CheckLicense(string license)
        {
            bool offlineCheck = false;

            // get real product key
            string realKey = GetProductKey(GetActivationCode());

            // check if license is legit
            offlineCheck = (license == realKey);

            return (offlineCheck);
        }

        /// <summary>
        /// Gets activation code of current MAC address
        /// </summary>
        /// <returns>Activation code (encrypted)</returns>
        public static string GetActivationCode()
        {
            // acquire clear text
            string clearText = AppIDConst1 + IdentifierC.GetMacAddress() + AppIDConst2;

            // Encrypt clear text
            string encrypted = Keygen.Encrypt(clearText, SKeys.rgbIV1, SKeys.key1);

            // return hashed encrypted text
            return Keygen.CalculateMD5Hash(encrypted);
        }

        /// <summary>
        /// Gets the product key of a specified activation code.
        /// </summary>
        /// <param name="actCode">Activation code.</param>
        /// <returns>Product key (encrypted)</returns>
        internal static string GetProductKey(string actCode)
        {
            // encrypt activation code to get product key
            string productKey = Keygen.Encrypt(actCode, SKeys.rgbIV2, SKeys.key2);

            // return hashed
            return Keygen.CalculateMD5Hash(productKey);
        }

        /// <summary>
        /// Gets the time code.
        /// </summary>
        /// <returns>Time code (encrypted)</returns>
        internal static string GetTimeCode()
        {
            string timeCode = string.Empty;
            string timeString = DateTime.Now.ToString();
            timeCode = Keygen.Encrypt(timeString, SKeys.rgbIV3, SKeys.key3);

            return timeCode;
        }

        /// <summary>
        /// Check if time code is valid.
        /// </summary>
        /// <param name="pastTimeCodeStrEnc">time code to check (encrypted)</param>
        /// <returns></returns>
        public static bool CheckTimeCode(string pastTimeCodeStrEnc)
        {
            bool valid = false;
            int offset = SKeys.TimeOffset;

            // decrypt past time code string
            string pastTimeStr = Keygen.Decrypt(pastTimeCodeStrEnc, SKeys.rgbIV3, SKeys.key3);
            string nowTimeStr = DateTime.Now.ToString();

            DateTime nowTime, pastTime;
            if (DateTime.TryParse(nowTimeStr, out nowTime) && DateTime.TryParse(pastTimeStr, out pastTime))
            {
                TimeSpan timeDiff = nowTime - pastTime;
                if (timeDiff.TotalSeconds <= offset)
                {
                    valid = true;
                }
                else
                {
                    valid = false;
                }
            }
            else
            {
                return false;
            }

            return valid;
        }

        /// <summary>
        /// Checks the DRM Code lists.
        /// </summary>
        /// <param name="codeList">Activation code list.</param>
        /// <returns></returns>
        public static bool CheckDRMList(List<string> codeList)
        {
            if (codeList.Count == 0) return false;
            bool valid = false;


            foreach (string actCode in codeList)
            {
                if (actCode == Licenser.GetActivationCode())
                {
                    valid = true;
                }
            }

            return valid;
        }

        /// <summary>
        /// Parse the drm list.
        /// </summary>
        /// <param name="document">Encrypted XML of drm list.</param>
        /// <returns>A list of valid activation codes.</returns>
        public static List<string> ParseDRMList(XDocument document)
        {
            List<string> actCodeList = new List<string>();

            var selectedApp = from r in document.Descendants("app").Where
               (r => (string)r.Attribute("id") == Licenser.AppIDConst1)
                              select new
                              {
                                  Codes = r.Descendants("code"),
                              };

            foreach (var r in selectedApp)
            {
                // decrypt Code
                foreach (string code in r.Codes)
                {
                    string clearCode = ListEnc.Decrypt(code, SKeys.DRMListPass);
                    actCodeList.Add(clearCode);
                }
            }

            return actCodeList;
        }
    }
}
