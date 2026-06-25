/* This is for recreate the database
    for demo purposue
*/
USE LibraryDB;
GO --Batching statement -MS SQLServer exclusive

--Section 1 -DDL

DROP TABLE IF EXISTS dbo.Loan;
DROP TABLE IF EXISTS dbo.Book;
DROP TABLE IF EXISTS dbo.Member;
DROP TABLE IF EXISTS dbo.Author;
GO

CREATE TABLE dbo.Author
(
    AuthorId INT IDENTITY(1,1) NOT NULL, -- In SQL Server identity allow us define primary key with auto-increment
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    BirthYear INT NULL,

    --We can add some named constraint after define columns

    CONSTRAINT PK_Author PRIMARY KEY (AuthorId),
    CONSTRAINT CK_Author_BirthYear CHECK(BirthYear IS NULL OR BirthYear BETWEEN 300 AND 2050) 
);

GO
CREATE TABLE dbo.Member
(
    MemberId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(250) NOT NULL UNIQUE,
    JoinedDate DATE NOT NULL DEFAULT (GETDATE())
);

GO
CREATE TABLE dbo.Book
(
    BookId INT IDENTITY(1,1) NOT NULL,
    Title VARCHAR(500) NOT NULL,
    ISBN CHAR(13) NOT NULL,
    PublishedYear INT NOT NULL,
    CategoryName VARCHAR(60) NOT NULL CONSTRAINT DF_Book_CategoryName DEFAULT ('General'),
    AuthorId INT NOT NULL,
    TotalCopies INT NOT NULL CONSTRAINT DF_Book_TotalCopies DEFAULT(1),
    AvailableCopies INT NOT NULL CONSTRAINT DF_Book_AvailableCopies DEFAULT(1),

    CONSTRAINT PK_Book PRIMARY KEY (BookId),
    CONSTRAINT UQ_Book_ISBN UNIQUE(ISBN),
    --Foreign key
    CONSTRAINT FK_Book_Author FOREIGN KEY (AuthorId) REFERENCES dbo.Author (AuthorId) ON DELETE CASCADE,
    CONSTRAINT CK_Book_Copies CHECK(TotalCopies >= AvailableCopies)
);

GO
CREATE TABLE dbo.Loan
(
    LoanId INT IDENTITY(1,1) NOT NULL,
    BookId INT NOT NULL,
    MemberId INT NOT NULL,
    LoanDate DATE NOT NULL CONSTRAINT DF_Loan_Date DEFAULT(GETDATE()),
    DueDate DATE NOT NULL,
    ReturnDate DATE NULL,

    CONSTRAINT PK_Loan PRIMARY KEY (LoanId),
    CONSTRAINT FK_Loan_Book FOREIGN KEY (BookId) REFERENCES dbo.Book (BookId),
    CONSTRAINT FK_Loan_Member FOREIGN KEY (MemberId) REFERENCES dbo.Member (MemberId),
    CONSTRAINT CK_Loan_Dates CHECK(DueDate >= LoanDate)
);
GO

ALTER TABLE dbo.Book ADD Edition INT NOT NULL CONSTRAINT DF_Book_Edition DEFAULT(1);

ALTER TABLE dbo.Book ALTER COLUMN Title VARCHAR(250) NOT NULL;

--DROP TABLE dbo.Loan; --Eliminate data and structrure

--TRUNCATE TABLE dbo.Loan; --Only eliminate data

-- Section 2: DML + DQL - Reading and writing (CRUD)
-- DML Insert, Update, Delete
-- DQL Select (another keywords like Group by, having, Where, etc. live)
INSERT INTO dbo.Author (FirstName, LastName, BirthYear)
VALUES('Robert', 'Martin', 1952);

INSERT INTO dbo.Author (FirstName, LastName, BirthYear) VALUES
    ('Martin', 'Fowler', 1963),
    ('Frank', 'Herbert', 1920),
    ('Kent', 'Beck', 1961);

SELECT * FROM dbo.Author;

INSERT INTO dbo.Book (Title, ISBN, PublishedYear, CategoryName, AuthorId, 
                            TotalCopies, AvailableCopies, Edition)
VALUES('Clean Code', '9780132350884', 2008, 'Software', 1, 3, 3, 1);

SELECT * FROM dbo.Book;
GO

INSERT INTO dbo.Author (FirstName, LastName, BirthYear) VALUES
    ('Robert',  'Martin',   1952),   -- 1
    ('Martin',  'Fowler',   1963),   -- 2
    ('Kent',    'Beck',     1961),   -- 3
    ('Erich',   'Gamma',    1961),   -- 4
    ('Andrew',  'Hunt',     1964),   -- 5
    ('David',   'Thomas',   1956);   -- 6
