External Tool
=============
The external tool is needed in order for a python script to interact with the emulator.

Options Form
------------
The *Options* button in the external tool opens another form that allows you to configure certain
parts of the external tool, as well as how it should communicate with the python script.

Note: If order for the changes to take effect, you will need to close out of the external tool and re-open it.

Logging
-------
| Logs from the external tool are sent to the *Log Window* in the BizHawk emulator.
| (*View -> Open Log Window*)

The log level can be set in the Options Form.

Communication Modes
-------------------
The communicaton mode determines how the connected python script can interact with the emulator.

Manual
^^^^^^
The external tool will not execute input states sent from a connected python script,
and the user will still have full control over the emulator.

Things that do not interfere with play, such as reading from memory, are still functional during this time.

Commandeer
^^^^^^^^^^
The external tool will execute any input states sent from the connected python script,
and the user will be unable to input their own controller inputs into the loaded ROM.