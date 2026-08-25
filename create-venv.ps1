# Script for creating a new Python virtual environment and installing required packages.

# Clear screen
Clear-Host

Write-Host "Creating Python virtual environment ..."

# Change current directory to where script resides.
$script_dir = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
Set-Location $script_dir

# Create a python virtual environment in the current folder
$venv_path = Join-Path -Path $script_dir -ChildPath ".venv"
python -m venv $venv_path

# install pythonnet in the virtual environment
$activate_script = Join-Path -Path $venv_path -ChildPath "Scripts\Activate.ps1"
if (Test-Path -Path $activate_script) {
    & $activate_script
    python.exe -m pip install --upgrade pip
    pip install pythonnet
    Write-Host "`nPython virtual environment created and pythonnet installed."
} else {
    Write-Host "`nError: Activation script not found. Virtual environment may not have been created correctly."
}
