# User Secrets Configuration Guide

## Overview
User Secrets is a secure way to store sensitive configuration values during development. Secrets are stored locally on your machine and never committed to version control.

## Setting Up User Secrets

### 1. Initialize User Secrets for the API Project
Run this command in the `API` project directory:

```powershell
cd .\API
dotnet user-secrets init
```

This creates a `secrets.json` file in your local user profile (not in the project folder).

### 2. Set Email Configuration Secrets

Run the following commands to set the email settings:

```powershell
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "EmailSettings:Port" "587"
dotnet user-secrets set "EmailSettings:SenderName" "RemoteLMS"
dotnet user-secrets set "EmailSettings:SenderEmail" "your-actual-email@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "your-actual-app-password"
```

### 3. Set JWT Configuration Secrets (Optional but Recommended)

```powershell
dotnet user-secrets set "JwtSettings:Key" "your-secret-key-here"
dotnet user-secrets set "JwtSettings:Issuer" "RemoteLMS"
dotnet user-secrets set "JwtSettings:Audience" "RemoteLMSUsers"
dotnet user-secrets set "JwtSettings:DurationInMinutes" "60"
```

### 4. Set Database Connection String (Optional)

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
```

### 5. Verify Your Secrets

To see all configured secrets:

```powershell
dotnet user-secrets list
```

To remove a specific secret:

```powershell
dotnet user-secrets remove "EmailSettings:Password"
```

## How User Secrets Works

- During development, ASP.NET Core automatically loads secrets from your local machine
- In `Program.cs`, secrets are automatically injected into configuration
- In `appsettings.json`, you provide placeholder values (as done here)
- The actual secrets override the config file values at runtime

## Security Best Practices

✅ **DO:**
- Use User Secrets for development
- Use environment variables or Azure Key Vault for production
- Keep your secrets file secure and never commit it
- Rotate secrets regularly
- Use app-specific passwords for email (not your main password)

❌ **DON'T:**
- Commit credentials to version control
- Share secrets with teammates (each has their own)
- Use same secrets across environments
- Store production secrets in appsettings.json

## For Production Deployment

In production, use one of these approaches:

1. **Environment Variables**: Set secrets as environment variables on the server
2. **Azure Key Vault**: Recommended for Azure-hosted applications
3. **AWS Secrets Manager**: Recommended for AWS-hosted applications
4. **.NET Configuration Providers**: Use custom config providers for other platforms

Example in Program.cs (already supported):
```csharp
// Add custom configuration sources
if (app.Environment.IsProduction())
{
    // Load from Azure Key Vault, environment variables, etc.
}
```

## Troubleshooting

**Secrets not loading?**
- Restart Visual Studio
- Ensure user-secrets init was run
- Check that secrets.json exists in your user profile

**Location of secrets.json:**
- Windows: `%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json`
- macOS: `~/.microsoft/usersecrets/<UserSecretsId>/secrets.json`
- Linux: `~/.microsoft/usersecrets/<UserSecretsId>/secrets.json`

The `<UserSecretsId>` is stored in your `.csproj` file.
