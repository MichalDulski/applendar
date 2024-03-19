#!/bin/bash

# Get list of all migrations
migrations=$(dotnet ef migrations list -p Applendar.Infrastructure -s Applendar.API | grep -o "^[0-9]*_.*")
prev_migration="0"
# Iterate over all migrations
for migration in $migrations
do
    # Generate a script for the migration
    dotnet ef migrations script $prev_migration $migration -s Applendar.API -p Applendar.Infrastructure --output "Applendar.Infrastructure/Migrations/sql/$migration.sql" --idempotent

    # Store the current migration as the previous one for the next iteration
    prev_migration=$migration
done