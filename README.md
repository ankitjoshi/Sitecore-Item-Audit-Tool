# Sitecore-Item-Audit-Tool
Sitecore Item Audit Tool

This tool is to Audit Sitecore Items which is part of your Sitecore Instance and check following types of contents:

1) Page templates
2) Data templates
3) Feature templates
4) Rendering items
5) Placeholder settings
6) Common folter template items

This tool will give you an overview of different parameters associated with your content type like:

1) For page template- if presentation has been set or not.
2) If standard values exists.
3) If Icon exists.
4) If specific template is used (Page/data/features)
5) If insert options has been set
6) How many items has been created using default "Folder" template.


# RELEASE NOTES
This package installs the following to your Sitecore instance:- 

•	\bin\Sitecore.SharedSource.ItemAuditTool.dll

•	\sitecore\admin\SitecoreItemAudit.aspx


# How to use

Once package installed- tool can be accessed like- http://yourhostname/sitecore/admin/SitecoreItemAudit.aspx

Fields description:

1) Root path/ID(Page Items)- This field is to specify Path or ID under which your page items(which has presentation) exists.
	For example- If you site structure is sitecore->content->home (under which you have all your page items) then you would have to set the ID of home item or path of home item to this field.
	
2) Page template folder path/ID- This field is to specify the folder path or ID under which you have your page templates created, for example- if your template structure is- /sitecore/templates/Project/SiteName/Page Types, 
then you would have to set this path or ID of this to this field.
If you have multiple locations where your page templates are stored, then you can pass multiple IDs or paths to this field.
Root path/ID(Page Items) field is required to show the correct report of page templates and once done you will have the following details from each template:
* ID
* Name
* Path
* Template in use
* Standard values exists
* Icon set

3) Page template folder path/ID to exclude- This field is to specify the folder path or ID which you want to exclude from your Page template report.
Note- Only one path/ID is allowed to set here.


4) Root path/ID(Global Items)- This field is to specify Path or ID under which your data items(global/data items) exists.
	For example- If you site structure is sitecore->content->Global (under which you have all your global items) then you would have to set the ID of Global item or path of Global item to this field.

5) Data template folder path/ID- This field is to specify the folder path or ID under which you have your data templates created, for example- if your template structure is- /sitecore/templates/Project/SiteName/Content Types, 
then you would have to set this path or ID of this item to this field.
If you have multiple locations where your data templates are stored, then you can pass multiple IDs or paths to this field.
Root path/ID(Global Items) field is required to show the correct report of data templates and once done you will have the following details from each template:
* ID
* Name
* Path
* Template in use (this is based on if any item found under global folder whcih is based on specific template)
* Standard values exists
* Insert options set
* Icon set

6) Data template folder path/ID to exclude- This field is to specify the folder path or ID which you want to exclude from your data template report.
Note- Only one path/ID is allowed to set here.

7) Rendering folder path/ID - This field is to specify the folder path or ID under which you have your renderings created, for example- if your rendering structure is- /sitecore/layout/Renderings/Feature, 
then you would have to set this path or ID of this item to this field.
If you have multiple locations where your renderings are stored, then you can pass multiple IDs or paths to this field and once done you will have the following details for each item:
* ID
* Name
* Type
* Datasource location set
* Datasource template set
* Thumbnail set


8) Rendering folder path/ID to exclude- This field is to specify the folder path or ID which you want to exclude from your rendering report.
Note- Only one path/ID is allowed to set here.

9) Placeholder folder path/ID - This field is to specify the folder path or ID under which you have your placeholders created, for example- if your placeholders structure is- /sitecore/layout/Placeholder Settings/Feature, 
then you would have to set this path or ID of this item to this field.
If you have multiple locations where your placeholders are stored, then you can pass multiple IDs or paths to this field and once done you will have the following details for each item:
* ID
* Name
* Placeholder key present
* Allowed controls set

10) Placeholder folder path/ID to exclude- This field is to specify the folder path or ID which you want to exclude from your placeholders report.
Note- Only one path/ID is allowed to set here.

11) Database- To seelct the darget database (master/web)- if left blank- master database will be used.

12) Is Project Helix Based- If checked- then feature template will be enabled and option is provided to select include feature template path/ID or exclude feature template.
If you have multiple locations where your feature templates are stored, then you can pass multiple IDs or paths to this field and once done you will have the following details for each item:
* ID
* Name
* Path
* Fields exist (if any fields exists on this template-if no fields exists- then we can check more if this is required)
* Template in use (as this is a feature template- system will check if this template is used as a base template anywhere)
* Icon set (It will show if icon has been set or not, but for feature templates it will be No for most of the cases)

There is also report for items that are based on default folder template- as it is recommended to create new custom template for folder templates so that we could set the insert options accordingly.

"Yes" status is highlighted with Green color and "No" status with Red- this is to give visibility on where action is required, so that team can go that item directly and confirm if any chnages are required.


