CREATE TABLE [dbo].[WorkTime] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [WorkTimeTypeId]  INT            NOT NULL,
    [Date]            DATE           NOT NULL,
    [ExpectedSeconds] INT            NOT NULL,
    [ActualSeconds]   INT            NOT NULL,
    [Note]            NVARCHAR (MAX) NULL,
    [RowVersion]      ROWVERSION     NOT NULL,
    CONSTRAINT [PK_WorkTime_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WorkTime_WorkTimeTypeId_WorkTimeType_Id] FOREIGN KEY ([WorkTimeTypeId]) REFERENCES [dbo].[WorkTimeType] ([Id])
);

