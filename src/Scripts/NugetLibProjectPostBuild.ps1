[CmdletBinding()]
param($RepoDir, $SolutionDir, $ProjectDir, $ConfigurationName, [string[]]$Projects, [string[]]$NetmfVersions, [switch]$Disable)

if(-not ($RepoDir -and $SolutionDir -and $ProjectDir -and $ConfigurationName -and $Projects -and $NetmfVersions))
{
	Write-Error "RepoDir, SolutionDir, ConfigurationName, $Projects, and $NetmfVersions are all required"
	exit 1
}

function CleanNugetPackage([string]$projectName) {

	Write-Verbose "CLEAN"

	$nugetBuildDir = $SolutionDir + 'nuget\' + $ConfigurationName + '\' + $projectName + '\'
	$libDir = $nugetBuildDir + "lib\"
	$srcDir = $nugetBuildDir + "src\"
	$contentDir = $nugetBuildDir + "content\"

	Write-Verbose "Nuget build dir is $nugetBuildDir"
	$nuget = $SolutionDir + "tools\nuget.exe"

	if (test-path $nugetBuildDir) { ri -r -fo $nugetBuildDir }
}


function CopySource([string]$projectName) {

	Write-Verbose "COPYSOURCE"

	$nugetBuildDir = $SolutionDir + 'nuget\' + $ConfigurationName + '\' + $projectName + '\'
	$srcDir = $nugetBuildDir + "src\"
	mkdir $srcDir | out-null

	# Copy source files for symbol server
	$sharedProjectName = $projectName + '.Shared\'
	$sharedDir = $SolutionDir + 'common\' + $sharedProjectName
	if (test-path $sharedDir) {
		Copy-Item -Recurse -Path $sharedDir -Destination $srcDir -Filter "*.cs"
		# rename the copied dir to remove the .Shared
		$sharedTargetPath = $srcDir + $sharedProjectName
		Rename-Item -Path $sharedTargetPath -NewName $projectName
	}

	# no longer needed since there are no generated files in shared source projects
	#$target = $srcDir + $projectName
	#if (test-path $target"\obj") { Remove-Item -Recurse $target"\obj" | out-null }
	#if (test-path $target"\bin") { Remove-Item -Recurse $target"\bin" | out-null }
}

function PrepareNugetPackage([string]$projectName, [string]$netmfVersion) {

	Write-Verbose "PREPARE"

	$nugetBuildDir = $SolutionDir + 'nuget\' + $ConfigurationName + '\' + $projectName + '\'
	$packageProjectDir = $ProjectDir + '..\' + $projectName + '.' + $netmfVersion + '\'
	$libDir = $nugetBuildDir + "lib\"
	$contentSourceDir = $packageProjectDir + "content\"
	$contentDir = $nugetBuildDir + "content\"
	$targetDir = $packageProjectDir + 'bin\' + $ConfigurationName + '\'

	if (test-path $targetDir) {
		if (test-path $targetDir"be") {
			mkdir $libDir"\netmf"$netMFVersion"\be" | out-null
			Copy-Item -Path $targetDir"be\*" -Destination $libDir"\netmf"$netMFVersion"\be" -Include "$projectname.dll","$projectname.pdb","$projectname.xml","$projectname.pdbx","$projectname.pe"
		}
		if (test-path $targetDir"le") {
			mkdir $libDir"\netmf"$netMFVersion"\le" | out-null
			Copy-Item -Path $targetDir"le\*" -Destination $libDir"\netmf"$netMFVersion"\le" -Include "$projectname.dll","$projectname.pdb","$projectname.xml","$projectname.pdbx","$projectname.pe"
		}
		Copy-Item -Path $targetDir"*" -Destination $libDir"\netmf"$netMFVersion -Include "$projectname.dll","$projectname.pdb","$projectname.xml","$projectname.pdbx","$projectname.pe"
	}

	if (test-path $contentSourceDir)	{
		mkdir $contentDir | out-null
		Copy-Item -Path $contentSourceDir -Destination $contentDir
	}

}

function PublishNugetPackage([string]$projectName) {

	Write-Verbose "PUBLISH"

	$nuspec = $ProjectDir + "..\nuspec\" + $projectName + '.nuspec'
	Write-Verbose "nuspec file $nuspec"

	$nugetBuildDir = $SolutionDir + 'nuget\' + $ConfigurationName + '\' + $projectName + '\'
	$libDir = $nugetBuildDir + "lib\"
	$srcDir = $nugetBuildDir + "src\"
	$nuget = $SolutionDir + "tools\nuget.exe"

	# Create the nuget package
	$output = $repoDir + $ConfigurationName
	if (-not (test-path $output)) { mkdir $output | out-null }

	$args = 'pack', $nuspec, '-Symbols', '-basepath', $nugetBuildDir, '-OutputDirectory', $output
	& $nuget $args
}

foreach ($project in $Projects) {
	CleanNugetPackage $project
	CopySource $project
	foreach ($version in $NetmfVersions) {
		PrepareNugetPackage  $project $version
	}
	PublishNugetPackage $project
}
