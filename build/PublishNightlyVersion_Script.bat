@echo off

echo =======================================================================
echo Cosmos.EntityFrameworkCore
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
dotnet pack src/Cosmos.EntityFrameworkCore/Cosmos.EntityFrameworkCore._build.csproj                                -c Release -o nuget_packages --no-restore
dotnet pack src/Cosmos.EntityFrameworkCore.MySql/Cosmos.EntityFrameworkCore.MySql._build.csproj                    -c Release -o nuget_packages --no-restore
::dotnet pack src/Cosmos.EntityFrameworkCore.MySqlConnector/Cosmos.EntityFrameworkCore.MySqlConnector._build.csproj  -c Release -o nuget_packages --no-restore
dotnet pack src/Cosmos.EntityFrameworkCore.Oracle/Cosmos.EntityFrameworkCore.Oracle._build.csproj                  -c Release -o nuget_packages --no-restore
dotnet pack src/Cosmos.EntityFrameworkCore.PostgreSql/Cosmos.EntityFrameworkCore.PostgreSql._build.csproj          -c Release -o nuget_packages --no-restore
dotnet pack src/Cosmos.EntityFrameworkCore.SqlServer/Cosmos.EntityFrameworkCore.SqlServer._build.csproj            -c Release -o nuget_packages --no-restore
dotnet pack src/Cosmos.EntityFrameworkCore.Sqlite/Cosmos.EntityFrameworkCore.Sqlite._build.csproj                  -c Release -o nuget_packages --no-restore

for /R "nuget_packages" %%s in (*symbols.nupkg) do (
    del "%%s"
)

echo.
echo.

::push nuget packages to server
for /R "nuget_packages" %%s in (*.nupkg) do (
    dotnet nuget push "%%s" -s "Nightly" --skip-duplicate
	echo.
)

::get back to build folder
cd build