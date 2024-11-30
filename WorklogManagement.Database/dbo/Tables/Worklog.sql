CREATE TABLE [dbo].[Worklog] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Date]             DATE           NOT NULL,
    [TicketId]         INT            NOT NULL,
    [Description]      NVARCHAR (MAX) NULL,
    [TimeSpent]        TIME (7)       NOT NULL,
    [TimeSpentSeconds] INT            NOT NULL,
    [RowVersion]       ROWVERSION     NOT NULL,
    CONSTRAINT [PK_Worklog_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Worklog_TicketId_Ticket_Id] FOREIGN KEY ([TicketId]) REFERENCES [dbo].[Ticket] ([Id]) ON DELETE CASCADE
);

