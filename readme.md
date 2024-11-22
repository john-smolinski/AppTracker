
## Database Configuration

This is a development project. The database connection is set to IntegratedSecurity=True and 
TrustServerCertificate=True by default. Adjust this as required for running in containers etc.



## Publishing the database

Add-Migration InitialMigration

Update-Database

