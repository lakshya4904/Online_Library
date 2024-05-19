--#region Drop Tables
Drop Table IF EXISTS BookGenres;
Drop Table IF EXISTS Books;
Drop Table IF EXISTS Genres;
Drop Table IF EXISTS BookCovers;
Drop Table IF EXISTS BookContents;
GO
--#endregion

--#region Drop Procedures
Drop Procedure IF EXISTS CREATE_Books;
Drop Procedure IF EXISTS READ_Books;
Drop Procedure IF EXISTS UPDATE_Books;
Drop Procedure IF EXISTS DELETE_Books;
Go

Drop Procedure IF EXISTS CREATE_Genres;
Drop Procedure IF EXISTS READ_Genres;
Drop Procedure IF EXISTS UPDATE_Genres;
Drop Procedure IF EXISTS DELETE_Genres;
Go

Drop Procedure IF EXISTS CREATE_BookGenres;
Drop Procedure IF EXISTS READ_BookGenres;
Drop Procedure IF EXISTS UPDATE_BookGenres;
Drop Procedure IF EXISTS DELETE_BookGenres;
Go

Drop Procedure IF EXISTS CREATE_BookCovers;
Drop Procedure IF EXISTS READ_BookCovers;
Drop Procedure IF EXISTS UPDATE_BookCovers;
Drop Procedure IF EXISTS DELETE_BookCovers;
Go

Drop Procedure IF EXISTS CREATE_BookContents;
Drop Procedure IF EXISTS READ_BookContents;
Drop Procedure IF EXISTS UPDATE_BookContents;
Drop Procedure IF EXISTS DELETE_BookContents;
GO
--#endregion


DROP TABLE IF EXISTS Books;
CREATE TABLE Books (
  ID INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
  Title NVARCHAR(255) NOT NULL,
  FirstName NVARCHAR(50) NULL,
  LastName NVARCHAR(50) NULL,
  PublicationDate DATE NULL,
  Publisher NVARCHAR(255) NULL,
  Edition NVARCHAR(50) NULL,
  ISBN VARCHAR(13) NULL,
  LanguageCode NVARCHAR(2) NULL,
  Pages INT NULL,
  Description TEXT NULL,
  Format NVARCHAR(50) NULL,
  IsAvailable BIT DEFAULT 1,
);

DROP PROCEDURE IF EXISTS CREATE_Books;
DROP PROCEDURE IF EXISTS READ_Books;
DROP PROCEDURE IF EXISTS UPDATE_Books;
DROP PROCEDURE IF EXISTS DELETE_Books;
GO

-- #region CREATE_Books
CREATE PROCEDURE [dbo].[CREATE_Books] (
  @Title            NVARCHAR(255) NULL,
  @FirstName        NVARCHAR(50) NULL,
  @LastName         NVARCHAR(50) NULL,
  @PublicationDate  DATE,
  @Publisher        NVARCHAR(255) NULL,
  @Edition          NVARCHAR(50) NULL,
  @ISBN             VARCHAR(13) NULL,
  @LanguageCode     NVARCHAR(2) NULL,
  @Pages            INT NULL,
  @Description      TEXT NULL,
  @Format           NVARCHAR(50) NULL,
  @IsAvailable      BIT NULL,
  @OUTID            INT OUTPUT
) AS
BEGIN
  -- Check if @PublicationDate is NULL, set it to current date if NULL
  IF @PublicationDate IS NULL
    SET @PublicationDate = GETDATE();

  -- Insert into the Books table
  INSERT INTO Books (
    Title,
    FirstName,
    LastName,
    PublicationDate,
    Publisher,
    Edition,
    ISBN,
    LanguageCode,
    Pages,
    Description,
    Format,
    IsAvailable
  ) VALUES (
    @Title,
    @FirstName,
    @LastName,
    @PublicationDate,
    @Publisher,
    @Edition,
    @ISBN,
    @LanguageCode,
    @Pages,
    @Description,
    @Format,
    @IsAvailable
  );

  -- Get the ID of the inserted book record
  SET @OUTID = SCOPE_IDENTITY();
