
/****** Object:  Table [dbo].[ARMTemplates]    Script Date: 04-03-2020 12.32.32 PM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ARMTemplates](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ARMTempalteID] [uniqueidentifier] NULL,
	[ARMTempalteName] [varchar](225) NULL,
	[TemplateLocation] [varchar](225) NULL,
	[Isactive] [bit] Not NULL,
	[CreateDate] [datetime] NULL,
	[UserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [PlanEventsMapping] Add  [ARMTemplateId] [uniqueidentifier]
ALTER TABLE Plans ADD DeployToCustomerSubscription BIT
ALTER TABLE PlanEventsOutPut ADD ArmtemplateId Uniqueidentifier 
ALTER TABLE WebJobSubscriptionStatus ADD ARMTemplateID UniqueIdentifier
ALTER TABLE WebJobSubscriptionStatus ADD DeploymentStatus Varchar(225)
GO


  
/*     
Exec spGetPlanEvents 'B8F4D276-15EB-4EB6-89D4-E600FF1098EF'    
*/    
ALTER Procedure [dbo].[spGetPlanEvents]    
(    
@PlanId Uniqueidentifier    
)    
AS    
BEGIN    
     
Declare @OfferId Uniqueidentifier     
--Set @OfferId=(Select OfferId from Plans where PlanGuId =@PlanId )    
--isnull(PlanAttributeId,ID),ParameterId,DisplayName,DisplaySequence,isnull(IsEnabled,0)    
    
SELECT      
 Cast(ROW_NUMBER() OVER ( ORDER BY E.EventsId)  as Int) RowNumber    
 ,ISNULL(OEM.Id,0)  Id    
,ISNULL(OEM.PlanId,@PlanId) PlanId    
,OEM.ARMTemplateId    
,ISNULL(OEM.Isactive,0) Isactive    
,ISNULL(OEM.CopyToCustomer,0) CopyToCustomer  
,ISNULL(OEM.SuccessStateEmails,'')SuccessStateEmails    
,ISNULL(OEM.FailureStateEmails,'')FailureStateEmails    
,E.EventsId as EventId    
    
,E.EventsName    
from Events  E    
left  join     
PlanEventsMapping  OEM    
on    
E.EventsId= OEM.EventId and  OEM.PlanId= @PlanId    
where      
E.Isactive=1     
    
END    

GO


