CREATE PROCEDURE [dbo].[spSale_SaleReport]
as
begin

	set nocount on;

	select  [SaleDate], [Subtotal], [Tax], [Total], [u].[FirstName], [u].[LastName], [u].[EmailAddress]
	from dbo.Sale s
	inner join dbo.[User] u on s.CashierId = u.Id;

end
