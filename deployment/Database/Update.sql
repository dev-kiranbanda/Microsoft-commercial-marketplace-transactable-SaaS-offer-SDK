IF not exists (select 1 from sys.tables where name='BatchLog')
Begin
		Create Table BatchLog(
			ID Int Identity(1,1) PRIMARY KEY,
			ReferenceID uniqueidentifier,
			FileName Varchar(225),
			UploadedBy Varchar(225),
			UploadedOn Datetime,
			BatchStatus Varchar(225)
							)
END
GO

IF not exists (select 1 from sys.tables where name='BulkUploadUsageStaging')
Begin
		Create Table BulkUploadUsageStaging(
			ID Int Identity(1,1) PRIMARY KEY,
			BatchLogID Int,
			SubscriptionID Varchar(MAX),
			APIType Varchar(MAX),
			ConsumedUnits Varchar(MAX),
			ValidationStatus BIT,
			ValidationErrorDetail Varchar(MAX),
			StagedOn Datetime,
			ProcessedOn Datetime,
			FOREIGN KEY (BatchLogID) REFERENCES BatchLog(ID)
		
							)
END
GO

IF NOT EXISTS (SELECT * FROM SYS.TABLES WHERE NAME = 'BatchUsageUploadHistory')
BEGIN

	CREATE TABLE [dbo].[BatchUsageUploadHistory](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Request] [nvarchar](max) NULL,
		[Response] [nvarchar](max) NULL,
		[BatchId] [nvarchar](50) NULL,
		[Filename] [nvarchar](50) NULL,
		[UploadBy] [int] NULL,
		[UploadDate] [date] NULL,
	 CONSTRAINT [PK_BatchUsageUploadHistory] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
	)

END
GO


IF EXISTS (SELECT *FROM dbo.sysobjects WHERE ID = OBJECT_ID(N'[dbo].[spGetBatchUsageHistory]')AND type IN (N'P', N'PC'))
BEGIN
       DROP PROCEDURE [dbo].[spGetBatchUsageHistory]
END
GO
/*  
exec spGetBatchUsageHistory NULL,'RecordUsageTemplate.csv'  
*/  
CREATE PROCEDURE [dbo].[spGetBatchUsageHistory] (  
	@UploadedDate DATETIME,  
	@FileName VARCHAR(255)  
)  
AS  
BEGIN  
  
IF @UploadedDate IS NOT NULL AND @FileName IS NOT NULL  
BEGIN  
	SELECT * FROM BatchUsageUploadHistory WHERE Filename = @FileName AND UploadDate = @UploadedDate  
END  
  
ELSE IF @UploadedDate IS NOT NULL   
BEGIN  
	SELECT * FROM BatchUsageUploadHistory WHERE UploadDate = @UploadedDate  
END  
  
ELSE IF @FileName IS NOT NULL   
BEGIN  
	SELECT * FROM BatchUsageUploadHistory WHERE Filename = @FileName   
END  
  
ELSE    
BEGIN  
	SELECT * FROM BatchUsageUploadHistory  
END  
  
END 
GO

IF NOT EXISTS (SELECT * FROM ApplicationConfiguration WHERE Name = 'ApplicationLogo')
BEGIN
	INSERT INTO ApplicationConfiguration (Name,Value,Description)
	SELECT 'ApplicationLogo','~/teradata.png','Application Logo'
END
GO

IF NOT EXISTS (SELECT * FROM ApplicationConfiguration WHERE Name = 'ClientHomePageContent')
BEGIN
	INSERT INTO ApplicationConfiguration (Name,Value,Description)
	SELECT 'ClientHomePageContent','<div class="pt-5">
    <div class="card p-3 ">
        <div class="text-center">
            <h1 class="display-4">Welcome</h1>
        </div>
        <div class="text-left">
            <p>
                Get started with integrating your <b> Software as a Service (SaaS) </b> solution with the <b> SaaS fulfillment APIs version 2 in Microsoft commercial marketplace. </b>
            </p>
            <p><a href="https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2" target="_blank">Click here </a><span>for the API documentation.</span> </p>
        </div>
        <div>
            <b>To purchase this SaaS offer:</b>
            <ul>
                <li>Search for resources of type <b> Software as a Service (SaaS) </b></li>
                <li>Search for your offer</li>
                <li>Click <b> Create</b> to subscribe to the offer</li>
                <li>Click <b>Configure Account </b> after the subscription created</li>
            </ul>
        </div>
    </div>
</div>','Client HomePage Content'
END
GO

IF NOT EXISTS (SELECT * FROM ApplicationConfiguration WHERE Name = 'Footer')
BEGIN
	INSERT INTO ApplicationConfiguration (Name,Value,Description)
	SELECT '<div > <div class = "center"> <a href="https://www.teradata.com/Privacy" target="_blank">Privacy Policy</a> | <a href="https://www.teradata.com/Legal/Terms-of-Use" target="_blank">Terms of use</a> ©2020 Teradata. All Rights Reserved </div> </div>  ','Footer'
END
GO

IF NOT EXISTS (SELECT * FROM ApplicationConfiguration WHERE Name = 'PublisherHomePageContent')
BEGIN
	INSERT INTO ApplicationConfiguration (Name,Value,Description)
	SELECT 'PublisherHomePageContent','<div class="pt-5">
    <div class="card p-3 ">
        <div class="text-center">
            <h1 class="display-4">Welcome</h1>
        </div>
        <div class="text-left">
            <p>This is the publisher solution.</p>
        </div>
    </div>
</div>','Publisher HomePage Content'
END
GO