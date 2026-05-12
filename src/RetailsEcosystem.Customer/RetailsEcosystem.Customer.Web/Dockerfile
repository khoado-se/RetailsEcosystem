FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RetailsEcosystem.Customer.slnx .
COPY RetailsEcosystem.Customer.Web/RetailsEcosystem.Customer.Web.csproj RetailsEcosystem.Customer.Web/
COPY RetailsEcosystem.Customer.Application/RetailsEcosystem.Customer.Application.csproj RetailsEcosystem.Customer.Application/
COPY RetailsEcosystem.Customer.Domain/RetailsEcosystem.Customer.Domain.csproj RetailsEcosystem.Customer.Domain/
COPY RetailsEcosystem.Customer.Infrastructure/RetailsEcosystem.Customer.Infrastructure.csproj RetailsEcosystem.Customer.Infrastructure/
COPY RetailsEcosystem.Customer.Shared/RetailsEcosystem.Customer.Shared.csproj RetailsEcosystem.Customer.Shared/
RUN dotnet restore RetailsEcosystem.Customer.Web/RetailsEcosystem.Customer.Web.csproj

COPY . .
RUN dotnet publish RetailsEcosystem.Customer.Web/RetailsEcosystem.Customer.Web.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8081
ENTRYPOINT ["dotnet", "RetailsEcosystem.Customer.Web.dll"]
