<#
.SYNOPSIS
Initialize a new development database.
#>

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

Set-Variable productionDb -option Constant -value $dbConnection["Database"]
Set-Variable productionDbUser -option Constant -value $dbConnection["User Id"]
Set-Variable dbPassword -option Constant -value $dbConnection["Password"]

$psql = Get-PostgresInteractiveTerminalPath
$createDb = Get-PostgresCreateDbPath

Write-Host "Note: This is going to prompt for the postgres user's password a lot."
Write-Host
Write-Host "Checking for $productionDb locally..." -foregroundcolor Yellow
Write-Host

$dbExists = & $psql.Fullname --tuples-only --no-align --command="SELECT 1 FROM pg_database WHERE datname = '$productionDb'" --username=postgres | Out-String

if ($? -eq $False)
{
    Throw "Error querying for the existence of $productionDb"
}

if ([string]::IsNullOrEmpty($dbExists) -eq $False)
{
    Write-Host "Error: $productionDb already exists locally. Nothing more to do." -foregroundcolor Red
    Exit
}

Write-Host "Creating local database $productionDb..." -foregroundcolor Yellow
Write-Host

# Windows doesn't have an "en_US.UTF-8" locale. Using "american_usa" is
# effectively the same as using "English_United States.1252" which is fine
# alongside UTF8 encoding.
& $createDb.FullName `
--encoding=UTF8 `
--locale=american_usa `
--template=template0 `
--username=postgres `
$productionDb

if ($? -eq $False)
{
    Throw "Could not create database"
}

Write-Host "Creating local user $productionDbUser..." -foregroundcolor Yellow
Write-Host

& $psql.FullName `
--username=postgres `
--command="CREATE ROLE \""$productionDbUser\"" WITH LOGIN ENCRYPTED PASSWORD '$dbPassword'"

if ($? -eq $False)
{
    Throw "Could not create user $productionDbUser"
}

Write-Host "Granting full privileges to $productionDb..." -foregroundcolor Yellow
Write-Host

& $psql.FullName `
--username=postgres `
--command="GRANT ALL PRIVILEGES ON DATABASE ""$productionDb"" TO \""$productionDbUser\"""

if ($? -eq $False)
{
    Throw "Could not create user $productionDbUser"
}
