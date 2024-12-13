
## Database Configuration

This is a development project. The database connection is set to IntegratedSecurity=True and 
TrustServerCertificate=True by default. Adjust this as required for running in containers etc.



## Publishing the database
Using developer command prompt of your choice. Change to the directory where the ApplicationTracker.Data.csproj file is located and execute the following command:

dotnet ef database update --startup-project ..\ApplicationTracker
