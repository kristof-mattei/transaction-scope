USE [TransactionScopeTests];

CREATE TABLE [dbo].[T1] (
    [Key]   INT IDENTITY (1, 1) NOT NULL,
    [Value] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Key] ASC)
);

CREATE TABLE [dbo].[T2] (
    [Key]   INT IDENTITY (1, 1) NOT NULL,
    [T1Key] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Key] ASC),
    CONSTRAINT [FK_T2_T1] FOREIGN KEY ([T1Key]) REFERENCES [dbo].[T1] ([Key])
);

CREATE TABLE [dbo].[T3] (
    [Key]   INT IDENTITY (1, 1) NOT NULL,
    [T1Key] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Key] ASC),
    CONSTRAINT [FK_T3_T1] FOREIGN KEY ([T1Key]) REFERENCES [dbo].[T1] ([Key])
);

INSERT INTO [T1] VALUES (1), (2), (3), (4), (5), (6);
INSERT INTO [T2] VALUES (1), (2), (3), (4), (5);
INSERT INTO [T3] VALUES (1), (2), (3), (4), (5);