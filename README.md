# Concert

ASP .NET Core MVC project to create a live music on-demand website to book a concert by selecting the setlist and adding different services, like an e-commerce website.

_TECHNOLOGIES USED_

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

_CONCEPTS USED_

- N-Tier architecture
- Design patterns: Repository and UnitOfWork
- Roles, Authorization and Session in .NET Core
- User management
- Seed Database with DbInitializer

_TODO_

- Areas/Identity:
  - Add ApplicationUser properties into Identity/Manage pages and edit pages format to ressemble Register and Log-in ones

- Areas/Customer
  - Add a way to sort songs in Home/Index by Language or by Genre
  - Change format of Home/Services
  - Add service by default to user when accessing setlist if he doesn't have it
  - Add Songs tab in Layout and move Home content to Songs tab
  - In Home tab add an explanation of how the Webpage works
  - OrderStatus is STATUS_PENDING when we place an order, we go to stripe checkout page but then we come back without paying. We could add a custom status for this saying that the order went to cart but it didn't go through. I could create a cleanup job to remove those orders.

- General
  - Add /// comments to some methods

_BUGS_

- Areas/Admin
  - In Admin/Order/Details view, when clicking "Confirm Order" button, the order is confirmed but the page doesn't refresh with the updated status
  - In Admin/User/RoleManagement, when Role is Company the Company dropdown is invisible by default but it should appear.