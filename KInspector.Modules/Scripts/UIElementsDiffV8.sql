select 
	UIElement.ElementDisplayName,
	UIElement.ElementName, 
	UIElement.ElementCaption, 
	UIElement.ElementTargetURL, 
	ResourceGUID, 
	Parent.ElementGUID as 'ParentElementGUID',
	UIElement.ElementChildCount,
	UIElement.ElementOrder,
	UIElement.ElementLevel,
	UIElement.ElementIconPath,
	UIElement.ElementIsCustom,
	UIElement.ElementGUID,
	UIElement.ElementSize,
	UIElement.ElementDescription,
    UIElement.ElementFromVersion,
    PageTemplate.PageTemplateGUID,
    UIElement.ElementType,
    UIElement.ElementProperties,
    UIElement.ElementIsMenu,
    UIElement.ElementFeature,
    UIElement.ElementIconClass,
    UIElement.ElementIsGlobalApplication,
    UIElement.ElementCheckModuleReadPermission,
    UIElement.ElementAccessCondition,
    UIElement.ElementVisibilityCondition
from cms_uielement as UIElement
left join CMS_Resource as R on ElementResourceID = ResourceID
left join CMS_UIElement as Parent on Parent.ElementID = UIElement.ElementParentID
left join CMS_PageTEmplate as PageTemplate on PageTemplate.PageTemplateID = UIElement.ElementPageTemplateID