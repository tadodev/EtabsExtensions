@echo off
REM ──────────────────────────────────────────────────────────────
REM EtabsExtension Solution Bootstrapper (Targeting .NET 8)
REM ──────────────────────────────────────────────────────────────

REM 1) Create the solution
set "SOLUTION_NAME=EtabsExtensions"
REM dotnet new sln -n %SOLUTION_NAME%

REM 2) Create Domain / Core / Infrastructure libraries targeting .NET 8

dotnet new classlib -n Domain         -o src\Domain         -f net8.0
mkdir src\Domain\Common
mkdir src\Domain\Entities
mkdir src\Domain\Events
mkdir src\Domain\Exception

dotnet new classlib -n Core           -o src\Core           -f net8.0
REM Add domain-agnostic utilities and frameworks in Core
dotnet add src\Core\Core.csproj package AutoMapper
dotnet add src\Core\Core.csproj package FluentValidation.DependencyInjectionExtensions
dotnet add src\Core\Core.csproj package Microsoft.EntityFrameworkCore
dotnet add src\Core\Core.csproj package Microsoft.Extensions.Hosting
REM (Removed ScottPlot from Core to keep Core UI-agnostic)

REM 3) Create Infrastructure library targeting .NET 8 Create Infrastructure library targeting .NET 8 .NET 8
dotnet new classlib -n Infrastructure -o src\Infrastructure -f net8.0
REM Add EF Core SQLite, relational tooling, reporting, and charting libs to Infrastructure
dotnet add src\Infrastructure\Infrastructure.csproj package Microsoft.EntityFrameworkCore.Sqlite
dotnet add src\Infrastructure\Infrastructure.csproj package Microsoft.EntityFrameworkCore.Relational
dotnet add src\Infrastructure\Infrastructure.csproj package Microsoft.EntityFrameworkCore.Tools
dotnet add src\Infrastructure\Infrastructure.csproj package ClosedXML
dotnet add src\Infrastructure\Infrastructure.csproj package QuestPDF
dotnet add src\Infrastructure\Infrastructure.csproj package ScottPlot

REM 4) Create Desktop WPF app targeting .NET 8
dotnet new wpf -n Desktop            -o src\Desktop        -f net8.0
mkdir src\Desktop\Views
mkdir src\Desktop\ViewModels
REM Add MVVM Community Toolkit and WPF plot control to Desktop project
dotnet add src\Desktop\Desktop.csproj package CommunityToolkit.Mvvm
dotnet add src\Desktop\Desktop.csproj package ScottPlot.WPF

REM 5) Create test projects targeting .NET 8
dotnet new nunit -n Domain.UnitTests               -o tests\Domain.UnitTests               -f net8.0
dotnet new nunit -n Core.UnitTests                 -o tests\Core.UnitTests                 -f net8.0
dotnet new nunit -n Infrastructure.IntegrationTests -o tests\Infrastructure.IntegrationTests -f net8.0
dotnet new nunit -n Core.FunctionalTests            -o tests\Core.FunctionalTests            -f net8.0
dotnet new nunit -n Desktop.UITests                -o tests\Desktop.UITests               -f net8.0

REM 6) Add all projects (and README) to the solution
dotnet sln %SOLUTION_NAME%.sln add src\Domain\Domain.csproj

dotnet sln %SOLUTION_NAME%.sln add src\Core\Core.csproj

dotnet sln %SOLUTION_NAME%.sln add src\Infrastructure\Infrastructure.csproj

dotnet sln %SOLUTION_NAME%.sln add src\Desktop\Desktop.csproj

dotnet sln %SOLUTION_NAME%.sln add tests\Domain.UnitTests\Domain.UnitTests.csproj

dotnet sln %SOLUTION_NAME%.sln add tests\Core.UnitTests\Core.UnitTests.csproj

dotnet sln %SOLUTION_NAME%.sln add tests\Infrastructure.IntegrationTests\Infrastructure.IntegrationTests.csproj

dotnet sln %SOLUTION_NAME%.sln add tests\Core.FunctionalTests\Core.FunctionalTests.csproj

dotnet sln %SOLUTION_NAME%.sln add tests\Desktop.UITests\Desktop.UITests.csproj

REM 7) Wire up project references
dotnet add src\Core\Core.csproj           reference src\Domain\Domain.csproj

dotnet add src\Infrastructure\Infrastructure.csproj reference src\Core\Core.csproj

dotnet add src\Infrastructure\Infrastructure.csproj reference src\Domain\Domain.csproj

dotnet add src\Desktop\Desktop.csproj     reference src\Infrastructure\Infrastructure.csproj

dotnet add src\Desktop\Desktop.csproj     reference src\Core\Core.csproj

dotnet add src\Desktop\Desktop.csproj     reference src\Domain\Domain.csproj

dotnet add tests\Domain.UnitTests\Domain.UnitTests.csproj               reference src\Domain\Domain.csproj

dotnet add tests\Core.UnitTests\Core.UnitTests.csproj                   reference src\Core\Core.csproj

dotnet add tests\Infrastructure.IntegrationTests\Infrastructure.IntegrationTests.csproj reference src\Infrastructure\Infrastructure.csproj

dotnet add tests\Core.FunctionalTests\Core.FunctionalTests.csproj       reference src\Desktop\Desktop.csproj

dotnet add tests\Desktop.UITests\Desktop.UITests.csproj                 reference src\Desktop\Desktop.csproj


echo.
echo Solution %SOLUTION_NAME% (NET 8) with Domain, Core, Infrastructure, Desktop, and README created successfully!
pause
