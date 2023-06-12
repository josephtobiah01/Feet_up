Useful Links regarding Code first:

https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/

https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/existing-database

https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/automatic

https://www.entityframeworktutorial.net/code-first/code-based-migration-in-code-first.aspx


Code First Migrations has two primary commands that you are going to become familiar with.

Add-Migration will scaffold the next migration based on changes you have made to your model since the last migration was created

Update-Database will apply any pending migrations to the database




Reverse engeneer code-first code from existing database:


https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/?tabs=dotnet-core-cli


Create Context

using (var ctx = new FitAppContext(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))))