END;
GO
-- #endregion

-- #region READ_Books
CREATE PROCEDURE [dbo].[READ_Books] (
  @ID           INT         = 0,
  @Title         NVARCHAR(255) = NULL,
  @FirstName   NVARCHAR(50)  = NULL,
  @LastName     NVARCHAR(50)  = NULL,
  @PublicationDate DATE       = NULL,
  @Publisher   NVARCHAR(255) = NULL,
  @Edition     NVARCHAR(50)  = NULL,
  @ISBN        VARCHAR(13)   = NULL,
  @LanguageCode  NVARCHAR(2)  = NULL,
  @Pages        INT         = NULL,
  @Description   TEXT        = NULL,
  @Format      NVARCHAR(50)  = NULL,
  @IsAvailable  BIT         = NULL,
  @CurrentPage   INT         = 1,
  @PageSize    INT         = 20,
  @SortBy        INT         = 1,
  @ISAsc        BIT         = 1,
  @RecordCount   BIGINT OUTPUT
) AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @PARAMETERS NVARCHAR(MAX) = N'
    @ID           INT         = 0,
    @Title         NVARCHAR(255) = NULL,
    @FirstName   NVARCHAR(50)  = NULL,
    @LastName     NVARCHAR(50)  = NULL,
    @PublicationDate DATE       = NULL,
    @Publisher   NVARCHAR(255) = NULL,
    @Edition     NVARCHAR(50)  = NULL,
    @ISBN        VARCHAR(13)   = NULL,
    @LanguageCode  NVARCHAR(2)  = NULL,
    @Pages        INT         = NULL,
    @Description   TEXT        = NULL,
    @Format      NVARCHAR(50)  = NULL,
    @IsAvailable  BIT         = NULL,
    @CurrentPage   INT         = 1,
    @PageSize    INT         = 20,
    @SortBy        INT         = 1,
    @ISAsc        BIT         = 1,
    @OutRecordCount BIGINT     OUTPUT';

  DECLARE @Query NVARCHAR(MAX);
  DECLARE @QueryCount NVARCHAR(MAX);
  DECLARE @Condition NVARCHAR(MAX) = '';
  DECLARE @Order NVARCHAR(10);

  SET @Order =
    CASE WHEN @ISAsc = 1 THEN ' ASC' ELSE ' DESC' END;

  SET @Query = 'SELECT
    B.ID,
    B.Title,
    B.FirstName,
    B.LastName,
    B.PublicationDate,
    B.Publisher,
    B.Edition,
    B.ISBN,
    B.LanguageCode,
    B.Pages,
    B.Description,
    B.Format,
    B.IsAvailable
  FROM Books B WITH (NOLOCK) ';

  SET @QueryCount = 'SELECT @OutRecordCount = COUNT(*) FROM Books B ';

  IF @ID <> 0 SET @Condition += ' AND (B.ID = @ID)';
  IF @Title IS NOT NULL SET @Condition += ' AND (B.Title = @Title)';
  IF @FirstName IS NOT NULL SET @Condition += ' AND (B.FirstName = @FirstName)';
  IF @LastName IS NOT NULL SET @Condition += ' AND (B.LastName = @LastName)';
  IF @PublicationDate IS NOT NULL SET @Condition += ' AND (B.PublicationDate = @PublicationDate)';
  IF @Publisher IS NOT NULL SET @Condition += ' AND (B.Publisher = @Publisher)';
  IF @Edition IS NOT NULL SET @Condition += ' AND (B.Edition = @Edition)';
  IF @ISBN IS NOT NULL SET @Condition += ' AND (B.ISBN = @ISBN)';
  IF @LanguageCode IS NOT NULL SET @Condition += ' AND (B.LanguageCode = @LanguageCode)';
  IF @Pages IS NOT NULL SET @Condition += ' AND (B.Pages = @Pages)';
  IF @Description IS NOT NULL SET @Condition += ' AND (B.Description = @Description)';
  IF @Format IS NOT NULL SET @Condition += ' AND (B.Format = @Format)';
  IF @IsAvailable IS NOT NULL SET @Condition += ' AND (B.IsAvailable = @IsAvailable)';

  SET @Query = @Query + @Condition;
  SET @QueryCount = @QueryCount + @Condition;

  SET @Query = @Query +
    CASE
      WHEN @SortBy = 2 THEN ' ORDER BY B.Title ' + @Order
      WHEN @SortBy = 3 THEN ' ORDER BY B.FirstName + '' '' + B.LastName ' + @Order
      -- Add additional sorting criteria here if needed
      ELSE ' ORDER BY B.ID ' + @Order
    END;

  SET @Query = @Query + ' OFFSET @PageSize * (@CurrentPage - 1) ROWS FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE) ';

  PRINT @Query;
  PRINT @QueryCount;

  EXECUTE sp_executesql @Query, @PARAMETERS,
    @ID,
    @Title,
    @FirstName,
    @LastName,
    @PublicationDate,
    @Publisher,
    @Edition,
    @ISBN,
    @LanguageCode,
    @Pages,
    @Description,
    @Format,
    @IsAvailable,
    @CurrentPage,
    @PageSize,
    @SortBy,
    @ISAsc,
    @RecordCount = @RecordCount OUTPUT;

  EXECUTE sp_executesql @QueryCount, @PARAMETERS,
    @ID,
    @Title,
    @FirstName,
    @LastName,
    @PublicationDate,
    @Publisher,
    @Edition,
    @ISBN,
    @LanguageCode,
    @Pages,
    @Description,
    @Format,
    @IsAvailable,
    @CurrentPage,
    @PageSize,
    @SortBy,
    @ISAsc,
    @RecordCount = @RecordCount OUTPUT;
