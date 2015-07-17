-- DECLARE VARIABLES
DECLARE @pageTemplateID int
DECLARE @pageTemplateDisplayName nvarchar(200)
DECLARE @documentCulture nvarchar(10)
DECLARE @nodeAliasPath nvarchar(450)
DECLARE @WebPartName nvarchar(100)
DECLARE @cnt int
DECLARE @cntMax int
DECLARE @webPartXML nvarchar(max)
	DECLARE @webPartStartIndex int
	DECLARE @webPartEndIndex int
	DECLARE @webPartLength int
DECLARE @allWebPartXML nvarchar(max)
DECLARE @webPartID nvarchar(200)
	DECLARE @webPartIDStartIndex int
	DECLARE @webPartIDEndIndex int  
DECLARE @webPartType nvarchar(200)
	DECLARE @webPartTypeStartIndex int
	DECLARE @webPartTypeEndIndex int  

-- CLEAR OPTION TO OUTPUT MODIFIED TABLES
SET NOCOUNT ON
 
DECLARE @CMS_PageTemplate TABLE (
    idx int Primary Key IDENTITY(1,1),
    PageTemplateDisplayName nvarchar(200),
    PageTemplateWebParts nvarchar(max)    
)

DECLARE @MY_Documents TABLE (
	idx int Primary Key IDENTITY(1,1),
	NodeAliasPath nvarchar(450),
	DocumentCulture nvarchar(10)    
)

DECLARE @emptyColumnsXML nvarchar(50)
	SET @emptyColumnsXML = '<property name="columns"></property>'
DECLARE @webPartEndXML nvarchar(30)
	SET @webPartEndXML = '</webpart>'
DECLARE @webPartStartXML nvarchar(30)
	SET @webPartStartXML = '<webpart controlid='
DECLARE @webPartControlidXML nvarchar(30)
	SET @webPartControlidXML = 'controlid="'
DECLARE @webPartControlidEndXML nvarchar(30)
	SET @webPartControlidEndXML = 'type="'
DECLARE @webPartTypeEndXML nvarchar(30)
	SET @webPartTypeEndXML = 'guid="'

-- SELECT ALL PAGE TEMPLATES WHERE COLUMNS PROPERTY OCCURS (USED BY SOME WEB PART) AND NOT UTILIZED / FILLED
DECLARE wTemp CURSOR LOCAL FAST_FORWARD FOR 
	SELECT PageTemplateID, PageTemplateDisplayName FROM CMS_PageTemplate WHERE PageTemplateWebParts LIKE '%' + @emptyColumnsXML + '%' 

OPEN wTemp
	FETCH NEXT FROM wTemp INTO @pageTemplateID, @pageTemplateDisplayName 
	WHILE @@FETCH_STATUS = 0
		BEGIN
		--SET @pageTemplateDisplayName = (SELECT PageTemplateDisplayName FROM CMS_PageTemplate WHERE PageTemplateID = @pageTemplateID)
		SET @allWebPartXML = (SELECT PageTemplateWebParts FROM CMS_PageTemplate WHERE PageTemplateID = @pageTemplateID)
	    PRINT @pageTemplateDisplayName
		-- SELECT DOCUMENTS USING PAGE TEMPLATE 
		INSERT @MY_Documents SELECT DISTINCT NodeAliasPath, DocumentCulture FROM View_CMS_Tree_Joined LEFT JOIN CMS_WorkflowStep ON View_CMS_Tree_Joined.DocumentWorkflowStepID=CMS_WorkflowStep.StepID LEFT JOIN CMS_Culture ON View_CMS_Tree_Joined.DocumentCulture = CMS_Culture.CultureCode WHERE (NodeAliasPath LIKE N'/%') AND (DocumentPageTemplateID = @pageTemplateID)
		
		SELECT @cnt=1;
		SELECT @cntMax = Count(idx) FROM @MY_Documents 

		IF @cntMax > 0
		BEGIN
			PRINT N' - Documents:'
			WHILE @cnt <= @cntMax
			BEGIN
			  WITH TempLoop AS (
				SELECT row_number() OVER (ORDER BY idx) AS Row,* FROM  @MY_Documents
				)
					--SELECT @documentCulture = (SELECT DocumentCulture FROM TempLoop WHERE Row = @cnt)
					SELECT @nodeAliasPath = (SELECT NodeAliasPath FROM TempLoop WHERE Row = @cnt)
					PRINT N'	- ' + @nodeAliasPath
					SELECT @cnt = @cnt+1
			END
		DELETE FROM @MY_Documents
		END
		
		PRINT N' - Web parts:'
		WHILE  @allWebPartXML LIKE '%' + @webPartEndXML + '%'
			BEGIN
				SET @webPartStartIndex = (SELECT PATINDEX('%' + @webPartStartXML + '%' , @allWebPartXML)) 
				SET @webPartEndIndex = (SELECT PATINDEX('%' + @webPartEndXML + '%' , @allWebPartXML)) 
				SET @webPartXML = (SELECT SUBSTRING(@allWebPartXML ,@webPartStartIndex , (@webPartEndIndex - @webPartStartIndex) + LEN(@webPartEndXML)))
				SET @webPartLength = LEN(@webPartXML)

				IF (SELECT PATINDEX('%' + @emptyColumnsXML + '%' , @webPartXML)) > 0
					BEGIN
						-- OUTPUT WEB PART ControlID
						SET @webPartIDStartIndex = (SELECT PATINDEX('%' + @webPartControlidXML +'%' , @webPartXML))
						SET @webPartIDEndIndex = (SELECT PATINDEX('%' + @webPartControlidEndXML + '%' , @webPartXML))
						SET @webPartID = (SELECT SUBSTRING(@webPartXML ,@webPartIDStartIndex + LEN(@webPartControlidXML) , (@webPartIDEndIndex - @webPartIDStartIndex) - 13))
						
						-- OUTPUT WEB PART TYPE
						SET @webPartTypeStartIndex = (SELECT PATINDEX('%' + @webPartControlidEndXML + '%' , @webPartXML))
						SET @webPartTypeEndIndex = (SELECT PATINDEX('%' + @webPartTypeEndXML + '%' , @webPartXML))
						SET @webPartType = (SELECT SUBSTRING(@webPartXML ,@webPartTypeStartIndex + LEN(@webPartControlidEndXML) , (@webPartTypeEndIndex - @webPartTypeStartIndex) - 8))

						PRINT N'	- ' + @webPartType + N'(ID:' +  @webPartID  +  N')';
					END
				SET @allWebPartXML = STUFF(@allWebPartXML, @webPartStartIndex, @webPartLength, '')
			END
	   FETCH NEXT FROM wTemp INTO @pageTemplateID, @pageTemplateDisplayName
	END
CLOSE wTemp
DEALLOCATE wTemp
