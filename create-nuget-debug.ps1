# Git command used for getting latest commit
$gitCmdName = "git"
$gitCmdParameter = "rev-parse HEAD"

$currentDate = Get-Date
# Write-Host $currentDate.ToUniversalTime()

$latestGitCommitHashFull = "Git is not installed"
$latestGitCommitHashShort = "Git is not installed"

if (Get-Command $gitCmdName -errorAction SilentlyContinue)
{
	$latestGitCommitHashFull = &git rev-parse HEAD
	$latestGitCommitHashShort = &git rev-parse --short HEAD
    # Write-Host "$gitCmdName exists"
}

Write-Host $latestGitCommitHashFull $latestGitCommitHashShort
$finalCommand = "dotnet pack" + " " + "--configuration Debug" + " " + "/p:InformationalVersion=""" + $currentDate.ToUniversalTime() + " " + $latestGitCommitHashFull + """" + " " + "--version-suffix" + " " + $latestGitCommitHashShort
Write-Host $finalCommand
