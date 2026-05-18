# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SGC.API/SGC.API.csproj", "SGC.API/"]
COPY ["SGC.IOC/SGC.IOC.csproj", "SGC.IOC/"]
COPY ["SGC.Application/SGC.Application.csproj", "SGC.Application/"]
COPY ["SGC.Domain/SGC.Domain.csproj", "SGC.Domain/"]
COPY ["SGC.Infraestructure/SGC.Infraestructure.csproj", "SGC.Infraestructure/"]
COPY ["SGC.Persistence/SGC.Persistence.csproj", "SGC.Persistence/"]

RUN dotnet restore "SGC.API/SGC.API.csproj"

COPY . .
WORKDIR /src/SGC.API
RUN dotnet publish "SGC.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SGC.API.dll"]
