-- This file contains SQL statements that will be executed after the build script.
PRINT N'EXECUTE Post Deployment scripts'

-- Entries with Foreign Keys
:r ../Seeds/dbo/mainCodes.sql