CREATE DATABASE [WorklogManagement]
GO

USE [WorklogManagement]
GO

-- CREATE TABLE [Sprint]
-- (
-- 	[Id] INT NOT NULL IDENTITY(1, 1),
-- 	[Title] NVARCHAR(255) NOT NULL,
-- 	[Description] NVARCHAR(MAX) NULL,
-- 	[StartAt] DATE NOT NULL,
-- 	[EndAt] DATE NOT NULL,
-- 	CONSTRAINT PK_Sprint_Id PRIMARY KEY ([Id]),
-- 	CONSTRAINT UX_Sprint_Title UNIQUE NONCLUSTERED ([Title])
-- )
-- GO

-- CREATE TABLE [Priority]
-- (
-- 	[Id] INT NOT NULL IDENTITY(1, 1),
-- 	[Name] NVARCHAR(255) NOT NULL,
-- 	CONSTRAINT PK_Priority_Id PRIMARY KEY ([Id]),
-- 	CONSTRAINT UX_Priority_Name UNIQUE NONCLUSTERED ([Name])
-- )
-- GO

CREATE TABLE [Status]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[Name] NVARCHAR(255) NOT NULL,
	CONSTRAINT PK_Status_Id PRIMARY KEY ([Id]),
	CONSTRAINT UX_Status_Name UNIQUE NONCLUSTERED ([Name])
)
GO

CREATE TABLE [Ticket]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[RefId] INT NULL, -- definiert dieses Ticket als Subticket oder Nachfolger
	[Title] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(MAX) NULL,
	-- [SprintId] INT NULL,
	-- [PriorityId] INT NOT NULL,
	[StatusId] INT NOT NULL,
	CONSTRAINT PK_Ticket_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_Ticket_RefId_Ticket_Id FOREIGN KEY ([RefId]) REFERENCES [Ticket] ([Id]),
	-- CONSTRAINT FK_Ticket_SprintId_Sprint_Id FOREIGN KEY ([SprintId]) REFERENCES [Sprint] ([Id]),
	-- CONSTRAINT FK_Ticket_PriorityId_Priority_Id FOREIGN KEY ([PriorityId]) REFERENCES [Priority] ([Id]),
	CONSTRAINT FK_Ticket_StatusId_Status_Id FOREIGN KEY ([StatusId]) REFERENCES [Status] ([Id]),
	CONSTRAINT UX_Ticket_Title UNIQUE NONCLUSTERED ([Title])
)
GO

-- CREATE TABLE [PriorityHistory]
-- (
-- 	[Id] INT NOT NULL IDENTITY(1, 1),
-- 	[TicketId] INT NOT NULL,
-- 	[PriorityId] INT NOT NULL,
-- 	[Comment] NVARCHAR(MAX) NULL,
-- 	CONSTRAINT PK_PriorityHistory_Id PRIMARY KEY ([Id]),
-- 	CONSTRAINT FK_PriorityHistory_TicketId_Ticket_Id FOREIGN KEY ([TicketId]) REFERENCES [Ticket] ([Id]),
-- 	CONSTRAINT FK_PriorityHistory_PriorityId_Priority_Id FOREIGN KEY ([PriorityId]) REFERENCES [Status] ([Id])
-- )
-- GO

CREATE TABLE [StatusHistory]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[TicketId] INT NOT NULL,
	[StatusId] INT NOT NULL,
	[Comment] NVARCHAR(MAX) NULL,
	CONSTRAINT PK_StatusHistory_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_StatusHistory_TicketId_Ticket_Id FOREIGN KEY ([TicketId]) REFERENCES [Ticket] ([Id]),
	CONSTRAINT FK_StatusHistory_StatusId_Status_Id FOREIGN KEY ([StatusId]) REFERENCES [Status] ([Id])
)
GO

CREATE TABLE [Workload]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[Name] NVARCHAR(255) NOT NULL,
	CONSTRAINT PK_Workload_Id PRIMARY KEY ([Id]),
	CONSTRAINT UX_Workload_Name UNIQUE NONCLUSTERED ([Name])
)
GO

CREATE TABLE [Day]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[Date] DATE NOT NULL,
	[IsMobile] BIT NOT NULL,
	[WorkloadId] INT NULL, -- NULL für Einträge außerhalb der Arbeit (Wochende, Urlaub, Feiertage, ...)
	[WorkloadComment] NVARCHAR(MAX) NULL,
	CONSTRAINT PK_Day_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_Day_WorkloadId_Workload_Id FOREIGN KEY ([WorkloadId]) REFERENCES [Workload] ([Id]),
	CONSTRAINT UX_Day_Date_Mobile UNIQUE NONCLUSTERED ([Date], [IsMobile])
)
GO

CREATE TABLE [Worklog]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[DayId] INT NOT NULL,
	[TicketId] INT NOT NULL,
	[Description] NVARCHAR(MAX) NULL,
	[TimeSpent] TIME NOT NULL,
	[TimeSpentComment] NVARCHAR(MAX) NULL,
	CONSTRAINT PK_Worklog_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_Worklog_DayId_Day_Id FOREIGN KEY ([DayId]) REFERENCES [Day] ([Id]),
	CONSTRAINT FK_Worklog_TicketId_Ticket_Id FOREIGN KEY ([TicketId]) REFERENCES [Ticket] ([Id])
)
GO

CREATE TABLE [Attachment]
(
	[Id] INT NOT NULL IDENTITY(1, 1),
	[WorklogId] INT NOT NULL,
	[Name] NVARCHAR(255) NOT NULL, -- Name mit Extension => Dateipfad: .../yyyy-MM-dd/[office/mobile]/worklogId/name
	[Comment] NVARCHAR(MAX) NOT NULL
	CONSTRAINT PK_Attachment_Id PRIMARY KEY ([Id]),
	CONSTRAINT FK_Attachment_WorklogId_Worklog_Id FOREIGN KEY ([WorklogId]) REFERENCES [Worklog] ([Id]),
	CONSTRAINT UX_Attachment_Name UNIQUE NONCLUSTERED ([WorklogId], [Name])
)
GO

-- INSERT [Priority]
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

INSERT [Status]
(
	[Name]
)
VALUES
(
	N'Todo'
),
(
	N'Blocked'
),
(
	N'Running'
),
(
	N'Paused'
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

INSERT [Workload]
(
	[Name]
)
VALUES
(
	N'Very High'
),
(
	N'High'
),
(
	N'Medium'
),
(
	N'Low'
),
(
	N'Very Low'
)
GO
