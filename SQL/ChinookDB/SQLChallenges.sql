-- Parking Lot*******
-- *                *
-- *                *
--- *****************



-- Comment can be done single line with --
-- Comment can be done multi line with /* */

/*
DQL - Data Query Language
Keywords:

SELECT - retrieve data, select the columns from the resulting set
FROM - the table(s) to retrieve data from
WHERE - a conditional filter of the data
GROUP BY - group the data based on one or more columns
HAVING - a conditional filter of the grouped data
ORDER BY - sort the data
*/


-- BASIC CHALLENGES
-- List all customers (full name, customer id, and country) who are not in the USA

SELECT FirstName + ' ' + LastName AS Full_Name, CustomerId, Country 
FROM dbo.Customer
WHERE Country != 'USA';

-- List all customers from Brazil
SELECT FirstName + ' ' + LastName AS Full_Name
FROM dbo.Customer
WHERE Country = 'Brazil';

-- List all sales agents

SELECT * 
FROM employee 
WHERE title LIKE '%Agent%';


-- Retrieve a list of all countries in billing addresses on invoices
SELECT DISTINCT BillingCountry 
FROM dbo.Invoice; 

-- Retrieve how many invoices there were in 2009, and what was the sales total for that year?
SELECT COUNT(*) AS Invoices, SUM(Total) AS SaleTotal
FROM dbo.Invoice
WHERE YEAR(InvoiceDate) = 2021;
--WHERE InvoiceDate >= '20220101'
--AND InvoiceDate < '20230101';

-- (challenge: find the invoice count sales total for every year using one query)
SELECT SUM(Total) AS Total
FROM dbo.Invoice
GROUP BY YEAR(InvoiceDate);

-- how many line items were there for invoice #37
SELECT SUM(Quantity) AS LineItems
FROM dbo.InvoiceLine 
WHERE InvoiceId = 37;

-- how many invoices per country? BillingCountry  # of invoices 
SELECT BillingCountry, SUM(InvoiceId) AS Invoices
FROM dbo.Invoice
GROUP BY BillingCountry;
-- Retrieve the total sales per country, ordered by the highest total sales first.
SELECT BillingCountry, SUM(Total) AS TotalSales
FROM dbo.Invoice
GROUP BY BillingCountry;

-- JOINS CHALLENGES
-- Every Album by Artist
SELECT al.Title, ar.Name
FROM dbo.Album AS al
JOIN dbo.Artist AS ar ON ar.ArtistId = al.ArtistId;

-- (inner keyword is optional for inner join)

-- All songs of the rock genre
SELECT t.Name, g.Name
FROM dbo.Track AS t
JOIN dbo.Genre AS g ON g.GenreId = t.GenreId
WHERE g.Name = 'Rock';

-- Show all invoices of customers from brazil (mailing address not billing)
SELECT i.InvoiceId, c.FirstName, c.Country
FROM dbo.Invoice AS i
JOIN dbo.Customer AS c ON c.CustomerId = i.CustomerId
WHERE c.Country = 'Brazil';

-- Show all invoices together with the name of the sales agent for each one
SELECT i.InvoiceId, e.FirstName + ' ' + e.LastName AS FullName
FROM dbo.Invoice AS i
JOIN dbo.Customer AS c ON c.CustomerId = i.CustomerId
JOIN dbo.Employee AS e ON e.EmployeeId = c.SupportRepId;

-- Which sales agent made the most sales in 2009?

SELECT TOP 1 e.FirstName, e.LastName, SUM(i.Total) AS TotalSales
FROM dbo.Invoice AS i
JOIN dbo.Customer AS c ON c.CustomerId = i.CustomerId
JOIN dbo.Employee AS e ON e.EmployeeId = c.SupportRepId
WHERE YEAR(i.InvoiceDate) = 2021
GROUP BY e.FirstName, e.LastName
ORDER BY TotalSales DESC;

-- How many customers are assigned to each sales agent?
SELECT e.FirstName + ' ' + e.LastName AS employees, SUM(c.CustomerId) AS customers
FROM dbo.Customer AS c
JOIN dbo.Employee AS e ON e.EmployeeId = c.SupportRepId
WHERE e.Title LIKE '%Agent%'
GROUP BY e.FirstName, e.LastName;

-- Which track was purchased the most in 2022?
SELECT TOP 1 t.Name, SUM(il.Quantity) AS Total
FROM dbo.InvoiceLine AS il
JOIN dbo.Track AS t ON t.TrackId = il.TrackId
GROUP BY t.Name
ORDER BY Total DESC;

-- Show the top three best selling artists.
SELECT TOP 3 ar.Name AS ArtistName , SUM(il.Quantity*il.UnitPrice) AS TotalRevenue
FROM dbo.Artist AS ar
JOIN dbo.Album AS al ON ar.ArtistId = al.ArtistId
JOIN dbo.Track AS t ON al.AlbumId = t.AlbumId
JOIN dbo.InvoiceLine AS il ON t.TrackId = il.TrackId
GROUP BY ar.Name
ORDER BY TotalRevenue DESC;

-- Which customers have the same initials as at least one other customer?
WITH CustomerInitials AS (
    SELECT FirstName, LastName,
        CONCAT(LEFT(FirstName, 1),LEFT(LastName, 1)) AS Initials
    FROM Customer
)
SELECT FirstName, LastName, Initials
FROM CustomerInitials
WHERE Initials IN (
    SELECT Initials
    FROM CustomerInitials
    GROUP BY Initials
    HAVING COUNT(*) > 1
)
ORDER BY Initials, LastName, FirstName;

-- Which countries have the most invoices?
SELECT TOP 10 c.Country, SUM(i.InvoiceId) AS Invoices
FROM dbo.Invoice AS i
JOIN dbo.Customer AS c ON c.CustomerId = i.CustomerId
GROUP BY c.Country
ORDER BY Invoices DESC;

-- Which city has the customer with the highest sales total?
SELECT TOP 1 c.City, i.Total
FROM dbo.Invoice AS i
JOIN dbo.Customer AS c ON c.CustomerId = i.CustomerId
GROUP BY c.City, i.Total
ORDER BY i.Total DESC;

-- Who is the highest spending customer?
SELECT TOP 1 c.FirstName + ' ' + c.LastName AS FullName, i.Total
FROM dbo.Invoice AS i
JOIN dbo.Customer AS c ON c.CustomerId = i.CustomerId
GROUP BY c.FirstName, c.LastName, i.Total
ORDER BY i.Total DESC;

-- Return the email and full name of of all customers who listen to Rock.


-- Which artist has written the most Rock songs?


-- Which artist has generated the most revenue?




-- ADVANCED CHALLENGES
-- solve these with a mixture of joins, subqueries, CTE, and set operators.
-- solve at least one of them in two different ways, and see if the execution
-- plan for them is the same, or different.

-- 1. which artists did not make any albums at all?


-- 2. which artists did not record any tracks of the Latin genre?


-- 3. which video track has the longest length? (use media type table)



-- 4. boss employee (the one who reports to nobody)


-- 5. how many audio tracks were bought by German customers, and what was
--    the total price paid for them?



-- 6. list the names and countries of the customers supported by an employee
--    who was hired younger than 35.




-- DML exercises

-- 1. insert two new records into the employee table.

-- 2. insert two new records into the tracks table.

-- 3. update customer Aaron Mitchell's name to Robert Walter

-- 4. delete one of the employees you inserted.

-- 5. delete customer Robert Walter.
