# prepare server as base
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# restore packages and build csproj as build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CarRentalSystem.Statistics/CarRentalSystem.Statistics.csproj", "CarRentalSystem.Statistics/"]
COPY ["CarRentalSystem/CarRentalSystem.csproj", "CarRentalSystem/"]
RUN dotnet restore "CarRentalSystem.Statistics/CarRentalSystem.Statistics.csproj"
COPY . .  
WORKDIR "/src/CarRentalSystem.Statistics"
RUN dotnet build "CarRentalSystem.Statistics.csproj" -c Release -o /app/build

# publish in app folder
FROM build AS publish
RUN dotnet publish "CarRentalSystem.Statistics.csproj" -c Release -o /app/publish

# remove all, take only publish (.dll) and run
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarRentalSystem.Statistics.dll"]