END;
GO
-- #endregion


-- #region UPDATE_Books
CREATE PROCEDURE [dbo].[UPDATE_Books] (
  @ID          INT,
  @Title       NVARCHAR(255) = NULL,
  @FirstName   NVARCHAR(50) = NULL,
  @LastName    NVARCHAR(50) = NULL,
  @PublicationDate DATE = NULL,
  @Publisher   NVARCHAR(255) = NULL,
  @Edition     NVARCHAR(50) = NULL,
  @ISBN        VARCHAR(13) = NULL,
  @LanguageCode  NVARCHAR(2) = NULL,
  @Pages        INT = NULL,
  @Description   TEXT = NULL,
  @Format      NVARCHAR(50) = NULL,
  @IsAvailable  BIT = NULL
) AS
BEGIN
  -- Update the Books table
  UPDATE Books
  SET 
    Title = ISNULL(@Title, Title),
    FirstName = ISNULL(@FirstName, FirstName),
    LastName = ISNULL(@LastName, LastName),
    PublicationDate = ISNULL(@PublicationDate, PublicationDate),
    Publisher = ISNULL(@Publisher, Publisher),
    Edition = ISNULL(@Edition, Edition),
    ISBN = ISNULL(@ISBN, ISBN),
    LanguageCode = ISNULL(@LanguageCode, LanguageCode),
    Pages = ISNULL(@Pages, Pages),
    Description = ISNULL(@Description, Description),
    Format = ISNULL(@Format, Format),
    IsAvailable = ISNULL(@IsAvailable, IsAvailable)
  WHERE ID = @ID;

  -- Delete existing genre associations for this book
  DELETE FROM BookGenres WHERE BookID = @ID;
END;
GO
-- #endregion

