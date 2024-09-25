# Use the official ASP.NET runtime image as the base for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official .NET SDK image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Online Learning Platform.csproj", "./"]
RUN dotnet restore "./Online Learning Platform.csproj"

# Copy the remaining files and build the project
COPY . .
WORKDIR "/src/."
RUN dotnet build "Online Learning Platform.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "Online Learning Platform.csproj" -c Release -o /app/publish

# Final stage: use the runtime image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Online Learning Platform.dll"]
