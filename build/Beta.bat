@echo off

echo =======================================================================
echo Cosmos.EfCore
echo =======================================================================

::go to parent folder
cd ..

::create nuget_packages
if not exist nuget_packages (
    md nuget_packages
    echo Created nuget_packages folder.
)

::clear nuget_packages
for /R "nuget_packages" %%s in (*) do (
    del "%%s"
)
echo Cleaned up all nuget packages.
echo.

::start to package all projects
dotnet pack src/Cosmos.EntityFrameworkCore/Cosmos.EntityFrameworkCore.csproj                                -c Release -o nuget_packages --no-restore
dotnet pack src/Cosmos.EntityFrameworkCore.MySql/Cosmos.EntityFrameworkCore.MySql.csproj                    -c Release -o nuget_packages --no-restore
dotnet pack src/Cosmos.EntityFrameworkCore.MySqlConnector/Cosmos.EntityFrameworkCore.MySqlConnector.csproj  -c Release -o nuget_packages --no-restore
dotnet pack src/Cosmos.EntityFrameworkCore.Oracle/Cosmos.EntityFrameworkCore.Oracle.csproj                  -c Release -o nuget_packages --no-restore
dotnet pack src/Cosmos.EntityFrameworkCore.PostgreSql/Cosmos.EntityFrameworkCore.PostgreSql.csproj          -c Release -o nuget_packages --no-restore
dotnet pack src/Cosmos.EntityFrameworkCore.SqlServer/Cosmos.EntityFrameworkCore.SqlServer.csproj            -c Release -o nuget_packages --no-restore
dotnet pack src/Cosmos.EntityFrameworkCore.Sqlite/Cosmos.EntityFrameworkCore.Sqlite.csproj                  -c Release -o nuget_packages --no-restore

for /R "nuget_packages" %%s in (*symbols.nupkg) do (
    del "%%s"
)

echo.
echo.

::push nuget packages to server
for /R "nuget_packages" %%s in (*.nupkg) do ( 	
    dotnet nuget push "%%s" -s "Beta" --skip-duplicate
	echo.
)

::get back to build folder
cd build