--#region DELETE_Books
CREATE PROCEDURE [dbo].[DELETE_Books] (
	@ID				  INT				= NULL,
    @Title            NVARCHAR(255)		= NULL,
    @Author           NVARCHAR(255)		= NULL,
    @PublicationDate  DATE				= NULL,
    @ISBN             VARCHAR(13)		= NULL,
    @Language         NVARCHAR(50)		= NULL,
    @Pages            INT				= NULL,
    @Description      TEXT				= NULL,
    @GenreID          VARCHAR(50)		= NULL,
    @CoverImage       VARBINARY(MAX)	= NULL,
    @Format           NVARCHAR(50)		= NULL,
    @Content          VARBINARY(MAX)	= NULL,
    @OUTID            INT OUTPUT
) AS
BEGIN
  -- Delete genre associations for this book
  DELETE FROM BookGenres WHERE BookID = @ID;

  -- Delete the book record from the Books table
  DELETE FROM Books WHERE ID = @ID;

END
GO
--#endregion

Drop Table IF EXISTS Genres;
CREATE TABLE Genres (
  ID INT IDENTITY(1, 1) PRIMARY KEY,
  Name NVARCHAR(50) NOT NULL
);

Drop Procedure IF EXISTS CREATE_Genres;
Drop Procedure IF EXISTS READ_Genres;
Drop Procedure IF EXISTS UPDATE_Genres;
Drop Procedure IF EXISTS DELETE_Genres;
GO

-- #region CREATE_Genres
CREATE PROCEDURE [dbo].[CREATE_Genres] (
  @Name NVARCHAR(50) NULL,
  @OUTID INT OUTPUT
) AS
BEGIN
  INSERT INTO Genres (Name) VALUES (@Name);
  SET @OUTID = SCOPE_IDENTITY();
END;
GO
-- #endregion

-- #region READ_Genres
CREATE PROCEDURE [dbo].[READ_Genres] (
  @ID INT = NULL,
  @Name NVARCHAR(50) = NULL,
  @CurrentPage INT = 1,
  @PageSize INT = 20,
  @SortBy INT = 1,
  @ISAsc BIT = 1,
  @RecordCount BIGINT OUTPUT
) AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @PARAMETERS NVARCHAR(MAX) = N'
	@ID INT = NULL,
	@Name NVARCHAR(50) = NULL,
	@CurrentPage INT = 1,
	@PageSize INT = 20,
	@SortBy INT = 1,
	@ISAsc BIT = 1,
	@OutRecordCount BIGINT OUTPUT';

  DECLARE @Query NVARCHAR(MAX);
  DECLARE @QueryCount NVARCHAR(MAX);
  DECLARE @Condition NVARCHAR(MAX) = '';
  DECLARE @Order NVARCHAR(10);

  SET @Order =
	CASE WHEN @ISAsc = 1 THEN ' ASC' ELSE ' DESC' END;

  SET @Query = 'SELECT ID, Name FROM Genres WITH (NOLOCK) WHERE (1=1) ';
  SET @QueryCount = 'SELECT @OutRecordCount = COUNT(*) FROM Genres WITH (NOLOCK) WHERE (1=1) ';

  IF @ID IS NOT NULL SET @Condition += ' AND ID = @ID';
  IF @Name IS NOT NULL SET @Condition += ' AND Name = @Name';

  SET @Query = @Query + @Condition;
  SET @QueryCount = @QueryCount + @Condition;

  SET @Query = @Query +
	CASE
	  WHEN @SortBy = 2 THEN ' ORDER BY Name ' + @Order
	  ELSE ' ORDER BY ID ' + @Order  -- Default sorting by ID
	END;

  SET @Query = @Query + ' OFFSET @PageSize * (@CurrentPage - 1) ROWS FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE) ';

  PRINT @Query;
  PRINT @QueryCount;

  EXECUTE sp_executesql @Query, @PARAMETERS,
	@ID,
	@Name,
	@CurrentPage,
	@PageSize,
	@SortBy,
	@ISAsc,
	@RecordCount = @RecordCount OUTPUT;

  EXECUTE sp_executesql @QueryCount, @PARAMETERS,
	@ID,
	@Name,
	@CurrentPage,
	@PageSize,
	@SortBy,
	@ISAsc,
	@RecordCount = @RecordCount OUTPUT;
