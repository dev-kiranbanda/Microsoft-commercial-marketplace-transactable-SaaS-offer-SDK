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
	SELECT 'ApplicationLogo','~/logo.jpg','Application Logo'
END
GO

IF NOT EXISTS (SELECT * FROM ApplicationConfiguration WHERE Name = 'ClientHomePageContent')
BEGIN
	INSERT INTO ApplicationConfiguration (Name,Value,Description)
	SELECT 'ClientHomePageContent','<div class="row" style="margin-top:60px;">
   <div class="col col-md-12 text-center">
      <h2 >Fastest Path to Hybrid & Multicloud
      </h2>
   </div>
</div>
<div class="row mt20">
   <div class="col col-md-3 col-12 text-center ">
      <img src="https://www.nutanix.com/content/dam/nutanix/global/icons/secondary-icons/120/SI_deploy.png" />
      <h4>Seamless Application Mobility</h4>
      <p>A true hybrid architecture between Nutanix private cloud and multiple public clouds enables seamless application mobility across clouds with absolutely no code changes needed.</p>
   </div>
   <div class="col col-md-1 d-none d-md-block">
   </div>
   <div class="col col-md-3 col-12 text-center">
      <img src="https://www.nutanix.com/content/dam/nutanix/global/icons/v2/png/120/icn-businesscontinuity.png" />
      <h4 class="text-center">Unified Infrastructure Management</h4>
      <p>Centralized management of all your computing infrastructure - on-prem or in public clouds - with a single console.</p>
   </div>
   <div class="col col-md-1 d-none d-md-block">
   </div>
   <div class="col col-md-3 col-12 text-center">
      <img  src="https://www.nutanix.com/content/dam/nutanix/it/global/icons/secondary-icons/120/SI_lower_costs.png" />
      <h4  class="text-center">Cost Intelligent Operations</h4>
      <p>Take control of your hybrid cloud spend with automated cost governance policies. Run the same Nutanix software, same licenses across all supported clouds to fully utilize your IT investment.</p>
   </div>
</div>','Client HomePage Content'
END
GO

IF NOT EXISTS (SELECT * FROM ApplicationConfiguration WHERE Name = 'Footer')
BEGIN
	INSERT INTO ApplicationConfiguration (Name,Value,Description)
	SELECT 'Footer','<div >
   <div class = "center">  Handcrafted in Sunny California. © Nutanix 2020. <a href="https://www.nutanix.com/legal/privacy-statement" target="_blank">Privacy Statement</a> </div>
</div>','Footer'
END
GO

IF NOT EXISTS (SELECT * FROM ApplicationConfiguration WHERE Name = 'PublisherHomePageContent')
BEGIN
	INSERT INTO ApplicationConfiguration (Name,Value,Description)
	SELECT 'PublisherHomePageContent','<div class="row" style="margin-top:60px;">
   <div class="col col-md-12 text-center">
      <h2 >Fastest Path to Hybrid & Multicloud
      </h2>
   </div>
</div>
<div class="row mt20">
   <div class="col col-md-3 col-12 text-center ">
      <img src="https://www.nutanix.com/content/dam/nutanix/global/icons/secondary-icons/120/SI_deploy.png" />
      <h4>Seamless Application Mobility</h4>
      <p>A true hybrid architecture between Nutanix private cloud and multiple public clouds enables seamless application mobility across clouds with absolutely no code changes needed.</p>
   </div>
   <div class="col col-md-1 d-none d-md-block">
   </div>
   <div class="col col-md-3 col-12 text-center">
      <img src="https://www.nutanix.com/content/dam/nutanix/global/icons/v2/png/120/icn-businesscontinuity.png" />
      <h4 class="text-center">Unified Infrastructure Management</h4>
      <p>Centralized management of all your computing infrastructure - on-prem or in public clouds - with a single console.</p>
   </div>
   <div class="col col-md-1 d-none d-md-block">
   </div>
   <div class="col col-md-3 col-12 text-center">
      <img  src="https://www.nutanix.com/content/dam/nutanix/it/global/icons/secondary-icons/120/SI_lower_costs.png" />
      <h4  class="text-center">Cost Intelligent Operations</h4>
      <p>Take control of your hybrid cloud spend with automated cost governance policies. Run the same Nutanix software, same licenses across all supported clouds to fully utilize your IT investment.</p>
   </div>
</div>','Publisher HomePage Content'
END
GO