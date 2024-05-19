# Cinema Ticket Reservation System - SOAP Project

## Overview
This project is designed to create a cinema ticket reservation system using SOAP (Simple Object Access Protocol). The system consists of two main components: a server application and two (currently only one - User) WPF client applications. The server manages the cinema's data, including movie screenings and reservations, while the clients provide an interface for users to view and manage reservations. Future versions will include Admin desktop client app to manage cinema's data. 

## Project Structure

### Server Application: 
+ A WEB application in .NET 6.0 that hosts the SOAP web services. It uses SQL Dabatase to store data and Entity Framework Core 6.0.10 to retreive the informations.
### Client Application 1 (Admin): 
+ A WPF application for administrators to manage screenings and reservations (to add in the future).
### Client Application 2 (User): 
+ A WPF application in .NET 6.0 for users to view screenings and manage theirs reservations.

## Features
### Server Application:

+ Manage movie screenings.
+ Handle ticket reservations.
+ Provide SOAP web services for client applications.

### Client Application 1 (Admin):

+ View the list of movie screenings.
+ Add, update, or delete movie screenings.
+ View and manage reservations.

### Client Application 2 (User):

+ View the list of available screenings.
+ Make ticket reservations for screenings.
+ View user's reservations.


## Usage
### Server Application
The server application will start and host the SOAP web services.
The endpoint for the services will be displayed in the console output.

### Client Application 1 (Admin)
Launch the admin client application.
Use the interface to add, update, or delete movie screenings.
Manage reservations by viewing and modifying existing reservations.

### Client Application 2 (User)
Launch the user client application.
Browse the list of available screenings.
Make reservations for the desired screenings.
View and manage user's reservations.