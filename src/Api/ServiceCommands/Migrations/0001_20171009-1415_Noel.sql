-- <Migration ID="03f2e6c4-86f0-4cd3-b122-c458282a338f" />
GO

PRINT N'Creating [dbo].[Subscribers]'
GO
CREATE TABLE [dbo].[Subscribers]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[TimeToExpire] [time] NOT NULL,
[LastUpdatedDate] [datetime] NULL,
[Completed] [bit] NOT NULL,
[RetryCount] [int] NOT NULL CONSTRAINT [DF__Subscribe__Retry__1273C1CD] DEFAULT ((0)),
[RetriesAllowed] [int] NOT NULL CONSTRAINT [DF__Subscribe__Retri__1367E606] DEFAULT ((6)),
[TypeName] [nvarchar] (max) NULL,
[WorkloadType] [nvarchar] (max) NULL,
[ServiceCommandId] [int] NULL,
[SerializedWorkload] [nvarchar] (max) NULL,
[SerializedCommand] [nvarchar] (max) NULL,
[BinarySerializedWorkload] [varbinary] (max) NULL,
[BinarySerializedCommand] [varbinary] (max) NULL,
[Workload] [nvarchar] (max) NULL
)
GO
PRINT N'Creating primary key [PK_dbo.Subscribers] on [dbo].[Subscribers]'
GO
ALTER TABLE [dbo].[Subscribers] ADD CONSTRAINT [PK_dbo.Subscribers] PRIMARY KEY CLUSTERED  ([Id])
GO
PRINT N'Creating [dbo].[ServiceCommands]'
GO
CREATE TABLE [dbo].[ServiceCommands]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[ReceivedDate] [datetime] NOT NULL,
[CompletedDate] [datetime] NULL,
[SerializedCommand] [nvarchar] (max) NULL,
[Completed] [bit] NOT NULL,
[CommandType] [nvarchar] (100) NULL,
[EndpointId] [nvarchar] (100) NULL,
[UniqueKey] [nvarchar] (100) NOT NULL UNIQUE,
[User] [nvarchar] (100) NULL,
[CreatedBy] [nvarchar] (50) NULL
)
GO
PRINT N'Creating primary key [PK_dbo.ServiceCommands] on [dbo].[ServiceCommands]'
GO
ALTER TABLE [dbo].[ServiceCommands] ADD CONSTRAINT [PK_dbo.ServiceCommands] PRIMARY KEY CLUSTERED  ([Id])
GO
