# ASP.NET MVC Application with Clean Architecture

This project is a robust **ASP.NET MVC application** built using **Clean 3-Layered Architecture (Presentation, Business, Data Access)** and **Entity Framework Core (Code-First)**. 
It demonstrates advanced ASP.NET Identity management, structured access control, and scalable design patterns suitable for real-world enterprise applications.

---

## Architecture

- **Presentation Layer:** Handles the UI, MVC Controllers, Views, and AJAX interactions.  
- **Business Layer:** Contains services, business logic, and manages coordination between Data Access and Presentation layers.  
- **Data Access Layer:** Implements **Entity Framework Core** (Code-First) with migrations for database management.  

---

##  Authentication & Authorization

- **ASP.NET Identity** integrated for user management.  
- **Advanced Role-Based Access:** Admin and User roles separated with structured permissions.  
- **Remember Me** feature for persistent authentication.  
- **Structured Access Control:** Different permissions for Admin and regular users.  

---

##  Design Patterns & Best Practices

- **Generic Repository:** Reusable repository for CRUD operations on any entity.  
- **Unit of Work:** Ensures atomic transactions and clean data handling.  
- **AutoMapper:** Maps between entities and ViewModels seamlessly.  
- **Dependency Injection (DI):** Applied across all layers to ensure decoupling and testability.  

---

##  Features & Functionality

- **CRUD Operations:** Full Create, Read, Update, Delete for all entities (Department, User, Role).  
- **AJAX Real-Time Search:** Instant filtering of data without page reloads.  
- **Soft Delete:** Ensures records can be "deleted" safely without permanent removal.  
- **User & Role Management:** Add/Remove users from roles dynamically with proper validation.  
- **Structured Access Control:** Ensures only authorized users can access sensitive operations.  


