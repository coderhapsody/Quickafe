USE [Quickafe]
GO

/****** Object:  Table [dbo].[Uom]    Script Date: 08/16/2016 05:07:15 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Uom]') AND type in (N'U'))
DROP TABLE [dbo].[Uom]
GO

USE [Quickafe]
GO

/****** Object:  Table [dbo].[Uom]    Script Date: 08/16/2016 05:07:16 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventoryType]') AND type in (N'U'))
DROP TABLE [dbo].[InventoryType]
GO

USE [Quickafe]
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Inventory]') AND type in (N'U'))
DROP TABLE [dbo].[Inventory]
GO

USE [Quickafe]
GO

/****** Object:  Table [dbo].[Inventory]    Script Date: 08/16/2016 05:36:56 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [Quickafe]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventoryDetail_Inventory]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventoryDetail]'))
ALTER TABLE [dbo].[InventoryDetail] DROP CONSTRAINT [FK_InventoryDetail_Inventory]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventoryDetail_Product]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventoryDetail]'))
ALTER TABLE [dbo].[InventoryDetail] DROP CONSTRAINT [FK_InventoryDetail_Product]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InventoryDetail_Uom]') AND parent_object_id = OBJECT_ID(N'[dbo].[InventoryDetail]'))
ALTER TABLE [dbo].[InventoryDetail] DROP CONSTRAINT [FK_InventoryDetail_Uom]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_WarehouseDetail_Qty]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[InventoryDetail] DROP CONSTRAINT [DF_WarehouseDetail_Qty]
END

GO

USE [Quickafe]
GO

/****** Object:  Table [dbo].[InventoryDetail]    Script Date: 08/16/2016 05:37:15 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InventoryDetail]') AND type in (N'U'))
DROP TABLE [dbo].[InventoryDetail]
GO

USE [Quickafe]
GO

/****** Object:  Table [dbo].[InventoryDetail]    Script Date: 08/16/2016 05:37:15 ******/
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
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

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

ALTER TABLE [dbo].[InventoryDetail] ADD  CONSTRAINT [DF_WarehouseDetail_Qty]  DEFAULT ((0)) FOR [Qty]
GO