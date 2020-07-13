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
