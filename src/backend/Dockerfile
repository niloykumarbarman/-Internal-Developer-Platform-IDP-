FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["EnterpriseIDP.API/EnterpriseIDP.API.csproj", "EnterpriseIDP.API/"]
COPY ["EnterpriseIDP.Application/EnterpriseIDP.Application.csproj", "EnterpriseIDP.Application/"]
COPY ["EnterpriseIDP.Domain/EnterpriseIDP.Domain.csproj", "EnterpriseIDP.Domain/"]
COPY ["EnterpriseIDP.Infrastructure/EnterpriseIDP.Infrastructure.csproj", "EnterpriseIDP.Infrastructure/"]
COPY ["EnterpriseIDP.Contracts/EnterpriseIDP.Contracts.csproj", "EnterpriseIDP.Contracts/"]
RUN dotnet restore "EnterpriseIDP.API/EnterpriseIDP.API.csproj"
COPY . .
RUN dotnet publish "EnterpriseIDP.API/EnterpriseIDP.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EnterpriseIDP.API.dll"]
