EF Migrations
-------------
Update-Database -Environment Production
uses the appsetting.Production.json connectionstring. Be sure not to check this into the repo.

dotnet ef migrations add "Identity Table" --startup-project ../Obsidian.MessageNode\Obsidian.MessageNode.csproj


dotnet ef database drop --startup-project ../Obsidian.MessageNode\Obsidian.MessageNode.csproj

