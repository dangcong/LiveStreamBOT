-- =============================================================
-- PROD KHONG CO bang __EFMigrationsHistory.
-- Buoc 1: tao bang lich su (neu chua co).
-- Buoc 2: danh dau cac migration 2023 la DA APPLY (prod da co san schema nay)
--         de EF khong chay lai chung va de cac IF NOT EXISTS ben duoi hoat dong.
-- Buoc 3: cac block ben duoi se apply 3 migration 2025 con thieu.
-- =============================================================
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
SELECT v.[MigrationId], N'6.0.2'
FROM (VALUES
    (N'20230308081529_add_autoGenerateKey_User'),
    (N'20230315101453_add_col_tb_ServicePack'),
    (N'20230412081511_add_tb_ConfigSystem'),
    (N'20230412081650_update_col_BankAccountName'),
    (N'20230412090122_add_col_LinkFBAdmin_tb_ConfigSystem'),
    (N'20230413083020_add_tb_StreamHistory'),
    (N'20230417105300_add_col_CPU_RAM_Bandwidth_tb_ManagerBOT'),
    (N'20230417132507_add_col_ManagerBOT'),
    (N'20230418041619_add_col_IsUsed_tb_ManagerBOT'),
    (N'20230418091854_add_col_Password_tb_AppUser'),
    (N'20230419023405_add_config_tb_StreamHistory'),
    (N'20230419025840_add_tb_ManagerGroup'),
    (N'20230420083851_add_col_LinkYoutube_tb_ConfigSystem'),
    (N'20230421155409_add_col_UpdatedTime_tb_BOT'),
    (N'20230424033837_add_tb_UserManagerBOT'),
    (N'20230504075919_add_col_DeletedTime_tb_AppUser'),
    (N'20230504083543_update_col_LiveType_tb_ManagerBOT'),
    (N'20230510080718_add_col_NumberOfThreads_tb_UserManagerBOT'),
    (N'20230526094238_add_col_UpdateTime_tb_User'),
    (N'20231014125744_add_col_DiskTotal_DiskUsed_tb_ManagementBot'),
    (N'20231110110200_add_col_UserManager_tb_User'),
    (N'20231110110420_add_col_UserManager_tb_User'),
    (N'20231121082024_add_col_Telegrams_tb_Setting'),
    (N'20231123040735_add_col_ManagerBackup'),
    (N'20231125105249_add_col_TelegramBot'),
    (N'20231214073805_add_col_DisconnectedTime_tb_ManagerBot'),
    (N'20231216111831_add_col_IsSendNotification_tb_StreamHistory')
) AS v([MigrationId])
WHERE NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory] h WHERE h.[MigrationId] = v.[MigrationId]
);
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529091643_rename some telegram, add discord')
BEGIN
    EXEC sp_rename N'[ConfigSystems].[Telegrams]', N'TelegramUserIds', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529091643_rename some telegram, add discord')
BEGIN
    ALTER TABLE [ConfigSystems] ADD [DiscordUserIds] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529091643_rename some telegram, add discord')
BEGIN
    EXEC sp_rename N'[AppUsers].[TelegramBOT]', N'TelegramUserIdReceiveNotificationsFromStaff', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529091643_rename some telegram, add discord')
BEGIN
    EXEC sp_rename N'[AppUsers].[TelegramAPI]', N'TelegramUserId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529091643_rename some telegram, add discord')
BEGIN
    ALTER TABLE [AppUsers] ADD [DiscordUserId] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529091643_rename some telegram, add discord')
BEGIN
    ALTER TABLE [AppUsers] ADD [DiscordUserIdReceiveNotificationsFromStaff] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529091643_rename some telegram, add discord')
BEGIN
    EXEC(N'UPDATE [AppRoles] SET [ConcurrencyStamp] = N''81ee82fe-56f4-404a-a06c-2c7c6849c9cc''
    WHERE [Id] = N''c1105ce5-9dbc-49a9-a7d5-c963b6daa62a'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529091643_rename some telegram, add discord')
BEGIN

    IF NOT EXISTS (SELECT 1 FROM [AppRoles] WHERE [NormalizedName] = N'Manager')
        INSERT INTO [AppRoles] ([Id], [ConcurrencyStamp], [Description], [Name], [NormalizedName])
        VALUES (N'd32b2cc9-aabe-43b2-8bc0-66c8dfd77080', N'537cf5f8-9b32-414f-b64b-da9ac05139d1', N'Manager role', N'Manager', N'Manager');

