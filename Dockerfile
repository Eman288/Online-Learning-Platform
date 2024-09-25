FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Online Learning Platform.csproj", "./"]
RUN dotnet restore "./Online Learning Platform.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Online Learning Platform.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Online Learning Platform.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Online Learning Platform.dll"]
