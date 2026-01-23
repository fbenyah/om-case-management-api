# Project Instructions

## Getting Started

1. Clone the project repository.
2. Install all project dependencies with `dotnet restore`
3. Build the project with `dotnet build`
4. Open the package manager console and select the default project to be the integration.data project
5a. In the package manger console type `Add-Migration InitialDbCreation` and press enter to run the migrations for any changes to the database entities
5b. In the package manger console type `Update-Database` and press enter to deploy the migrations to the database
6. Run the project with `dotnet run`