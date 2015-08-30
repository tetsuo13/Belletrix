<#
.SYNOPSIS
Downloads and deletes the DB backup from the FTP site.

.DESCRIPTION
The backup is generated into /App_Data directory of the site as the file name
DB_92352_belletrix_backup.bak every time. It must be deleted before another
backup can be made.
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

$ftpSite = "ftp.belletrix.org"

function Download-BackupFile()
{
    Param(
        [Parameter(Mandatory = $true)]
        [System.Management.Automation.PSCredential]$ftpCredentials,

        [Parameter(Mandatory = $true)]
        [string]$sourceFilename,

        [Parameter(Mandatory = $true)]
        [string]$destination
    )

    # Remove the leading backslash.
    $username = $ftpCredentials.Username.Substring(1)

    $path = "ftp://" + $ftpSite + "/App_Data/" + $sourceFilename

    Write-Output "Downloading $path to $destination ..."

    $webclient = New-Object System.Net.WebClient
    $webclient.Credentials = New-Object System.Net.NetworkCredential($username, $ftpCredentials.GetNetworkCredential().Password)
    $webclient.DownloadFile($path, $destination)

    if ($? -eq $false)
    {
        throw
    }

    Write-Output "Deleting $path ..."

    $ftp = [System.Net.FtpWebRequest]::Create($path)
    $ftp.Method = [System.Net.WebRequestMethods+Ftp]::DeleteFile
    $ftp.Credentials = new-object System.Net.NetworkCredential($username, $ftpCredentials.GetNetworkCredential().Password)
    $response = [System.Net.FtpWebResponse]$ftp.GetResponse()
    $response.Close()
}

# TODO: Working but credentials and path need to be passed in.
function Ftp-ListDirectory()
{
    [void] [System.Reflection.Assembly]::LoadWithPartialName("System.Net")

    $server = "ftp://ftp.belletrix.org/App_Data"
    $ftp = [system.net.ftpwebrequest] [system.net.webrequest]::create($server)
    $ftp.Method = [System.Net.WebRequestMethods+FTP]::ListDirectoryDetails
    $ftp.Credentials = New-Object System.Net.NetworkCredential("", "")
    $response = $ftp.getresponse()
    $stream = $response.getresponsestream()

    $buffer = New-Object System.Byte[] 1024 
    $encoding = New-Object System.Text.AsciiEncoding 

    $outputBuffer = "" 
    $foundMore = $false 

    ## Read all the data available from the stream, writing it to the 
    ## output buffer when done. 
    do 
    { 
        ## Allow data to buffer for a bit 
        start-sleep -m 1000 

        ## Read what data is available 
        $foundmore = $false 
        $stream.ReadTimeout = 1000 

        do 
        { 
            try 
            { 
                $read = $stream.Read($buffer, 0, 1024) 

                if ($read -gt 0) 
                { 
                    $foundmore = $true 
                    $outputBuffer += ($encoding.GetString($buffer, 0, $read)) 
                } 
            }
            catch
            {
                $foundMore = $false
                $read = 0
            }
        }
        while ($read -gt 0) 
    }
    while ($foundmore) 

    $outputBuffer
}

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

Write-Output "This script requires FTP login details..."

$ftpCredentials = $host.ui.PromptForCredential("Credentials for FTP", "Please enter credentials for " + $ftpSite, "", "")

Download-BackupFile $ftpCredentials ($dbConnection["Initial Catalog"] + "_backup.bak") (Join-Path $backupFolder $filename)
