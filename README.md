# birds-eye
A BizHawk external tool + python library, giving a (hopefully) convenient interface for python scripts to communicate with the BizHawk emulator. <br/>

This is the first project I've made with the intent of others using it, so any kind of suggestion would be incredibly helpful. <br/>
Thank you :]

This library is VERY bares bones at the moment, with only the current features available: <br/>
- Connecting to the external tool <br/>
- Retrieving memory data from the external tool <br/>
- Sending inputs to be executed in the emulator <br/>

I plan on adding a lot more as I continue to use this library myself. <br/>

Tested using BizHawk version 2.6.2, however other versions *might* work.

## Setup

### External Tool
The external tool can be found and downloaded in the [birds-eye github repository](https://github.com/SkiHatDuckie/birds-eye). (check releases)

Move BirdsEye.dll to the ExternalTools directory in your BizHawk folder. <br/>
It should look like this afterwords:

```
BizHawk
+--ExternalTools
|   +--BirdsEye.dll
|   +--...
+--EmuHawk.exe
+--...
```

If everything is correct, then you should be able to open up the external tool by running the emulator, then going to <br/>
Tools -> External Tools -> BirdsEye

### Installing the library
`pip install birds-eye-lib`

## Communication Modes
Determines how the external tool communicates with the emulator and a connected python client.

### Manual
The external tool will not execute input states sent from a connected python client. User will still have
full control over the emulator using the external tool or the emulator itself. Things that do not interfere with
play, such as reading from memory, will still be functional.

### Commandeer
This mode will allow a connected python client to begin interacting with the emulator by sending and receiving
messages to and from the external tool.

## Updates
I will now be creating a list of features that I plan on adding or deleting or fixing as an issue ticket to make 
it easier for both myself and others to see what needs to be done for the next update.