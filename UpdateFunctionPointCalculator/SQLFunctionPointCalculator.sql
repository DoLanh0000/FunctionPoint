CREATE DATABASE FunctionPointDB;
GO

USE FunctionPointDB;
GO

CREATE TABLE FunctionPointResults (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UFP DECIMAL(18,2),
    VAF DECIMAL(18,2),
    FP DECIMAL(18,2),
    CalculationDate DATETIME
);

SELECT * FROM FunctionPointResults;

USE FunctionPointDB;
GO

ALTER TABLE FunctionPointResults
ADD SrsFileName NVARCHAR(255) NULL,
    SrsAnalysisResult NVARCHAR(MAX) NULL;

DELETE FROM FunctionPointResults;
DBCC CHECKIDENT ('FunctionPointResults', RESEED, 0);