GO

INSERT INTO dbo.Member (FirstName, LastName, Email, JoinedDate) VALUES
    ('Ada',     'Lovelace', 'ada@example.com',     '2025-01-10'),  -- 1
    ('Grace',   'Hopper',   'grace@example.com',   '2025-02-02'),  -- 2
    ('Alan',    'Turing',   'alan@example.com',    '2025-02-20'),  -- 3
    ('Linus',   'Torvalds', 'linus@example.com',   '2025-03-15'),  -- 4
    ('Margaret','Hamilton', 'margaret@example.com','2025-04-01'),  -- 5
    ('Dennis',  'Ritchie',  'dennis@example.com',  '2025-05-05');  -- 6
GO

INSERT INTO dbo.Book (Title, ISBN, PublishedYear, CategoryName, AuthorId, TotalCopies, AvailableCopies, Edition) VALUES
    ('Clean Architecture',                 '9780134494166', 2017, 'Software',            5, 2, 2, 1),
    ('Refactoring',                        '9780201485677', 1999, 'Software',      6, 2, 1, 2),
    ('Patterns of Enterprise Application Architecture','9780321127426',2002,'Software', 6, 1, 1, 1),
    ('Test Driven Development',            '9780321146533', 2002, 'Testing',         7, 2, 2, 1),
    ('Extreme Programming Explained',      '9780321278654', 2004, 'Process',           7, 1, 0, 2),
    ('Design Patterns',                    '9780201633610', 1994, 'Software',          8, 2, 2, 1);
    /*('The Pragmatic Programmer',           '9780201616224', 1999, 'Software',           5, 4, 3, 1),
    ('The Pragmatic Programmer 20th Anniv','9780135957059', 2019, 'Software',            5, 2, 2, 2),
    ('Programming Ruby',                   '9780974514055', 2004, 'Languages',           6, 1, 1, 1);*/
GO

INSERT INTO dbo.Loan(BookId, MemberId, DueDate, ReturnDate) VALUES
    (10,1,'2026-06-30', NULL);

SELECT * FROM dbo.Author;
SELECT * FROM dbo.Member;
SELECT * FROM dbo.Book;

UPDATE dbo.Book
SET Edition = 2
WHERE BookId = 3;

UPDATE dbo.Book
SET AvailableCopies = AvailableCopies - 1
WHERE BookId = 1;

DELETE FROM dbo.Member WHERE Email = 'dennis@example.com';

DELETE FROM dbo.Author WHERE AuthorId = 4;

--DELETE FROM dbo.Author WHERE AuthorId = 6;

GO
--DQL - SELECT to return data
SELECT * FROM dbo.Book;

SELECT Title, PublishedYear, AvailableCopies FROM dbo.Book;

SELECT Title, TotalCopies - AvailableCopies AS CopiesOut FROM dbo.Book;

SELECT Title, PublishedYear 
FROM dbo.Book
WHERE PublishedYear >= 2000; --Filter

--BETWEEN, LIKE and IS 

-- 1999 and 2004
SELECT Title
FROM dbo.Book
WHERE PublishedYear 
BETWEEN 1999 AND 2004;

-- software or testing
SELECT Title, CategoryName
FROM dbo.Book
WHERE CategoryName
IN ('Software', 'Testing'); -- By default, many RDBMS systems are case-insesitive for comparisons

-- Title start with "Clean"
SELECT Title
FROM dbo.Book
WHERE Title
LIKE 'Test%';

--Title were category is software and available copies greater than 1
SELECT Title
FROM dbo.Book
WHERE CategoryName = 'Software' AND AvailableCopies > 1;

--Title where publishedyear was not provided
SELECT Title
FROM dbo.Book
WHERE PublishedYear IS NULL; --In SQL null is does not equal anything. Absence of value.

-- = matches one exact value.
-- IN matches any value in the provided list.
-- LIKE matches some pattern with wildcards %.

-- ORDER BY and DISTINCT
SELECT Title, PublishedYear 
FROM dbo.Book
ORDER BY PublishedYear DESC, Title ASC; --ASC is default

--Using distinct
-- All categories
SELECT DISTINCT CategoryName
FROM dbo.Book
ORDER BY CategoryName;

