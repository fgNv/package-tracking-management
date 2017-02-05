#r "../packages/Owin/lib/net40/Owin.dll"
#r "../PackageTrackingManagement.Domain/bin/Debug/PackageTrackingManagement.Domain.dll"
#r "../packages/SQLProvider/lib/FSharp.Data.SqlProvider.dll"
#r "../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "../packages/Npgsql/lib/net45/Npgsql.dll"
#r "../packages/FSharp.Management/lib/net40/FSharp.Management.dll"

#load "PgSqlPersistence.fs"
#load "Migrations.fs"

Migrations.updateDatabase(Migrations.FolderDiscovery.Absolute)