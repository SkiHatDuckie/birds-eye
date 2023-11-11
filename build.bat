:: Builds the external tool, then launches EmuHawk.exe for testing.
@echo off
cd exttool
dotnet build
cd ../BizHawk
EmuHawk.exe
cd ..