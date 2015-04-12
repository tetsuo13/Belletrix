<#
.SYNOPSIS
Dump production database to local file.
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
Set-Variable productionDbHost -option Constant -value $dbConnection["Server"]
Set-Variable productionDbUser -option Constant -value $dbConnection["User Id"]
Set-Variable dbPassword -option Constant -value $dbConnection["Password"]

Write-Host

$pgDump = Get-PostgresDumpPath

$dump = "{0}-{1}.sql.gz" -f $productionDb, (Get-Date -Format "yyyyMMdd-HHmmss")

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
