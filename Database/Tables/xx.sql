﻿CREATE TABLE [dbo].[Addresses]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1)
	, [PersonId] INT NOT NULL CONSTRAINT FK_Addresses_Persons FOREIGN KEY REFERENCES Persons(Id)
	, [Street] NVARCHAR (255) NOT NULL
	, [Number] NVARCHAR (20) NOT NULL
	, [Zip] NVARCHAR(10) NOT NULL
	, [State] NVARCHAR(255) NULL
	, [Country] NVARCHAR(100) NOT NULL
)
