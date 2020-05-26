
GO
/****** Object:  Table [dbo].[SubscriptionLicenses]    Script Date: 3/12/2020 1:28:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscriptionLicenses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LicenseKey] [varchar](255) NULL,
	[IsActive] [bit] NULL,
	[SubscriptionID] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NULL,
 CONSTRAINT [PK_SubscriptionLicenses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[SubscriptionLicenses]  WITH CHECK ADD FOREIGN KEY([SubscriptionID]) REFERENCES [dbo].[Subscriptions] ([Id])

Go

IF NOT EXISTS (SELECT Name FROM ApplicationConfiguration WHERE NAME = 'IsLicenseManagementEnabled')
BEGIN
	INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'IsLicenseManagementEnabled', N'True', N'To Enable or Disable Licenses Menu') 
END
GO
