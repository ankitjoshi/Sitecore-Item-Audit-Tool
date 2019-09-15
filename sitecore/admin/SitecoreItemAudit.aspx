<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SitecoreItemAudit.aspx.cs" Inherits="Sitecore.SharedSource.ItemAuditTool.sitecore.admin.SitecoreItemAudit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sitecore Item Audit Tool</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scriptManagerItemAudit" runat="server"></asp:ScriptManager>
        <div class="page-title">
            Sitecore Item Audit Tool
        </div>
        <asp:UpdatePanel ID="updatePanelToolBody" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="form-section">
                    <div>
                        <b>***Note: Please review the Readme.txt file to know field details</b>
                    </div>
                    <div>
                        <span id="lblDatabase">Database</span>
                        <asp:DropDownList ID="ddlDatabase" runat="server"></asp:DropDownList>
                        <span id="databaseDescription">(If left blank, master database will be used)</span>
                    </div>
                    <div>
                        <span id="lblHelixProject">Is Project Helix Based</span>
                        <asp:CheckBox ID="chkIsHelix" runat="server" OnCheckedChanged="chkIsHelix_CheckedChanged" AutoPostBack="true" />
                        <span id="lblIsHelix"></span>
                    </div>
                    <div>
                        <span id="lblContentRootPath">Root path/ID(Page Items)<div class="error-message">
                            <asp:Label ID="lblContentError" runat="server"></asp:Label>
                        </div>
                        </span>
                        <asp:TextBox ID="txtContentRootPath" runat="server" ToolTip="Only one path/ID is allowed"></asp:TextBox>
                        <span id="lblDataItems">Root path/ID(Global Items)<div class="error-message">
                            <asp:Label ID="lblGlobalItems" runat="server"></asp:Label>
                        </div>
                        </span>
                        <asp:TextBox ID="txtDataItems" runat="server" ToolTip="Multiple Ids or path can be used using pipe delimiter"></asp:TextBox>
                    </div>
                    <div>
                        <span id="lblIncludePageTemplates">Page template folder path/ID<div class="error-message">
                            <asp:Label ID="lblPagetemplateError" runat="server"></asp:Label>
                        </div>
                        </span>
                        <asp:TextBox ID="txtPageTemplatesLocation" runat="server" ToolTip="Multiple Ids or path can be used using pipe delimiter"></asp:TextBox>
                        <span id="lblExcludePageTemplates">Page template folder path/ID to exclude<div class="error-message">
                            <asp:Label ID="lblExcludePageTemplateError" runat="server"></asp:Label>
                        </div>
                        </span>
                        <asp:TextBox ID="txtExcludePageTemplates" runat="server" ToolTip="Only one ID/path is allowed"></asp:TextBox>
                    </div>
                    <div id="featureTemplates" runat="server" visible="false">
                        <span id="lblFeatureTemplates">Feature template folder path/ID<div class="error-message">
                            <asp:Label ID="lblFeatureTemplateError" runat="server"></asp:Label>
                        </div>
                        </span>
                        <asp:TextBox ID="txtFeatureTemplates" runat="server" ToolTip="Multiple Ids or path can be used using pipe delimiter"></asp:TextBox>
                        <span>Feature template folder path/ID to exclude
                    <div class="error-message">
                        <asp:Label ID="lblExcludeFeatureTemplateError" runat="server"></asp:Label>
                    </div>
                        </span>
                        <asp:TextBox ID="txtExcludeFeatureTemplates" runat="server" ToolTip="Only one ID/path is allowed"></asp:TextBox>
                    </div>
                    <div>
                        <span id="lblProjectTemplates">Data template folder path/ID<div class="error-message">
                            <asp:Label ID="lblProjectTemplateError" runat="server"></asp:Label>
                        </div>
                        </span>
                        <asp:TextBox ID="txtProjectTemplates" runat="server" ToolTip="Multiple Ids or path can be used using pipe delimiter"></asp:TextBox>
                        <span id="lblExcludeProjectTemplates">Data template folder path/ID to exclude<div class="error-message">
                            <asp:Label ID="lblExcludeProjectTemplateError" runat="server"></asp:Label>
                        </div>
                        </span>
                        <asp:TextBox ID="txtExcludeProjectTemplates" runat="server" ToolTip="Only one ID/path is allowed"></asp:TextBox>
                    </div>
                    <div>
                        <span id="lblRenderingsRootPath">Rendering folder path/ID<div class="error-message">
                            <asp:Label ID="lblRenderingError" runat="server"></asp:Label>
                        </div>
                        </span>
                        <asp:TextBox ID="txtRenderingRootPath" runat="server" ToolTip="Multiple Ids or path can be used using pipe delimiter"></asp:TextBox>
                        <span id="lblExcludedRenderings">Rendering folder path/ID to exclude<div class="error-message">
                            <asp:Label ID="lblExcludeRenderingError" runat="server"></asp:Label>
                        </div>
                        </span>
                        <asp:TextBox ID="txtExcludedRenderings" runat="server" ToolTip="Only one ID/path is allowed"></asp:TextBox>
                    </div>

                    <div>
                        <span id="lblPlaceholderSettingsPath">Placeholder folder path/ID<div class="error-message">
                            <asp:Label ID="lblPlaceholderError" runat="server"></asp:Label>
                        </div>
                        </span>
                        <asp:TextBox ID="txtPlaceholderSettingsPath" runat="server" ToolTip="Multiple Ids or path can be used using pipe delimiter"></asp:TextBox>
                        <span id="lblExcludePlaceholderSettings">Placeholder folder path/ID to exclude<div class="error-message">
                            <asp:Label ID="lblPlaceholderExcludeError" runat="server"></asp:Label>
                        </div>
                        </span>
                        <asp:TextBox ID="txtExcludePlaceholderSettings" runat="server" ToolTip="Only one ID/path is allowed"></asp:TextBox>
                    </div>
                    <div class="buttonRow">
                        <asp:Button ID="btnClear" runat="server" Text="Clear" OnClick="btnClear_Click" ToolTip="Reset field values" />
                        <asp:Button ID="btnGenerateReport" runat="server" Text="Generate Report" OnClick="btnGenerateReport_Click" ToolTip="Generate Report" />
                    </div>
                </div>
                <br />
                <br />
                <asp:UpdateProgress ID="updateProgress" runat="server">
                    <ProgressTemplate>
                        <div class="processingMessage">
                            <asp:Label ID="lblProcessing" runat="server" Text="Processing request...!! Please wait!!" />
                            <br />
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <asp:Literal ID="ltlPageTemplates" runat="server" />
                <asp:Literal ID="ltlDataTemplates" runat="server" />
                <asp:Literal ID="ltlCommonFolderItems" runat="server" />
                <asp:Literal ID="ltlFeatureTemplates" runat="server" />
                <asp:Literal ID="ltlRenderings" runat="server" />
                <asp:Literal ID="ltlPlaceholderSettings" runat="server" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnClear" />
                <asp:AsyncPostBackTrigger ControlID="btnGenerateReport" />
            </Triggers>
        </asp:UpdatePanel>

        <div class="information">
            <i class="fa fa-info-circle"></i>
            <div class="information-text">
                Review the status of different columns which is highlighted in red and revist the item again to confirm the status and fix if applicable.
            </div>
        </div>
        <br />
        <br />
        <br />
        <br />
        <br />
        <div class="footer">
            <div class="footer-left">
                Current Date:
                <div id="currentDate"></div>
            </div>
            <div class="footer-right">
                Sitecore Item Audit Tool
                <div>
                    Version: <b>
                        <asp:Label ID="lblToolVersion" runat="server"></asp:Label></b>
                </div>
            </div>
        </div>
    </form>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
    <script>
        $(document).ready(function () {
            $("#btnGenerateReport").on('click', function () {
                $(".loader").show();
                setTimeout(function () { $(".loader").hide(); }, 3000);
            });
            $(".information").on('click', function () {
                $(".information").animate({ right: '0' });
            });
            $(document).mouseup(function (e) {
                var container = $(".information");
                if (!container.is(e.target) && container.has(e.target).length === 0) {
                    $(".information").animate({ right: '-266px' });
                }
            });

            var dNow = new Date();
            var localdate = (dNow.getMonth() + 1) + '/' + dNow.getDate() + '/' + dNow.getFullYear() + ' ' + dNow.getHours() + ':' + dNow.getMinutes();
            $('#currentDate').text(localdate)

        });
    </script>
    <style>
        body {
            font-family: "Trebuchet MS", Arial, Helvetica, sans-serif;
            font-size: 12px;
            margin: 0;
            padding: 0;
        }

        .page-title {
            font-size: 24px;
            text-align: center;
            padding: 20px 0;
            clear: both;
            background: #161c27;
            color: #fff;
            margin-bottom: 15px;
        }

        body form {
            max-width: 1000px;
            width: 100%;
            margin: 0 auto;
        }

        .table-format {
            border: 1px solid #000;
            padding: 20px 10px 10px 10px;
            position: relative;
        }

            .table-format .table-title {
                position: absolute;
                background: #fff;
                padding: 6px 10px;
                top: -16px;
                font-weight: bold;
                font-size: 16px;
                color: #000;
            }

            .table-format .table-section {
                max-height: 300px;
                min-width: 800px;
                overflow: auto;
            }

            .table-format table {
                width: 100%;
            }

            .table-format td, .table-format th {
                border-bottom: 1px solid #ddd;
                padding: 8px;
                font-size: 12px;
                font-family: "Trebuchet MS", Arial, Helvetica, sans-serif;
            }

            .table-format tr:nth-child(even) {
                background-color: #f2f2f2;
            }

            .table-format tr:hover {
                background-color: #ddd;
            }

            .table-format th {
                padding-top: 12px;
                padding-bottom: 12px;
                text-align: left;
                background-color: #4CAF50;
                color: white;
            }

        .footer, .header {
            color: rgba(255,255,255,.6);
            background-color: #161c27;
            width: 100%;
            float: left;
            box-sizing: border-box;
        }

        .header {
            padding: 15px;
        }

        .footer-left {
            display: inline-block;
            width: 49%;
            padding: 10px 20px;
            float: left;
            box-sizing: border-box;
        }

        .footer-right {
            display: inline-block;
            width: 49%;
            padding: 10px 20px;
            float: right;
            text-align: right;
            box-sizing: border-box;
        }

            .footer-right ul {
                margin: 0;
                padding: 10px 20px;
            }

                .footer-right ul li {
                    display: inline-block;
                    list-style: none;
                    padding: 0 5px;
                }

                    .footer-right ul li a {
                        color: rgba(255,255,255,.6);
                        text-decoration: none;
                    }

        .form-section div {
            width: 100%;
            display: inline-block;
            padding: 10px 0;
        }

            .form-section div .error-message {
                padding: 0;
                color: red;
            }

                .form-section div .error-message span {
                    display: block !important;
                    width: 100% !important;
                    padding-left: 0 !important;
                }

        .not-found-error {
            color: red;
            padding: 0 20px;
            font-size: 14px;
            line-height: 24px;
        }

        .form-section div:nth-child(even) {
            background-color: #f2f2f2;
        }

        input[type=text], select, textarea {
            width: 25%;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 4px;
            box-sizing: border-box;
            resize: vertical;
        }

        input[type="submit"] {
            background-color: #4CAF50;
            color: white;
            padding: 12px 20px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            float: right;
            margin-bottom: 20px;
        }

        #btnClear {
            float: left;
            background: gray;
        }

        .form-section div span {
            width: 24%;
            display: inline-block;
            vertical-align: middle;
            padding-left: 10px;
            box-sizing: border-box;
        }

        .loader {
            background: rgba(0,0,0,.4);
            position: fixed;
            width: 100%;
            height: 100%;
            z-index: 10;
            left: 0;
            top: 0;
            display: none;
        }

            .loader img {
                position: absolute;
                top: 50%;
                left: 50%;
                transform: translate(-50%, -50%);
            }

        .information {
            background: #161c27;
            padding: 5px 10px;
            position: fixed;
            right: 0;
            width: 300px;
            color: #fff;
            top: 100px;
            right: -266px;
        }

        .fa-info-circle {
            font-size: 40px;
            color: red;
            vertical-align: middle;
            cursor: pointer;
        }

        .information-text {
            display: inline-block;
            width: 200px;
            padding-left: 15px;
            vertical-align: middle;
        }

        .Yes {
            color: green;
        }

        .No {
            color: red;
        }

        .buttonRow {
            background-color: #f2f2f2;
        }

        .processingMessage {
            font-size: 20px;
            color: blue;
            vertical-align: middle;
            text-align: center
        }
    </style>
</body>
</html>
