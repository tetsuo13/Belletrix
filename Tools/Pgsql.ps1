<#
.SYNOPSIS
Launch Postgres shell to remote production/staging databases.
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$build
)

Import-Module .\ModuleFunctions.psm1

try
{
    $dbConnection = Get-ConnectionString $build
}
catch
{
    Write-Error $_
    Exit
}

Write-Host

$psql = Get-ChildItem (Join-Path ${env:ProgramFiles(x86)} "PostgreSQL") -Recurse -Filter psql.exe

if ($psql -eq $null)
{
    Write-Host "Could not find PostgreSQL shell"
    Exit
}

$env:PGPASSWORD = $dbConnection["Password"]

& $psql.FullName -h $dbConnection["Server"] -U $dbConnection["User Id"] -d $dbConnection["Database"] -p 5432

Remove-Item Env:\PGPASSWORD
