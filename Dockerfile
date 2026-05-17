# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore as a separate layer so package downloads are cached.
COPY Directory.Build.props ./
COPY CRUD-Sample.sln ./
COPY src/CRUD-Sample.Api/CRUD-Sample.Api.csproj  src/CRUD-Sample.Api/
COPY src/CRUD-Sample.Core/CRUD-Sample.Core.csproj src/CRUD-Sample.Core/
COPY tests/CRUD-Sample.Tests/CRUD-Sample.Tests.csproj tests/CRUD-Sample.Tests/
RUN dotnet restore src/CRUD-Sample.Api/CRUD-Sample.Api.csproj

# Copy the rest and publish a trimmed release.
COPY . .
RUN dotnet publish src/CRUD-Sample.Api/CRUD-Sample.Api.csproj \
    -c Release -o /app/publish \
    --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "CRUD-Sample.Api.dll"]
