USE [Quickafe]
GO

/****** Object:  View [dbo].[Vw_StockCard]    Script Date: 08/27/2016 03:01:55 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[Vw_StockCard]'))
DROP VIEW [dbo].[Vw_StockCard]
GO

USE [Quickafe]
GO

/****** Object:  View [dbo].[Vw_StockCard]    Script Date: 08/27/2016 03:01:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



Create View [dbo].[Vw_StockCard]
as (
Select a.InventoryDate,b.ProductId,
       sum(Case a.InventoryIO When 'I' then b.Qty else 0 end) as InQty,  
       sum(case a.inventoryIO when 'O' then b.qty else 0 end) as OutQty
From Inventory a
inner join inventoryDetail b on A.id = b.inventoryId
Group By a.InventoryDate,ProductId)


GO