--GROUP BY and HAVING - a preview
--Category name and the count of books in that category where the count is more that 2.
--Order results by book count descending
SELECT CategoryName, COUNT(*) AS BookCount -- Aggregate function that counts the rows in each group
FROM dbo.Book
GROUP BY CategoryName --Collapses all rows with the same category into one group
HAVING COUNT(*) > 2 
ORDER BY BookCount DESC;
--HAVING vs WHERE
-- HAVING filters groups in a Group By.
-- WHERE filters rows.

-- ---- DROP: children before parents ----------------------------------------------
DROP TABLE IF EXISTS dbo.Loan;
DROP TABLE IF EXISTS dbo.BookAuthor;
DROP TABLE IF EXISTS dbo.Book;
DROP TABLE IF EXISTS dbo.Member;
DROP TABLE IF EXISTS dbo.Author;
DROP TABLE IF EXISTS dbo.Category;
GO

-- ---- CREATE normalized: parents before children ---------------------------------
-- NEW: Category is now its own entity (was the free-text Book.CategoryName).
CREATE TABLE dbo.Category
(
    CategoryId  INT IDENTITY(1,1) NOT NULL,
    Name        VARCHAR(60)  NOT NULL,
    Description VARCHAR(200) NULL,
    CONSTRAINT PK_Category PRIMARY KEY (CategoryId),
    CONSTRAINT UQ_Category_Name UNIQUE (Name)
);

CREATE TABLE dbo.Author
(
    AuthorId  INT IDENTITY(1,1) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName  VARCHAR(50) NOT NULL,
    BirthYear INT NULL,
    CONSTRAINT PK_Author PRIMARY KEY (AuthorId),
    CONSTRAINT CK_Author_BirthYear CHECK (BirthYear IS NULL OR BirthYear BETWEEN 300 AND 2050)
);

CREATE TABLE dbo.Member
(
    MemberId   INT IDENTITY(1,1) NOT NULL,
    FirstName  VARCHAR(50)  NOT NULL,
    LastName   VARCHAR(50)  NOT NULL,
    Email      VARCHAR(125) NOT NULL,
    JoinedDate DATE NOT NULL CONSTRAINT DF_Member_JoinedDate DEFAULT (GETDATE()),
    CONSTRAINT PK_Member PRIMARY KEY (MemberId),
    CONSTRAINT UQ_Member_Email UNIQUE (Email)
);

-- CHANGED: Book now carries CategoryId (FK) instead of CategoryName, and has NO AuthorId
-- (authorship lives in the BookAuthor bridge). Edition and the wider Title are folded into
-- CREATE rather than added later with ALTER.
CREATE TABLE dbo.Book
(
    BookId          INT IDENTITY(1,1) NOT NULL,
    Title           VARCHAR(250) NOT NULL,
    ISBN            CHAR(13) NOT NULL,
    PublishedYear   INT NULL,
    CategoryId      INT NOT NULL,
    TotalCopies     INT NOT NULL CONSTRAINT DF_Book_TotalCopies     DEFAULT (1),
    AvailableCopies INT NOT NULL CONSTRAINT DF_Book_AvailableCopies DEFAULT (1),
    Edition         INT NOT NULL CONSTRAINT DF_Book_Edition         DEFAULT (1),
    CONSTRAINT PK_Book PRIMARY KEY (BookId),
    CONSTRAINT UQ_Book_ISBN UNIQUE (ISBN),
    CONSTRAINT CK_Book_Copies CHECK (TotalCopies >= AvailableCopies),
    CONSTRAINT FK_Book_Category FOREIGN KEY (CategoryId) REFERENCES dbo.Category (CategoryId)
);

-- NEW: the M:N bridge between Book and Author. Composite PK, no extra attributes.
CREATE TABLE dbo.BookAuthor
(
    BookId   INT NOT NULL,
    AuthorId INT NOT NULL,
    CONSTRAINT PK_BookAuthor PRIMARY KEY (BookId, AuthorId),
    CONSTRAINT FK_BookAuthor_Book   FOREIGN KEY (BookId)   REFERENCES dbo.Book   (BookId)   ON DELETE CASCADE,
    CONSTRAINT FK_BookAuthor_Author FOREIGN KEY (AuthorId) REFERENCES dbo.Author (AuthorId) ON DELETE CASCADE
);

CREATE TABLE dbo.Loan
(
    LoanId     INT IDENTITY(1,1) NOT NULL,
    BookId     INT NOT NULL,
    MemberId   INT NOT NULL,
    LoanDate   DATE NOT NULL CONSTRAINT DF_Loan_LoanDate DEFAULT (GETDATE()),
    DueDate    DATE NOT NULL,
    ReturnDate DATE NULL,
    CONSTRAINT PK_Loan PRIMARY KEY (LoanId),
    CONSTRAINT FK_Loan_Book   FOREIGN KEY (BookId)   REFERENCES dbo.Book   (BookId),
    CONSTRAINT FK_Loan_Member FOREIGN KEY (MemberId) REFERENCES dbo.Member (MemberId),
    CONSTRAINT CK_Loan_Dates  CHECK (DueDate >= LoanDate)
);
GO


