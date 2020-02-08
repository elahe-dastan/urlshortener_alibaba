# URL shortener

-[Introduction](#Introduction)</br>
-[Installation](#Installation)</br>
-[Run Tests](#Test)

## Introduction 
This repository contains a URL shortener web API project implemented with .NET Core. 
This simply matches each long URL given to it to a short URL and returns this back to the user, if one the short URLs are given to the project it redirects the user to the proper URL

---

## Installation
This project uses entity framework to communicate with database so run the below command 

    dotnet add package Microsoft.EntityFrameworkCore.SqlServer 

As menthioned, this project needs a database so after setting up your own database change the 
connection string in the appsettings.json.
Then to make the tables needed with the help of entity framework run below commands


```bash
dotnet tool install --global dotnet -ef
dotnet ef migration add URLshortenerContext
dotnet ef database update
```
---
## Test
There are some tests written for this project using xuinit, in order to run the tests xunit package is needed

     dotnet add package xunit.core

---

## License & copyright
© Elahe Dastan, Computer Eng. @ Amirkabir university of technology