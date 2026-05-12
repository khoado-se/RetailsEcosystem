FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RetailsEcosystem.Customer.slnx .
COPY RetailsEcosystem.Customer.API/RetailsEcosystem.Customer.API.csproj RetailsEcosystem.Customer.API/
COPY RetailsEcosystem.Customer.Application/RetailsEcosystem.Customer.Application.csproj RetailsEcosystem.Customer.Application/
COPY RetailsEcosystem.Customer.Domain/RetailsEcosystem.Customer.Domain.csproj RetailsEcosystem.Customer.Domain/
COPY RetailsEcosystem.Customer.Infrastructure/RetailsEcosystem.Customer.Infrastructure.csproj RetailsEcosystem.Customer.Infrastructure/
COPY RetailsEcosystem.Customer.Shared/RetailsEcosystem.Customer.Shared.csproj RetailsEcosystem.Customer.Shared/
RUN dotnet restore RetailsEcosystem.Customer.API/RetailsEcosystem.Customer.API.csproj

COPY . .
RUN dotnet publish RetailsEcosystem.Customer.API/RetailsEcosystem.Customer.API.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "RetailsEcosystem.Customer.API.dll"]
