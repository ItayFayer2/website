CREATE TABLE [dbo].[Users] (
    [username] VARCHAR(50) NOT NULL PRIMARY KEY,
    [password] VARCHAR(50) NOT NULL,
    [email] VARCHAR(50) NULL,
    [phone_number] VARCHAR(50) NULL,
    [city] VARCHAR(50) NULL,
    [country] VARCHAR(50) NULL,
    [age] INT NULL
);