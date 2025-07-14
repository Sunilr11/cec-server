# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ./GDT.CEC.sln ./
COPY ./GDT.CEC.Repository/*.csproj ./GDT.CEC.Repository/
COPY ./GDT.CEC.Service/*.csproj ./GDT.CEC.Service/
COPY ./GDT.CEC.Web/*.csproj ./GDT.CEC.Web/



RUN dotnet restore
    
COPY . .
WORKDIR /src/GDT.CEC.Repository
RUN dotnet build -c Release -o /app

WORKDIR /src/GDT.CEC.Service
RUN dotnet build -c Release -o /app

WORKDIR /src/GDT.CEC.Web
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app
    
FROM base AS final
WORKDIR /app

ADD ./GDT.CEC.Web/wwwroot /app/wwwroot

COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GDT.CEC.Web.dll"]
