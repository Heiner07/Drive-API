# Drive-API
A .NET WEB API similar to Google Drive where the registered users can create folders and upload files / Una API WEB .NET similar a Google Drive donde los usuarios registrados pueden crear carpetas y cargar archivos.

## Some features / Algunas características

- User account management (Register, Login, Edit User) with JWT (Json web token).
- Users can create folders and upload (and download) files to their accounts.
- Endpoints to rename, delete and move files and folders.
- Developed in .NET Core 3.2 (It should work in this version or higher).

## Endpoints information / Información de los endpoints

| Http Request	| Endpoint					| Request Data in	| Request Data	| Response Data	| Description			|
| --------------| --------------------------| ------------------| --------------| --------------| ----------------------|
| POST/GET			| "/api/Users/Login"		| Body			 | [AuthenticationRequest](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Models/Requests/AuthenticationRequest.cs) | [AuthenticatedUser](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Models/AuthenticatedUser.cs) | Authenticates a user |
| POST			| "/api/Users/Register"		| Body			 | [RegisterUserRequest](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Models/Requests/RegisterUserRequest.cs) | [AuthenticatedUser](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Models/AuthenticatedUser.cs) | Registers a user |
| PUT			| "/api/Users"				| Body			 | [EditUserRequest](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Models/Requests/EditUserRequest.cs)| Only the HTTP Code (200, 400, etc) | Edits a user |
| GET			| "/api/Registers/FromFolder/{id}" | Query parameter | An integer | [A list of Registers](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Database/Register.cs) | Gets all the Registers (folders and files) form a folder (0 for user's root folder) |
| GET			| "/api/Registers/{id}" | Query parameter | An integer | [Register](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Database/Register.cs) | Get a Registers of the user |
| GET			| "/api/Registers/DownloadRegister/{id}" | Query parameter | An integer | A file | Download a file (only files at the moment) of the user |
| PUT			| "/api/Registers/ChangeName" | Body | [ChangeRegisterNameRequest](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Models/Requests/ChangeRegisterNameRequest.cs) | [Register](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Database/Register.cs) | Changes the name of a register (file or folder) |
| PUT			| "/api/Registers/Move" | Body | [MoveRegisterRequest](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Models/Requests/MoveRegisterRequest.cs) | [Register](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Database/Register.cs) | Moves a register (file or folder) |
| POST			| "/api/Registers/AddFile" | Form | [AddFileRequest](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Models/Requests/AddFileRequest.cs) | [Register](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Database/Register.cs) | Add (Upload) a file |
| POST			| "/api/Registers/AddFolder" | Body | [AddFolderRequest](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Models/Requests/AddFolderRequest.cs) | [Register](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Database/Register.cs) | Add (Create) a folder |
| DELETE		| "/api/Registers/{id}" | Query parameter | An integer | [Register](https://github.com/Heiner07/Drive-API/blob/main/DriveAPI/Database/Register.cs) | Deletes a register (file or folder) |

When an id is needed and you want to refer the user's root folder, it will be 0 o null (where applicable).

## Important Points / Puntos importantes

- The implementation can be improved in some aspects. For example:
- This implementation maps the folders in the physical disk on the server such as the user sees on a client app.
- A better approach could be manage the folders only in the database, in a logical way.
- Use the unique id of each register as name on the disk could be a better option than the name assigned by the user (even when the name is validated in this implementation).
- Scan each file that is uploaded to the server, with an antivirus, is something to consider to protect the server and users.
- Use of cloud services to store all the files is definitely a better option, for example: Amazon S3, Azure Storage or Firestore.
- Use of Swagger in the project for documentation, etc.

Note: WeatherForecastController was not intentionally removed just to see something when the project loads :)