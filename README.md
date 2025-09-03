# VueBugTrackerProject

This is a web application I created that allows users to record and track bugs encountered in a software development project. 

Below is a cut-down overview of the project. If you just want to run the application, please skip to the section with the header "How to run the application".

## Background

The idea for the project came from a video I watched around late 2022, which claimed that developing a bug tracking application would guarantee a job in software development. The reasoning behind it is while I develop the application, I can record and monitor bugs I encounter within the application and have a record of them.

When I first saw that video, I had graduated from university in July 2022 and found entering employment difficult (I got my first role in March 2023), and I decided to work on that project in the hope it would draw the attention of a suitable employer, albeit with ASP.NET Razor Pages. When I became redundant from my role in March 2025, I chose to revisit the idea to improve my coding portfolio and make it easier for me to re-enter employment.

## Tech stack

The stack used to develop the application consists of the following languages, frameworks and libraries:

### Frontend/client:
- Vue.js
- TypeScript/JavaScript
- HTML
- Quasar (for styling the frontend)

### Backend/server
- ASP.NET MVC
- C#
- Entity Framework (to read from and write to a database)
- SQL Server

I also used MailKit/MimeKit by Jeffrey Stedfast to handle sending emails via a mock SMTP server. Links to the GitHub repository and official website are below.

https://github.com/jstedfast/MailKit

https://mimekit.net/

The stack is similar to the one recommended in the video I watched, except for the Vue framework and Quasar library, as the video advised to use Bootstrap instead to style existing HTML elements to save time. I saw this as an opportunity to practice using a JavaScript framework in a larger scale project, having previously used React in my professional career.

## How to run the application

Before running the application, please note the following:

- A functioning SQL database is required for the application to work properly and perform read/write operations.
- An SMTP server is needed to use the email sending functionality.

The following instructions assume you do not have a functioning database set up and you have the project open in Visual Studio 2022.

1. Open the SQL Server Object Explorer menu, either through the Views menu in the top menu bar or the key combination Ctrl+\\, Ctrl+S.
2. If there are no SQL servers visible, click on the Add SQL Server button and follow the instructions in the dialog box to add a server.
3. Expand a server and the Databases folder. If there are no databases, right click on the Databases folder, click on Add New Database and use the dialog to select a database name and location.
4. Expand the database you want to use, right click on it and select Properties. In the Properties menu, look for the section labelled "Connection string" and copy the value.
5. In the Solution Explorer, expand VueBugTrackerProject.Server and open the file appsettings.json. Replace the text {INSERT CONNECTION STRING HERE} with the connection string you are using.
6. Open the Package Manager Console and type the following commands, pressing enter after each one:
```
EntityFrameworkCore\Add-Migration "{migration name}"
EntityFrameworkCore\Update-Database`
```

To run the application in Visual Studio, press F5 or click on the Start Debugging button in the toolbar.

### Notes on setting up the email client

The email client is only needed if you plan to use the account recovery functionality. You will need an SMTP server or service to use this, with some examples below (these only send emails to a dummy inbox, not to an actual recipient):

- Ethereal (https://ethereal.email/)
- Papercut SMTP (https://www.papercut-smtp.com/)

The code for sending emails is in the file VueBugTrackerProject.Server/Services/EmailService.cs and consists of the following commented out lines:

```
client.Connect("");
client.Authenticate("{username}", "{password}");
```

To use the SMTP server, uncomment the `client.Connect("");` line and insert the URL or IP address of the SMTP server you are using into the function, surrounded in quotation marks.
If the service you are using requires authentication, uncomment the `client.Authenticate("{username}", "{password}");` line and replace the {username} and {password} fields with the username and password for the service.