END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529091643_rename some telegram, add discord')
BEGIN
    EXEC(N'UPDATE [AppUsers] SET [ConcurrencyStamp] = N''90a70351-e535-43b4-bca2-842ab6d32b83'', [PasswordHash] = N''AQAAAAEAACcQAAAAEF3HqXkOyhcvpnfVB5zDnwZFZ/yBAGo4x71iqNz3XPNUM/I+dqIMeUiAGMl2jpytWA==''
    WHERE [Id] = N''ffded6b0-3769-4976-841b-69459049a62d'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529091643_rename some telegram, add discord')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250529091643_rename some telegram, add discord', N'6.0.2');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529101911_fix missing seed ConfigSystem')
BEGIN
    EXEC(N'UPDATE [AppRoles] SET [ConcurrencyStamp] = N''42c3c03f-ef08-4c1d-93f3-ba51ad6175cc''
    WHERE [Id] = N''c1105ce5-9dbc-49a9-a7d5-c963b6daa62a'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529101911_fix missing seed ConfigSystem')
BEGIN
    EXEC(N'UPDATE [AppRoles] SET [ConcurrencyStamp] = N''cfc30bea-f347-4ab9-9a18-af819a735ace''
    WHERE [Id] = N''d32b2cc9-aabe-43b2-8bc0-66c8dfd77080'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529101911_fix missing seed ConfigSystem')
BEGIN
    EXEC(N'UPDATE [AppUsers] SET [ConcurrencyStamp] = N''e80c35cc-6482-4f01-a8a2-1577783c4c85'', [PasswordHash] = N''AQAAAAEAACcQAAAAEF/GB3Kcf5d0ES7vDF9fVFt66IJL98QMjnVE/52BOYhqPG+xVbJ0u+vRP8EAsofwXw==''
    WHERE [Id] = N''ffded6b0-3769-4976-841b-69459049a62d'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529101911_fix missing seed ConfigSystem')
BEGIN

    IF NOT EXISTS (SELECT 1 FROM [ConfigSystems] WHERE [Id] = 1)
    BEGIN
        SET IDENTITY_INSERT [ConfigSystems] ON;
        INSERT INTO [ConfigSystems] ([Id], [BankAccountName], [BankName], [BankNumber], [DiscordUserIds], [LinkFBAdmin], [LinkYoutube], [TelegramUserIds])
        VALUES (1, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
        SET IDENTITY_INSERT [ConfigSystems] OFF;
    END

END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529101911_fix missing seed ConfigSystem')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250529101911_fix missing seed ConfigSystem', N'6.0.2');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529105727_remove LinkTelegram')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AppUsers]') AND [c].[name] = N'LinkTelegram');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [AppUsers] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [AppUsers] DROP COLUMN [LinkTelegram];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529105727_remove LinkTelegram')
BEGIN
    EXEC(N'UPDATE [AppRoles] SET [ConcurrencyStamp] = N''8917027d-3fdd-41b1-abe0-bc55cddfd741''
    WHERE [Id] = N''c1105ce5-9dbc-49a9-a7d5-c963b6daa62a'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529105727_remove LinkTelegram')
BEGIN
    EXEC(N'UPDATE [AppRoles] SET [ConcurrencyStamp] = N''fffb431f-85fe-4a2c-94ae-b62d194f76bb''
    WHERE [Id] = N''d32b2cc9-aabe-43b2-8bc0-66c8dfd77080'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529105727_remove LinkTelegram')
BEGIN
    EXEC(N'UPDATE [AppUsers] SET [ConcurrencyStamp] = N''e22cc615-9fb7-46fe-8998-4bc14c62a969'', [PasswordHash] = N''AQAAAAEAACcQAAAAELRu9QUSGfsCuBqslHOFKPSr4RyNZ7rnEHEyd4MadvHqKoZcjefJ2lvJ7cbi9NY6lw==''
    WHERE [Id] = N''ffded6b0-3769-4976-841b-69459049a62d'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250529105727_remove LinkTelegram')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250529105727_remove LinkTelegram', N'6.0.2');
END;
GO

COMMIT;
GO

