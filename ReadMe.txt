1. Prerequisites
Before you begin, make sure you have the following installed:

-.NET SDK 7.0 

-SQL Server (Local or Remote instance for database storage).

-Visual Studio Code or Visual Studio (optional, for editing the code).

-Hangfire (for background job management) - handled via NuGet packages.

2. Clone the Repository
First, clone the repository to your local machine.

3. Setup the Database
SQL Server Database Configuration
The project uses SQL Server for data storage. You can configure the database connection string by following these steps:

Open appsettings.json in the root directory of the project.

Locate the "ConnectionStrings" section.

Replace your_server_name and your_database_name with your actual SQL Server details.

4. Install Dependencies
Open a terminal or Package Manager Console (if using Visual Studio).

Install the necessary NuGet packages. (For Visual Studio, dependencies should automatically be restored when opening the project.)
Open Package Manager Console and run Update-Database


THEN RUN PROJECT