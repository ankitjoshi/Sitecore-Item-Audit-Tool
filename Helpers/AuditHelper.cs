using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using Sitecore.SharedSource.ItemAuditTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.SharedSource.ItemAuditTool.Helpers
{
    public static class AuditHelper
    {
        //******************************************************************************************************
        //******************************************************************************************************

        /// <summary>
        /// GetPageTemplateList
        /// </summary>
        /// <param name="templateList">list of item templates</param>
        /// <param name="contentRootPath">Root folder path/ID of the page item</param>
        /// <param name="objDatabase">Selected database</param>
        /// <returns>Return model of template items with template status</returns>
        public static List<AuditPageTemplate> GetPageTemplateList(List<Item> templateList, string contentRootPath, Database objDatabase)
        {
            var pageTemplates = new List<AuditPageTemplate>();
            var currentItem = objDatabase.GetItem(contentRootPath);
            foreach (var templateItem in templateList)
            {
                pageTemplates.Add(new AuditPageTemplate
                {
                    Id = templateItem.ID.ToString(),
                    Name = templateItem.Name,
                    Path = templateItem.Paths.Path,
                    IsUsed = IsTemplateInUse(currentItem.Paths.Path, currentItem, templateItem, objDatabase) ? "Yes" : "No",
                    StandardValuesExists = IsTemplateStandardValueExists(templateItem) ? "Yes" : "No",
                    IsTemplateIconSet = IsTemplateIconSet(templateItem, objDatabase) ? "Yes" : "No",
                });
            }
            return pageTemplates;
        }

        //******************************************************************************************************

        /// <summary>
        /// GetDataTemplateList
        /// </summary>
        /// <param name="templateList">List of template Items (data or feature templates)</param>
        /// <param name="objDatabase">Selected database</param>
        /// <param name="globalDataItems">Global items to check the template use</param>
        /// <returns>Return list of template items</returns>
        public static List<AuditDataTemplate> GetDataTemplateList(List<Item> templateList, Database objDatabase, List<Item> globalDataItems)
        {
            var lstAuditTemplateItems = new List<AuditDataTemplate>();
            Item templateRootItem = null;
            List<Item> allTemplateItems = null;
            if (globalDataItems==null)
            {
                templateRootItem = objDatabase.GetItem("/sitecore/templates");
                allTemplateItems = templateRootItem.Axes.GetDescendants().Where(x => x.TemplateID.ToString() == Constants.TemplateIDs.DefaultTemplate).ToList();
            }

            foreach (var templateItem in templateList)
            {
                lstAuditTemplateItems.Add(new AuditDataTemplate
                {
                    Id = templateItem.ID.ToString(),
                    Name = templateItem.Name,
                    Path = templateItem.Paths.Path,
                    IsUsed = IsTemplateInUse(globalDataItems, templateItem, allTemplateItems,objDatabase) ? "Yes" : "No",
                    StandardValuesExists = IsTemplateStandardValueExists(templateItem) ? "Yes" : "No",
                    IsInsertOptionsSet = IsInsertOptionSet(templateItem, objDatabase) ? "Yes" : "No",
                    IsTemplateIconSet = IsTemplateIconSet(templateItem, objDatabase) ? "Yes" : "No",
                    IsFieldAvailable = IsFieldAvailable(templateItem) ? "Yes" : "No"
                });
            }
            return lstAuditTemplateItems;
        }

        //******************************************************************************************************

        /// <summary>
        /// GetPlaceholderSettingsList
        /// </summary>
        /// <param name="placeholderSettings">Placeholder item list</param>
        /// <returns>Return placeholder item model list</returns>
        public static List<AuditPlaceholderItem> GetPlaceholderSettingsList(List<Item> placeholderSettings)
        {
            var placeholderItems = new List<AuditPlaceholderItem>();
            try
            {
                foreach (var placeholderItem in placeholderSettings)
                {
                    placeholderItems.Add(new AuditPlaceholderItem
                    {
                        Id = placeholderItem.ID.ToString(),
                        Name = placeholderItem.Name,
                        KeyExist = placeholderItem.Fields[Constants.Fields.PlaceholderKey].Value != "" ? "Yes" : "No",
                        AllowedControls = placeholderItem.Fields[Constants.Fields.AllowedControls].Value != "" ? "Yes" : "No"
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error("*********************Sitecore Item Audit Tool: Error in GetPlaceholderSettingsList **********************", ex.Message);
            }
            return placeholderItems;
        }

        //******************************************************************************************************

        /// <summary>
        /// GetCommonFolderItemsList
        /// </summary>
        /// <param name="commonFolderItems">Folder item list</param>
        /// <returns>Folder template model list</returns>
        public static List<FolderItem> GetCommonFolderItemsList(List<Item> commonFolderItems)
        {
            var folderItems = new List<FolderItem>();
            try
            {
                foreach (var folderItem in commonFolderItems)
                {
                    folderItems.Add(new FolderItem
                    {
                        Id = folderItem.ID.ToString(),
                        Name = folderItem.Name,
                        Path = folderItem.Paths.Path
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error("*********************Sitecore Item Audit Tool: Error in GetCommonFolderItemsList **********************", ex.Message);
            }
            return folderItems;
        }

        //******************************************************************************************************

        /// <summary>
        /// GetRenderingsList
        /// </summary>
        /// <param name="renderingsList">Rendering item list</param>
        /// <returns>Returns Rendering model list</returns>
        public static List<AuditRenderingItem> GetRenderingsList(List<Item> renderingsList)
        {
            var renderingItems = new List<AuditRenderingItem>();
            try
            {
                foreach (var renderingItem in renderingsList)
                {
                    renderingItems.Add(new AuditRenderingItem
                    {
                        Id = renderingItem.ID.ToString(),
                        Name = renderingItem.Name,
                        RenderingType = GetRenderingType(renderingItem),
                        DatasourceLocation = renderingItem.Fields[Constants.Fields.DatasourceLocation].Value != "" ? "Yes" : "No",
                        DatacourceTemplate = renderingItem.Fields[Constants.Fields.DatasourceTemplate].Value != "" ? "Yes" : "No",
                        ThumbnailSet = renderingItem.Fields[Constants.Fields.Thumbnail].Value != "" ? "Yes" : "No"
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error("*********************Sitecore Item Audit Tool: Error in GetRenderingsList **********************", ex.Message);
            }
            return renderingItems;
        }

        //******************************************************************************************************

        /// <summary>
        /// GetRenderingType
        /// </summary>
        /// <param name="renderingItem">Rendering Item</param>
        /// <returns>Return rendering type (controller/view renderings)</returns>
        private static string GetRenderingType(Item renderingItem)
        {
            var renderingType = "";
            if (renderingItem.TemplateID.ToString() == Constants.TemplateIDs.ControllerRendering)
            {
                renderingType = Constants.Constant.ControllerRendering;
            }
            else if (renderingItem.TemplateID.ToString() == Constants.TemplateIDs.ViewRendering)
            {
                renderingType = Constants.Constant.ViewRendering;
            }
            else
            {
                renderingType = Constants.Constant.ToolNotSupportedRendering;
            }
            return renderingType;
        }

        //******************************************************************************************************

        private static bool IsTemplateInUse(string path, Item currentItem, Item templateItem, Database objDatabase)
        {
            try
            {
                string query = path + "//*[@@templatename = '" + templateItem.Name + "']";
                var selectedItems = objDatabase.SelectItems(query);
                if (selectedItems.Any() || currentItem.TemplateName == templateItem.Name)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("*********************Sitecore Item Audit Tool: Error in IsTemplateInUse(Page Items) **********************", ex.Message);
            }
            return false;
        }

        //******************************************************************************************************

        private static bool IsTemplateInUse(List<Item> globalDataItems, Item templateItem, List<Item> templateList, Database objDatabase)
        {
            try
            {
                if (globalDataItems == null)
                {
                    // check for feature base template here...
                    bool isUsed = false;
                    foreach (var template in templateList)
                    {
                        Template objTemplateItem = TemplateManager.GetTemplate(new ID(template.ID.ToString()), objDatabase);
                        if (objTemplateItem.GetBaseTemplates().Where(x => x.Name == templateItem.Name).Any())
                        {
                            isUsed = true;
                            break;
                        }
                    }
                    return isUsed;
                }
                else
                {
                    var temmplateItem = globalDataItems.Where(x => x.TemplateID.ToString() == templateItem.ID.ToString()).FirstOrDefault();
                    if (temmplateItem != null)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Info("*********************Sitecore Item Audit Tool: Error in IsTemplateInUse(Data) **********************", ex.Message);
            }
            return false;
        }

        //******************************************************************************************************

        private static bool IsTemplateStandardValueExists(Item templateItem)
        {
            return templateItem.Fields[Constants.Fields.StandardValues] != null && templateItem.Fields[Constants.Fields.StandardValues].Value != "";
        }

        //******************************************************************************************************

        private static bool IsFieldAvailable(Item templateItem)
        {
            TemplateItem objTemplateItem = templateItem;
            return objTemplateItem.OwnFields.Any();
        }

        //******************************************************************************************************

        private static bool IsInsertOptionSet(Item templateItem, Database objDatabase)
        {
            try
            {
                Item standardValueItem = objDatabase.GetItem(templateItem.Paths.Path + "/__Standard Values");
                if (standardValueItem != null)
                {
                    return standardValueItem.Fields[Constants.Fields.InsertOptions] != null && standardValueItem.Fields[Constants.Fields.InsertOptions].Value != "";
                }
            }
            catch (Exception ex)
            {
                Log.Error("*********************Sitecore Item Audit Tool: Error in IsInsertOptionSet **********************", ex.Message);
            }
            return false;
        }

        //******************************************************************************************************

        private static bool IsTemplateIconSet(Item templateItem, Database objDatabase)
        {
            try
            {
                if (templateItem != null)
                {
                    if ((templateItem.Fields[Constants.Fields.Icon] != null && templateItem.Fields[Constants.Fields.Icon].Value == ""))
                    {
                        // Get the standard values and check for template icon...
                        Item standardValueItem = objDatabase.GetItem(templateItem.Paths.Path + "/__Standard Values");
                        if (standardValueItem != null)
                        {
                            return standardValueItem.Fields[Constants.Fields.Icon] != null && standardValueItem.Fields[Constants.Fields.Icon].Value != "";
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("*********************Sitecore Item Audit Tool: Error in IsTemplateIconSet **********************", ex.Message);
            }
            return false;
        }

        //******************************************************************************************************

        public static bool IsPresentationSet(Item templateItem, Database objDatabase)
        {
            try
            {
                if (templateItem != null)
                {
                    if (!(templateItem.Fields[Sitecore.FieldIDs.LayoutField] != null
                           && !String.IsNullOrEmpty(templateItem.Fields[Sitecore.FieldIDs.LayoutField].Value)))
                    {
                        Item standardValueItem = objDatabase.GetItem(templateItem.Paths.Path + "/__Standard Values");
                        if (standardValueItem != null)
                        {
                            return standardValueItem.Fields[Sitecore.FieldIDs.LayoutField] != null
                           && !String.IsNullOrEmpty(standardValueItem.Fields[Sitecore.FieldIDs.LayoutField].Value);
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("*********************Sitecore Item Audit Tool: Error in IsPresentationSet **********************", ex.Message);
            }
            return false;
        }

        //******************************************************************************************************
        //******************************************************************************************************
    }
}