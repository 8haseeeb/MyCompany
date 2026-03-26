/*
  SSOServiceDb — delete all Users and insert one dev row.
  BCrypt hash below matches password: haseeb123 (same sample as docs/TROUBLESHOOTING_SSO_LOGIN.md).

  sqlcmd example:
  sqlcmd -S "(localdb)\MSSQLLocalDB" -d SSOServiceDb -i "c:\MyCompany\MyCompany\scripts\sql\03-sso-users-clear-and-seed.sql"
*/
SET NOCOUNT ON;
USE SSOServiceDb;
GO

DELETE FROM dbo.Users;
GO

DBCC CHECKIDENT (N'dbo.Users', RESEED, 0);
GO

INSERT INTO dbo.Users ([Name], [Email], [PasswordHash], [Role], [CurrentSessionId], [RefreshToken], [RefreshTokenExpiry])
VALUES (
    N'devuser',
    N'dev@local.test',
    N'$2a$11$7Lw4yoUV9I4Y2RWJ3VahJuFX6vBjhT7a/jTmmCRxJZmEM7k5QSjAm',
    N'User',
    NULL,
    NULL,
    NULL
);
GO

SELECT Id, Name, Email, LEN(PasswordHash) AS HashLen, Role FROM dbo.Users;
GO
