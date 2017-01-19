CREATE TABLE [dbo].[Documents] (
    [Id]                [uniqueidentifier] NOT NULL,
    [Created]           [datetime] NOT NULL,
    [CreatedBy]         [int] NOT NULL,
    [LastModified]      [datetime],
    [LastModifiedBy]    [int],
    [Deleted]           [datetime],
    [DeletedBy]         [int],
    [ActivityLogId]     [int],
    [Title]             [nvarchar](1024) NOT NULL,
    [Size]              [int] NOT NULL,
    [MimeType]          [varchar](128) NOT NULL,
    [Content]           [varbinary](max) NOT NULL,

    CONSTRAINT [FK_Documents_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Documents_ActivityLogId] FOREIGN KEY ([ActivityLogId]) REFERENCES [dbo].[ActivityLog] ([Id]),
    CONSTRAINT [FK_Documents_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[Users] ([Id])
);
