# birds-eye
WORK IN PROGRESS

Requires BizHawk v2.6.2

## Setup
Move BirdsEye.dll to the ExternalTools directory in your BizHawk folder.
It should look like this afterwords:

```
BizHawk
+--ExternalTools
|   +--BirdsEye.dll
|   +--...
+--EmuHawk.exe
+--...
```

From the command-line, in the directory of your BizHawk folder, run 
`.\EmuHawk.exe --open-ext-tool-dll=BirdsEye`

## Connection Modes

### Manual
The external tool will not send or receive messages to and from any connected scripts. User will still have
full control over the emulator using the external tool or the emulator itself.

### Commandeer
This mode will allow any connected scripts to begin interacting with the emulator by sending and receiving
messages to and from the external tool. Frame advancement is determined by time it takes for the external tool
to send a message to any connected scripts and receive a message back.