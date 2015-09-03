[void][System.Reflection.Assembly]::LoadWithPartialName('Microsoft.SqlServer.ConnectionInfo')
[void][System.Reflection.Assembly]::LoadWithPartialName('Microsoft.SqlServer.Management.Sdk.Sfc')
[void][System.Reflection.Assembly]::LoadWithPartialName('Microsoft.SqlServer.SMO')
[void][System.Reflection.Assembly]::LoadWithPartialName('Microsoft.SqlServer.SMOExtended')

Import-Module .\ModuleFunctions.psm1

try
{
    $dbConnection = Get-ConnectionString
}
catch
{
    Write-Error $_
    Exit
}

Write-Output ("Started at: " + (Get-Date))

# Remove possible "tcp:" prefix in data source since colons are illegal
# characters in filenames.
if ($dbConnection["Data Source"] -Match ":")
{
    $dataSource = $dbConnection["Data Source"].Substring($dbConnection["Data Source"].IndexOf(":") + 1)
}
else
{
    $dataSource = $dbConnection["Data Source"]
}

$backupFolder = "C:\DBBackup"

$timestamp = Get-Date -format "yyyyMMdd_HHmmss"
$filename = $dataSource + "_" + $dbConnection["Initial Catalog"] + "_full_" + $timestamp + ".bak"

$connectionString = "Data Source={0};Initial Catalog={1};User ID={2};Password={3}" -f `
    $dbConnection["Data Source"], $dbConnection["Initial Catalog"], $dbConnection["User ID"], $dbConnection["Password"]

$server = New-Object Microsoft.SqlServer.Management.Smo.Server $dbConnection["Data Source"]
$server.ConnectionContext.LoginSecure = $false
$server.ConnectionContext.EncryptConnection = $true
$server.ConnectionContext.ConnectionString = $connectionString
$server.ConnectionContext.StatementTimeout = 0

$db = $server.Databases[$dbConnection["Initial Catalog"]]

$backupFile = (Join-Path "\\belletri.w20.wh-2.com\App_Data" $filename)
#$server.Settings.BackupDirectory = $backupFolder

$backup = New-Object Microsoft.SqlServer.Management.Smo.Backup
$backup.Action = "Database"
$backup.Database = $db.Name
$backup.Devices.AddDevice($backupFile, "File")
$backup.BackupSetDescription = "Full backup of " + $db.Name + " " + $timestamp
$backup.Incremental = 0

#$server.Information

# Starting full backup process.
try
{
    $backup.SqlBackup($server)
}
catch
{
    Write-Error $_.Exception.InnerException
}

$server.ConnectionContext.Disconnect()

Write-Output ("Finished at: " + (Get-Date))
