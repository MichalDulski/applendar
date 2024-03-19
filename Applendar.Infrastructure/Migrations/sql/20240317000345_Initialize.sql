CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240317000345_Initialize') THEN
    CREATE TABLE "ApplendarUsers" (
        "Id" uuid NOT NULL,
        "ExternalId" text NOT NULL,
        "FirstName" text NOT NULL,
        "LastName" text NOT NULL,
        "Preferences_NotifyAboutOnlineEvents" boolean NOT NULL,
        "Preferences_NotifyAboutOfflineEvents" boolean NOT NULL,
        "Preferences_NotifyAboutEventsWithPets" boolean NOT NULL,
        "Preferences_NotifyAboutEventsWithCompanions" boolean NOT NULL,
        "LastActivityDateUtc" timestamp with time zone NOT NULL,
        "ArchivedAtUtc" timestamp with time zone NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_ApplendarUsers" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240317000345_Initialize') THEN
    CREATE TABLE "Events" (
        "Id" uuid NOT NULL,
        "EventType" integer NOT NULL,
        "Image" bytea NULL,
        "IsCompanionAllowed" boolean NOT NULL,
        "IsPetAllowed" boolean NOT NULL,
        "Location_IsOnline" boolean NOT NULL,
        "Location_Url" text NULL,
        "Location_Name" text NULL,
        "Location_City" text NULL,
        "Location_ZipCode" text NULL,
        "Location_Address" text NULL,
        "Location_Country" text NULL,
        "MaximumNumberOfParticipants" integer NULL,
        "Name" character varying(255) NOT NULL,
        "OrganizerId" uuid NOT NULL,
        "StartAtUtc" timestamp with time zone NOT NULL,
        "ArchivedAtUtc" timestamp with time zone NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Events" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Events_ApplendarUsers_OrganizerId" FOREIGN KEY ("OrganizerId") REFERENCES "ApplendarUsers" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240317000345_Initialize') THEN
    CREATE TABLE "EventInvitations" (
        "ApplendarUserId" uuid NOT NULL,
        "EventId" uuid NOT NULL,
        "Status" integer NOT NULL,
        "ArchivedAtUtc" timestamp with time zone NULL,
        "CreatedAtUtc" timestamp with time zone NOT NULL,
        "UpdatedAtUtc" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_EventInvitations" PRIMARY KEY ("ApplendarUserId", "EventId"),
        CONSTRAINT "FK_EventInvitations_ApplendarUsers_ApplendarUserId" FOREIGN KEY ("ApplendarUserId") REFERENCES "ApplendarUsers" ("Id"),
        CONSTRAINT "FK_EventInvitations_Events_EventId" FOREIGN KEY ("EventId") REFERENCES "Events" ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240317000345_Initialize') THEN
    CREATE INDEX "IX_EventInvitations_EventId" ON "EventInvitations" ("EventId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240317000345_Initialize') THEN
    CREATE INDEX "IX_Events_OrganizerId" ON "Events" ("OrganizerId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20240317000345_Initialize') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20240317000345_Initialize', '7.0.12');
    END IF;
END $EF$;
COMMIT;

