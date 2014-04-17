USE [TransactionScopeTests];

DECLARE @fromT1 INT = 0;
DECLARE @fromT2 INT = 0;
DECLARE @fromT3 INT = 0;

SELECT @fromT1 = COUNT([Key]) FROM [T1];
SELECT @fromT2 = COUNT([Key]) FROM [T2];
SELECT @fromT3 = COUNT([Key]) FROM [T3];


SELECT @fromT1, @fromT2, @fromT3