# birds-eye
A BizHawk external tool + python library, giving a (hopefully) convenient interface for python
scripts to communicate with the BizHawk emulator.

This is the first project I've made with the intent of others using it, so any kind of suggestion
would be incredibly helpful.

Thank you :]

This library is VERY bares bones at the moment, with only the current features available:
- Connecting to the external tool
- Retrieving memory data from the external tool
- Sending controller inputs to be executed in the emulator
- Retrieving the current framecount

I plan on adding a lot more as I continue to use this library myself.

Tested using BizHawk version 2.6.2, however other versions *might* work.

If you wish to develop the tool yourself, make sure to copy the contents of your BizHawk version
into `BizHawk/`. The file tree should match the example shown in the
[External Tool](#external-tool) section.

## Setup

### External Tool
The external tool can be found and downloaded in the
[birds-eye github repository](https://github.com/SkiHatDuckie/birds-eye). (check releases)

Move BirdsEye.dll to the ExternalTools directory in your BizHawk folder.<br/>
It should look like this afterwords:

```
BizHawk
+--ExternalTools
|  +--BirdsEye.dll
|  +--...
+--EmuHawk.exe
+--...
```

If everything is correct, then you should be able to open up the external tool by running the
emulator, then going to:<br/>
*Tools* -> *External Tools* -> *BirdsEye*

### Installing the library
`pip install birds-eye-lib`<br/>
[python library docs](https://birds-eye.readthedocs.io/en/latest/)

## Communication Modes
Determines how the external tool communicates with the emulator and a connected python client.

### Manual
The external tool will not execute input states sent from a connected python client. User will
still have full control over the emulator using the external tool or the emulator itself. Things
that do not interfere with play, such as reading from memory, will still be functional.

### Commandeer
This mode will allow a connected python client to begin interacting with the emulator by sending
and receiving messages to and from the external tool.