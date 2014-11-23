<#
.SYNOPSIS
Dump production database to local file.
#>

Import-Module .\ModuleFunctions.psm1

$dbConnection = Get-ConnectionString "Release"

Set-Variable productionDb -option Constant -value $dbConnection["Database"]
Set-Variable productionDbHost -option Constant -value $dbConnection["Server"]
Set-Variable productionDbUser -option Constant -value $dbConnection["User Id"]
Set-Variable dbPassword -option Constant -value $dbConnection["Password"]

Write-Host

$pgDump = Get-ChildItem (Join-Path ${env:ProgramFiles(x86)} "PostgreSQL") -Recurse -Filter pg_dump.exe

if ($pgDump -eq $null)
{
    Write-Host "Could not find PostgreSQL Dump"
    Exit
}

$dump = "{0}-{1}{2:00}{3:00}_{4:00}{5:00}{6:00}.sql.gz" `
    -f $productionDb, (Get-Date).Year, (Get-Date).Month, (Get-Date).Day, (Get-Date).Hour, (Get-Date).Minute, (Get-Date).Second

# Set the Postgres password in a session environment variable so all pg
# commands don't prompt for a password.
$env:PGPASSWORD = $dbPassword

& $pgDump.FullName `
--verbose `
--create `
--host=$productionDbHost `
--username=$productionDbUser `
--compress=9 `
--file=$dump $productionDb

Remove-Item Env:\PGPASSWORD

Write-Host
