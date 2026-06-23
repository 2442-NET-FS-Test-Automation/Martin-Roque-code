/* This is for recreate the database
    for demo purposue
*/
USE LibraryDB;
GO --Batching statement -MS SQLServer exclusive

--Section 1 -DDL

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
-- SELECT * FROM dbo.Author;