END;
GO
-- #endregion

-- #region UPDATE_Genres
CREATE PROCEDURE [dbo].[UPDATE_Genres] (
  @ID INT,
  @Name NVARCHAR(50)
) AS
BEGIN
  UPDATE Genres
  SET Name = @Name
  WHERE ID = @ID;
END;
GO
-- #endregion

-- #region DELETE_Genres
CREATE PROCEDURE [dbo].[DELETE_Genres] (
  @ID INT
) AS
BEGIN
  DELETE FROM Genres WHERE ID = @ID;
END;
GO
-- #endregion


Drop Table IF EXISTS BookGenres;
CREATE TABLE BookGenres (
  ID INT IDENTITY(1, 1) PRIMARY KEY,
  BookID INT FOREIGN KEY REFERENCES Books(ID),
  GenreID INT FOREIGN KEY REFERENCES Genres(ID)
);
Drop Procedure IF EXISTS CREATE_BookGenres;
Drop Procedure IF EXISTS READ_BookGenres;
Drop Procedure IF EXISTS UPDATE_BookGenres;
Drop Procedure IF EXISTS DELETE_BookGenres;
GO

-- #region CREATE_BookGenre
CREATE PROCEDURE [dbo].[CREATE_BookGenres] (
  @BookID INT,
  @GenreID INT,
  @OUTID INT OUTPUT
) AS
BEGIN
  INSERT INTO BookGenres (BookID, GenreID) VALUES (@BookID, @GenreID);
  SET @OUTID = SCOPE_IDENTITY();
END;
GO
-- #endregion

-- #region READ_BookGenre
CREATE PROCEDURE [dbo].[READ_BookGenres] (
  @BookID INT = NULL,
  @GenreID INT = NULL,
  @CurrentPage   INT = 1,
  @PageSize      INT = 20,
  @SortBy        INT = 1,
  @ISAsc        BIT = 1,
  @RecordCount   BIGINT OUTPUT
) AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @PARAMETERS NVARCHAR(MAX) = N'
    @BookID INT = NULL,
    @GenreID INT = NULL,
    @CurrentPage   INT = 1,
    @PageSize      INT = 20,
    @SortBy        INT = 1,
    @ISAsc        BIT = 1,
    @OutRecordCount  BIGINT OUTPUT';

  DECLARE @Query NVARCHAR(MAX);
  DECLARE @QueryCount NVARCHAR(MAX);
  DECLARE @Condition NVARCHAR(MAX) = '';
  DECLARE @Order NVARCHAR(10);

  SET @Order =
    CASE WHEN @ISAsc = 1 THEN ' ASC' ELSE ' DESC' END;

  SET @Query = 'SELECT BG.ID, BG.BookID, G.ID AS GenreID, G.Name AS GenreName
  FROM BookGenres BG
  INNER JOIN Genre G ON BG.GenreID = G.ID
  WITH (NOLOCK) WHERE (1=1) ';

  SET @QueryCount = 'SELECT @OutRecordCount = COUNT(*)
  FROM BookGenres BG
  INNER JOIN Genre G ON BG.GenreID = G.ID ';

  IF @BookID IS NOT NULL SET @Condition += ' AND BG.BookID = @BookID';
  IF @GenreID IS NOT NULL SET @Condition += ' AND BG.GenreID = @GenreID';

  SET @Query = @Query + @Condition;
  SET @QueryCount = @QueryCount + @Condition;

  SET @Query = @Query +
    CASE
      WHEN @SortBy = 1 THEN ' ORDER BY BG.ID ' + @Order
      WHEN @SortBy = 2 THEN ' ORDER BY G.Name ' + @Order
      ELSE ' ORDER BY BG.ID ' + @Order  -- Default sorting by BookGenre ID
    END;

  SET @Query = @Query + ' OFFSET @PageSize * (@CurrentPage - 1) ROWS FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE) ';

  PRINT @Query;
  PRINT @QueryCount;

  EXECUTE sp_executesql @Query, @PARAMETERS,
    @BookID,
    @GenreID,
    @CurrentPage,
    @PageSize,
    @SortBy,
    @ISAsc,
    @RecordCount = @RecordCount OUTPUT;

  EXECUTE sp_executesql @QueryCount, @PARAMETERS,
    @BookID,
    @GenreID,
    @CurrentPage,
    @PageSize,
    @SortBy,
    @ISAsc,
    @RecordCount = @RecordCount OUTPUT;
