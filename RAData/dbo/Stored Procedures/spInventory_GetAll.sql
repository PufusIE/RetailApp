CREATE PROCEDURE [dbo].[spInventory_GetAll]
as

begin

	set nocount on;

	select [ProductId], [Quantity], [PurchasePrice], [PurchaseDate]
	from dbo.Inventory;

end
