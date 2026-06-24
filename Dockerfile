FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY src/Domain/DT-ASPNET.Domain.csproj src/Domain/
COPY src/Application/DT-ASPNET.Application.csproj src/Application/
COPY src/Infrastructure/DT-ASPNET.Infrastructure.csproj src/Infrastructure/
COPY src/Api/DT-ASPNET.Api.csproj src/Api/

RUN dotnet restore src/Api/DT-ASPNET.Api.csproj

COPY src/ src/

RUN dotnet publish src/Api/DT-ASPNET.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081

ENTRYPOINT ["dotnet", "DT-ASPNET.Api.dll"]