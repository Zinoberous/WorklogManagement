-- Todo, Running, Paused, Blocked, Done, Canceled, Continues
CREATE TABLE [dbo].[TicketStatus] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_TicketStatus_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UX_TicketStatus_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

