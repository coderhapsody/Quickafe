USE [Quickafe]
GO

/****** Object:  StoredProcedure [dbo].[proc_InventoryMutation]    Script Date: 08/27/2016 03:04:58 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[proc_InventoryMutation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[proc_InventoryMutation]
GO

USE [Quickafe]
GO

/****** Object:  StoredProcedure [dbo].[proc_InventoryMutation]    Script Date: 08/27/2016 03:04:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[proc_InventoryMutation]
    @StartDate datetime,
    @EndDate datetime
AS

create table #InventoryMut
(
    ProductCategoryId Bigint,
    ProductCategoryName varchar(50),
    ProductId	bigint,
    ProductCode varchar(20),
    ProductName varchar(50),
    --UOMId bigInt,
    --UOMCode varchar(20),
	BeginBal money,
	InQty	money,
	OutQty	money,
	EndBal	money
)

Declare 
  @Sql varchar(8000),
  @Condition varchar(8000),
  @Condition1 varchar(8000)
    
-- Insert Data Product
Insert Into #InventoryMut
Select Distinct c.id, c.name as ProductCategoryName,a.ProductId, B.Code,b.name as ProductName,
       0,0,0,0
From Vw_StockCard a
inner join Product b on a.productid = b.id
inner join productcategory c on b.productcategoryid = c.id
where a.InventoryDate <= CONVERT(varchar,@EndDate,112)

-- Cari BeginBal
Update #InventoryMut set BeginBal = a.Qty
from (
select a.ProductId,SUM(a.InQty-a.OutQty) as Qty
From Vw_StockCard a
where a.InventoryDate < CONVERT(varchar,@StartDate,112)
Group By a.ProductId
) as a
where #InventoryMut.ProductId = a.ProductId

-- Cari InQty
Update #InventoryMut set InQty = a.Qty
from (
select a.ProductId,SUM(a.InQty) as Qty
From Vw_StockCard a
where a.InventoryDate Between CONVERT(varchar,@StartDate,112) and CONVERT(varchar,@EndDate,112)
Group By a.ProductId
) as a
where #InventoryMut.ProductId = a.ProductId


-- Cari OutQty
Update #InventoryMut set OutQty = a.Qty
from (
select a.ProductId,SUM(a.OutQty) as Qty
From Vw_StockCard a
where a.InventoryDate Between CONVERT(varchar,@StartDate,112) and CONVERT(varchar,@EndDate,112)
Group By a.ProductId
) a
where #InventoryMut.ProductId = a.ProductId


-- Update EndBal
Update #InventoryMut Set EndBal = BeginBal+InQty-OutQty 

delete from #InventoryMut where BeginBal = 0 and InQty = 0 and OutQty = 0 and EndBal = 0

Select * from #InventoryMut



GO


