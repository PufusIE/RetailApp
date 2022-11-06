CREATE PROCEDURE [dbo].[spProduct_GetById]
	@Id int
AS
	set nocount on;

	select [Id], [ProductName], [Description], [RetailPrice], [QuantityInStock], [IsTaxable], [ProductImage]
	from dbo.Product p
	where p.Id = @Id;
RETURN 0
