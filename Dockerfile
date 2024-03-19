FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY . .
RUN dotnet restore "Applendar.API/Applendar.API.csproj"
WORKDIR "/src/Applendar.API"
RUN dotnet build "Applendar.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Development
RUN dotnet publish "Applendar.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN ls
ENTRYPOINT ["dotnet", "Applendar.API.dll"]
