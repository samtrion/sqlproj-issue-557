CREATE TABLE [dbo].[mainCode] (
    [id]            INT            NOT NULL IDENTITY(1,1) ,
    [code]          NVARCHAR (100) NOT NULL,
    [description]   NVARCHAR (MAX) NOT NULL,
    [isActive]      BIT            NOT NULL,
    [isRewritable]  BIT            NOT NULL,
    [isDeleted]     BIT            NOT NULL DEFAULT ((0)),
    CONSTRAINT [PK_mainCode] PRIMARY KEY CLUSTERED ([id] ASC)
);

