CREATE TABLE [dbo].[WorklogAttachment] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [WorklogId]  INT            NOT NULL,
    [Name]       NVARCHAR (255) NOT NULL,
    [Comment]    NVARCHAR (MAX) NULL,
    [RowVersion] ROWVERSION     NOT NULL,
    CONSTRAINT [PK_WorklogAttachment_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WorklogAttachment_WorklogId_Worklog_Id] FOREIGN KEY ([WorklogId]) REFERENCES [dbo].[Worklog] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [UX_WorklogAttachment_WorklogId_Name] UNIQUE NONCLUSTERED ([WorklogId] ASC, [Name] ASC)
);

