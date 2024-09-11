#!/bin/sh
# Builds the external tool, then launches EmuHawk.exe for testing.
set -e
cd exttool
dotnet build
../BizHawk/EmuHawkMono.sh --mono-no-redirect --open-ext-tool-dll=BirdsEye
cd ..