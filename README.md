# Zenzinger_Eshop
We have 3 type of users **- Admin, Manager, User -**, only admin and manager **CRUD** products, carousels background. Which type of user is signed in Eshop recognize after Authentication. Admin on more than **CRUD** Orders, which manager can't. User after Authentication cant only see products and their orders. Approved and current. After creating and approving order, user get invoice of that order sent by an e-mail and of course, user can generate manually invoice and print it in **My Orders** section.
## Technologies
* Bootstrap
* C#
* MVC
* .Net 5
* EntityFramework
* MySQL database
* MySQL Workbench
### Subject
* Programming advanced websites
* Shortcut: ***AP5_PW***
### ToDo module - **DONE**
Create your own invoice generator with 2 actions. Generating automatically
by Order ID or user can generate invoice manually in form by entering own informations
about itself. User can print invoice in PDF format or send it by email.
***
### Database migration
``` 
dotnet ef migrations add "MySQL 1.0.2"
dotnet ef database update
```
