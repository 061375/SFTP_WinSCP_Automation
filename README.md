# SFTP Automation with WinSCP

### A Simple Tool for Automating EDI File Transfers  
Created by: Jeremy Heminger  
Contact: [jeremy.heminger@aquamor.com](mailto:jeremy.heminger@aquamor.com), [contact@jeremyheminger.com](mailto:contact@jeremyheminger.com) 
  
  ᓚᘏᗢ 

---

## Overview

This tool is a lightweight wrapper designed to **automate file uploads and downloads** via FTP or SFTP, primarily for **EDI (Electronic Data Interchange)** transactions.

It simplifies and schedules file transfers, integrates with DelmiaWorks or standalone scripts, and includes support for logging, email notifications, and testing modes.

---

## Dependencies

Make sure the following components are installed:

- 💧 `AQHelpers`
- 💧 `AQFTP`
- 💧 `WinSCPnet`

---

## Features

- Automated uploads/downloads using FTP or SFTP.
- Supports logging and email alerts.
- Can run on a schedule or loop continuously.
- Simple configuration via XML.
- Designed for EDI data paths.

---

## To-Do

- Add support for file type filtering.

---

## Version Tracking

- [AQFTP Versions](/AQFTP/versions.md)  
- [DLL/AQFTP Versions](/DLL/AQFTP/versions.md)  
- [DLL/AQHelpers Versions](/DLL/AQHelpers/versions.md)  

---

## Configuration

Settings are stored in an XML configuration file. You can specify the path to this file as a command-line argument.

Example:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8" />
  </startup>
  <appSettings>
    <add key="Loop" value="False" />
    <add key="LoopTime" value="1800000" />
    <add key="Testing" value="True" />
    <add key="LogEvents" value="True" />
    <add key="ConsoleLog" value="True" />
    <add key="LogPath" value="" />
    
    <add key="EmailUpdates" value="True" />
    <add key="EmailErrors" value="True" />
    <add key="EmailAddresses" value="test@test.com,test2@test2.com" />
    
    <add key="TESTEDIIN" value="C:\EDI\IN\" />
    <add key="TESTEDIOUT" value="C:\EDI\OUT\" />
    <add key="EDIIN" value="M:\EDI\IN\" />
    <add key="EDIOUT" value="M:\EDI\Out\" />
    
    <add key="UseSFTP" value="False" />
    <add key="FtpServer" value="ftp.[ftpserver].com" />
    <add key="FtpServerUserName" value="[username]" />
    <add key="FtpServerPassword" value="[password]" />
    
    <add key="SFTPServer" value="sftp.[ftpserver].com" />
    <add key="SFTPServerUserName" value="[username]" />
    <add key="SFTPServerPassword" value="[password]" />
    <add key="SshHostKeyFingerprint" value="ssh-rsa 2048 [hash]" />
    <add key="SFTPport" value="22" />
    
    <add key="Inbound" value="EDI/Inbound" />
    <add key="Outbound" value="EDI/Outbound" />
    
    <add key="SMTP" value="smtp.[smtp].com" />
    <add key="EmailFrom" value="no-reply@[domain].com" />
    <add key="EmailPort" value="587" />
    <add key="EmailCredentialsUser" value="no-reply@[domain].com" />
    <add key="EmailCredentialsPass" value="[password]" />
  </appSettings>
</configuration>
```

### Key Settings Explained

- **Testing**: Enables test folder paths (`TESTEDIIN`, `TESTEDIOUT`).
- **Loop**: Runs the application continuously if set to `True`.
- **LoopTime**: Time (in milliseconds) between cycles (default: 30 minutes).
- **Logging**: Enable `LogEvents` and `ConsoleLog` for logs. Set `LogPath` to store `.log` files.
- **Email Notifications**: Set `EmailUpdates` and/or `EmailErrors` to `True`, and define `EmailAddresses` (comma-separated).
- **FTP/SFTP Settings**: Set credentials and remote directories.
- **Email Server (SMTP)**: Used to send alerts or updates.

---

## How to Use

### With eServer or IQAlert

1. Add the `.exe` file as an action.
2. Set the run interval (e.g., every 30 minutes).
3. Provide the path to the `.config` file as the first argument.

Example:

```bash
\\DELMIAWORKS\BIN\AQFTP\AQ_FTP.exe \\DELMIAWORKS\BIN\AQFTP\AQ_FTP.exe.config
```

### Via PowerShell or Terminal

Run manually, via Task Scheduler, or in scripts (PowerShell/BASH/CRON).

---

## Creating an SSH Host Key Fingerprint

Some SFTP servers require a host fingerprint. Here's how to generate it:

### On Windows (PowerShell)

```powershell
ssh-keyscan -t rsa host.example.com 2>$null | ssh-keygen -lf - | ForEach-Object { $p = $_ -split '\s+'; "ssh-rsa $($p[0]) $($p[1])" }
```

### On Mac/Linux

```bash
ssh-keyscan -t rsa host.example.com 2>/dev/null | ssh-keygen -lf - | awk '{print "ssh-rsa", $1, $2}'
```
