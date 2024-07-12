# FileUploader

This project is a web application built with .NET and Angular that allows users to upload files securely to a server. The application features include file selection, multiple file uploads, and secure redirection of uploaded files to a specified IP address using SSL certificates.

## Key Features:
- **File Upload:** Users can select and upload multiple files simultaneously.
- **File Preview:** Displays a list of selected files before uploading.
- **File Management:** Options to cancel the selection or remove the last selected file.
- **Secure Upload:** Implements SSL certificates for secure file transmission to the server.
- **Database Integration:** Stores details of uploaded files including file name, file path, and upload time in a database.
- **Error Handling:** Provides feedback on upload success or failure with options to retry on failure.

## Technologies Used:
- **Backend:** ASP.NET Core
- **Frontend:** Angular
- **Database:** SQLite
- **HTTP Client:** Handles file upload requests and SSL configuration

## Setup Instructions:
1. **Build and Run the Backend:**
   - Navigate to the API directory: `cd API`
   - Run the application: `dotnet run`
2. **Build and Run the Frontend:**
   - Navigate to the client directory: `cd client`
   - Install dependencies: `npm install`
   - Run the application: `ng serve --open`

Feel free to contribute to the project by submitting issues or pull requests.
