FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

ARG PROJECT=Api

COPY src/Domain/DT-ASPNET.Domain.csproj src/Domain/
COPY src/Application/DT-ASPNET.Application.csproj src/Application/
COPY src/Infrastructure/DT-ASPNET.Infrastructure.csproj src/Infrastructure/
COPY src/Api/DT-ASPNET.Api.csproj src/Api/
COPY src/Web/DT-ASPNET.Web.csproj src/Web/

RUN dotnet restore src/${PROJECT}/DT-ASPNET.${PROJECT}.csproj

COPY src/ src/

RUN dotnet publish src/${PROJECT}/DT-ASPNET.${PROJECT}.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ARG PORT=8081
ARG PROJECT=Api

EXPOSE ${PORT}
ENV ASPNETCORE_URLS=http://+:${PORT}
ENV APP_DLL=DT-ASPNET.${PROJECT}.dll

ENTRYPOINT ["sh", "-c", "dotnet $APP_DLL"]