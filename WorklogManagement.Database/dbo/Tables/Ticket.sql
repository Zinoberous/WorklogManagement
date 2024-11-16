CREATE TABLE [dbo].[Ticket] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [RefId]          INT            NULL,
    [Title]          NVARCHAR (255) NOT NULL,
    [Description]    NVARCHAR (MAX) NULL,
    [TicketStatusId] INT            NOT NULL,
    [CreatedAt]      DATETIME       DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_Ticket_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Ticket_RefId_Ticket_Id] FOREIGN KEY ([RefId]) REFERENCES [dbo].[Ticket] ([Id]),
    CONSTRAINT [FK_Ticket_TicketStatusId_TicketStatus_Id] FOREIGN KEY ([TicketStatusId]) REFERENCES [dbo].[TicketStatus] ([Id])
);

