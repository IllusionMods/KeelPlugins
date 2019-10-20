param (
	[Parameter(Mandatory=$true)][string]$config
)

try {
	[xml]$XmlDocument = Get-Content -Path ..\BuildSettings.AISyoujyo.props
    $XmlDocument.Project.PropertyGroup.Where({$_.condition.contains($config)}).OutputPath
}
catch {
	exit 1
}