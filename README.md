# Seller Catalogue

Seller Catalogue is a small project to showcase update of a seller's catalogue by creating versions of the seller's database
* Create DAL for interacting with the database
* Create BAL for making business decisions to choose how to process data before storing and displaying
* Create a view for uploading and downloading csv files

Software Required:
* .NET Core SDK 2.2 or later
* MySQL database

Steps to run the website:
1. Take a clone of the repository
2. Create a new schema sellercatalogue using the sellercatalogue.sql file in the project root folder.
3. In appsettings.json in the root folder, in the DatabaseSettings property, change the connection string to match your own database
4. After completing the above steps, navigate to the project root in the CLI and type in the command "dotnet run"
5. Navigate to http://localhost:5001/ to view the project