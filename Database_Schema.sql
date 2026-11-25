-- ========================================================
-- Database Schema for Sunshine Hostel Management System
-- Compatible with SQL Server 2016+ / LocalDB / Express
-- Target Database: [Hostel dupli]
-- ========================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'Hostel dupli')
BEGIN
    CREATE DATABASE [Hostel dupli];
END
GO

USE [Hostel dupli];
GO

-- ========================================================
-- 1. Table Definitions
-- ========================================================

-- Admin table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Admintab')
BEGIN
    CREATE TABLE Admintab (
        id INT PRIMARY KEY,
        admin_name VARCHAR(100) NOT NULL,
        admin_phone VARCHAR(20) NULL,
        admin_email VARCHAR(100) NULL
    );
END
GO

-- Student/User table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Usertab')
BEGIN
    CREATE TABLE Usertab (
        id INT PRIMARY KEY,
        name VARCHAR(100) NOT NULL,
        gender VARCHAR(20) NULL,
        age INT NULL,
        phone VARCHAR(20) NULL,
        email VARCHAR(100) NULL,
        address VARCHAR(255) NULL,
        join_date DATETIME NULL
    );
END
GO

-- Central Authentication Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Logintab')
BEGIN
    CREATE TABLE Logintab (
        rid INT NOT NULL,
        username VARCHAR(100) NOT NULL,
        password VARCHAR(100) NOT NULL,
        logintype VARCHAR(50) NOT NULL
    );
END
GO

-- Rooms Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Roomtab')
BEGIN
    CREATE TABLE Roomtab (
        Room_id INT IDENTITY(1,1) PRIMARY KEY,
        Rent VARCHAR(50) NOT NULL,
        Typeofsharing VARCHAR(100) NOT NULL,
        Occupied VARCHAR(50) NOT NULL
    );
END
GO

-- Requests Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'Requesttab')
BEGIN
    CREATE TABLE Requesttab (
        Request_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NOT NULL,
        RoomType VARCHAR(100) NOT NULL,
        Rent VARCHAR(50) NOT NULL,
        Status VARCHAR(50) NOT NULL DEFAULT 'Pending'
    );
END
GO

-- ========================================================
-- 2. Stored Procedures
-- ========================================================

-- sp_maximum: Returns maximum registration id
CREATE OR ALTER PROCEDURE sp_maximum
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ISNULL(MAX(rid), 0) FROM Logintab;
END
GO

-- sp_admininsert: Registers a new administrator
CREATE OR ALTER PROCEDURE sp_admininsert
    @id INT,
    @admin_name VARCHAR(100),
    @admin_phone VARCHAR(20),
    @admin_email VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Admintab (id, admin_name, admin_phone, admin_email)
    VALUES (@id, @admin_name, @admin_phone, @admin_email);
END
GO

-- sp_userinsert: Registers a new student/user
CREATE OR ALTER PROCEDURE sp_userinsert
    @id INT,
    @name VARCHAR(100),
    @gen VARCHAR(20),
    @age INT,
    @phone VARCHAR(20),
    @email VARCHAR(100),
    @addr VARCHAR(255),
    @joind DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Usertab (id, name, gender, age, phone, email, address, join_date)
    VALUES (@id, @name, @gen, @age, @phone, @email, @addr, @joind);
END
GO

-- sp_insertlogin: Inserts login credentials
CREATE OR ALTER PROCEDURE sp_insertlogin
    @rid INT,
    @una VARCHAR(100),
    @pwd VARCHAR(100),
    @ltype VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Logintab (rid, username, password, logintype)
    VALUES (@rid, @una, @pwd, @ltype);
END
GO

-- sp_countid: Validates user credentials
CREATE OR ALTER PROCEDURE sp_countid
    @uname VARCHAR(100),
    @pwd VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) FROM Logintab WHERE username = @uname AND password = @pwd;
END
GO

-- sp_regid: Retrieves the registration id for valid credentials
CREATE OR ALTER PROCEDURE sp_regid
    @una VARCHAR(100),
    @pwd VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 rid FROM Logintab WHERE username = @una AND password = @pwd;
END
GO

-- sp_logintype: Retrieves role type (Admin / User)
CREATE OR ALTER PROCEDURE sp_logintype
    @una VARCHAR(100),
    @pwd VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 logintype FROM Logintab WHERE username = @una AND password = @pwd;
END
GO

-- sp_rooms: Inserts a new room
CREATE OR ALTER PROCEDURE sp_rooms
    @rent VARCHAR(50),
    @typesharing VARCHAR(100),
    @occupied VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Roomtab (Rent, Typeofsharing, Occupied)
    VALUES (@rent, @typesharing, @occupied);
END
GO

-- sp_getAllRooms: Lists all rooms
CREATE OR ALTER PROCEDURE sp_getAllRooms
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Rent, Typeofsharing, Occupied FROM Roomtab;
END
GO

-- sp_insertrequest: Creates a new room booking request
CREATE OR ALTER PROCEDURE sp_insertrequest
    @uid INT,
    @rtype VARCHAR(100),
    @rent VARCHAR(50),
    @status VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Requesttab (user_id, RoomType, Rent, Status)
    VALUES (@uid, @rtype, @rent, @status);
END
GO

-- sp_pendingreq: Retrieves all requests awaiting admin review
CREATE OR ALTER PROCEDURE sp_pendingreq
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Request_id, user_id, RoomType, Rent, Status 
    FROM Requesttab 
    WHERE Status = 'Pending';
END
GO

-- sp_update: Updates room occupancy count
CREATE OR ALTER PROCEDURE sp_update
    @roomType VARCHAR(100),
    @newOccupied INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Roomtab 
    SET Occupied = CAST(@newOccupied AS VARCHAR(50)) 
    WHERE Typeofsharing = @roomType;
END
GO

-- sp_getAllUsers: Retrieves registered student records
CREATE OR ALTER PROCEDURE sp_getAllUsers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id, name, gender, age, phone, email, address, join_date 
    FROM Usertab 
    ORDER BY id DESC;
END
GO

-- ========================================================
-- 3. Seed Default Data (Idempotent)
-- ========================================================

-- Seed Admin credentials if not present
IF NOT EXISTS (SELECT 1 FROM Logintab WHERE username = 'admin')
BEGIN
    INSERT INTO Admintab (id, admin_name, admin_phone, admin_email)
    VALUES (1, 'System Administrator', '+91-9876543210', 'admin@sunshinehostel.com');

    INSERT INTO Logintab (rid, username, password, logintype)
    VALUES (1, 'admin', 'admin123', 'Admin');
END
GO

-- Seed Default Demo Student if not present
IF NOT EXISTS (SELECT 1 FROM Logintab WHERE username = 'student')
BEGIN
    INSERT INTO Usertab (id, name, gender, age, phone, email, address, join_date)
    VALUES (2, 'John Doe', 'Male', 21, '+91-9876500001', 'john.doe@example.com', '123 University Road, Kakkanad', GETDATE());

    INSERT INTO Logintab (rid, username, password, logintype)
    VALUES (2, 'student', 'user123', 'User');
END
GO

-- Seed Sample Rooms if empty
IF NOT EXISTS (SELECT 1 FROM Roomtab)
BEGIN
    INSERT INTO Roomtab (Rent, Typeofsharing, Occupied) VALUES
    ('8500', 'Single Room (AC)', '1'),
    ('5500', '2-Sharing (Non-AC)', '2'),
    ('4500', '3-Sharing (Non-AC)', '1'),
    ('4000', '4-Sharing (Non-AC)', '3');
END
GO
