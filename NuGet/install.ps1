param($installPath, $toolsPath, $package, $project)

function MakeRelativePath($Origin, $Target)
{
    $originUri = New-Object Uri('file://' + $Origin)
    $targetUri = New-Object Uri('file://' + $Target)
    $originUri.MakeRelativeUri($targetUri).ToString().Replace('/', [System.IO.Path]::DirectorySeparatorChar)
}

function InsertImport($LoadedProject, $AfterChild, $Path)
{
    $Import = $LoadedProject.Xml.CreateImportElement($Path)
    $Import.Condition = "Exists('" + $Path + "')"
    $LoadedProject.Xml.InsertAfterChild($Import, $AfterChild)
}

function InsertAssemblyAttribute($AssemblyInfoPath, $ExistingSubstring, $Attribute)
{
    if (-not (Get-Content $AssemblyInfoPath | Select-String $ExistingSubstring))
    {
        Add-Content $AssemblyInfoPath "`r`n$Attribute"
    }
}

# Find ProjectLink source
$projectName = [System.IO.Path]::GetFileNameWithoutExtension($project.FullName)
$projectLinkSourceName = if ($projectName.EndsWith(".Web")) { $projectName.SubString(0, $projectName.Length - 4) } else { "SourceProject" }
$projectLinkSource = "..\$projectLinkSourceName\$projectLinkSourceName.csproj"

# Try to copy RootNamespace and AssemblyName from the ProjectLink source project
$projectLinkSourceFullName = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FullName), $projectLinkSource)
if (Test-Path $projectLinkSourceFullName)
{
    $projectCollection = New-Object Microsoft.Build.Evaluation.ProjectCollection
    $projectLinkSourceProject = $projectCollection.LoadProject($projectLinkSourceFullName)
    
    $project.Properties.Item("RootNamespace").Value = $projectLinkSourceProject.GetPropertyValue("RootNamespace")
    $project.Properties.Item("AssemblyName").Value = $projectLinkSourceProject.GetPropertyValue("AssemblyName")

    $projectCollection.Dispose()
}

# Create ProjectLink.targets
$projectLinkTargetsPath = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FullName), "ProjectLink.targets")
if (-not (Test-Path $projectLinkTargetsPath))
{
    $projectLinkTargetsTemplatePath = [System.IO.Path]::Combine($toolsPath, "ProjectLink.targets.template")
    Get-Content $projectLinkTargetsTemplatePath | % {$_ -replace "ProjectLinkSourcePath", $projectLinkSource }  | Out-File $projectLinkTargetsPath
    $project.ProjectItems.AddFromFile($projectLinkTargetsPath) | Out-Null
}

# Create project files
$assemblyName = $project.Properties.Item("AssemblyName").Value

# Create bridge.json, targeting to a single <AssemblyName>.js file
$bridgeJsonPath = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FullName), "bridge.json")
if (-not (Test-Path $bridgeJsonPath))
{
    "{`r`n    ""fileName"": ""$assemblyName"",`r`n    ""fileNameCasing"": ""None"",`r`n    ""outputBy"": ""Project"",`r`n}" | Out-File $bridgeJsonPath
    $project.ProjectItems.AddFromFile($bridgeJsonPath) | Out-Null
}

# Create <AssemblyName>.html
$applicationHtmlPath = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FullName), "$assemblyName.html")
if (-not (Test-Path $applicationHtmlPath))
{
    $applicationHtmlTemplatePath = [System.IO.Path]::Combine($toolsPath, "Application.html.template")
    Get-Content $applicationHtmlTemplatePath | % {$_ -replace "Application", $assemblyName }  | Out-File $applicationHtmlPath
    $applicationHtmlItem = $project.ProjectItems.AddFromFile($applicationHtmlPath)
    $applicationHtmlItem.Properties.Item("BuildAction").Value = 0
    $applicationHtmlItem.Properties.Item("CopyToOutputDirectory").Value = 1
}


# Update AssemblyInfo.cs
$assemblyInfoPath = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FullName), "Properties\AssemblyInfo.cs")

# Add a default reflectability assembly attribute
InsertAssemblyAttribute -AssemblyInfoPath $assemblyInfoPath -ExistingSubstring "Reflectable" -Attribute "[assembly: Bridge.Reflectable(Bridge.MemberAccessibility.ProtectedInstanceProperty)]"

# Add a reference to a type in Granular.Host
InsertAssemblyAttribute -AssemblyInfoPath $assemblyInfoPath -ExistingSubstring "Granular.Host" -Attribute "[assembly: Granular.Compatibility.AssemblyReference(typeof(Granular.Host.WebApplicationHost))]"

# Add a reference to a type in Presentation.Generic
InsertAssemblyAttribute -AssemblyInfoPath $assemblyInfoPath -ExistingSubstring "Presentation.Generic" -Attribute "[assembly: Granular.Compatibility.AssemblyReference(typeof(Granular.Presentation.Generic.ButtonChrome))]"


# Insert targets
$loadedProject = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

# Get old targets import elements
$oldTargets = $loadedProject.Xml.Imports | ? { $_.Project.EndsWith("\Granular.targets") -or $_.Project.EndsWith("\ProjectLink.targets") -or $_.Project.EndsWith("\GeneratedItems.targets") }
$targetsLocation = $oldTargets | Select-Object -Last 1
if (-not $targetsLocation)
{
    $targetsLocation = $loadedProject.Xml.Imports | Select-Object -Last 1
    if (-not $targetsLocation)
    {
        $targetsLocation = $loadedProject.Xml.Children | Select-Object -Last 1
    }
}

# Insert project's ProjectLink.targets
InsertImport -LoadedProject $loadedProject -AfterChild $targetsLocation -Path "ProjectLink.targets"

# Insert build\ProjectLink.targets
InsertImport -LoadedProject $loadedProject -AfterChild $targetsLocation -Path "`$(SolutionDir)$(MakeRelativePath -Origin $project.DTE.Solution.FullName -Target ([System.IO.Path]::Combine($installPath, ""build\ProjectLink.targets"")))"

# Insert project's GeneratedItems.targets
InsertImport -LoadedProject $loadedProject -AfterChild $targetsLocation -Path "GeneratedItems.targets"

# Insert build\GeneratedItems.targets
InsertImport -LoadedProject $loadedProject -AfterChild $targetsLocation -Path "`$(SolutionDir)$(MakeRelativePath -Origin $project.DTE.Solution.FullName -Target ([System.IO.Path]::Combine($installPath, ""build\GeneratedItems.targets"")))"

# Insert build\Granular.targets
InsertImport -LoadedProject $loadedProject -AfterChild $targetsLocation -Path "`$(SolutionDir)$(MakeRelativePath -Origin $project.DTE.Solution.FullName -Target ([System.IO.Path]::Combine($installPath, ""build\Granular.targets"")))"

# Remove old targets elements
if ($oldTargets)
{
    $oldTargets | % { $loadedProject.Xml.RemoveChild($_) }
}

$project.Save()
