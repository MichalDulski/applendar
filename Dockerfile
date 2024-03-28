FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /src
COPY . .
RUN dotnet restore "Applendar.API/Applendar.API.csproj"
WORKDIR "/src/Applendar.API"
RUN dotnet build "Applendar.API.csproj" -o /app/build

RUN dotnet publish "Applendar.API.csproj" -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Applendar.API.dll"]
