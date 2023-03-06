## Quetzalcoatl Auth

### EF Core Migrations useful CLI commands

- **add migration**:
```bash
dotnet ef migrations add InitDatabase --project YourDataAccessLibraryName -s YourWebProjectName -c YourDbContextClassName --verbose 
```
- **update database**:
```bash
dotnet ef database update InitDatabase --project YourDataAccessLibraryName -s YourWebProjectName -c YourDbContextClassName --verbose
```
- **remove migration**:
```bash
dotnet ef migrations remove --project YourDataAccessLibraryName -s YourWebProjectName -c YourDbContextClassName --verbose
```
