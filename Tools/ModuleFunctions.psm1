function Get-ConnectionString()
{
<#
.SYNOPSIS
Returns a dictionary for the database connection details for a given build.

.DESCRIPTION
Uses the current working directory to go to the parent and into the MVC root
directory. From there it loads the "Web.$build.config" XML file into a
dictionary.

.PARAMETER build
Standard ASP.NET build -- Debug, Release, etc.
#>

    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$True)]
        [string]$build
    )

    $PSScriptRoot = Split-Path $script:MyInvocation.MyCommand.Path
    $webConfigPath = (Join-Path (Join-Path (Get-Item $PSScriptRoot).parent.FullName "AbroadAdvisor") "Web.$build.config")

    $webConfig = New-Object XML
    $webConfig.Load($webConfigPath)

    $dbConnectionString = New-Object System.Data.Common.DbConnectionStringBuilder
    $dbConnectionString.set_ConnectionString($webConfig.configuration.connectionStrings.add.connectionString)

    return $dbConnectionString
}
