
namespace Sitecore.SharedSource.ItemAuditTool.Constants
{
    public struct Fields
    {
        /// <summary>
        /// Standard Values
        /// </summary>
        public static readonly string StandardValues = "__Standard Values";
        /// <summary>
        /// Insert Options
        /// </summary>
        public static readonly string InsertOptions = "__Masters";
        /// <summary>
        /// Icon
        /// </summary>
        public static readonly string Icon = "__Icon";
        /// <summary>
        /// Datasource Location
        /// </summary>
        public static readonly string DatasourceLocation = "Datasource Location";
        /// <summary>
        /// Datasource Template
        /// </summary>
        public static readonly string DatasourceTemplate = "Datasource Template";
        /// <summary>
        /// Thumbnail
        /// </summary>
        public static readonly string Thumbnail = "__Thumbnail";
        public static readonly string PlaceholderKey = "Placeholder Key";
        public static readonly string AllowedControls = "Allowed Controls";

    }

    //******************************************************************************************************

    public struct TemplateIDs
    {
        /// <summary>
        /// TemplateIDs
        /// </summary>
        public static readonly string ControllerRendering = "{2A3E91A0-7987-44B5-AB34-35C2D9DE83B9}";
        public static readonly string ViewRendering = "{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}";
        public static readonly string DefaultTemplate = "{AB86861A-6030-46C5-B394-E8F99E8B87DB}";
        public static readonly string DefaultPlaceholderSettings = "{5C547D4E-7111-4995-95B0-6B561751BF2E}";
        public static readonly string CommonFolder = "{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}";
        public static readonly string RenderingFolder = "{7EE0975B-0698-493E-B3A2-0B2EF33D0522}";
    }

    //******************************************************************************************************

    public struct Constant
    {
        /// <summary>
        /// Constant
        /// </summary>
        public static readonly string ControllerRendering = "Controller Rendering";
        public static readonly string ViewRendering = "View Rendering";
        public static readonly string ToolNotSupportedRendering = "N/A";
        public static readonly string ToolVersion = "1.0";
    }

    //******************************************************************************************************

    public struct Messages
    {
        /// <summary>
        /// Messages
        /// </summary>
        public static readonly string NoPageTemplatesFound = "No Page template(s) found!!";
        public static readonly string NoDataTemplatesFound = "No Data template(s) found!!";
        public static readonly string NoFeatureTemplatesFound = "No Feature template(s) found!!";
        public static readonly string NoRenderingsFound = "No Rendering(s) found!!";
        public static readonly string NoPlaceholdersFound = "No Placeholder(s) found!!";
        public static readonly string NoCommonFolderItemsFound = "No Common folder template item(s) found!!";
        public static readonly string EmptyContentPath = "Field required";
        public static readonly string EmptyGlobalPath = "Field required";

        // Invalid content messages... different variables just in case want to show different message for different content type...
        public static readonly string InvalidContentPath = "Invalid Path/ID";
        public static readonly string InvalidPageTemplatePath = "Invalid Path/ID";
        public static readonly string InvalidDataTemplatePath = "Invalid Path/ID";
        public static readonly string InvalidFeatureTemplatePath = "Invalid Path/ID";
        public static readonly string InvalidRenderingsPath = "Invalid Path/ID";
        public static readonly string InvalidPlaceholderPath = "Invalid Path/ID";
        public static readonly string InvalidCommonFolderPath = "Invalid Path/ID";
    }

    //******************************************************************************************************
    //******************************************************************************************************
}