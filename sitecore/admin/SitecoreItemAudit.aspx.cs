using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SharedSource.ItemAuditTool.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sitecore.SharedSource.ItemAuditTool.sitecore.admin
{
    public partial class SitecoreItemAudit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
                ClearControls();
                ClearMessages();
            }
        }

        //******************************************************************************************************

        public void BindData()
        {
            lblToolVersion.Text = Constants.Constant.ToolVersion;
            BindDatabaseDropdown();
        }

        //******************************************************************************************************

        private void BindDatabaseDropdown()
        {
            ddlDatabase.Items.Add(new ListItem()
            {
                Text = "--Select--",
                Value = "select",
                Selected = true
            });
            ddlDatabase.Items.Add(new ListItem()
            {
                Text = "master",
                Value = "master"
            });
            ddlDatabase.Items.Add(new ListItem()
            {
                Text = "web",
                Value = "web"
            });
        }
        //******************************************************************************************************

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            Log.Info("*********************Sitecore Item Audit Tool: START Item Audit Report **********************", this);

            ClearMessages();

            #region control values

            var pageTemplateLocation = txtPageTemplatesLocation.Text;
            var featureTemplateLocation = txtFeatureTemplates.Text;
            var dataTemplateLocation = txtProjectTemplates.Text;
            var renderingsRootPath = txtRenderingRootPath.Text;
            var contentRootPath = txtContentRootPath.Text;
            var placeholderRootPath = txtPlaceholderSettingsPath.Text;
            var dataItemsPath = txtDataItems.Text;
            var selectedDatabase = "master";

            #endregion

            // Get master database...
            if (ddlDatabase.SelectedIndex != 0)
            {
                selectedDatabase = ddlDatabase.SelectedValue;
            }
            Sitecore.Data.Database objDatabase = Sitecore.Configuration.Factory.GetDatabase(selectedDatabase);

            #region Page Templates

            if (!String.IsNullOrEmpty(pageTemplateLocation))
            {
                BindPageTemplateReport(pageTemplateLocation, contentRootPath, objDatabase);
            }

            #endregion

            #region Data Templates

            if (chkIsHelix.Checked)
            {
                if (!String.IsNullOrEmpty(featureTemplateLocation))
                {
                    BindFeatureTemplateReport(featureTemplateLocation, objDatabase);
                }
            }
            if (!String.IsNullOrEmpty(dataTemplateLocation))
            {
                BindDataTemplateReport(dataTemplateLocation, objDatabase);
            }

            #endregion

            #region Renderings

            if (!String.IsNullOrEmpty(renderingsRootPath))
            {
                BindRenderingsReport(renderingsRootPath, objDatabase);
            }

            #endregion

            #region PlaceholderSettings

            if (!String.IsNullOrEmpty(placeholderRootPath))
            {
                BindPlaceholderSettingsReport(placeholderRootPath, objDatabase);
            }

            #endregion

            #region Common Folder template

            if (!string.IsNullOrEmpty(dataItemsPath))
            {
                BindCommonFolderTemplateReport(dataItemsPath, objDatabase);
            }

            #endregion
            Log.Info("*********************Sitecore Item Audit Tool: END Item Audit Report **********************", this);
        }

        //******************************************************************************************************

        private void BindPageTemplateReport(string pageTemplateLocation, string contentRootPath, Database objDatabase)
        {
            var isValid = false;
            if (String.IsNullOrEmpty(contentRootPath) || !CustomHelper.isValidSource(contentRootPath, objDatabase))
            {
                if (String.IsNullOrEmpty(contentRootPath))
                {
                    lblContentError.Text = Constants.Messages.EmptyContentPath;
                }
                else
                {
                    lblContentError.Text = Constants.Messages.InvalidContentPath;
                }
                return;
            }
            var excludedPageTemplates = txtExcludePageTemplates.Text;
            var pageTemplateList = new List<Item>();
            string[] templatePaths = pageTemplateLocation.Split('|');
            var isValidExcludeTemplatePath = false;
            if (!String.IsNullOrEmpty(excludedPageTemplates))
            {
                isValidExcludeTemplatePath = CustomHelper.isValidSource(excludedPageTemplates, objDatabase);
                if (!isValidExcludeTemplatePath)
                {
                    lblExcludePageTemplateError.Text = Constants.Messages.InvalidPageTemplatePath;
                }
            }
            foreach (var path in templatePaths)
            {
                isValid = CustomHelper.isValidSource(path, objDatabase);
                if (!isValid)
                    break;
                Sitecore.Data.Items.Item pageTemplateRootItem = objDatabase.GetItem(path);
                //Add page templates only when specified path is valid...
                pageTemplateList.AddRange(pageTemplateRootItem.Axes.GetDescendants().Where(x => x.TemplateID.ToString() == Constants.TemplateIDs.DefaultTemplate && AuditHelper.IsPresentationSet(x, objDatabase)).ToList());
            }
            if (isValidExcludeTemplatePath)
            {
                var excludedItem = objDatabase.GetItem(excludedPageTemplates);
                if (excludedItem != null)
                {
                    // Exclude the items based on selected Path/ID...
                    pageTemplateList = pageTemplateList.Where(x => !x.Paths.Path.StartsWith(excludedItem.Paths.Path)).ToList();
                }
            }
            if (!isValid)
            {
                lblPagetemplateError.Text = Constants.Messages.InvalidPageTemplatePath;
            }
            else
            {
                if (pageTemplateList.Any())
                {
                    var templateList = AuditHelper.GetPageTemplateList(pageTemplateList, contentRootPath, objDatabase);
                    Log.Info("*********************Sitecore Item Audit Tool: START-Page Templates Audit **********************", this);

                    if (templateList != null && templateList.Any())
                    {
                        StringBuilder htmlTablePageTemplateString = new StringBuilder();
                        htmlTablePageTemplateString.AppendLine("<div class='table-format'>");
                        htmlTablePageTemplateString.AppendLine("<div class='table-title'>");
                        htmlTablePageTemplateString.AppendLine("Page template(s): " + templateList.Count() + "");
                        htmlTablePageTemplateString.AppendLine("</div>");
                        htmlTablePageTemplateString.AppendLine("<div class='table-section'>");
                        htmlTablePageTemplateString.AppendLine("<table cellspacing='1'>");
                        htmlTablePageTemplateString.AppendLine("<tbody>");
                        htmlTablePageTemplateString.AppendLine("<tr>");
                        htmlTablePageTemplateString.AppendLine("<th>ID</th>");
                        htmlTablePageTemplateString.AppendLine("<th>Name</th>");
                        htmlTablePageTemplateString.AppendLine("<th>Path</th>");
                        htmlTablePageTemplateString.AppendLine("<th>Template in use</th>");
                        htmlTablePageTemplateString.AppendLine("<th>Standard values exist</th>");
                        htmlTablePageTemplateString.AppendLine("<th>Icon set</th>");
                        htmlTablePageTemplateString.AppendLine("</tr>");

                        foreach (var pageTemplateItem in templateList)
                        {
                            htmlTablePageTemplateString.AppendLine("<tr>");
                            htmlTablePageTemplateString.AppendLine("<td>" + pageTemplateItem.Id + "</td>");
                            htmlTablePageTemplateString.AppendLine("<td>" + pageTemplateItem.Name + "</td>");
                            htmlTablePageTemplateString.AppendLine("<td>" + pageTemplateItem.Path + "</td>");
                            htmlTablePageTemplateString.AppendLine("<td class=" + pageTemplateItem.IsUsed + "> " + pageTemplateItem.IsUsed + "</td>");
                            htmlTablePageTemplateString.AppendLine("<td class=" + pageTemplateItem.StandardValuesExists + ">" + pageTemplateItem.StandardValuesExists + "</td>");
                            htmlTablePageTemplateString.AppendLine("<td class=" + pageTemplateItem.IsTemplateIconSet + ">" + pageTemplateItem.IsTemplateIconSet + "</td>");
                            htmlTablePageTemplateString.AppendLine("</tr>");
                        }
                        htmlTablePageTemplateString.AppendLine("</tbody>");
                        htmlTablePageTemplateString.AppendLine("</table>");
                        htmlTablePageTemplateString.AppendLine("</div>");
                        htmlTablePageTemplateString.AppendLine("</div>");
                        htmlTablePageTemplateString.AppendLine("</br>");
                        htmlTablePageTemplateString.AppendLine("</br>");
                        ltlPageTemplates.Text = htmlTablePageTemplateString.ToString();
                        Log.Info("*********************Sitecore Item Audit Tool: Page template(s) report created successfully!!**********************", this);
                    }
                    else
                    {
                        ltlPageTemplates.Text = CustomHelper.GetErrorMessage(Constants.Messages.NoPageTemplatesFound) + "<br/><br/>";
                    }
                }
                else
                {
                    ltlPageTemplates.Text = CustomHelper.GetErrorMessage(Constants.Messages.NoPageTemplatesFound) + "<br/><br/>";
                }
            }
            Log.Info("*********************Sitecore Item Audit Tool: END-Page Templates Audit **********************", this);
        }

        //******************************************************************************************************

        private void BindDataTemplateReport(string dataTemplateLocation, Database objDatabase)
        {
            var isValid = false;
            var excludedDataTemplates = txtExcludeProjectTemplates.Text;
            var dataTemplateList = new List<Item>();
            string[] templatePaths = dataTemplateLocation.Split('|');
            var isValidExcludeTemplatePath = false;
            Item excludedItem = null;
            if (!String.IsNullOrEmpty(excludedDataTemplates))
            {
                isValidExcludeTemplatePath = CustomHelper.isValidSource(excludedDataTemplates, objDatabase);
                if (!isValidExcludeTemplatePath)
                {
                    lblExcludeProjectTemplateError.Text = Constants.Messages.InvalidPageTemplatePath;
                }
                else
                {
                    excludedItem = objDatabase.GetItem(excludedDataTemplates);
                }
            }
            foreach (var path in templatePaths)
            {
                isValid = CustomHelper.isValidSource(path, objDatabase);
                if (!isValid)
                    break;
                Sitecore.Data.Items.Item dataTemplateRootItem = objDatabase.GetItem(path);
                dataTemplateList.AddRange(dataTemplateRootItem.Axes.GetDescendants().Where(x => x.TemplateID.ToString() == Constants.TemplateIDs.DefaultTemplate && !AuditHelper.IsPresentationSet(x, objDatabase)).ToList());
            }
            if (isValidExcludeTemplatePath && excludedItem != null)
            {
                // Exclude the items based on selected Path/ID...
                dataTemplateList = dataTemplateList.Where(x => !x.Paths.Path.StartsWith(excludedItem.Paths.Path)).ToList();
            }
            if (!isValid)
            {
                lblProjectTemplateError.Text = Constants.Messages.InvalidDataTemplatePath;
            }
            else
            {
                if (dataTemplateList.Any())
                {
                    var globalDataItems = new List<Item>();
                    if (String.IsNullOrEmpty(txtDataItems.Text))
                    {
                        lblGlobalItems.Text = Constants.Messages.EmptyGlobalPath;
                        return;
                    }
                    else
                    {
                        // Get global items...
                        string[] globalDataPath = txtDataItems.Text.Split('|');
                        foreach (var path in globalDataPath)
                        {
                            var isValidPath = CustomHelper.isValidSource(path, objDatabase);
                            if (!isValidPath)
                                continue;
                            Sitecore.Data.Items.Item dataParentFolderItem = objDatabase.GetItem(path);
                            globalDataItems.AddRange(dataParentFolderItem.Axes.GetDescendants().ToList());
                        }
                    }

                    var objDatatemplateList = AuditHelper.GetDataTemplateList(dataTemplateList, objDatabase, globalDataItems);
                    Log.Info("*********************Sitecore Item Audit Tool: START-Data Templates Audit **********************", this);

                    if (objDatatemplateList != null && objDatatemplateList.Any())
                    {
                        StringBuilder htmlTableDataTemplateString = new StringBuilder();
                        htmlTableDataTemplateString.AppendLine("<div class='table-format'>");
                        htmlTableDataTemplateString.AppendLine("<div class='table-title'>");
                        htmlTableDataTemplateString.AppendLine("Data template(s):  " + objDatatemplateList.Count() + "");
                        htmlTableDataTemplateString.AppendLine("</div>");
                        htmlTableDataTemplateString.AppendLine("<div class='table-section'>");
                        htmlTableDataTemplateString.AppendLine("<table cellspacing='1'>");
                        htmlTableDataTemplateString.AppendLine("<tbody>");
                        htmlTableDataTemplateString.AppendLine("<tr>");
                        htmlTableDataTemplateString.AppendLine("<th>ID</th>");
                        htmlTableDataTemplateString.AppendLine("<th>Name</th>");
                        htmlTableDataTemplateString.AppendLine("<th>Path</th>");
                        htmlTableDataTemplateString.AppendLine("<th>Template in use</th>");
                        htmlTableDataTemplateString.AppendLine("<th>Standard values exist</th>");
                        htmlTableDataTemplateString.AppendLine("<th>Insert options set</th>");
                        htmlTableDataTemplateString.AppendLine("<th>Icon set</th>");
                        htmlTableDataTemplateString.AppendLine("</tr>");

                        foreach (var templateItem in objDatatemplateList)
                        {
                            htmlTableDataTemplateString.AppendLine("<tr>");
                            htmlTableDataTemplateString.AppendLine("<td>" + templateItem.Id + "</td>");
                            htmlTableDataTemplateString.AppendLine("<td>" + templateItem.Name + "</td>");
                            htmlTableDataTemplateString.AppendLine("<td>" + templateItem.Path + "</td>");
                            htmlTableDataTemplateString.AppendLine("<td class=" + templateItem.IsUsed + ">" + templateItem.IsUsed + "</td>");
                            htmlTableDataTemplateString.AppendLine("<td class=" + templateItem.StandardValuesExists + ">" + templateItem.StandardValuesExists + "</td>");
                            htmlTableDataTemplateString.AppendLine("<td class=" + templateItem.IsInsertOptionsSet + ">" + templateItem.IsInsertOptionsSet + "</td>");
                            htmlTableDataTemplateString.AppendLine("<td class=" + templateItem.IsTemplateIconSet + ">" + templateItem.IsTemplateIconSet + "</td>");
                            htmlTableDataTemplateString.AppendLine("</tr>");
                        }
                        htmlTableDataTemplateString.AppendLine("</tbody>");
                        htmlTableDataTemplateString.AppendLine("</table>");
                        htmlTableDataTemplateString.AppendLine("</div>");
                        htmlTableDataTemplateString.AppendLine("</div>");
                        htmlTableDataTemplateString.AppendLine("</br>");
                        htmlTableDataTemplateString.AppendLine("</br>");
                        ltlDataTemplates.Text = htmlTableDataTemplateString.ToString();
                        Log.Info("*********************Sitecore Item Audit Tool: Data template(s) report created successfully!!**********************", this);
                    }
                    else
                    {
                        ltlDataTemplates.Text = CustomHelper.GetErrorMessage(Constants.Messages.NoDataTemplatesFound) + "<br/><br/>";
                    }
                }
                else
                {
                    ltlDataTemplates.Text = CustomHelper.GetErrorMessage(Constants.Messages.NoDataTemplatesFound) + "<br/><br/>";
                }
            }
            Log.Info("*********************Sitecore Item Audit Tool: END-Data Templates Audit **********************", this);
        }

        //******************************************************************************************************

        private void BindFeatureTemplateReport(string featureTemplateLocation, Database objDatabase)
        {
            var isValid = false;
            var excludedFeatureTemplates = txtExcludeFeatureTemplates.Text;
            var featureTemplateList = new List<Item>();
            string[] templatePaths = featureTemplateLocation.Split('|');
            var isValidExcludeTemplatePath = false;
            Item excludedItem = null;
            if (!String.IsNullOrEmpty(excludedFeatureTemplates))
            {
                isValidExcludeTemplatePath = CustomHelper.isValidSource(excludedFeatureTemplates, objDatabase);
                if (!isValidExcludeTemplatePath)
                {
                    lblExcludeFeatureTemplateError.Text = Constants.Messages.InvalidPageTemplatePath;
                }
                else
                {
                    excludedItem = objDatabase.GetItem(excludedFeatureTemplates);
                }
            }
            foreach (var path in templatePaths)
            {
                isValid = CustomHelper.isValidSource(path, objDatabase);
                if (!isValid)
                    break;
                Sitecore.Data.Items.Item featureTemplateRootItem = objDatabase.GetItem(path);
                featureTemplateList.AddRange(featureTemplateRootItem.Axes.GetDescendants().Where(x => x.TemplateID.ToString() == Constants.TemplateIDs.DefaultTemplate && !AuditHelper.IsPresentationSet(x, objDatabase)).ToList());
            }
            if (isValidExcludeTemplatePath && excludedItem != null)
            {
                featureTemplateList = featureTemplateList.Where(x => !x.Paths.Path.StartsWith(excludedItem.Paths.Path)).ToList();
            }
            if (!isValid)
            {
                lblFeatureTemplateError.Text = Constants.Messages.InvalidFeatureTemplatePath;
            }
            else
            {
                if (featureTemplateList.Any())
                {
                    var objFeaturetemplateList = AuditHelper.GetDataTemplateList(featureTemplateList, objDatabase, null);
                    Log.Info("*********************Sitecore Item Audit Tool: START-Feature Templates Audit**********************", this);

                    if (objFeaturetemplateList != null && objFeaturetemplateList.Any())
                    {
                        StringBuilder htmlTableDataFeatureString = new StringBuilder();
                        htmlTableDataFeatureString.AppendLine("<div class='table-format'>");
                        htmlTableDataFeatureString.AppendLine("<div class='table-title'>");
                        htmlTableDataFeatureString.AppendLine("Feature template(s):  " + objFeaturetemplateList.Count() + "");
                        htmlTableDataFeatureString.AppendLine("</div>");
                        htmlTableDataFeatureString.AppendLine("<div class='table-section'>");
                        htmlTableDataFeatureString.AppendLine("<table cellspacing='1'>");
                        htmlTableDataFeatureString.AppendLine("<tbody>");

                        htmlTableDataFeatureString.AppendLine("<tr>");
                        htmlTableDataFeatureString.AppendLine("<th>ID</th>");
                        htmlTableDataFeatureString.AppendLine("<th>Name</th>");
                        htmlTableDataFeatureString.AppendLine("<th>Path</th>");
                        htmlTableDataFeatureString.AppendLine("<th>Fields exist</th>");
                        htmlTableDataFeatureString.AppendLine("<th>Template in use</th>");
                        htmlTableDataFeatureString.AppendLine("<th>Icon set</th>");
                        htmlTableDataFeatureString.AppendLine("</tr>");

                        foreach (var templateItem in objFeaturetemplateList)
                        {
                            htmlTableDataFeatureString.AppendLine("<tr>");
                            htmlTableDataFeatureString.AppendLine("<td>" + templateItem.Id + "</td>");
                            htmlTableDataFeatureString.AppendLine("<td>" + templateItem.Name + "</td>");
                            htmlTableDataFeatureString.AppendLine("<td>" + templateItem.Path + "</td>");
                            htmlTableDataFeatureString.AppendLine("<td class=" + templateItem.IsFieldAvailable + ">" + templateItem.IsFieldAvailable + "</td>");
                            htmlTableDataFeatureString.AppendLine("<td class=" + templateItem.IsUsed + ">" + templateItem.IsUsed + "</td>");
                            htmlTableDataFeatureString.AppendLine("<td class=" + templateItem.IsTemplateIconSet + ">" + templateItem.IsTemplateIconSet + "</td>");
                            htmlTableDataFeatureString.AppendLine("</tr>");
                        }
                        htmlTableDataFeatureString.AppendLine("</tbody>");
                        htmlTableDataFeatureString.AppendLine("</table>");
                        htmlTableDataFeatureString.AppendLine("</div>");
                        htmlTableDataFeatureString.AppendLine("</div>");
                        htmlTableDataFeatureString.AppendLine("</br>");
                        htmlTableDataFeatureString.AppendLine("</br>");
                        ltlFeatureTemplates.Text = htmlTableDataFeatureString.ToString();
                        Log.Info("*********************Sitecore Item Audit Tool: Feature template(s) report created successfully!!**********************", this);
                    }
                    else
                    {
                        ltlFeatureTemplates.Text = CustomHelper.GetErrorMessage(Constants.Messages.NoFeatureTemplatesFound) + "<br/><br/>";
                    }
                }
                else
                {
                    ltlFeatureTemplates.Text = CustomHelper.GetErrorMessage(Constants.Messages.NoFeatureTemplatesFound) + "<br/><br/>";
                }
            }
            Log.Info("*********************Sitecore Item Audit Tool: END-Feature Templates Audit **********************", this);
        }

        //******************************************************************************************************

        private void BindRenderingsReport(string renderingsRootPath, Database objDatabase)
        {
            var isValid = false;
            var renderingList = new List<Item>();
            var excludedRenderinglocation = txtExcludedRenderings.Text;
            string[] renderingsPath = renderingsRootPath.Split('|');
            var isValidExcludeRenderingPath = false;
            Item excludedItem = null;
            if (!String.IsNullOrEmpty(excludedRenderinglocation))
            {
                isValidExcludeRenderingPath = CustomHelper.isValidSource(excludedRenderinglocation, objDatabase);
                if (!isValidExcludeRenderingPath)
                {
                    lblExcludeRenderingError.Text = Constants.Messages.InvalidRenderingsPath;
                }
                else
                {
                    excludedItem = objDatabase.GetItem(excludedRenderinglocation);
                }
            }

            foreach (var path in renderingsPath)
            {
                isValid = CustomHelper.isValidSource(path, objDatabase);
                if (!isValid)
                    break;
                Sitecore.Data.Items.Item renderingRootItem = objDatabase.GetItem(path);
                //Supports Controller and view renderings only as part of phase 1...
                renderingList.AddRange(renderingRootItem.Axes.GetDescendants().Where(x => x.TemplateID.ToString() == Constants.TemplateIDs.ControllerRendering || x.TemplateID.ToString() == Constants.TemplateIDs.ViewRendering).ToList());
            }

            if (isValidExcludeRenderingPath && excludedItem != null)
            {
                renderingList = renderingList.Where(x => !x.Paths.Path.StartsWith(excludedItem.Paths.Path)).ToList();
            }

            if (!isValid)
            {
                lblRenderingError.Text = Constants.Messages.InvalidRenderingsPath;
            }
            else
            {
                if (renderingList.Any())
                {
                    var renderingItemList = AuditHelper.GetRenderingsList(renderingList);
                    Log.Info("*********************Sitecore Item Audit Tool: START-Rendering Audit **********************", this);

                    if (renderingItemList != null && renderingItemList.Any())
                    {
                        StringBuilder htmlTableRenderingsString = new StringBuilder();

                        htmlTableRenderingsString.AppendLine("<div class='table-format'>");
                        htmlTableRenderingsString.AppendLine("<div class='table-title'>");
                        htmlTableRenderingsString.AppendLine("Rendering(s):  " + renderingItemList.Count() + "");
                        htmlTableRenderingsString.AppendLine("</div>");
                        htmlTableRenderingsString.AppendLine("<div class='table-section'>");
                        htmlTableRenderingsString.AppendLine("<table cellspacing='1'>");
                        htmlTableRenderingsString.AppendLine("<tbody>");
                        htmlTableRenderingsString.AppendLine("<tr>");
                        htmlTableRenderingsString.AppendLine("<th>ID</th>");
                        htmlTableRenderingsString.AppendLine("<th>Name</th>");
                        htmlTableRenderingsString.AppendLine("<th>Type</th>");
                        htmlTableRenderingsString.AppendLine("<th>Datasource location set</th>");
                        htmlTableRenderingsString.AppendLine("<th>Datasource template set</th>");
                        htmlTableRenderingsString.AppendLine("<th>Thumbnail set</th>");
                        htmlTableRenderingsString.AppendLine("</tr>");

                        foreach (var renderingItem in renderingItemList)
                        {
                            htmlTableRenderingsString.AppendLine("<tr>");
                            htmlTableRenderingsString.AppendLine("<td>" + renderingItem.Id + "</td>");
                            htmlTableRenderingsString.AppendLine("<td>" + renderingItem.Name + "</td>");
                            htmlTableRenderingsString.AppendLine("<td>" + renderingItem.RenderingType + "</td>");
                            htmlTableRenderingsString.AppendLine("<td class=" + renderingItem.DatasourceLocation + ">" + renderingItem.DatasourceLocation + "</td>");
                            htmlTableRenderingsString.AppendLine("<td class=" + renderingItem.DatacourceTemplate + ">" + renderingItem.DatacourceTemplate + "</td>");
                            htmlTableRenderingsString.AppendLine("<td class=" + renderingItem.ThumbnailSet + ">" + renderingItem.ThumbnailSet + "</td>");
                            htmlTableRenderingsString.AppendLine("</tr>");
                        }
                        htmlTableRenderingsString.AppendLine("</tbody>");
                        htmlTableRenderingsString.AppendLine("</table>");
                        htmlTableRenderingsString.AppendLine("</div>");
                        htmlTableRenderingsString.AppendLine("</div>");
                        htmlTableRenderingsString.AppendLine("</br>");
                        htmlTableRenderingsString.AppendLine("</br>");
                        ltlRenderings.Text = htmlTableRenderingsString.ToString();
                        Log.Info("*********************Sitecore Item Audit Tool: Rendering(s) report created successfully!!**********************", this);
                    }
                    else
                    {
                        ltlRenderings.Text = CustomHelper.GetErrorMessage(Constants.Messages.NoRenderingsFound) + "<br/><br/>";
                    }
                }
                else
                {
                    ltlRenderings.Text = CustomHelper.GetErrorMessage(Constants.Messages.NoRenderingsFound) + "<br/><br/>";
                }
            }
            Log.Info("*********************Sitecore Item Audit Tool: END-Rendering Audit **********************", this);
        }

        //******************************************************************************************************

        private void BindPlaceholderSettingsReport(string placeholderRootPath, Database objDatabase)
        {
            var isValid = false;
            var excludedPlaceholderPath = txtExcludePlaceholderSettings.Text;
            var placeholderSettingsList = new List<Item>();
            string[] placeholderSettingsPath = placeholderRootPath.Split('|');
            var isValidExcludePlaceholderPath = false;
            Item excludedItem = null;
            if (!String.IsNullOrEmpty(excludedPlaceholderPath))
            {
                isValidExcludePlaceholderPath = CustomHelper.isValidSource(excludedPlaceholderPath, objDatabase);
                if (!isValidExcludePlaceholderPath)
                {
                    lblPlaceholderExcludeError.Text = Constants.Messages.InvalidRenderingsPath;
                }
                else
                {
                    excludedItem = objDatabase.GetItem(excludedPlaceholderPath);
                }
            }
            foreach (var path in placeholderSettingsPath)
            {
                isValid = CustomHelper.isValidSource(path, objDatabase);
                if (!isValid)
                    break;
                Sitecore.Data.Items.Item placeholderSettingRootItem = objDatabase.GetItem(path);
                placeholderSettingsList.AddRange(placeholderSettingRootItem.Axes.GetDescendants().Where(x => x.TemplateID.ToString() == Constants.TemplateIDs.DefaultPlaceholderSettings).ToList());
            }

            if (isValidExcludePlaceholderPath && excludedItem != null)
            {
                placeholderSettingsList = placeholderSettingsList.Where(x => !x.Paths.Path.StartsWith(excludedItem.Paths.Path)).ToList();
            }

            if (!isValid)
            {
                lblPlaceholderError.Text = Constants.Messages.InvalidPlaceholderPath;
            }
            else
            {
                if (placeholderSettingsList.Any())
                {
                    var objPlaceholderSettingsList = AuditHelper.GetPlaceholderSettingsList(placeholderSettingsList);
                    Log.Info("*********************Sitecore Item Audit Tool: START-Placeholder Settings Audit **********************", this);

                    if (objPlaceholderSettingsList != null && objPlaceholderSettingsList.Any())
                    {
                        StringBuilder htmlTablePlaceholderSettingsString = new StringBuilder();
                        htmlTablePlaceholderSettingsString.AppendLine("<div class='table-format'>");
                        htmlTablePlaceholderSettingsString.AppendLine("<div class='table-title'>");
                        htmlTablePlaceholderSettingsString.AppendLine("Placeholder(s):  " + objPlaceholderSettingsList.Count() + "");
                        htmlTablePlaceholderSettingsString.AppendLine("</div>");
                        htmlTablePlaceholderSettingsString.AppendLine("<div class='table-section'>");
                        htmlTablePlaceholderSettingsString.AppendLine("<table cellspacing='1'>");
                        htmlTablePlaceholderSettingsString.AppendLine("<tbody>");
                        htmlTablePlaceholderSettingsString.AppendLine("<tr>");
                        htmlTablePlaceholderSettingsString.AppendLine("<th>ID</th>");
                        htmlTablePlaceholderSettingsString.AppendLine("<th>Name</th>");
                        htmlTablePlaceholderSettingsString.AppendLine("<th>Placeholder key present</th>");
                        htmlTablePlaceholderSettingsString.AppendLine("<th>Allowed controls set</th>");
                        htmlTablePlaceholderSettingsString.AppendLine("</tr>");

                        foreach (var placeholderItem in objPlaceholderSettingsList)
                        {
                            htmlTablePlaceholderSettingsString.AppendLine("<tr>");
                            htmlTablePlaceholderSettingsString.AppendLine("<td>" + placeholderItem.Id + "</td>");
                            htmlTablePlaceholderSettingsString.AppendLine("<td>" + placeholderItem.Name + "</td>");
                            htmlTablePlaceholderSettingsString.AppendLine("<td class=" + placeholderItem.KeyExist + ">" + placeholderItem.KeyExist + "</td>");
                            htmlTablePlaceholderSettingsString.AppendLine("<td class=" + placeholderItem.AllowedControls + ">" + placeholderItem.AllowedControls + "</td>");
                            htmlTablePlaceholderSettingsString.AppendLine("</tr>");
                        }
                        htmlTablePlaceholderSettingsString.AppendLine("</tbody>");
                        htmlTablePlaceholderSettingsString.AppendLine("</table>");
                        htmlTablePlaceholderSettingsString.AppendLine("</div>");
                        htmlTablePlaceholderSettingsString.AppendLine("</div>");
                        htmlTablePlaceholderSettingsString.AppendLine("</br>");
                        htmlTablePlaceholderSettingsString.AppendLine("</br>");
                        ltlPlaceholderSettings.Text = htmlTablePlaceholderSettingsString.ToString();
                        Log.Info("*********************Sitecore Item Audit Tool: Placeholder(s) report created successfully!!**********************", this);
                    }
                    else
                    {
                        ltlPlaceholderSettings.Text = CustomHelper.GetErrorMessage(Constants.Messages.NoPlaceholdersFound) + "<br/><br/>";
                    }
                }
                else
                {
                    ltlPlaceholderSettings.Text = CustomHelper.GetErrorMessage(Constants.Messages.NoPlaceholdersFound) + "<br/><br/>";
                }
            }
            Log.Info("*********************Sitecore Item Audit Tool: END-Placeholder Settings Audit **********************", this);
        }

        //******************************************************************************************************

        private void BindCommonFolderTemplateReport(string dataItemsPath, Database objDatabase)
        {
            var isValid = false;
            string[] commonFolderPathPath = dataItemsPath.Split('|');
            var commonFolderItemList = new List<Item>();
            foreach (var path in commonFolderPathPath)
            {
                isValid = CustomHelper.isValidSource(path, objDatabase);
                if (!isValid)
                    break;
                Sitecore.Data.Items.Item dataParentFolderItem = objDatabase.GetItem(path);
                commonFolderItemList.AddRange(dataParentFolderItem.Axes.GetDescendants().Where(x => x.TemplateID.ToString() == Constants.TemplateIDs.CommonFolder).ToList());
            }
            if (!isValid)
            {
                lblGlobalItems.Text = Constants.Messages.InvalidCommonFolderPath;
            }
            else
            {
                if (commonFolderItemList.Any())
                {
                    var objCommonFolderItemList = AuditHelper.GetCommonFolderItemsList(commonFolderItemList);
                    Log.Info("*********************Sitecore Item Audit Tool: START-Common folder template items Audit**********************", this);

                    if (objCommonFolderItemList != null && objCommonFolderItemList.Any())
                    {
                        StringBuilder htmlTableCommonFolderString = new StringBuilder();

                        htmlTableCommonFolderString.AppendLine("<div class='table-format'>");
                        htmlTableCommonFolderString.AppendLine("<div class='table-title'>");
                        htmlTableCommonFolderString.AppendLine("Common folder template Item(s):  " + objCommonFolderItemList.Count() + "");
                        htmlTableCommonFolderString.AppendLine("</div>");
                        htmlTableCommonFolderString.AppendLine("<div class='table-section'>");
                        htmlTableCommonFolderString.AppendLine("<table cellspacing='1'>");
                        htmlTableCommonFolderString.AppendLine("<tbody>");
                        htmlTableCommonFolderString.AppendLine("<tr>");
                        htmlTableCommonFolderString.AppendLine("<th>ID</th>");
                        htmlTableCommonFolderString.AppendLine("<th>Name</th>");
                        htmlTableCommonFolderString.AppendLine("<th>Path</th>");
                        htmlTableCommonFolderString.AppendLine("</tr>");

                        foreach (var folderItem in objCommonFolderItemList)
                        {
                            htmlTableCommonFolderString.AppendLine("<tr>");
                            htmlTableCommonFolderString.AppendLine("<td>" + folderItem.Id + "</td>");
                            htmlTableCommonFolderString.AppendLine("<td class='No'>" + folderItem.Name + "</td>");
                            htmlTableCommonFolderString.AppendLine("<td>" + folderItem.Path + "</td>");
                            htmlTableCommonFolderString.AppendLine("</tr>");
                        }
                        htmlTableCommonFolderString.AppendLine("</tbody>");
                        htmlTableCommonFolderString.AppendLine("</table>");
                        htmlTableCommonFolderString.AppendLine("</div>");
                        htmlTableCommonFolderString.AppendLine("</div>");
                        htmlTableCommonFolderString.AppendLine("</br>");
                        htmlTableCommonFolderString.AppendLine("</br>");
                        ltlCommonFolderItems.Text = htmlTableCommonFolderString.ToString();
                        Log.Info("*********************Sitecore Item Audit Tool: Folder template(s) report created successfully!!**********************", this);
                    }
                    else
                    {
                        ltlCommonFolderItems.Text = CustomHelper.GetErrorMessage(Constants.Messages.NoCommonFolderItemsFound) + "<br/><br/>";
                    }
                }
                else
                {
                    ltlCommonFolderItems.Text = CustomHelper.GetErrorMessage(Constants.Messages.NoCommonFolderItemsFound) + "<br/><br/>";
                }
            }
            Log.Info("*********************Sitecore Item Audit Tool: END-Common folder template items Audit **********************", this);
        }

        //******************************************************************************************************

        protected void chkIsHelix_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIsHelix.Checked)
            {
                txtFeatureTemplates.Text = "/sitecore/templates/Feature";
                txtProjectTemplates.Text = "/sitecore/templates/Project";
                featureTemplates.Visible = true;
            }
            else
            {
                txtFeatureTemplates.Text = "";
                txtProjectTemplates.Text = "";
                featureTemplates.Visible = false;
            }
        }

        //******************************************************************************************************

        private void ClearMessages()
        {
            lblContentError.Text = "";
            lblPagetemplateError.Text = "";
            lblExcludePageTemplateError.Text = "";
            lblProjectTemplateError.Text = "";
            lblExcludeProjectTemplateError.Text = "";
            lblFeatureTemplateError.Text = "";
            lblExcludeFeatureTemplateError.Text = "";
            lblRenderingError.Text = "";
            lblExcludeRenderingError.Text = "";
            lblPlaceholderError.Text = "";
            lblPlaceholderExcludeError.Text = "";
            lblGlobalItems.Text = "";
            ltlDataTemplates.Text = "";
            ltlFeatureTemplates.Text = "";
            ltlPageTemplates.Text = "";
            ltlPlaceholderSettings.Text = "";
            ltlRenderings.Text = "";
            ltlCommonFolderItems.Text = "";
        }

        //******************************************************************************************************

        private void ClearControls()
        {
            txtContentRootPath.Text = "";
            txtDataItems.Text = "";
            txtExcludedRenderings.Text = "";
            txtExcludeFeatureTemplates.Text = "";
            txtExcludePageTemplates.Text = "";
            txtExcludePlaceholderSettings.Text = "";
            txtExcludeProjectTemplates.Text = "";
            txtFeatureTemplates.Text = "";
            txtPageTemplatesLocation.Text = "";
            txtPlaceholderSettingsPath.Text = "";
            txtProjectTemplates.Text = "";
            txtRenderingRootPath.Text = "";
        }

        //******************************************************************************************************

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearMessages();
            ClearControls();
        }

        //******************************************************************************************************
        //******************************************************************************************************
    }
}