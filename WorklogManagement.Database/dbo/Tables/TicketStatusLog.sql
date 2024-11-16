CREATE TABLE [dbo].[TicketStatusLog] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [TicketId]       INT            NOT NULL,
    [TicketStatusId] INT            NOT NULL,
    [StartedAt]      DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [Note]           NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TicketStatusLog_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TicketStatusLog_TicketId_Ticket_Id] FOREIGN KEY ([TicketId]) REFERENCES [dbo].[Ticket] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TicketStatusLog_TicketStatusId_TicketStatus_Id] FOREIGN KEY ([TicketStatusId]) REFERENCES [dbo].[TicketStatus] ([Id])
);

