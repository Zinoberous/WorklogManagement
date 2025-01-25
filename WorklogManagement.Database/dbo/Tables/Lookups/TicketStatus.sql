-- Todo, Running, Paused, Blocked, Done, Canceled, Continues
CREATE TABLE [dbo].[TicketStatus] (
    [Id]         INT            NOT NULL,
    [Name]       NVARCHAR (255) NOT NULL,
    [RowVersion] ROWVERSION     NOT NULL,
    CONSTRAINT [PK_TicketStatus_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UX_TicketStatus_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