END;
GO
-- #endregion

-- #region UPDATE_BookGenre
CREATE PROCEDURE [dbo].[UPDATE_BookGenres] (
  @BookID INT,
  @GenreIDs NVARCHAR(MAX) = NULL
) AS
BEGIN
  -- Delete existing genre associations for this book
  DELETE FROM BookGenres WHERE BookID = @BookID;

  -- Split comma-separated genre IDs and insert new associations (if provided)
  DECLARE @genreID INT;

  IF @GenreIDs IS NOT NULL
  BEGIN
    WHILE CHARINDEX(',', @GenreIDs) > 0
    BEGIN
      SELECT @genreID = CONVERT(INT, SUBSTRING(@GenreIDs, 1, CHARINDEX(',', @GenreIDs) - 1));
      SET @GenreIDs = SUBSTRING(@GenreIDs, CHARINDEX(',', @GenreIDs) + 1, LEN(@GenreIDs));

      -- Insert into the BookGenres junction table
      INSERT INTO BookGenres (BookID, GenreID) VALUES (@BookID, @genreID);
    END;

    -- Handle the last genre ID (if any)
    IF LEN(@GenreIDs) > 0
    BEGIN
      SET @genreID = CONVERT(INT, @GenreIDs);
      INSERT INTO BookGenres (BookID, GenreID) VALUES (@BookID, @genreID);
    END;
  END;
END;
GO
-- #endregion

-- #region DELETE_BookGenre
CREATE PROCEDURE [dbo].[DELETE_BookGenres] (
  @BookID INT,
  @GenreID INT
) AS
BEGIN
  DELETE FROM BookGenres WHERE BookID = @BookID AND GenreID = @GenreID;
END;
GO
-- #endregion


Drop Table IF EXISTS BookCovers;
CREATE TABLE BookCovers (
  ID INT IDENTITY(1, 1) PRIMARY KEY,
  BookID INT FOREIGN KEY REFERENCES Books(ID),
  Edition INT NULL,
  FileName NVARCHAR(255) NOT NULL,
  FileType NVARCHAR(50) NOT NULL,
);

Drop Procedure IF EXISTS CREATE_BookCovers;
Drop Procedure IF EXISTS READ_BookCovers;
Drop Procedure IF EXISTS UPDATE_BookCovers;
Drop Procedure IF EXISTS DELETE_BookCovers;
GO

 --#region CREATE_BookCover
CREATE PROCEDURE [dbo].[CREATE_BookCovers] (
  @BookID INT,
  @Edition INT,
  @FileName NVARCHAR(255),
  @FileType NVARCHAR(50)
) AS
BEGIN
  INSERT INTO BookCovers (BookID, Edition, FileName, FileType)
  VALUES (@BookID, @Edition, @FileName, @FileType);
END;
GO
-- #endregion

