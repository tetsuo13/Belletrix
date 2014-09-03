<#
.SYNOPSIS
Replace everything in the development DB with what's in production.
#>

Set-Variable productionDb -option Constant -value "neoanime_abroadadvisor"
Set-Variable developmentDb -option Constant -value "neoanime_abroadadvisor"
Set-Variable dbHost -option Constant -value "box450.bluehost.com"
Set-Variable dbUser -option Constant -value "neoanime_abroadadvisor"
Set-Variable dbOwner -option Constant -value "neoanime"

# TODO: This should come from grepping Web.Release.config
Set-Variable dbPassword -option Constant -value "uZtVIgiToZP4RxTPD"

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
Write-Host "Exporting ``$productionDb``..."

& $pgDump.FullName --host=$dbHost --username=$dbUser --file=$dump $productionDb

# Wipe out everything in the development database.
Write-Host "Purging everything in ``$developmentDb``..."

$purgeDev = [IO.Path]::GetTempFileName()

# Write the SQL statements to drop every table in the database.
& $psql.FullName `
--host=localhost `
--username=$dbUser `
--dbname=$developmentDb `
--tuples-only `
--command="SELECT 'DROP TABLE \""' || tablename || '\"" CASCADE;' FROM pg_tables WHERE schemaname = 'public';" | Out-File $purgeDev

# Avoid the "invalid byte sequence for encoding "UTF8": 0xff" error from
# attempting to read a file that has the Byte Order Mark (BOM). Write the
# contents in UTF8 without the BOM to make psql happy.
[System.IO.File]::WriteAllLines($purgeDev, (Get-Content $purgeDev))

& $psql.FullName `
--host=localhost `
--username=$dbOwner `
--dbname=$developmentDb `
--file=$purgeDev

# Import the production database dump.
Write-Host "Importing into ``$developmentDb``..."

& $psql.FullName `
--host=localhost `
--username=$dbOwner `
--dbname=$developmentDb `
--file=$dump

Remove-Item $dump
Remove-Item $purgeDev

Write-Host
