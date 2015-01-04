<#
.SYNOPSIS
Replace everything in the development DB with what's in production.
#>

Import-Module .\ModuleFunctions.psm1

try
{
    $dbConnection = Get-ConnectionString "Release"
}
catch
{
    Write-Error $_
    Exit
}

Set-Variable productionDb -option Constant -value $dbConnection["Database"]
Set-Variable developmentDb -option Constant -value $dbConnection["Database"]
Set-Variable productionDbHost -option Constant -value $dbConnection["Server"]
Set-Variable productionDbUser -option Constant -value $dbConnection["User Id"]
Set-Variable productionDbOwner -option Constant -value "neoanime"
Set-Variable developmentDbHost -option Constant -value "localhost"
Set-Variable dbPassword -option Constant -value $dbConnection["Password"]

Write-Host

$pgDump = Get-ChildItem (Join-Path ${env:ProgramFiles(x86)} "PostgreSQL") -Recurse -Filter pg_dump.exe

if ($pgDump -eq $null)
{
    Write-Host "Could not find PostgreSQL Dump"
    Exit
}

$psql = Get-ChildItem (Join-Path ${env:ProgramFiles(x86)} "PostgreSQL") -Recurse -Filter psql.exe

if ($psql -eq $null)
{
    Write-Host "Could not find PostgreSQL shell"
    Exit
}

# Set the Postgres password in a session environment variable so all pg
# commands don't prompt for a password.
$env:PGPASSWORD = $dbPassword

$dump = [IO.Path]::GetTempFileName()

# Export production database.
Write-Host "Exporting $productionDb@$productionDbHost..." -foregroundcolor Yellow
Write-Host

& $pgDump.FullName --host=$productionDbHost --username=$productionDbUser --file=$dump $productionDb

# Wipe out everything in the development database.
Write-Host "Purging everything in $developmentDb..." -foregroundcolor Yellow
Write-Host

$purgeDev = [IO.Path]::GetTempFileName()

# Write the SQL statements to drop every table in the database.
& $psql.FullName `
--host=$developmentDbHost `
--username=$productionDbUser `
--dbname=$developmentDb `
--tuples-only `
--command="SELECT 'DROP TABLE \""' || tablename || '\"" CASCADE;' FROM pg_tables WHERE schemaname = 'public';" | Out-File $purgeDev

# Avoid the "invalid byte sequence for encoding "UTF8": 0xff" error from
# attempting to read a file that has the Byte Order Mark (BOM). Write the
# contents in UTF8 without the BOM to make psql happy.
[System.IO.File]::WriteAllLines($purgeDev, (Get-Content $purgeDev))

& $psql.FullName `
--host=$developmentDbHost `
--username=$productionDbOwner `
--dbname=$developmentDb `
--file=$purgeDev

# Import the production database dump.
Write-Host
Write-Host "Importing into $developmentDb@$developmentDbHost..."  -foregroundcolor Yellow
Write-Host

& $psql.FullName `
--host=$developmentDbHost `
--username=$productionDbOwner `
--dbname=$developmentDb `
--file=$dump

Remove-Item Env:\PGPASSWORD
Remove-Item $dump
Remove-Item $purgeDev

Write-Host