-- #region READ_BookCover
CREATE PROCEDURE [dbo].[READ_BookCovers] (
  @BookID INT = NULL,
  @Edition INT = NULL,
  @CurrentPage INT = 1,
  @PageSize INT = 20,
  @SortBy INT = 1,
  @ISAsc BIT = 1,
  @RecordCount BIGINT OUTPUT
) AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @PARAMETERS NVARCHAR(MAX) = N'
	@BookID INT = NULL,
	@Edition INT = NULL,
	@CurrentPage INT = 1,
	@PageSize INT = 20,
	@SortBy INT = 1,
	@ISAsc BIT = 1,
	@OutRecordCount BIGINT OUTPUT';

  DECLARE @Query NVARCHAR(MAX);
  DECLARE @QueryCount NVARCHAR(MAX);
  DECLARE @Condition NVARCHAR(MAX) = '';
  DECLARE @Order NVARCHAR(10);

  SET @Order =
	CASE WHEN @ISAsc = 1 THEN ' ASC' ELSE ' DESC' END;

  SET @Query = 'SELECT ID, BookID, Edition, FileName, FileType
  FROM BookCovers WITH (NOLOCK) WHERE (1=1) ';

  SET @QueryCount = 'SELECT @OutRecordCount = COUNT(*)
  FROM BookCovers WITH (NOLOCK) WHERE (1=1) ';

  IF @BookID IS NOT NULL SET @Condition += ' AND BookID = @BookID';
  IF @Edition IS NOT NULL SET @Condition += ' AND Edition = @Edition';

  SET @Query = @Query + @Condition;
  SET @QueryCount = @QueryCount + @Condition;

  SET @Query = @Query +
	CASE
	  WHEN @SortBy = 1 THEN ' ORDER BY BookID ' + @Order
	  WHEN @SortBy = 2 THEN ' ORDER BY Edition ' + @Order
	  ELSE ' ORDER BY ID ' + @Order  -- Default sorting by ID
	END;

  SET @Query = @Query + ' OFFSET @PageSize * (@CurrentPage - 1) ROWS FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE) ';

  PRINT @Query;
  PRINT @QueryCount;

  EXECUTE sp_executesql @Query, @PARAMETERS,
	@BookID,
	@Edition,
	@CurrentPage,
	@PageSize,
	@SortBy,
	@ISAsc,
	@RecordCount = @RecordCount OUTPUT;

  EXECUTE sp_executesql @QueryCount, @PARAMETERS,
	@BookID,
	@Edition,
	@CurrentPage,
	@PageSize,
	@SortBy,
	@ISAsc,
	@RecordCount = @RecordCount OUTPUT;
END;
GO

-- #endregion

-- #region UPDATE_BookCover
CREATE PROCEDURE [dbo].[UPDATE_BookCovers] (
  @ID INT,
  @BookID INT,
  @Edition INT,
  @FileName NVARCHAR(255),
  @FileType NVARCHAR(50)
) AS
BEGIN
  UPDATE BookCovers
  SET
	BookID = @BookID,
	Edition = @Edition,
	FileName = @FileName,
	FileType = @FileType
  WHERE ID = @ID;
END;
GO
-- #endregion

-- #region DELETE_BookCover
CREATE PROCEDURE [dbo].[DELETE_BookCovers] (
    @ID INT,
  @BookID INT,
  @Edition INT,
  @FileName NVARCHAR(255),
  @FileType NVARCHAR(50)
) AS
BEGIN
  DELETE FROM BookCovers WHERE ID = @ID;
END;
GO
 --#endregion


Drop Table IF EXISTS BookContents;
CREATE TABLE BookContents (
  ID INT IDENTITY(1, 1) PRIMARY KEY,
  BookID INT FOREIGN KEY REFERENCES Books(ID),
  Format NVARCHAR(50) NOT NULL,
  FileName NVARCHAR(255) NOT NULL,
  FileType NVARCHAR(50) NOT NULL,
);

Drop Procedure IF EXISTS CREATE_BookContents;
Drop Procedure IF EXISTS READ_BookContents;
Drop Procedure IF EXISTS UPDATE_BookContents;
Drop Procedure IF EXISTS DELETE_BookContents;
GO

