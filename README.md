# FileUploader

This project is a web application built with .NET and Angular that allows users to upload files securely to a server. The application features include file selection, multiple file uploads, and secure redirection of uploaded files to a specified IP address using SSL certificates. Uploaded files are scanned by the Kaspersky Scan Engine, which returns the scan result (Clean, Infected, etc.) before acceptance

## Key Features:
- **File Upload:** Users can select and upload multiple files simultaneously.
- **File Preview:** Displays a list of selected files before uploading.
- **File Management:** Options to cancel the selection or remove the last selected file.
- **Secure Upload:** Implements SSL certificates for secure file transmission to the server.
- **Kaspersky Integration:** Uploaded files are scanned by the Kaspersky Scan Engine before acceptance.
- **Error Handling:** Provides feedback on upload success or failure with options to retry on failure.

## Technologies Used:
- **Backend:** ASP.NET Core
- **Frontend:** Angular
- **HTTP Client:** Handles file upload requests and SSL configuration

## Setup Instructions:
1. **Build and Run the Backend:**
   - Navigate to the API directory: `cd API`
   - Run the application: `dotnet run`
2. **Build and Run the Frontend:**
   - Navigate to the client directory: `cd client`
   - Install dependencies: `npm install`
   - Run the application: `ng serve`

## Additional Information:
- **Backend:** Runs on *http://localhost:5011* and *https://localhost:5012*
- **Frontend:** Runs on *http://localhost:4201*
- **Kaspersky Scan Results:** For better understanding of the Kaspersky scan results, visit [Kaspersky Scan Engine Results](https://support.kaspersky.com/ScanEngine/2.1/en-US/193001.htm)

Feel free to contribute to the project by submitting issues or pull requests.
