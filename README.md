# Concert

ASP .NET Core MVC project to create a live music on-demand website to book a concert by selecting the setlist and adding different services, like an e-commerce website.

__TECHNOLOGIES USED__

- ASP .NET Core MVC: Web application development framework based on the MVC architecture
- Entity Framework Core: Object Relational Mapping (ORM) framework
- SQL Server: Relational DataBase Management System (RDBMS)
- Bootstrap: CSS Framework for developing responsive and mobile-first websites
- Javascript libraries:
  - Toastr: Gnome/Growl type non-blocking notifications
  - TinyMCE: Rich text editor
  - Datatables.net: Manipulate interactive and enhance tables
  - Sweetalert2: Customizable alert popup boxes
  - Stripe: Online payment processing solution
- C# libraries:
  - SendGrid: Twilio library to send emails

___TODO___

_Areas/Identity
- Add ApplicationUser properties into Identity/Manage pages and edit pages format to ressemble Register and Log-in ones

_Areas/Customer
- Add a way to sort songs in Home/Index by Language or by Genre
- Change format of Home/Services
- Add service by default to user when accessing setlist if he doesn't have it
- Add Songs tab in Layout and move Home content to Songs tab
- In Home tab add an explanation of how the Webpage works

_General
- Add /// comments to some methods

___BUGS___

_Areas/Admin
- In Admin/Order/Details view, when clicking "Confirm Order" button, the order is confirmed but the page doesn't refresh with the updated status