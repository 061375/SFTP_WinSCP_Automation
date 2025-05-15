
# SFTP WinSCP Automation

## A simple wrapper to automate uploading and downloading of files ( specifically for EDI transactions )

## Jeremy Heminger <jeremy.heminger@aquamor.com>, <contact@jeremyheminger.com>

  

ᓚᘏᗢ

#### Automates uploading and downloading files using FTP/SFTP specifically for EDI transactions

# Dependancies

💧 AQHelpers

💧 AQFTP

💧 WinSCPnet

### TODO
- Add ability to run independantly with an internal loop

[versions.md](versions)

[AQFTP/versions.md](/DLL/AQFTP/versions.md)

[AQFTP/versions.md](/DLL/AQFTP/versions.md)

[AQHelpers/versions.md](/DLL/AQHelpers/versions.md)

### Configuration

The configuration is in an XML file. The path to this file can be specified as an argument *See Usage*

    <?xml version="1.0" encoding="utf-8" ?>
	<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8" />
    </startup>
	  <appSettings>
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
    
    <add key="SFTPServer" value="sftp.s[ftpserver].com" />
    <add key="SFTPServerUserName" value="[username]" />
    <add key="SFTPServerPassword" value="[password" />
    <add key="SshHostKeyFingerprint" value="ssh-rsa 2048 [hash]" />
    <add key="SFTPport" value="22" />

    <add key="Inbound" value="EDI/Inbound" />
    <add key="Outbound" value="EDI/Outbound" />

    <add key="SMTP" value="smtp.[smptp].com" />
    <add key="EmailFrom" value="no-reply@[domain].com" />
    <add key="EmailPort" value="587" />
    <add key="EmailCredentialsUser" value="no-reply@[domain].com" />
    <add key="EmailCredentialsPass" value="[password]" />
	  </appSettings>
	</configuration>

 - Testing [boolean] if True then the config will use the TEST configurations
 - LogEvents [boolean] if True then events will be logged
 - ConsoleLog [boolean] If True then Console events will be logged (note: some events will always be logged)
 - LogPath [string] If set this is a path to log files *.log
 - EmailUpdates [boolean] If True and if EmailAddresses is set then an email will be dispatched at certain events
 - EmailErrors [boolean] if True then email is dispatched when errors occur
 - EmailAddresses [string] email addresses of events. Can be comma delimited
 - UseSFTP [boolean] If True then then the requests will be sent via SFTP
 - TESTEDIIN [string] Path to a local folder for testing. Files will be downloaded here
 - TESTEDIOUT [string] Path to a local folder for testing. Files will be uploaded from here
 - EDIIN [string] Files will be downloaded here
 - EDIOUT [string] Files will be uploaded from here
 - FtpServer [string] FTP server domain
 - FtpServerUserName [string] FTP user name
 - FtpServerPassword [string] FTP password
 - SFTPServer [string]  SFTP server domain
 - SFTPServerUserName [string] SFTP user name
 - SFTPServerPassword [string] SFTP password
 - SshHostKeyFingerprint [string] *optional* - See Create SshHostKeyFingerprint  below
 - SFTPport [int] The port number *default is 22*
 - Inbound [string] the remote folder where inbound EDI files should land
 - Outbound [string] the remote folder where EDI files will be uploaded to
 - SMTP [string] the domain of the SMTP email server
 - EmailFrom [string] an email address the SMTP will be sending from *typically re-reply@domain.com*
 - EmailPort [int] The number of the SMTP port *default is 587*
 - EmailCredentialsUser [string] the SMTP user name
 - EmailCredentialsPass [string] the SMTP password

### Usage
#### eServer or IQAlert
Follow the instructions for adding an EXE as an Action.
Set the interval to a reasonable timeframe *30 minutes for example*
It will probably be necessary to set the path to the config file. Do so as the first argument 
*Example*

    \\DELMIAWORKS\BIN\AQFTP\AQ_FTP.exe \\DELMIAWORKS\BIN\AQFTP\AQ_FTP.exe.config

#### PowerShell Command Line (CLI) or Terminal
These can be run from the Wndows Task Scheduler or from the CRON tab they can also be run from a Powershell script or a BASH script.

### Create SshHostKeyFingerprint  
Some servers may require an SSH Host fingerprint. This can be created in:
#### Powershell

    ssh-keyscan -t rsa host.example.com 2>$null | ssh-keygen -lf - | ForEach-Object { $p = $_ -split '\s+' "ssh-rsa $($p[0]) $($p[1])" }

#### Mac and Linux

    ssh-keyscan -t rsa host.example.com 2>/dev/null | ssh-keygen -lf - | awk '{print "ssh-rsa", $1, $2}'
