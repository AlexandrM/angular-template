$params = @{ 
	"publish_folder" = "_Published"
};

function Write2 {
	Param ([string] $str1)
	
	Write-Output "-----------------------------------------------"
	Write-Output $str1
	Write-Output "-----------------------------------------------"
}

function ReadParams {
	Param ([string] $str1)
	
	Get-Content -Path $str1| ForEach-Object {
		$x = $_.split('=');
		$params[$x[0]] = $x[1];
	}
}

$publish_folder="_Published"
$params_file="params.txt"

$dotnet = $args | where { $_ -eq "dotnet" }
$spa = $args | where { $_ -eq "spa" }
$file = $args | where { ($_ -ne "dotnet" -and $_ -ne "spa") }

if ($file) {
	$params_file = $file
}

ReadParams($params_file)

if ((!$dotnet) -and (!$spa)) {
	$dotnet = "dotnet"
	$spa = "spa"
}

if ($dotnet -eq "dotnet") {
	Write2 "dotnet publish"
	dotnet publish AngularTemplate.Web\AngularTemplate.Web.csproj -c Release -f net6.0 -o $params["publish_folder"]
}
if ($spa -eq "spa") {
	Write2 "ng build"
	cd AngularTemplate.SPA
	$path = "..\" + $params["publish_folder"] + "\wwwroot"
	ng build --output-path $path
	cd ..
}
