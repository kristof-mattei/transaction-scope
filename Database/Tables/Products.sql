CREATE TABLE [dbo].[Products]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1)
	, [ProductName] NVARCHAR(255) NOT NULL
    , [CategoryId] INT NOT NULL CONSTRAINT FK_Products_Categories FOREIGN KEY REFERENCES Categories(Id)
    , [BrandId] INT NOT NULL CONSTRAINT FK_Products_Brands FOREIGN KEY REFERENCES Brands(Id)
    , [Price] DECIMAL(14,2) NOT NULL
    , [Stock] INT NOT NULL DEFAULT(0)
	
)
