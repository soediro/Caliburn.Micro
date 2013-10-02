function Get-ScriptDirectory {
  $Invoc = (Get-Variable MyInvocation -Scope 1).Value
  Split-Path $Invoc.MyCommand.Path 
}

# Hang on to current location
pushd . 

# Move to the Package working directory
$pkgroot = (Get-ScriptDirectory) + "\lib_package"
cd $pkgroot

# Clear out the lib which are the references that will be added.
ls -Filter lib | del -recurse
mkdir lib

# Move the built binaries (release) into their respective convention folders
copy ..\..\bin\net40\Release .\lib\net40 -Recurse
copy ..\..\bin\net45\Release .\lib\net45 -Recurse
copy ..\..\bin\sl4\Release .\lib\sl4 -Recurse
copy ..\..\bin\sl5\Release .\lib\sl5 -Recurse
copy ..\..\bin\wp71\Release .\lib\wp71 -Recurse
copy ..\..\bin\wp8\Release .\lib\wp8 -Recurse

# Move WinRT binaries BUT exclude Windows.UI.Interactivity and Callisto
mkdir lib\win8
copy ..\..\bin\win8\Release\* .\lib\win8 -Exclude "Windows.UI.Interactivity.*"
del .\lib\win8\Callisto.*

# Move WinRT81 binaries BUT exclude Windows.UI.Interactivity
mkdir lib\win81
copy ..\..\bin\win81\Release\* .\lib\win81 -Exclude "Windows.UI.Interactivity.*"

popd

.\nuget.exe pack .\lib_package\caliburn.micro.nuspec
.\nuget.exe pack .\start_package\caliburn.micro.start.nuspec
