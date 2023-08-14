
# File Management System User Guide

The `FileUtility` class offers a simple and consistent interface for file management operations, such as checking file existence, reading and writing text files, and deleting files. This utility class provides platform-specific implementations based on the runtime platform.

## Classes

### FileUtility
Provides utility functions for file management, including checking, reading, and writing files.
Public Properties
- `bool canLoadAndSaveFiles`: Indicates whether the platform can load and save files.
Public Methods
- `bool FileExists(string fileName)`
- `void DeleteFile(string fileName)`
- `string ReadTextFile(string fileName)`
- `void WriteTextFile(string fileName, string content)`

## Usage Examples

### Checking Platform Compatibility

Before using the `FileUtility` methods, you can check whether the platform supports file loading and saving using the `canLoadAndSaveFiles` property:

```csharp
if (FileUtility.canLoadAndSaveFiles)
{
    // Platform supports file loading and saving
}
else
{
    // Platform does not support file loading and saving (e.g., WebGL)
}
```
### File Existence Check

To check if a file exists, use the `FileExists(string fileName)` method:

```csharp
string fileName = "myFile.txt";
bool exists = FileUtility.FileExists(fileName);

if (exists)
{
    Debug.Log($"{fileName} exists.");
}
else
{
    Debug.Log($"{fileName} does not exist.");
}
```
### Delete File
```csharp
string fileName = "fileToDelete.txt";
FileUtility.DeleteFile(fileName);
```
### Read Text File
Read the content of a text file using the `ReadTextFile(string fileName)` method:
```csharp
string fileName = "myTextFile.txt";
string fileContent = FileUtility.ReadTextFile(fileName);
Debug.Log($"File content:\n{fileContent}");
```
### Write Text File
To write content to a text file, use the `WriteTextFile(string fileName, string content)` method:
```csharp
string fileName = "newTextFile.txt";
string content = "Hello, World!";
FileUtility.WriteTextFile(fileName, content);
```
## Platform-Specific Implementation
-   For platforms other than WebGL, the methods are implemented using standard file I/O operations (`File.Exists`, `File.Delete`, `File.ReadAllText`, `File.WriteAllText`).
-   For WebGL, file operations are not supported, and the methods return default values or perform no action.

The `FileUtility` class provides a consistent and easy-to-use interface for file management operations, making it convenient to manage files in Unity projects across different platforms. By utilizing the platform-specific implementations, you can handle file operations seamlessly, enhancing the robustness of your application's file management.
