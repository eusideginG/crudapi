## Introduction
This manual provides an overview and guide for the CRUD_API project, which is a RESTful API built with ASP.NET Core for managing custom forms.
Each form contains numeric values and can have up to 10 fields. The forms include metadata such as a unique title, description, creation date,
and last updated date.

## Controllers
FormController.cs
The FormController handles the HTTP requests for the CRUD operations on forms.
It uses dependency injection to access the application context and the current user's ID.

### Parameters:
ApplicationDataContext context: Database context to access the application data.
IHttpContextAccessor httpContextAccessor: Used to access the HTTP context, primarily to retrieve the user's ID.

### Purpose:
Initializes the controller with the database context and retrieves the current user's ID from the HTTP context.


## Endpoints

### GET: api/form
Purpose: Retrieves all forms belonging to the current user.
Response: A JSON-serialized list of forms with their respective form data.

### GET: api/form/{id}
Purpose: Retrieves a specific form by ID for the current user.
Parameters:
Guid id: The ID of the form to retrieve.
Response: A JSON-serialized form with its respective form data.

### POST: api/form
Purpose: Creates a new form.
Parameters:
FormViewModel viewModel: The form data to create a new form.
Response: A status message indicating whether the form was successfully added.

### PUT: api/form/{id}
Purpose: Updates an existing form.
Parameters:
Guid id: The ID of the form to update.
FormViewModel viewModel: The updated form data.
Response: A status message indicating whether the form was successfully updated.

### DELETE: api/form/{id}
Purpose: Deletes a form or specific form data.
Parameters:
Guid id: The ID of the form to delete.
IEnumerable<Guid> ids: Optional parameter for deleting specific form data.
Response: A status message indicating whether the form or form data was successfully deleted.

### Summary
The FormController class in the CRUD_API project provides comprehensive CRUD operations for managing forms and their data.
Each method is designed to handle specific HTTP requests (GET, POST, PUT, DELETE) and interact with the database context to
perform the necessary operations. This manual should help you understand the structure and functionality of the FormController class.
# crudapi
