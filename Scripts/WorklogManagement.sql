--CREATE DATABASE [WorklogManagement]
--GO

USE [WorklogManagement]
GO

BEGIN TRAN

BEGIN TRY

IF OBJECT_ID('WorklogAttachment', 'U') IS NOT NULL
BEGIN
	DROP TABLE [WorklogAttachment]
END

IF OBJECT_ID('Worklog', 'U') IS NOT NULL
BEGIN
	DROP TABLE [Worklog]
END

IF OBJECT_ID('TicketStatusLog', 'U') IS NOT NULL
BEGIN
	DROP TABLE [TicketStatusLog]
END

-- IF OBJECT_ID('TicketCommentAttachment', 'U') IS NOT NULL
-- BEGIN
-- 	DROP TABLE [TicketCommentAttachment]
-- END

-- IF OBJECT_ID('TicketComment', 'U') IS NOT NULL
-- BEGIN
-- 	DROP TABLE [TicketComment]
-- END

IF OBJECT_ID('TicketAttachment', 'U') IS NOT NULL
BEGIN
	DROP TABLE [TicketAttachment]
END

IF OBJECT_ID('Ticket', 'U') IS NOT NULL
BEGIN
	DROP TABLE [Ticket]
END

IF OBJECT_ID('TicketStatus', 'U') IS NOT NULL
BEGIN
	DROP TABLE [TicketStatus]
END

-- IF OBJECT_ID('TicketPriority', 'U') IS NOT NULL
-- BEGIN
-- 	DROP TABLE [TicketPriority]
-- END

IF OBJECT_ID('CalendarEntry', 'U') IS NOT NULL
BEGIN
	DROP TABLE [CalendarEntry]
END

IF OBJECT_ID('CalendarEntryType', 'U') IS NOT NULL
BEGIN
	DROP TABLE [CalendarEntryType]
END

CREATE TABLE [CalendarEntryType]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[Name] NVARCHAR(255) NOT NULL,
	CONSTRAINT PK_CalendarEntryType_Id PRIMARY KEY ([Id]),
	CONSTRAINT UX_CalendarEntryType_Name UNIQUE NONCLUSTERED ([Name])
)

CREATE TABLE [CalendarEntry]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[Date] DATE NOT NULL,
	[Duration] TIME NOT NULL,
	[DurationSeconds] INT NOT NULL,
	[CalendarEntryTypeId] INT NOT NULL,
	[Note] NVARCHAR(MAX) NULL,
	CONSTRAINT PK_CalendarEntry_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_CalendarEntry_Date_CalendarEntryType_Id FOREIGN KEY ([CalendarEntryTypeId]) REFERENCES [CalendarEntryType] ([Id]),
	CONSTRAINT UX_CalendarEntry_Date_CalendarEntryTypeId UNIQUE NONCLUSTERED ([Date], [CalendarEntryTypeId])
)

-- CREATE TABLE [TicketPriority]
-- (
-- 	[Id] INT NOT NULL IDENTITY(1, 1),
-- 	[Name] NVARCHAR(255) NOT NULL,
-- 	CONSTRAINT PK_TicketPriority_Id PRIMARY KEY ([Id]),
-- 	CONSTRAINT UX_TicketPriority_Name UNIQUE NONCLUSTERED ([Name])
-- )

CREATE TABLE [TicketStatus]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[Name] NVARCHAR(255) NOT NULL,
	CONSTRAINT PK_TicketStatus_Id PRIMARY KEY ([Id]),
	CONSTRAINT UX_TicketStatus_Name UNIQUE NONCLUSTERED ([Name])
)

CREATE TABLE [Ticket]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[RefId] INT NULL, -- definiert dieses Ticket als Subticket oder Nachfolger
	[Title] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(MAX) NULL,
	-- [TicketPriorityId] INT NOT NULL,
	[TicketStatusId] INT NOT NULL,
	[CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	CONSTRAINT PK_Ticket_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_Ticket_RefId_Ticket_Id FOREIGN KEY ([RefId]) REFERENCES [Ticket] ([Id]),
	-- CONSTRAINT FK_Ticket_TicketPriorityId_TicketPriority_Id FOREIGN KEY ([TicketPriorityId]) REFERENCES [TicketPriority] ([Id]),
	CONSTRAINT FK_Ticket_TicketStatusId_TicketStatus_Id FOREIGN KEY ([TicketStatusId]) REFERENCES [TicketStatus] ([Id])
	--CONSTRAINT UX_Ticket_Title UNIQUE NONCLUSTERED ([Title])
)

CREATE TABLE [TicketAttachment]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[TicketId] INT NOT NULL,
	[Name] NVARCHAR(255) NOT NULL, -- Name mit Extension => Dateipfad: .../ticketId/name
	[Comment] NVARCHAR(MAX) NULL,
	CONSTRAINT PK_TicketAttachment_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_TicketAttachment_TicketId_Ticket_Id FOREIGN KEY ([TicketId]) REFERENCES [Ticket] ([Id]) ON DELETE CASCADE,
	CONSTRAINT UX_TicketAttachment_TicketId_Name UNIQUE NONCLUSTERED ([TicketId], [Name])
)

