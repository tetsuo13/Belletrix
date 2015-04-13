<#
.SYNOPSIS
Launch Postgres shell to remote production/staging databases.
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

Write-Host

$psql = Get-PostgresInteractiveTerminalPath

$env:PGPASSWORD = $dbConnection["Password"]

& $psql.FullName -h $dbConnection["Server"] -U $dbConnection["User Id"] -d $dbConnection["Database"] -p 5432

Remove-Item Env:\PGPASSWORD