-- ---- SEED normalized: parents before children -----------------------------------
-- Category rows are seeded directly (no SELECT DISTINCT off Book, since Book no longer
-- holds the name). Comments mark the IDENTITY value each row receives.
INSERT INTO dbo.Category (Name, Description) VALUES
    ('Software',  'Software design and craftsmanship'),  -- 1
    ('Testing',   'Testing and TDD'),                    -- 2
    ('Process',   'Process and methodology'),            -- 3
    ('Languages', 'Programming languages');              -- 4

INSERT INTO dbo.Author (FirstName, LastName, BirthYear) VALUES
    ('Robert', 'Martin', 1952),   -- 1
    ('Martin', 'Fowler', 1963),   -- 2
    ('Kent',   'Beck',   1961),   -- 3
    ('Erich',  'Gamma',  1961),   -- 4
    ('Andrew', 'Hunt',   1964),   -- 5
    ('David',  'Thomas', 1956);   -- 6

INSERT INTO dbo.Member (FirstName, LastName, Email, JoinedDate) VALUES
    ('Ada',     'Lovelace', 'ada@example.com',      '2025-01-10'),  -- 1
    ('Grace',   'Hopper',   'grace@example.com',    '2025-02-02'),  -- 2
    ('Alan',    'Turing',   'alan@example.com',     '2025-02-20'),  -- 3
    ('Linus',   'Torvalds', 'linus@example.com',    '2025-03-15'),  -- 4
    ('Margaret','Hamilton', 'margaret@example.com', '2025-04-01'),  -- 5
    ('Dennis',  'Ritchie',  'dennis@example.com',   '2025-05-05');  -- 6

-- Book references CategoryId (1=Software, 2=Testing, 3=Process, 4=Languages); no author column.
INSERT INTO dbo.Book (Title, ISBN, PublishedYear, CategoryId, TotalCopies, AvailableCopies, Edition) VALUES
    ('Clean Code',                                     '9780132350885', 2008, 1, 3, 3, 1),  -- 1
    ('Clean Architecture',                             '9780134494166', 2017, 1, 2, 2, 1),  -- 2
    ('Refactoring',                                    '9780201485677', 1999, 1, 2, 1, 2),  -- 3
    ('Patterns of Enterprise Application Architecture','9780321127426', 2002, 1, 1, 1, 1),  -- 4
    ('Test Driven Development',                        '9780321146533', 2002, 2, 2, 2, 1),  -- 5
    ('Extreme Programming Explained',                  '9780321278654', 2004, 3, 1, 0, 2),  -- 6
    ('Design Patterns',                                '9780201633610', 1994, 1, 2, 2, 1),  -- 7
    ('The Pragmatic Programmer',                       '9780201616224', 1999, 1, 4, 3, 1),  -- 8
    ('The Pragmatic Programmer 20th Anniv',            '9780135957059', 2019, 1, 2, 2, 2),  -- 9
    ('Programming Ruby',                               '9780974514055', 2004, 4, 1, 1, 1);  -- 10

-- Authorship via the bridge: the original one-author-per-book links, then real co-authors.
INSERT INTO dbo.BookAuthor (BookId, AuthorId) VALUES
    (1, 1), (2, 1), (3, 2), (4, 2), (5, 3),     -- original single-author links
    (6, 3), (7, 4), (8, 5), (9, 5), (10, 6),
    (7, 3),   -- Design Patterns co-authored (Beck)
    (8, 6);   -- The Pragmatic Programmer co-authored (Thomas)

INSERT INTO dbo.Loan (BookId, MemberId, LoanDate, DueDate, ReturnDate) VALUES
    (3, 1, '2026-06-01', '2026-06-15', NULL),         -- Refactoring, out to Ada
    (6, 2, '2026-05-20', '2026-06-03', NULL),         -- XP Explained, out to Grace (0 available)
    (8, 3, '2026-05-25', '2026-06-08', '2026-06-04'), -- Pragmatic Programmer, returned
    (1, 4, '2026-06-10', '2026-06-24', NULL),         -- Clean Code, out to Linus
    (8, 5, '2026-06-12', '2026-06-26', NULL);         -- Pragmatic Programmer, out to Margaret
GO