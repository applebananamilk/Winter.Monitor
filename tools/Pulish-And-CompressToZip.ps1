function Write-Info {
	param(
		[Parameter(Mandatory = $true)]
		[string]
		$text
	)

	Write-Host $text -ForegroundColor Black -BackgroundColor Green

	try {
		$host.UI.RawUI.WindowTitle = $text
	}		
	catch {
		#Changing window title is not suppoerted!
	}
}

function Get-Current-Version { 
	$csharpProjectFilePath = resolve-path "../src/Winter.Monitor/Winter.Monitor.csproj";
	$csharpProjectXmlCurrent = [xml](Get-Content $csharpProjectFilePath) 
	$currentVersion = $csharpProjectXmlCurrent.Project.PropertyGroup[0].Version.Trim()
	return $currentVersion
}

$targetRuntimes = (
    "win-x64",
    "linux-x64",
    "linux-arm"
)
$projectFolder = "../src/Winter.Monitor";
$currentFolder = (Get-Item -Path "./" -Verbose).FullName;
$pulishRootFolder = Join-Path $currentFolder "Releases"
$version = Get-Current-Version

Set-Location $projectFolder

Write-Info "开始进行发布..."

if (Test-Path -path $pulishRootFolder) {
    Remove-Item -path $pulishRootFolder -Recurse -Force
}

foreach ($targetRuntime in $targetRuntimes) {
    $pulishTargetFolder = Join-Path $pulishRootFolder "$targetRuntime-v$version"

    Write-Info "发布 $targetRuntime"

    # 发布
    dotnet publish -c Release -r $targetRuntime -o $pulishTargetFolder --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true

    # 压缩文件夹
    Compress-Archive -Path $pulishTargetFolder -DestinationPath "$pulishTargetFolder.zip"

    # 删除发布文件夹
    Remove-Item -path $pulishTargetFolder -Recurse -Force
}

Write-Info "发布结束 :)"