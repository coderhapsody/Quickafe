USE [master]
GO
/****** Object:  Database [Quickafe]    Script Date: 8/19/2016 0:29:48 ******/
CREATE DATABASE [Quickafe] 
GO
ALTER DATABASE [Quickafe] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Quickafe] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Quickafe] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Quickafe] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Quickafe] SET ARITHABORT OFF 
GO
ALTER DATABASE [Quickafe] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Quickafe] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Quickafe] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Quickafe] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Quickafe] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Quickafe] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Quickafe] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Quickafe] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Quickafe] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Quickafe] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Quickafe] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Quickafe] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Quickafe] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Quickafe] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Quickafe] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Quickafe] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Quickafe] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Quickafe] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Quickafe] SET RECOVERY FULL 
GO
ALTER DATABASE [Quickafe] SET  MULTI_USER 
GO
ALTER DATABASE [Quickafe] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Quickafe] SET DB_CHAINING OFF 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Quickafe', N'ON'
GO
USE [Quickafe]
GO
/****** Object:  StoredProcedure [dbo].[proc_ReportProductSalesByCategory]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--proc_ReportProductSalesByCategory '2016-01-01', '2016-12-31'
CREATE proc [dbo].[proc_ReportProductSalesByCategory]
(
	@FromDate date,
	@ToDate date
)
as
set nocount on

declare @sql nvarchar(max)

set @sql = 'select p.Code ProductCode, p.Name ProductName, pcat.Name ProductCategory, 
					sum(od.Qty) Qty, 
					sum(od.Qty*od.UnitPrice) GrossSales, 
					sum(od.DiscValue) TotalDiscounts,
					sum(od.Qty*od.UnitPrice-od.DiscValue) NettSales
			from [Order] o 
				left join OrderDetail od on o.Id = od.OrderId
				left join Product p on od.ProductId = p.Id
				left join ProductCategory pcat on p.ProductCategoryId = pcat.Id
			where o.[Date] >= @FromDate and o.[Date] <= @ToDate
			group by p.Code, p.Name, pcat.Name
			order by sum(od.Qty) desc, sum(od.Qty*od.UnitPrice) desc'
exec sp_executesql @sql, N'@FromDate date, @ToDate date', @FromDate, @ToDate


GO
/****** Object:  Table [dbo].[AutoNumber]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AutoNumber](
	[ModuleCode] [varchar](10) NOT NULL,
	[Year] [int] NOT NULL,
	[Month] [int] NOT NULL,
	[LastNumber] [bigint] NOT NULL,
 CONSTRAINT [PK_AutoNumber] PRIMARY KEY CLUSTERED 
(
	[ModuleCode] ASC,
	[Year] ASC,
	[Month] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Configuration]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Configuration](
	[Key] [varchar](50) NOT NULL,
	[Value] [varchar](500) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedWho] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Inventory]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Inventory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[LocationId] [bigint] NOT NULL,
	[InventoryNo] [varchar](20) NOT NULL,
	[InventoryDate] [date] NOT NULL,
	[InventoryTypeId] [bigint] NOT NULL,
	[InventoryIO] [varchar](1) NOT NULL,
	[Notes] [text] NULL,
	[VoidWhen] [datetime] NULL,
	[VoidReason] [text] NULL,
	[VoidAuth] [varchar](50) NULL,
	[CreatedWhen] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedBy] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Inventory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InventoryDetail]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryDetail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[InventoryId] [bigint] NOT NULL,
	[ProductId] [bigint] NOT NULL,
	[Qty] [money] NOT NULL,
	[UOMId] [bigint] NOT NULL,
	[NotesDtl] [text] NULL,
 CONSTRAINT [PK_InventoryDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InventoryType]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InventoryType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[InventoryTypeCode] [varchar](20) NOT NULL,
	[InventoryTypeName] [varchar](50) NOT NULL,
	[InventoryIO] [varchar](1) NOT NULL,
	[CreatedWhen] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedBy] [varchar](50) NOT NULL,
 CONSTRAINT [PK_InventoryType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Location]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Location](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Address] [varchar](200) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedWhen] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedBy] [varchar](50) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Menu]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Menu](
	[ID] [bigint] NOT NULL,
	[FAIcon] [varchar](50) NULL,
	[Title] [varchar](50) NOT NULL,
	[NavigationTo] [varchar](200) NOT NULL,
	[Seq] [int] NOT NULL CONSTRAINT [DF_Menu_Seq]  DEFAULT ((0)),
	[ParentMenuID] [bigint] NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_Menu_IsActive]  DEFAULT ((1)),
	[CreatedWhen] [datetime] NOT NULL,
	[CreatedWho] [varchar](30) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedWho] [varchar](30) NOT NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Order]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Order](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[LocationId] [bigint] NOT NULL,
	[OrderNo] [varchar](20) NOT NULL,
	[OrderType] [varchar](20) NOT NULL,
	[TableId] [bigint] NOT NULL,
	[Date] [date] NOT NULL,
	[Guests] [int] NOT NULL CONSTRAINT [DF_Order_Guests]  DEFAULT ((0)),
	[DeliveryCharge] [decimal](20, 2) NOT NULL CONSTRAINT [DF_Order_DeliveryCharge]  DEFAULT ((0)),
	[DiscPercent] [decimal](5, 2) NOT NULL CONSTRAINT [DF_Order_DiscPercent]  DEFAULT ((0)),
	[DiscValue] [decimal](20, 2) NOT NULL CONSTRAINT [DF_Order_DiscValue]  DEFAULT ((0)),
	[TaxAmount] [decimal](20, 2) NOT NULL CONSTRAINT [DF_Order_TaxAmount]  DEFAULT ((0)),
	[ServiceCharge] [decimal](20, 2) NOT NULL CONSTRAINT [DF_Order_ServiceCharge]  DEFAULT ((0)),
	[Notes] [text] NULL,
	[VoidWhen] [datetime] NULL,
	[VoidReason] [text] NULL,
	[VoidAuth] [varchar](50) NULL,
	[CreatedWhen] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedBy] [varchar](50) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderDetail]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetail](
	[Uid] [uniqueidentifier] NOT NULL,
	[OrderId] [bigint] NOT NULL,
	[ProductId] [bigint] NOT NULL,
	[Qty] [smallint] NOT NULL,
	[UnitPrice] [decimal](20, 2) NOT NULL,
	[DiscPercent] [decimal](5, 2) NOT NULL,
	[DiscValue] [decimal](20, 2) NOT NULL,
 CONSTRAINT [PK_OrderDetail] PRIMARY KEY CLUSTERED 
(
	[Uid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Payment]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Payment](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PaymentNo] [varchar](20) NOT NULL,
	[Date] [date] NOT NULL,
	[OrderId] [bigint] NOT NULL,
	[BilledAmount] [decimal](20, 2) NOT NULL,
	[PaidAmount] [decimal](20, 2) NOT NULL,
	[VoidWhen] [datetime] NULL,
	[VoidReason] [text] NULL,
	[VoidAuth] [varchar](50) NULL,
	[CreatedWhen] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedBy] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PaymentDetail]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PaymentDetail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PaymentId] [bigint] NOT NULL,
	[PaymentTypeId] [bigint] NOT NULL,
	[Amount] [decimal](20, 2) NOT NULL,
	[CardNo] [varchar](50) NULL,
	[Notes] [text] NULL,
 CONSTRAINT [PK_PaymentDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PaymentType]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PaymentType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsSystemDefault] [bit] NOT NULL CONSTRAINT [DF_PaymentType_IsSystemDefault]  DEFAULT ((0)),
	[CreatedWhen] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedBy] [varchar](50) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_PaymentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Product]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[ProductCategoryId] [bigint] NOT NULL,
	[UnitPrice] [decimal](20, 2) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedWhen] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedBy] [varchar](50) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProductCategory]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProductCategory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[CanOrder] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedWhen] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedBy] [varchar](50) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_ProductCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Role]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](30) NOT NULL,
	[IsSystemRole] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedWhen] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedBy] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RoleMenu]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleMenu](
	[RoleID] [bigint] NOT NULL,
	[MenuID] [bigint] NOT NULL,
 CONSTRAINT [PK_RoleMenu] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC,
	[MenuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Table]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Table](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](20) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedWhen] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedBy] [varchar](50) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Table] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Uom]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Uom](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UOMCode] [varchar](20) NOT NULL,
	[UOMName] [varchar](50) NOT NULL,
	[CreatedWhen] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedBy] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Uom] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserLogin]    Script Date: 8/19/2016 0:29:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserLogin](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](50) NOT NULL,
	[Password] [varchar](200) NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[LastLogin] [datetime] NULL,
	[LastChangePassword] [datetime] NULL,
	[AllowVoidOrder] [bit] NOT NULL CONSTRAINT [DF_UserLogin_AllowVoid]  DEFAULT ((0)),
	[AllowVoidPayment] [bit] NOT NULL CONSTRAINT [DF_UserLogin_AllowVoidPayment]  DEFAULT ((0)),
	[AllowPrintReceipt] [bit] NOT NULL CONSTRAINT [DF_UserLogin_AllowPrintReceipt]  DEFAULT ((0)),
	[IsSystemUser] [bit] NOT NULL CONSTRAINT [DF_UserLogin_IsSystemUser]  DEFAULT ((0)),
	[IsActive] [bit] NOT NULL,
	[CreatedWhen] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[ChangedWhen] [datetime] NOT NULL,
	[ChangedBy] [varchar](50) NOT NULL,
 CONSTRAINT [PK_UserLogin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[AutoNumber] ([ModuleCode], [Year], [Month], [LastNumber]) VALUES (N'INVOICE', 0, 0, 0)
GO
INSERT [dbo].[AutoNumber] ([ModuleCode], [Year], [Month], [LastNumber]) VALUES (N'ORDER', 0, 0, 1)
GO
INSERT [dbo].[AutoNumber] ([ModuleCode], [Year], [Month], [LastNumber]) VALUES (N'PAYMENT', 0, 0, 7)
GO
INSERT [dbo].[Configuration] ([Key], [Value], [ChangedWhen], [ChangedWho]) VALUES (N'InvoiceNumberingLength', N'8', CAST(N'2016-07-20 20:41:01.000' AS DateTime), N'script')
GO
INSERT [dbo].[Configuration] ([Key], [Value], [ChangedWhen], [ChangedWho]) VALUES (N'InvoicePrefix', N'INV', CAST(N'2016-07-20 20:42:30.613' AS DateTime), N'script')
GO
INSERT [dbo].[Configuration] ([Key], [Value], [ChangedWhen], [ChangedWho]) VALUES (N'LookUp.OrderType', N'DINE IN,TAKE AWAY,DELIVERY', CAST(N'2016-07-30 10:16:53.597' AS DateTime), N'guest')
GO
INSERT [dbo].[Configuration] ([Key], [Value], [ChangedWhen], [ChangedWho]) VALUES (N'OrderNumberingLength', N'6', CAST(N'2016-08-01 00:09:09.313' AS DateTime), N'admin')
GO
INSERT [dbo].[Configuration] ([Key], [Value], [ChangedWhen], [ChangedWho]) VALUES (N'OrderPrefix', N'ORD', CAST(N'2016-07-20 20:42:24.280' AS DateTime), N'script')
GO
INSERT [dbo].[Configuration] ([Key], [Value], [ChangedWhen], [ChangedWho]) VALUES (N'PaymentNumberingLength', N'8', CAST(N'2016-07-20 20:41:01.000' AS DateTime), N'script')
GO
INSERT [dbo].[Configuration] ([Key], [Value], [ChangedWhen], [ChangedWho]) VALUES (N'PaymentPrefix', N'PMT', CAST(N'2016-07-20 20:42:27.343' AS DateTime), N'script')
GO
INSERT [dbo].[Configuration] ([Key], [Value], [ChangedWhen], [ChangedWho]) VALUES (N'ServiceChargePercent', N'5', CAST(N'2016-07-20 20:31:19.053' AS DateTime), N'script')
GO
INSERT [dbo].[Configuration] ([Key], [Value], [ChangedWhen], [ChangedWho]) VALUES (N'StoreName', N'My Cafetaria', CAST(N'2016-07-31 23:47:15.670' AS DateTime), N'guest')
GO
INSERT [dbo].[Configuration] ([Key], [Value], [ChangedWhen], [ChangedWho]) VALUES (N'TaxPercent', N'10', CAST(N'2016-07-20 20:31:19.053' AS DateTime), N'script')
GO
SET IDENTITY_INSERT [dbo].[InventoryType] ON 

GO
INSERT [dbo].[InventoryType] ([Id], [InventoryTypeCode], [InventoryTypeName], [InventoryIO], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (1, N'001', N'IN', N'I', CAST(N'2016-08-19 00:08:00.467' AS DateTime), N'Script', CAST(N'2016-08-19 00:08:00.467' AS DateTime), N'Script')
GO
INSERT [dbo].[InventoryType] ([Id], [InventoryTypeCode], [InventoryTypeName], [InventoryIO], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (2, N'002', N'OUT', N'O', CAST(N'2016-08-19 00:08:00.467' AS DateTime), N'Script', CAST(N'2016-08-19 00:08:00.467' AS DateTime), N'Script')
GO
SET IDENTITY_INSERT [dbo].[InventoryType] OFF
GO
SET IDENTITY_INSERT [dbo].[Location] ON 

GO
INSERT [dbo].[Location] ([Id], [Code], [Name], [Address], [IsActive], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (1, N'DEF', N'Default', NULL, 1, CAST(N'2016-07-14 22:44:53.210' AS DateTime), N'script', CAST(N'2016-07-14 22:44:53.210' AS DateTime), N'script')
GO
SET IDENTITY_INSERT [dbo].[Location] OFF
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (1, N'fa fa-book', N'Master', N'#', 100, NULL, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (2, N'fa fa-pencil', N'Inventory', N'#', 200, NULL, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (3, N'fa fa-money', N'Sales', N'#', 300, NULL, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (4, N'fa fa-lock', N'Security', N'#', 400, NULL, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (5, N'fa fa-line-chart', N'Reports', N'#', 500, NULL, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (6, NULL, N'Configuration', N'~/Master/Configuration', 100, 1, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (7, NULL, N'Product', N'~/Master/Product', 101, 1, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (8, NULL, N'Product Category', N'~/Master/ProductCategory', 102, 1, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (9, NULL, N'Payment Type', N'~/Master/PaymentType', 103, 1, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (10, NULL, N'Table', N'~/Master/Table', 104, 1, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (11, NULL, N'Inventory In', N'#', 201, 2, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (12, NULL, N'Inventory Out', N'#', 202, 2, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (13, NULL, N'Order Entry', N'~/Sales/Order/Create', 301, 3, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (14, NULL, N'Outstanding Orders', N'~/Sales/Order', 302, 3, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (15, NULL, N'Order History', N'~/Sales/Order/History', 303, 3, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (16, NULL, N'Payment History', N'~/Sales/Payment/History', 306, 3, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (17, NULL, N'Roles', N'~/Security/Role', 401, 4, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (18, NULL, N'Users', N'~/Security/User', 402, 4, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (19, N'', N'Role Access', N'~/Security/RoleAccess', 403, 4, 1, CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script', CAST(N'2016-07-30 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (20, NULL, N'Daily Sales', N'~/Report/DailySales', 501, 5, 1, CAST(N'2016-08-06 00:00:00.000' AS DateTime), N'script', CAST(N'2016-08-06 00:00:00.000' AS DateTime), N'script')
GO
INSERT [dbo].[Menu] ([ID], [FAIcon], [Title], [NavigationTo], [Seq], [ParentMenuID], [IsActive], [CreatedWhen], [CreatedWho], [ChangedWhen], [ChangedWho]) VALUES (21, NULL, N'Product Sales by Category', N'~/Report/ProductSalesByCategory', 502, 5, 1, CAST(N'2016-08-06 00:00:00.000' AS DateTime), N'script', CAST(N'2016-08-06 00:00:00.000' AS DateTime), N'script')
GO
SET IDENTITY_INSERT [dbo].[Order] ON 

GO
INSERT [dbo].[Order] ([Id], [LocationId], [OrderNo], [OrderType], [TableId], [Date], [Guests], [DeliveryCharge], [DiscPercent], [DiscValue], [TaxAmount], [ServiceCharge], [Notes], [VoidWhen], [VoidReason], [VoidAuth], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (7, 1, N'ORD001', N'DINE IN', 1, CAST(N'2016-07-26' AS Date), 0, CAST(0.00 AS Decimal(20, 2)), CAST(0.00 AS Decimal(5, 2)), CAST(0.00 AS Decimal(20, 2)), CAST(0.00 AS Decimal(20, 2)), CAST(0.00 AS Decimal(20, 2)), NULL, NULL, NULL, NULL, CAST(N'2016-07-26 15:49:38.000' AS DateTime), N'guest', CAST(N'2016-07-26 15:49:38.000' AS DateTime), N'guest')
GO
INSERT [dbo].[Order] ([Id], [LocationId], [OrderNo], [OrderType], [TableId], [Date], [Guests], [DeliveryCharge], [DiscPercent], [DiscValue], [TaxAmount], [ServiceCharge], [Notes], [VoidWhen], [VoidReason], [VoidAuth], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (8, 1, N'ORD002', N'DINE IN', 1, CAST(N'2016-07-28' AS Date), 3, CAST(0.00 AS Decimal(20, 2)), CAST(10.00 AS Decimal(5, 2)), CAST(17500.00 AS Decimal(20, 2)), CAST(0.00 AS Decimal(20, 2)), CAST(0.00 AS Decimal(20, 2)), NULL, NULL, NULL, NULL, CAST(N'2016-07-28 14:12:47.000' AS DateTime), N'admin', CAST(N'2016-07-28 14:12:47.000' AS DateTime), N'admin')
GO
INSERT [dbo].[Order] ([Id], [LocationId], [OrderNo], [OrderType], [TableId], [Date], [Guests], [DeliveryCharge], [DiscPercent], [DiscValue], [TaxAmount], [ServiceCharge], [Notes], [VoidWhen], [VoidReason], [VoidAuth], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (9, 1, N'ORD000001', N'DINE IN', 1, CAST(N'2016-08-06' AS Date), 1, CAST(20000.00 AS Decimal(20, 2)), CAST(0.00 AS Decimal(5, 2)), CAST(0.00 AS Decimal(20, 2)), CAST(35000.00 AS Decimal(20, 2)), CAST(17500.00 AS Decimal(20, 2)), NULL, NULL, NULL, NULL, CAST(N'2016-08-06 19:13:52.000' AS DateTime), N'admin', CAST(N'2016-08-06 19:13:52.000' AS DateTime), N'admin')
GO
SET IDENTITY_INSERT [dbo].[Order] OFF
GO
INSERT [dbo].[OrderDetail] ([Uid], [OrderId], [ProductId], [Qty], [UnitPrice], [DiscPercent], [DiscValue]) VALUES (N'c711ea91-98ff-45c3-ab26-0b57ef8f7778', 7, 1, 3, CAST(35000.00 AS Decimal(20, 2)), CAST(0.00 AS Decimal(5, 2)), CAST(0.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[OrderDetail] ([Uid], [OrderId], [ProductId], [Qty], [UnitPrice], [DiscPercent], [DiscValue]) VALUES (N'399e2026-f34e-4436-8ebd-91ce5b2816bc', 7, 2, 2, CAST(175000.00 AS Decimal(20, 2)), CAST(0.00 AS Decimal(5, 2)), CAST(0.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[OrderDetail] ([Uid], [OrderId], [ProductId], [Qty], [UnitPrice], [DiscPercent], [DiscValue]) VALUES (N'68515a61-baf1-41cd-8dc6-9eca2eb8fd6b', 8, 2, 1, CAST(175000.00 AS Decimal(20, 2)), CAST(0.00 AS Decimal(5, 2)), CAST(0.00 AS Decimal(20, 2)))
GO
INSERT [dbo].[OrderDetail] ([Uid], [OrderId], [ProductId], [Qty], [UnitPrice], [DiscPercent], [DiscValue]) VALUES (N'60af6e7c-432a-4f60-8e60-bd51700fee0d', 9, 2, 2, CAST(175000.00 AS Decimal(20, 2)), CAST(0.00 AS Decimal(5, 2)), CAST(0.00 AS Decimal(20, 2)))
GO
SET IDENTITY_INSERT [dbo].[Payment] ON 

GO
INSERT [dbo].[Payment] ([Id], [PaymentNo], [Date], [OrderId], [BilledAmount], [PaidAmount], [VoidWhen], [VoidReason], [VoidAuth], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (7, N'PMT00000007', CAST(N'2016-08-17' AS Date), 9, CAST(422500.00 AS Decimal(20, 2)), CAST(430000.00 AS Decimal(20, 2)), NULL, NULL, NULL, CAST(N'2016-08-17 22:59:01.910' AS DateTime), N'admin', CAST(N'2016-08-17 22:59:01.910' AS DateTime), N'admin')
GO
SET IDENTITY_INSERT [dbo].[Payment] OFF
GO
SET IDENTITY_INSERT [dbo].[PaymentDetail] ON 

GO
INSERT [dbo].[PaymentDetail] ([Id], [PaymentId], [PaymentTypeId], [Amount], [CardNo], [Notes]) VALUES (6, 7, 1, CAST(430000.00 AS Decimal(20, 2)), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[PaymentDetail] OFF
GO
SET IDENTITY_INSERT [dbo].[PaymentType] ON 

GO
INSERT [dbo].[PaymentType] ([Id], [Name], [IsActive], [IsSystemDefault], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (1, N'Cash', 1, 1, CAST(N'2016-07-16 15:39:32.473' AS DateTime), N'guest', CAST(N'2016-07-16 15:39:32.473' AS DateTime), N'guest')
GO
INSERT [dbo].[PaymentType] ([Id], [Name], [IsActive], [IsSystemDefault], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (2, N'Debit', 1, 0, CAST(N'2016-07-24 21:21:03.627' AS DateTime), N'guest', CAST(N'2016-07-24 21:21:03.627' AS DateTime), N'guest')
GO
SET IDENTITY_INSERT [dbo].[PaymentType] OFF
GO
SET IDENTITY_INSERT [dbo].[Product] ON 

GO
INSERT [dbo].[Product] ([Id], [Code], [Name], [ProductCategoryId], [UnitPrice], [IsActive], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (1, N'PAK02', N'Paket Hemat 2', 1, CAST(35000.00 AS Decimal(20, 2)), 1, CAST(N'2016-07-16 00:00:00.000' AS DateTime), N'guest', CAST(N'2016-07-16 00:00:00.000' AS DateTime), N'guest')
GO
INSERT [dbo].[Product] ([Id], [Code], [Name], [ProductCategoryId], [UnitPrice], [IsActive], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (2, N'PAK01', N'Paket Hemat 1', 1, CAST(175000.00 AS Decimal(20, 2)), 1, CAST(N'2016-07-24 21:21:39.070' AS DateTime), N'guest', CAST(N'2016-07-24 21:21:39.070' AS DateTime), N'guest')
GO
SET IDENTITY_INSERT [dbo].[Product] OFF
GO
SET IDENTITY_INSERT [dbo].[ProductCategory] ON 

GO
INSERT [dbo].[ProductCategory] ([Id], [Name], [CanOrder], [IsActive], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (1, N'Meals', 1, 1, CAST(N'2016-07-14 22:50:55.000' AS DateTime), N'guest', CAST(N'2016-07-14 22:50:55.000' AS DateTime), N'guest')
GO
INSERT [dbo].[ProductCategory] ([Id], [Name], [CanOrder], [IsActive], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (2, N'Beverages', 1, 1, CAST(N'2016-07-14 22:51:07.000' AS DateTime), N'guest', CAST(N'2016-07-14 22:51:07.000' AS DateTime), N'guest')
GO
SET IDENTITY_INSERT [dbo].[ProductCategory] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

GO
INSERT [dbo].[Role] ([Id], [Name], [IsSystemRole], [IsActive], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (1, N'Administrator', 1, 1, CAST(N'2016-07-18 22:59:35.247' AS DateTime), N'script', CAST(N'2016-07-18 22:59:35.247' AS DateTime), N'script')
GO
INSERT [dbo].[Role] ([Id], [Name], [IsSystemRole], [IsActive], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (2, N'Cashier', 1, 1, CAST(N'2016-07-18 22:59:35.247' AS DateTime), N'script', CAST(N'2016-07-18 22:59:35.247' AS DateTime), N'script')
GO
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 1)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 2)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 3)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 4)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 5)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 6)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 7)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 8)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 9)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 10)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 11)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 12)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 13)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 14)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 15)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 16)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 17)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 18)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 19)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 20)
GO
INSERT [dbo].[RoleMenu] ([RoleID], [MenuID]) VALUES (1, 21)
GO
SET IDENTITY_INSERT [dbo].[Table] ON 

GO
INSERT [dbo].[Table] ([Id], [Code], [IsActive], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (1, N'TAB01', 1, CAST(N'2016-07-23 17:36:48.963' AS DateTime), N'script', CAST(N'2016-07-23 17:36:48.963' AS DateTime), N'script')
GO
SET IDENTITY_INSERT [dbo].[Table] OFF
GO
SET IDENTITY_INSERT [dbo].[Uom] ON 

GO
INSERT [dbo].[Uom] ([Id], [UOMCode], [UOMName], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (1, N'PCS', N'PCS', CAST(N'2016-08-19 00:08:00.463' AS DateTime), N'Script', CAST(N'2016-08-19 00:08:00.463' AS DateTime), N'Script')
GO
SET IDENTITY_INSERT [dbo].[Uom] OFF
GO
SET IDENTITY_INSERT [dbo].[UserLogin] ON 

GO
INSERT [dbo].[UserLogin] ([Id], [UserName], [Password], [RoleId], [LastLogin], [LastChangePassword], [AllowVoidOrder], [AllowVoidPayment], [AllowPrintReceipt], [IsSystemUser], [IsActive], [CreatedWhen], [CreatedBy], [ChangedWhen], [ChangedBy]) VALUES (1, N'admin', N'admin', 1, CAST(N'2016-08-19 00:21:52.743' AS DateTime), NULL, 1, 1, 0, 1, 1, CAST(N'2016-07-18 22:59:35.397' AS DateTime), N'script', CAST(N'2016-07-18 22:59:35.397' AS DateTime), N'admin')
GO
SET IDENTITY_INSERT [dbo].[UserLogin] OFF
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ_UserLogin]    Script Date: 8/19/2016 0:29:49 ******/
ALTER TABLE [dbo].[UserLogin] ADD  CONSTRAINT [UQ_UserLogin] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InventoryDetail] ADD  CONSTRAINT [DF_WarehouseDetail_Qty]  DEFAULT ((0)) FOR [Qty]
GO
ALTER TABLE [dbo].[InventoryDetail]  WITH CHECK ADD  CONSTRAINT [FK_InventoryDetail_Inventory] FOREIGN KEY([InventoryId])
REFERENCES [dbo].[Inventory] ([Id])
GO
ALTER TABLE [dbo].[InventoryDetail] CHECK CONSTRAINT [FK_InventoryDetail_Inventory]
GO
ALTER TABLE [dbo].[InventoryDetail]  WITH CHECK ADD  CONSTRAINT [FK_InventoryDetail_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[InventoryDetail] CHECK CONSTRAINT [FK_InventoryDetail_Product]
GO
ALTER TABLE [dbo].[InventoryDetail]  WITH CHECK ADD  CONSTRAINT [FK_InventoryDetail_Uom] FOREIGN KEY([UOMId])
REFERENCES [dbo].[Uom] ([Id])
GO
ALTER TABLE [dbo].[InventoryDetail] CHECK CONSTRAINT [FK_InventoryDetail_Uom]
GO
ALTER TABLE [dbo].[Menu]  WITH CHECK ADD  CONSTRAINT [FK_Menu_ref_ParentMenu] FOREIGN KEY([ParentMenuID])
REFERENCES [dbo].[Menu] ([ID])
GO
ALTER TABLE [dbo].[Menu] CHECK CONSTRAINT [FK_Menu_ref_ParentMenu]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Location] FOREIGN KEY([LocationId])
REFERENCES [dbo].[Location] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Location]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Table] FOREIGN KEY([TableId])
REFERENCES [dbo].[Table] ([Id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Table]
GO
ALTER TABLE [dbo].[OrderDetail]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetail_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[OrderDetail] CHECK CONSTRAINT [FK_OrderDetail_Order]
GO
ALTER TABLE [dbo].[OrderDetail]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetail_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[OrderDetail] CHECK CONSTRAINT [FK_OrderDetail_Product]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_Order]
GO
ALTER TABLE [dbo].[PaymentDetail]  WITH CHECK ADD  CONSTRAINT [FK_PaymentDetail_Payment] FOREIGN KEY([PaymentId])
REFERENCES [dbo].[Payment] ([Id])
GO
ALTER TABLE [dbo].[PaymentDetail] CHECK CONSTRAINT [FK_PaymentDetail_Payment]
GO
ALTER TABLE [dbo].[PaymentDetail]  WITH CHECK ADD  CONSTRAINT [FK_PaymentDetail_PaymentType] FOREIGN KEY([PaymentTypeId])
REFERENCES [dbo].[PaymentType] ([Id])
GO
ALTER TABLE [dbo].[PaymentDetail] CHECK CONSTRAINT [FK_PaymentDetail_PaymentType]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductCategory] FOREIGN KEY([ProductCategoryId])
REFERENCES [dbo].[ProductCategory] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ProductCategory]
GO
ALTER TABLE [dbo].[RoleMenu]  WITH CHECK ADD  CONSTRAINT [FK_RoleMenu_Menu] FOREIGN KEY([MenuID])
REFERENCES [dbo].[Menu] ([ID])
GO
ALTER TABLE [dbo].[RoleMenu] CHECK CONSTRAINT [FK_RoleMenu_Menu]
GO
ALTER TABLE [dbo].[RoleMenu]  WITH CHECK ADD  CONSTRAINT [FK_RoleMenu_Role] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[RoleMenu] CHECK CONSTRAINT [FK_RoleMenu_Role]
GO
ALTER TABLE [dbo].[UserLogin]  WITH CHECK ADD  CONSTRAINT [FK_UserLogin_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[UserLogin] CHECK CONSTRAINT [FK_UserLogin_Role]
GO
USE [master]
GO
ALTER DATABASE [Quickafe] SET  READ_WRITE 
GO
