UPDATE ApplicationConfiguration 
SET value = '<div class="text-center" style="margin-top:100px">           <h1 style="font-size:70px;">BETTER TOGETHER:</h1><br>           <h2>VANTAGE + AZURE</h2>        </div> <hr style="width:500px;background-color:#F37440">'
WHERE Name ='ClientHomePageContent'

UPDATE ApplicationConfiguration 
SET value = '<div > <div class = "center"> <a href="https://www.teradata.com/Privacy" target="_blank">Privacy Policy</a> ©2020 Teradata. All Rights Reserved </div> </div>  '
WHERE name ='Footer'


IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name='log')
BEGIN
CREATE TABLE [dbo].[log](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NULL,
	[Thread] [nvarchar](max) NULL,
	[Level] [nvarchar](max) NULL,
	[Logger] [nvarchar](max) NULL,
	[Message] [nvarchar](max) NULL,
	[Exception] [nvarchar](max) NULL
) ON [PRIMARY]
End
GO

IF NOT EXISTS (SELECT * FROM DatabaseVersionHistory WHERE VersionNumber = 3)
BEGIN
	INSERT INTO DatabaseVersionHistory (VersionNumber,ChangeLog,CreateDate,CreateBy)
	SELECT 3,'User Home Page Content and Footer script update',GETDATE(),''
END
GO