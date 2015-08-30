<#
.SYNOPSIS
Returns a dictionary for the database connection details for a given build.

.DESCRIPTION
Uses the current working directory to go to the parent and into the MVC root
directory. From there it loads the "Web.$build.config" XML file into a
dictionary.
#>
function Get-ConnectionString()
{
    $PSScriptRoot = Split-Path $script:MyInvocation.MyCommand.Path
    $webConfigPath = (Join-Path (Join-Path (Join-Path (Get-Item $PSScriptRoot).parent.FullName "src") "Belletrix") "Web.Release.config")

    $webConfig = New-Object XML
    $webConfig.Load($webConfigPath)

    $dbConnectionString = New-Object System.Data.Common.DbConnectionStringBuilder
    $dbConnectionString.set_ConnectionString($webConfig.configuration.connectionStrings.add.connectionString)

    return $dbConnectionString
}