/****** Object:  Table [dbo].[OfferAttributes]    Script Date: 04-15-2020 12.33.08 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DeploymentAttributes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ParameterId] [varchar](225) NULL,
	[DisplayName] [varchar](225) NULL,
	[Description] [varchar](225) NULL,
	[ValueTypeId] [int] NULL,
	[FromList] [bit] NOT NULL,
	[ValuesList] [varchar](max) NULL,
	[Max] [int] NULL,
	[Min] [int] NULL,
	[Type] [varchar](225) NULL,
	[DisplaySequence] [int] NULL,
	[Isactive] [bit] NOT NULL,
	[CreateDate] [datetime] NULL,
	[UserId] [int] NULL,
	[OfferId] [varchar](225) NULL,
	[IsDelete] [bit] NULL,
	[IsRequired] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE ARMTemplateParameters
(
ID INT Identity Primary Key,
ARMTemplateID UniqueIdentifier Not Null,
Parameter Varchar(225) Not NUll,
ParameterDataType Varchar(225) Not NUll,
Value Varchar(225) Not NUll,
ParameterType Varchar(225) Not NUll,
CreateDate Datetime Not NUll,
UserID Int Not NUll
)
GO


Declare @Datatype Int =(select ValueTypeId from ValueTypes where ValueType = 'String')

Insert into DeploymentAttributes
Select 'TenantID','Tenant ID','Tenant ID',@Datatype,0,NUll,NULL,NULL, 'Deployment', 1,1,GetDate(),1,NUll,0,1 UNion ALL
Select 'SubscriptionID', 'Subscription ID', 'Subscription ID',@Datatype,0,NUll,NULL,NULL, 'Deployment', 2,1,GetDate(),1,NUll,0,1 UNion ALL
Select 'ServicePrincipalID', 'Service Principal ID','Application ID' ,@Datatype,0,NUll,NULL,NULL, 'Deployment', 3,1,GetDate(),1,NUll,0,1 UNion ALL
Select 'ClientSecret', 'Client Secret','Client Secret',@Datatype,0,NUll,NULL,NULL, 'Deployment', 4,1,GetDate(),1,NUll,0,1 

GO





GO
CREATE TABLE [dbo].[SubscriptionTemplateParameters]
(
Id Int identity(1,1) Primary Key
,OfferName Varchar(225)
,OfferGUId UniqueIdentifier
,PlanGUID UniqueIdentifier
,PlanId   Varchar(225)
,ARMTemplateID UniqueIdentifier 
,Parameter Varchar(225)
,ParameterDataType Varchar(225)
,Value Varchar(225)
,ParameterType Varchar(225)
,EventId Int
,EventsName Varchar(225)
,AMPSubscriptionId UniqueIdentifier
,SubscriptionStatus Varchar(225)
,SubscriptionName Varchar(225)
,CreateDate Datetime
,UserId Int
)

GO



  
CREATE Procedure spGetSubscriptionTemplateParameters  
(    
@SubscriptionId Uniqueidentifier,    
@PlanId Uniqueidentifier    
)    
AS    
BEGIN    
     
Declare @OfferId Uniqueidentifier     
Set @OfferId=(Select OfferId from Plans where PlanGuId =@PlanId )  
If exists(select 1 from SubscriptionTemplateParameters where AMPSubscriptionId=@SubscriptionId)
BEGIN
Select 
Id
,OfferName
,OfferGUId
,PlanGUID
,PlanId
,ARMTemplateID
,Parameter
,ParameterDataType
,Value
,ParameterType
,EventId
,EventsName
,AMPSubscriptionId
,SubscriptionStatus
,SubscriptionName

 from SubscriptionTemplateParameters where AMPSubscriptionId=@SubscriptionId
 END
 ELSE
 BEGIN
SELECT      
Cast( ROW_NUMBER() OVER ( ORDER BY ART.ID) as Int)RowId    
,0 ID    
,ofr.OfferName  
,ofr.OfferGUId   
,pln.PlanGUID  
,pln.PlanId  
,art.ARMTemplateID  
,art.Parameter  
,art.ParameterDataType  
,art.Value  
,art.ParameterType  
,PE.EventId  
,EV.EventsName  
,Sub.AMPSubscriptionId  
,Sub.SubscriptionStatus  
,sub.Name AS SubscriptionName  
from   
Offers ofr  
inner join Plans pln on ofr.OfferGUId=pln.OfferID  
inner join PlanEventsMapping PE on pln.PlanGUID=pe.PlanId  
inner join ARMTemplateParameters ART on PE.ARMTemplateId=ART.ARMTemplateId   
inner Join Subscriptions Sub on pln.PlanId=Sub.AMPPlanId  
inner join [Events] Ev on EV.EventsId=PE.EventId  
 Where Sub.AMPSubscriptionId =@SubscriptionId  
 and EV.EventsName  ='Activate'
  END
   
  
END  
  
  
  GO

CREATE TABLE [dbo].[SubscriptionTemplateParametersOutPut]
(
RowID int  NOT NULL
,Id Int 
,OfferName Varchar(225)
,OfferGUId UniqueIdentifier
,PlanGUID UniqueIdentifier
,PlanId   Varchar(225)
,ARMTemplateID UniqueIdentifier 
,Parameter Varchar(225)
,ParameterDataType Varchar(225)
,Value Varchar(225)
,ParameterType Varchar(225)
,EventId Int
,EventsName Varchar(225)
,AMPSubscriptionId UniqueIdentifier
,SubscriptionStatus Varchar(225)
,SubscriptionName Varchar(225)
)

GO

CREATE TABLE SubscriptionKeyVault
(
Id Int Identity(1,1) Primary Key,
SubscriptionId UniqueIdentifier,
SecureId Varchar(max),
CreateDate DateTime,
UserId Int
)


Go

GO


Create Table SubscriptionKeyValueOutPut (
Id Int Identity(1,1) Primary key,
[Key] Varchar(100), 
[Value] varchar(100)
)

go
INSERT INTO [DatabaseVersionHistory] 
Select 2.3,'ARM Templates and DeploymentAttributes.',GETDATE(),'DB user'

GO
