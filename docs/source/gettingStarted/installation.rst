Installation
============

The Emulator
------------
birds-eye communicates with the BizHawk emulator, which can be found `here`_.
.. _here: https://tasvideos.org/Bizhawk

Currently, only BizHawk version 2.6.2 has been tested with the library and external tool, however other
versions *might* work.

The External Tool
-----------------
The external tool can be found and downloaded in the `birds-eye github repository`_. (check releases)
.. _birds-eye github repository: https://github.com/SkiHatDuckie/birds-eye

Move `BirdsEye.dll` to the `ExternalTools` directory in your BizHawk folder.
It should look like this afterwords:

``BizHawk
+--ExternalTools
|   +--BirdsEye.dll
|   +--...
+--EmuHawk.exe
+--...``

If everything is correct, then you should be able to open up the external tool by running the emulator 
(EmuHawk.exe), then going to `Tools -> External Tools -> BirdsEye`.

The Python library
------------------
The python library can be installed using PyPI

``pip install birds-eye-lib``