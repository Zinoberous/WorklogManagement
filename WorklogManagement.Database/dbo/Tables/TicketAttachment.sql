CREATE TABLE [dbo].[TicketAttachment] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [TicketId] INT            NOT NULL,
    [Name]     NVARCHAR (255) NOT NULL,
    [Comment]  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TicketAttachment_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TicketAttachment_TicketId_Ticket_Id] FOREIGN KEY ([TicketId]) REFERENCES [dbo].[Ticket] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [UX_TicketAttachment_TicketId_Name] UNIQUE NONCLUSTERED ([TicketId] ASC, [Name] ASC)
);

