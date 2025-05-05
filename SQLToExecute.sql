use master
go

create database MasterServices 
go
Create database TrxServices
go

use MasterServices 
go

create table products(
 id uniqueidentifier primary key,
 ProductsName varchar(100) not null,
 ProductsPrices decimal(18,2) not null,
 quantity int not null
)

use TrxServices
go

create table Transactions(
 id uniqueidentifier primary key,
 ProductId uniqueidentifier not null,
 quantity int not null,
 totalPerTrx decimal(18,2) not null,
 CreatedDate datetime not null
)

create table Reporting(
id uniqueidentifier primary key,
TransactionId uniqueidentifier null,
TotalPayment decimal(18,2) not null,
CreatedDate datetime not null
)

go 

Create View DailyReport As
Select c.ProductsName,sum(a.TotalPayment) as totalDaily from TrxServices.dbo.Reporting a 
join TrxServices.dbo.Transactions b on a.TransactionId = b.id
join MasterServices.dbo.products c on c.id = b.ProductId
where cast(b.CreatedDate as date) =  cast(GETDATE()as date)
group by c.ProductsName

go
Create View MonthlyReport As
Select c.ProductsName,sum(a.TotalPayment) as totalMonthly from TrxServices.dbo.Reporting a 
join TrxServices.dbo.Transactions b on a.TransactionId = b.id
join MasterServices.dbo.products c on c.id = b.ProductId
WHERE 
    YEAR(b.CreatedDate) = YEAR(GETDATE()) AND
    MONTH(b.CreatedDate) = MONTH(GETDATE())
group by c.ProductsName

go
Create View YearlyReport As
Select c.ProductsName,sum(a.TotalPayment) as totalYearly from TrxServices.dbo.Reporting a 
join TrxServices.dbo.Transactions b on a.TransactionId = b.id
join MasterServices.dbo.products c on c.id = b.ProductId
where YEAR(b.CreatedDate) = YEAR(GETDATE())
group by c.ProductsName

go
create View WeeklyReport as 
Select c.ProductsName,sum(a.TotalPayment) as totalWeekly from TrxServices.dbo.Reporting a 
join TrxServices.dbo.Transactions b on a.TransactionId = b.id
join MasterServices.dbo.products c on c.id = b.ProductId
where 
 DATEPART(YEAR, b.CreatedDate) = DATEPART(YEAR, GETDATE()) AND
    DATEPART(WEEK, b.CreatedDate) = DATEPART(WEEK, GETDATE())
group by c.ProductsName


go
create View DetailTransaction as 
Select b.*,c.ProductsName from TrxServices.dbo.Reporting a 
join TrxServices.dbo.Transactions b on a.TransactionId = b.id
join MasterServices.dbo.products c on c.id = b.ProductId
go