-- #region CREATE_BookContents
CREATE PROCEDURE [dbo].[CREATE_BookContents] (
  @BookID INT,
  @Format NVARCHAR(50),
  @FileName NVARCHAR(255),
  @FileType NVARCHAR(50)
) AS
BEGIN
  INSERT INTO BookContents (BookID, Format, FileName, FileType)
  VALUES (@BookID, @Format, @FileName, @FileType);
END;
GO
-- #endregion

-- #region READ_BookContents
CREATE PROCEDURE [dbo].[READ_BookContents] (
  @BookID INT = NULL,
  @Format NVARCHAR(50) = NULL,
  @CurrentPage INT = 1,
  @PageSize INT = 20,
  @SortBy INT = 1,
  @ISAsc BIT = 1,
  @RecordCount BIGINT OUTPUT
) AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @PARAMETERS NVARCHAR(MAX) = N'
	@BookID INT = NULL,
	@Format NVARCHAR(50) = NULL,
	@CurrentPage INT = 1,
	@PageSize INT = 20,
	@SortBy INT = 1,
	@ISAsc BIT = 1,
	@OutRecordCount BIGINT OUTPUT';

  DECLARE @Query NVARCHAR(MAX);
  DECLARE @QueryCount NVARCHAR(MAX);
  DECLARE @Condition NVARCHAR(MAX) = '';
  DECLARE @Order NVARCHAR(10);

  SET @Order =
	CASE WHEN @ISAsc = 1 THEN ' ASC' ELSE ' DESC' END;

  SET @Query = 'SELECT ID, BookID, Format, FileName, FileType
  FROM BookContents WITH (NOLOCK) WHERE (1=1) ';

  SET @QueryCount = 'SELECT @OutRecordCount = COUNT(*)
  FROM BookContents WITH (NOLOCK) WHERE (1=1) ';

  IF @BookID IS NOT NULL SET @Condition += ' AND BookID = @BookID';
  IF @Format IS NOT NULL SET @Condition += ' AND Format = @Format';

  SET @Query = @Query + @Condition;
  SET @QueryCount = @QueryCount + @Condition;

  SET @Query = @Query +
	CASE
	  WHEN @SortBy = 1 THEN ' ORDER BY BookID ' + @Order
	  WHEN @SortBy = 2 THEN ' ORDER BY Format ' + @Order
	  ELSE ' ORDER BY ID ' + @Order  -- Default sorting by ID
	END;

  SET @Query = @Query + ' OFFSET @PageSize * (@CurrentPage - 1) ROWS FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE) ';

  PRINT @Query;
  PRINT @QueryCount;

  EXECUTE sp_executesql @Query, @PARAMETERS,
	@BookID,
	@Format,
	@CurrentPage,
	@PageSize,
	@SortBy,
	@ISAsc,
	@RecordCount = @RecordCount OUTPUT;

  EXECUTE sp_executesql @QueryCount, @PARAMETERS,
	@BookID,
	@Format,
	@CurrentPage,
	@PageSize,
	@SortBy,
	@ISAsc,
	@RecordCount = @RecordCount OUTPUT;
END;
GO
-- #endregion

-- #region UPDATE_BookContents
CREATE PROCEDURE [dbo].[UPDATE_BookContents] (
  @ID INT,
  @BookID INT,
  @Format NVARCHAR(50),
  @FileName NVARCHAR(255),
  @FileType NVARCHAR(50)
) AS
BEGIN
  UPDATE BookContents
  SET
	BookID = @BookID,
	Format = @Format,
	FileName = @FileName,
	FileType = @FileType
  WHERE ID = @ID;
END;
GO
-- #endregion

-- #region DELETE_BookContents
CREATE PROCEDURE [dbo].[DELETE_BookContents] (
  @ID INT,
  @BookID INT,
  @Format NVARCHAR(50),
  @FileName NVARCHAR(255),
  @FileType NVARCHAR(50)
) AS
BEGIN
  DELETE FROM BookContents WHERE ID = @ID;
END;
GO
-- #endregion


