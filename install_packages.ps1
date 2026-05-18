Write-Host "Installing missing packages for SGC.Application"
Set-Location "c:\Users\johan\source\repos\SGC\SGC.Application"
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions

Write-Host "Installing missing packages for SGC.Persistence"
Set-Location "c:\Users\johan\source\repos\SGC\SGC.Persistence"
dotnet add package Microsoft.EntityFrameworkCore.Design

Write-Host "Installing missing packages for SGC.Infraestructure"
Set-Location "c:\Users\johan\source\repos\SGC\SGC.Infraestructure"
dotnet add package Anthropic.SDK
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File

Write-Host "Installing missing packages for SGC.ApplicationTest"
Set-Location "c:\Users\johan\source\repos\SGC\SGC.ApplicationTest"
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Moq
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package FluentAssertions
dotnet add package Microsoft.EntityFrameworkCore.InMemory

Write-Host "Installing missing packages for SGC.Desktop"
Set-Location "c:\Users\johan\source\repos\SGC\SGC.Desktop"
dotnet add package Microsoft.AspNetCore.SignalR.Client
dotnet add package System.IdentityModel.Tokens.Jwt

Write-Host "Restoring entire solution"
Set-Location "c:\Users\johan\source\repos\SGC"
dotnet restore
