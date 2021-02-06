USE [Quickafe]
GO

/****** Object:  StoredProcedure [dbo].[proc_InventoryStockCard]    Script Date: 08/27/2016 03:05:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[proc_InventoryStockCard]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[proc_InventoryStockCard]
GO

USE [Quickafe]
GO

/****** Object:  StoredProcedure [dbo].[proc_InventoryStockCard]    Script Date: 08/27/2016 03:05:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





CREATE PROCEDURE [dbo].[proc_InventoryStockCard]
    @StartDate datetime,
    @EndDate datetime
AS

create table #InventorySC
(   Seq int identity(1,1),
	InventoryNo varchar(30),
	InventoryDate Datetime,
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
  @Condition1 varchar(8000),
  @Seq int,
  @EndBal int
    
-- Insert Data Product 
Insert Into #InventorySC
Select Distinct 'Saldo','1990-01-01',c.id, c.name as ProductCategoryName,a.ProductId, B.Code,b.name as ProductName,
       0,0,0,0
From Vw_StockCard a
inner join Product b on a.productid = b.id
inner join productcategory c on b.productcategoryid = c.id
where a.InventoryDate <= CONVERT(varchar,@EndDate,112)

-- Cari Saldo Awal
Update #InventorySC set BeginBal = a.Qty
from (
select a.ProductId,SUM(a.InQty-a.OutQty) as Qty
From Vw_StockCard a
where a.InventoryDate < CONVERT(varchar,@StartDate,112)
Group By a.ProductId
) as a
where #InventorySC.ProductId = a.ProductId
and #InventorySC.InventoryNo = 'Saldo'

-- Cari Transaksi In
Insert Into #InventorySC
select a.InventoryNo,a.InventoryDate,c.id, c.name as ProductCategoryName,b.ProductId, d.Code,d.name as ProductName,
       0,b.Qty,0,0
From Inventory a
inner join InventoryDetail b on a.Id = b.InventoryId
inner join Product d on b.ProductId = d.Id
inner join ProductCategory c on d.ProductCategoryId = c.Id
where a.InventoryDate between CONVERT(varchar,@StartDate,112) and CONVERT(varchar,@EndDate,112)
and a.InventoryIO ='I'

-- Cari Transaksi Out
Insert Into #InventorySC
select a.InventoryNo,a.InventoryDate,c.id, c.name as ProductCategoryName,b.ProductId, d.Code,d.name as ProductName,
       0,0,b.Qty,0
From Inventory a
inner join InventoryDetail b on a.Id = b.InventoryId
inner join Product d on b.ProductId = d.Id
inner join ProductCategory c on d.ProductCategoryId = c.Id
where a.InventoryDate between CONVERT(varchar,@StartDate,112) and CONVERT(varchar,@EndDate,112)
and a.InventoryIO ='O'

-- Update EndBal
--Update #InventorySC Set EndBal = BeginBal+InQty-OutQty 

delete from #InventorySC where BeginBal = 0 and InQty = 0 and OutQty = 0 and EndBal = 0

declare c cursor for
   Select Seq
   from #InventorySC
   Order by ProductId,InventoryDate,Seq
   
open c
fetch next from c into @seq

While @@Fetch_Status = 0 
begin
  Update #InventorySC set EndBal = BeginBal + InQty - OutQty 
         where Seq = @Seq
  Set @EndBal = 0;       
  select @EndBal = EndBal from #InventorySC Where Seq = @Seq       
  Update #InventorySC set BeginBal = @EndBal
         where Seq = @Seq + 1
  fetch next from c into @seq       
end    
Close c 
Deallocate c     
  

Select * from #InventorySC Order By InventoryDate,InventoryNo,Seq


GO


