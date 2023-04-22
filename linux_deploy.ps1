$params = @{ 
	"publish_folder" = "_Published"
};

$params_file="params.txt"

$linux_run1 = "#!/bin/sh"
$linux_run2 = "cd /var/www/AngularTemplate/"
$linux_run3 = "dotnet AngularTemplate.Web.dll"
$linux_run4 = "exit"

function Write2 {
	Param ([string[]] $str1)
	
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

$copy = $args | where { $_ -eq "copy" }
$run = $args | where { $_ -eq "run" }
$file = $args | where { ($_ -ne "copy" -and $_ -ne "run") }

if ($file) {
	$params_file = $file
}
ReadParams($params_file)

if ((!$copy) -and (!$run)) {
	$copy = "copy"
	$run = "run"
}


$linux_host = $params["linux_user"] + "@" + $params["linux_host"]
$from = $params["publish_folder"] + "\*"
$to = $linux_host + ":" + $params["linux_folder"]

if ($copy -eq "copy") {
	Write2 "Copy files"
	scp -r $from $to
}

if ($run -eq "run") {
	Write2 "Run"
	# create run script
	$run_file = $params["linux_folder"] + "/run.sh"""
	$cmd = """printf '#/bin/sh\ncd " + $params["linux_folder"] + "\ndotnet AngularTemplate.Web.dll\nexit' > " + $run_file
	ssh $linux_host $cmd

	# run and wait
	ssh $linux_host bash $run_file
}