-- CREATE TABLE [TicketComment]
-- (
-- 	[Id] INT NOT NULL IDENTITY(1, 1),
-- 	[TicketId] INT NOT NULL,
-- 	[Comment] NVARCHAR(MAX) NOT NULL,
-- 	[CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
-- 	CONSTRAINT PK_TicketComment_Id PRIMARY KEY ([Id]),
-- 	CONSTRAINT FK_TicketComment_TicketId_Ticket_Id FOREIGN KEY ([TicketId]) REFERENCES [Ticket] ([Id]) ON DELETE CASCADE
-- )

-- CREATE TABLE [TicketCommentAttachment]
-- (
-- 	[Id] INT NOT NULL IDENTITY(1, 1),
-- 	[TicketCommentId] INT NOT NULL,
-- 	[Name] NVARCHAR(255) NOT NULL, -- Name mit Extension => Dateipfad: .../ticketId/ticketCommentId/name
-- 	[Comment] NVARCHAR(MAX) NOT NULL
-- 	CONSTRAINT PK_TicketCommentAttachment_Id PRIMARY KEY ([Id]),
-- 	CONSTRAINT FK_TicketCommentAttachment_TicketCommentId_TicketComment_Id FOREIGN KEY ([TicketCommentId]) REFERENCES [TicketComment] ([Id]) ON DELETE CASCADE,
-- 	CONSTRAINT UX_TicketCommentAttachment_TicketCommentId_Name UNIQUE NONCLUSTERED ([TicketCommentId], [Name])
-- )

CREATE TABLE [TicketStatusLog]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[TicketId] INT NOT NULL,
	[TicketStatusId] INT NOT NULL,
	[StartedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	[Note] NVARCHAR(MAX) NULL,
	CONSTRAINT PK_TicketStatusLog_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_TicketStatusLog_TicketId_Ticket_Id FOREIGN KEY ([TicketId]) REFERENCES [Ticket] ([Id]) ON DELETE CASCADE,
	CONSTRAINT FK_TicketStatusLog_TicketStatusId_TicketStatus_Id FOREIGN KEY ([TicketStatusId]) REFERENCES [TicketStatus] ([Id])
)

CREATE TABLE [Worklog]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[Date] DATE NOT NULL,
	[TicketId] INT NOT NULL,
	[Description] NVARCHAR(MAX) NULL,
	[TimeSpent] TIME NOT NULL,
	[TimeSpentSeconds] INT NOT NULL,
	CONSTRAINT PK_Worklog_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_Worklog_TicketId_Ticket_Id FOREIGN KEY ([TicketId]) REFERENCES [Ticket] ([Id]) ON DELETE CASCADE
)

CREATE TABLE [WorklogAttachment]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[WorklogId] INT NOT NULL,
	[Name] NVARCHAR(255) NOT NULL, -- Name mit Extension => Dateipfad: .../yyyy-MM-dd/[office/mobile]/worklogId/name
	[Comment] NVARCHAR(MAX) NULL
	CONSTRAINT PK_WorklogAttachment_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_WorklogAttachment_WorklogId_Worklog_Id FOREIGN KEY ([WorklogId]) REFERENCES [Worklog] ([Id]) ON DELETE CASCADE,
	CONSTRAINT UX_WorklogAttachment_WorklogId_Name UNIQUE NONCLUSTERED ([WorklogId], [Name])
)

INSERT [CalendarEntryType]
(
	[Name]
)
VALUES
(
	N'WorkTime' -- Pflichtarbeitszeit ...
),
(
	N'Office' -- ... davon im Büro
),
(
	N'Mobile' -- ... davon im Homeoffice
),
(
	N'Holiday'
),
(
	N'Vacation'
),
(
	N'TimeCompensation'
),
(
	N'Ill'
)

-- INSERT [TicketPriority]
-- (
-- 	[Name]
-- )
-- VALUES
-- (
-- 	N'Highest'
-- ),
-- (
-- 	N'High'
-- ),
-- (
-- 	N'Medium'
-- ),
-- (
-- 	N'Low'
-- ),
-- (
-- 	N'Lowest'
-- )

INSERT [TicketStatus]
(
	[Name]
)
VALUES
(
	N'Todo'
),
(
	N'Running'
),
(
	N'Paused'
),
(
	N'Blocked'
),
(
	N'Done'
),
(
	N'Canceled'
),
(
	N'Continuous'
)

COMMIT

SELECT * FROM [CalendarEntry]
SELECT * FROM [CalendarEntryType]
--SELECT * FROM [TicketPriority]
SELECT * FROM [TicketStatus]
SELECT * FROM [Ticket]
SELECT * FROM [TicketAttachment]
-- SELECT * FROM [TicketComment]
-- SELECT * FROM [TicketCommentAttachment]
SELECT * FROM [TicketStatusLog]
SELECT * FROM [Worklog]
SELECT * FROM [WorklogAttachment]

END TRY

BEGIN CATCH
	IF @@TRANCOUNT > 0
	BEGIN
		PRINT CHAR(13)+CHAR(10)
		DECLARE @Error NVARCHAR(MAX) = ERROR_MESSAGE()
		RAISERROR(@Error, 11, 0)
		ROLLBACK
	END
END CATCH
