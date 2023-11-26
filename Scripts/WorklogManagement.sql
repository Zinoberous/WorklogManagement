BEGIN TRAN

BEGIN TRY

--CREATE DATABASE [WorklogManagement]
--GO

USE [WorklogManagement]
GO

IF OBJECT_ID('WorklogAttachment', 'U') IS NOT NULL
    DROP TABLE [WorklogAttachment];

IF OBJECT_ID('Worklog', 'U') IS NOT NULL
    DROP TABLE [Worklog];

IF OBJECT_ID('TicketStatusLog', 'U') IS NOT NULL
    DROP TABLE [TicketStatusLog];

-- IF OBJECT_ID('TicketCommentAttachment', 'U') IS NOT NULL
--     DROP TABLE [TicketCommentAttachment];

-- IF OBJECT_ID('TicketComment', 'U') IS NOT NULL
--     DROP TABLE [TicketComment];

IF OBJECT_ID('TicketAttachment', 'U') IS NOT NULL
    DROP TABLE [TicketAttachment];

IF OBJECT_ID('Ticket', 'U') IS NOT NULL
    DROP TABLE [Ticket];

IF OBJECT_ID('TicketStatus', 'U') IS NOT NULL
    DROP TABLE [TicketStatus];

-- IF OBJECT_ID('TicketPriority', 'U') IS NOT NULL
--     DROP TABLE [TicketPriority];

IF OBJECT_ID('CalendarEntry', 'U') IS NOT NULL
    DROP TABLE [CalendarEntry];

IF OBJECT_ID('CalendarEntryType', 'U') IS NOT NULL
    DROP TABLE [CalendarEntryType];

GO

CREATE TABLE [CalendarEntryType]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[Name] NVARCHAR(255) NOT NULL,
	CONSTRAINT PK_CalendarEntryType_Id PRIMARY KEY ([Id]),
	CONSTRAINT UX_CalendarEntryType_Name UNIQUE NONCLUSTERED ([Name])
)
GO

CREATE TABLE [CalendarEntry]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[Date] DATE NOT NULL,
	[Duration] TIME NOT NULL,
	[CalendarEntryTypeId] INT NOT NULL,
	[Note] NVARCHAR(MAX) NULL,
	CONSTRAINT PK_CalendarEntry_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_CalendarEntry_Date_CalendarEntryType_Id FOREIGN KEY ([CalendarEntryTypeId]) REFERENCES [CalendarEntryType] ([Id]),
	CONSTRAINT UX_CalendarEntry_Date_CalendarEntryTypeId UNIQUE NONCLUSTERED ([Date], [CalendarEntryTypeId])
)
GO

-- CREATE TABLE [TicketPriority]
-- (
-- 	[Id] INT NOT NULL IDENTITY(1, 1),
-- 	[Name] NVARCHAR(255) NOT NULL,
-- 	CONSTRAINT PK_TicketPriority_Id PRIMARY KEY ([Id]),
-- 	CONSTRAINT UX_TicketPriority_Name UNIQUE NONCLUSTERED ([Name])
-- )
-- GO

CREATE TABLE [TicketStatus]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[Name] NVARCHAR(255) NOT NULL,
	CONSTRAINT PK_TicketStatus_Id PRIMARY KEY ([Id]),
	CONSTRAINT UX_TicketStatus_Name UNIQUE NONCLUSTERED ([Name])
)
GO

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
GO

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
GO

-- CREATE TABLE [TicketComment]
-- (
-- 	[Id] INT NOT NULL IDENTITY(1, 1),
-- 	[TicketId] INT NOT NULL,
-- 	[Comment] NVARCHAR(MAX) NOT NULL,
-- 	[CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
-- 	CONSTRAINT PK_TicketComment_Id PRIMARY KEY ([Id]),
-- 	CONSTRAINT FK_TicketComment_TicketId_Ticket_Id FOREIGN KEY ([TicketId]) REFERENCES [Ticket] ([Id]) ON DELETE CASCADE
-- )
-- GO

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
-- GO

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
GO

CREATE TABLE [Worklog]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[Date] DATE NOT NULL,
	[TicketId] INT NOT NULL,
	[Description] NVARCHAR(MAX) NULL,
	[TimeSpent] TIME NOT NULL,
	CONSTRAINT PK_Worklog_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_Worklog_TicketId_Ticket_Id FOREIGN KEY ([TicketId]) REFERENCES [Ticket] ([Id]) ON DELETE CASCADE
)
GO

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
GO

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
GO

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
-- GO

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
GO

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
	ROLLBACK
	THROW
END CATCH
