CREATE DATABASE FunctionPointDB;
GO

USE FunctionPointDB;
GO

CREATE TABLE FunctionPointResults (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UFP DECIMAL(18,2),
    VAF DECIMAL(18,2),
    FP DECIMAL(18,2),
    CalculationDate DATETIME,
    SrsFileName NVARCHAR(255) NULL,
    SrsAnalysisResult NVARCHAR(MAX) NULL,
    EiLow INT NULL,
    EiAvg INT NULL,
    EiHigh INT NULL,
    EoLow INT NULL,
    EoAvg INT NULL,
    EoHigh INT NULL,
    EqLow INT NULL,
    EqAvg INT NULL,
    EqHigh INT NULL,
    IlfLow INT NULL,
    IlfAvg INT NULL,
    IlfHigh INT NULL,
    EifLow INT NULL,
    EifAvg INT NULL,
    EifHigh INT NULL,
    F1 INT NULL,
    F2 INT NULL,
    F3 INT NULL,
    F4 INT NULL,
    F5 INT NULL,
    F6 INT NULL,
    F7 INT NULL,
    F8 INT NULL,
    F9 INT NULL,
    F10 INT NULL,
    F11 INT NULL,
    F12 INT NULL,
    F13 INT NULL,
    F14 INT NULL
);

SELECT * FROM FunctionPointResults;

DELETE FROM FunctionPointResults;
DBCC CHECKIDENT ('FunctionPointResults', RESEED, 0);


