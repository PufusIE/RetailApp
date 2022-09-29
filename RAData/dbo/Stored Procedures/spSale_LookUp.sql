CREATE PROCEDURE [dbo].[spSale_LookUp]
	@CashierId nvarchar(128),
	@SaleDate datetime2
AS
begin

	set nocount on;

	select Id
	from dbo.Sale s
	where s.CashierId = @CashierId and s.SaleDate = @SaleDate;

end
