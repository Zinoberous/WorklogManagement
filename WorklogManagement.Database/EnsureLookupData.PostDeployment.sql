/*
Vorlage für ein Skript nach der Bereitstellung							
--------------------------------------------------------------------------------------
 Diese Datei enthält SQL-Anweisungen, die an das Buildskript angefügt werden.		
 Schließen Sie mit der SQLCMD-Syntax eine Datei in das Skript nach der Bereitstellung ein.			
 Beispiel:   :r .\myfile.sql								
 Verwenden Sie die SQLCMD-Syntax, um auf eine Variable im Skript nach der Bereitstellung zu verweisen.		
 Beispiel:   :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

--AbsenceType
MERGE INTO [AbsenceType] AS [target]
USING
(
    VALUES
    (1, N'Holiday'),
    (2, N'Vacation'),
    (3, N'Ill')
) AS [source] ([Id], [Name])
    ON [target].[Id] = [source].[Id]
WHEN MATCHED THEN
    UPDATE SET [Name] = [source].[Name]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name])
    VALUES ([source].[Id], [source].[Name])
WHEN NOT MATCHED BY SOURCE THEN
    DELETE;

--TicketStatus
MERGE INTO [TicketStatus] AS [target]
USING
(
    VALUES
    (1, N'Todo'),
    (2, N'Running'),
    (3, N'Paused'),
    (4, N'Blocked'),
    (5, N'Done'),
    (6, N'Canceled'),
    (7, N'Continues')
) AS [source] ([Id], [Name])
    ON [target].[Id] = [source].[Id]
WHEN MATCHED THEN
    UPDATE SET [Name] = [source].[Name]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name])
    VALUES ([source].[Id], [source].[Name])
WHEN NOT MATCHED BY SOURCE THEN
    DELETE;

--WorkTimeType
MERGE INTO [WorkTimeType] AS [target]
USING
(
    VALUES -- Office, Mobile
    (1, N'Office'),
    (2, N'Mobile')
) AS [source] ([Id], [Name])
    ON [target].[Id] = [source].[Id]
WHEN MATCHED THEN
    UPDATE SET [Name] = [source].[Name]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name])
    VALUES ([source].[Id], [source].[Name])
WHEN NOT MATCHED BY SOURCE THEN
    DELETE;
