CREATE PROCEDURE [dbo].[spProduct_GetAll]

as
begin 
set nocount on;
	
	select [Id], [ProductName], [Description], [RetailPrice], [QuantityInStock], [IsTaxable], [ProductImage]
	from dbo.Product
	order by ProductName

end
