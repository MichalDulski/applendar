# Applendar
[![Conventional Commits](https://img.shields.io/badge/Conventional%20Commits-1.0.0-%23FE5196?logo=conventionalcommits&logoColor=white)](https://conventionalcommits.org)

Applendar is a C# project that uses SQL for data storage. It's a middleware that logs the last activity of a user. It uses Serilog for logging and Entity Framework Core for data access.

## Getting Started

When running locally remember to update the `SQL_CONNECTION_STRING` in the `appsettings.*.json` file with your PostgreSQL server details.

When running with Docker, the `docker-compose.yml` takes care of the database setup.

In both cases remember to setup Auth0 for authentication. You can do this by creating a new application in the Auth0 dashboard and updating the `Auth0` section in the `appsettings.*.json` file with your Auth0 application details.

## Running locally
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- .NET 7.0
- PostgreSQL server

### Installation

1. Clone the repository:
```shell
git clone https://github.com/MichalDulski/applendar.git
```

2. Navigate to the project directory:
```shell
cd applendar
```

3. Restore the .NET packages:
```shell
dotnet restore
```

4. Update the `ApplendarDb` in the `appsettings.{environment}.json` file with your PostgreSQL server details.

5. Run the migrations to create the database schema:
```shell
dotnet ef database update -p Applendar.Infrastructure -s Applendar.API
```

## Running the Application

You can run the application using the following command:

```shell
dotnet run --project Applendar.API
```

## Running with Docker

If you prefer to use Docker, you can use the provided `docker-compose.yml` file to start the application.

### Prerequisites

- Docker
- Docker Compose

### Instructions

1. Navigate to the Development directory:
```shell
cd applendar/Development
```

2. Build and start the Docker containers:
```shell
docker compose up --build -d
```

This will start the application and all its dependencies (like the PostgreSQL server) in separate Docker containers.

To enable https in the application, you need to provide the SSL certificate and key files. 
You can do this by updating the `ASPNETCORE_Kestrel__Certificates__Default__Path` and `ASPNETCORE_Kestrel__Certificates__Default__Password` environment variables in the `docker-compose.yml` file. (https://learn.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-7.0)

To stop the application and remove the containers, use the following command:

```shell
docker compose down
```

Please note that this project is released with a [Contributor Code of Conduct](https://www.contributor-covenant.org/version/2/0/code_of_conduct/). By participating in this project you agree to abide by its terms.

## License

This project is licensed under the AGPL-3.0 license - see the [LICENSE](LICENSE) file for details.