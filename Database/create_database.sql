-- ================================================================
-- Sistem Evidenta Cursuri si Inscrieri — Centru de Instruire
-- Script creare baza de date + date de test
-- Autor: LPhex9 | Practica 2026
-- ================================================================

USE master;
GO

-- Creare BD daca nu exista
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CentruInstruire')
BEGIN
    CREATE DATABASE CentruInstruire;
END
GO

USE CentruInstruire;
GO

-- ================================================================
-- Tabelul Cursant
-- ================================================================
IF OBJECT_ID('dbo.Inscriere', 'U') IS NOT NULL DROP TABLE dbo.Inscriere;
IF OBJECT_ID('dbo.Curs',     'U') IS NOT NULL DROP TABLE dbo.Curs;
IF OBJECT_ID('dbo.Cursant',  'U') IS NOT NULL DROP TABLE dbo.Cursant;
GO

CREATE TABLE Cursant (
    IdCursant  INT IDENTITY(1,1) PRIMARY KEY,
    Nume       NVARCHAR(100) NOT NULL,
    Prenume    NVARCHAR(100) NOT NULL,
    Telefon    NVARCHAR(20)  NOT NULL,
    Email      NVARCHAR(150) NOT NULL UNIQUE
);
GO

-- ================================================================
-- Tabelul Curs
-- ================================================================
CREATE TABLE Curs (
    IdCurs     INT IDENTITY(1,1) PRIMARY KEY,
    Denumire   NVARCHAR(200) NOT NULL,
    Formator   NVARCHAR(150) NOT NULL,
    Pret       DECIMAL(10,2) NOT NULL CHECK (Pret > 0),
    DurataZile INT           NOT NULL CHECK (DurataZile > 0)
);
GO

-- ================================================================
-- Tabelul Inscriere
-- ================================================================
CREATE TABLE Inscriere (
    IdInscriere   INT IDENTITY(1,1) PRIMARY KEY,
    IdCursant     INT          NOT NULL REFERENCES Cursant(IdCursant),
    IdCurs        INT          NOT NULL REFERENCES Curs(IdCurs),
    DataInscriere DATE         NOT NULL DEFAULT GETDATE(),
    StatusPlata   NVARCHAR(20) NOT NULL DEFAULT 'Neachitat'
        CHECK (StatusPlata IN ('Achitat', 'Neachitat'))
);
GO

-- ================================================================
-- Date de test: 6 cursanti
-- ================================================================
INSERT INTO Cursant (Nume, Prenume, Telefon, Email) VALUES
('Popescu',   'Ion',      '069123456', 'ion.popescu@mail.md'),
('Ciobanu',   'Maria',    '078234567', 'maria.ciobanu@mail.md'),
('Rusu',      'Alexandru','060345678', 'alex.rusu@mail.md'),
('Moraru',    'Elena',    '069456789', 'elena.moraru@mail.md'),
('Lungu',     'Andrei',   '078567890', 'andrei.lungu@mail.md'),
('Constantin','Natalia',  '060678901', 'natalia.constantin@mail.md');
GO

-- ================================================================
-- Date de test: 6 cursuri
-- ================================================================
INSERT INTO Curs (Denumire, Formator, Pret, DurataZile) VALUES
('Programare C# Fundamentals',  'Vasile Botnaru',  2500.00, 30),
('Baze de Date SQL Server',     'Ana Grigore',     1800.00, 20),
('Web Development cu React',    'Mihai Popescu',   3200.00, 45),
('Testare Software QA',         'Irina Cojocaru',  2000.00, 25),
('Managementul Proiectelor IT', 'Gheorghe Luca',   1500.00, 15),
('Data Analysis cu Python',     'Tatiana Vrabie',  2800.00, 35);
GO

-- ================================================================
-- Date de test: 8 inscrieri
-- ================================================================
INSERT INTO Inscriere (IdCursant, IdCurs, DataInscriere, StatusPlata) VALUES
(1, 1, '2026-04-21', 'Achitat'),
(1, 2, '2026-04-22', 'Achitat'),
(2, 3, '2026-04-21', 'Neachitat'),
(3, 4, '2026-04-23', 'Achitat'),
(4, 1, '2026-04-24', 'Neachitat'),
(5, 5, '2026-04-25', 'Achitat'),
(6, 6, '2026-04-26', 'Achitat'),
(2, 4, '2026-04-27', 'Neachitat');
GO

-- ================================================================
-- Verificare
-- ================================================================
SELECT 'Cursanti'  AS Tabel, COUNT(*) AS NrInregistrari FROM Cursant
UNION ALL
SELECT 'Cursuri',  COUNT(*) FROM Curs
UNION ALL
SELECT 'Inscrieri',COUNT(*) FROM Inscriere;
GO
