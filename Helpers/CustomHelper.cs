using Sitecore.Data;
using System.Text;

namespace Sitecore.SharedSource.ItemAuditTool.Helpers
{
    public static class CustomHelper
    {
        //******************************************************************************************************

        /// <summary>
        /// isValidSource
        /// </summary>
        /// <param name="path">Seelcted path</param>
        /// <param name="objDatabase">Selected database</param>
        /// <returns>Returns if item source is valid</returns>
        public static bool isValidSource(string path, Database objDatabase)
        {
            bool isValid = true;
            if (!Sitecore.Data.ID.IsID(path))
            {
                return IsValidPath(path, objDatabase);
            }
            return isValid;
        }

        //******************************************************************************************************

        /// <summary>
        /// IsValidPath
        /// </summary>
        /// <param name="path">Sitecore item path</param>
        /// <param name="objDatabase">Selected database</param>
        /// <returns>Returns if item path is valid... </returns>
        private static bool IsValidPath(string path, Database objDatabase)
        {
            bool isValid = false;
            var validItem = objDatabase.GetItem(path);
            if (validItem != null)
            {
                isValid = true;
            }
            return isValid;
        }

        //******************************************************************************************************

        public static string GetErrorMessage(string message)
        {
            StringBuilder htmlTableErrorMessageString = new StringBuilder();
            htmlTableErrorMessageString.AppendLine("<div class='not-found-error'>");
            htmlTableErrorMessageString.AppendLine("<span>" + message + "</span>");
            htmlTableErrorMessageString.AppendLine("</div>");
            return htmlTableErrorMessageString.ToString();
        }

        //******************************************************************************************************
        //******************************************************************************************************
    }
}