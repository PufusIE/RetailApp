CREATE PROCEDURE [dbo].[spUser_Lookup]
	@Id nvarchar(128) 
as	
begin
	set nocount on;

	select [Id], [FirstName], [LastName], [EmailAddress], [CreateDate]
	from dbo.[User] u
	where u.Id = @Id

end
