import socketserver
import threading
import os

returnedMemory = {}
memoryAddresses = ""
controllerInput = None
serverOutput = "None"
advance = False
memoryReceived = False


class ConnectionAddress:
    """
    Instantiate a connection address that will be used to for 
    socket communication between the server and hook.lua.

    :param host: The host ip of the socket connection.
    :type host: str

    :param port: The port of the socket connection.
    :type port: int
    """
    def __init__(self, host, port):
        self.host = host
        self.port = port


def addMemoryAddress(label, addr):
    """
    Add a memory address for hook.lua to read.

    :param label: A label that will be used as a key for storing
    returned data from `addr`. This will be what you pass in to
    functions asking for a label.
    :type label: str

    :param addr: String representing a memory address to read from.
    :type addr: str
    """
    global memoryAddresses

    returnedMemory[label] = []
    memoryAddresses += addr + ";"


def getMemory(label):
    """
    Get the latest memory data from and address received from 
    hook.lua. This will return the latest value received. 
    Returns None if there has been no data received yet, and
    throws a KeyError if the label is not a valid key

    :param label: The label set by 'addMemoryAddress' representing
    the address.
    :type label: str
    """
    try:
        return returnedMemory[label][-1]
    except IndexError:
        return None
    except KeyError:
        raise KeyError(label + " is not a key in returnedMemory.")


# Launch the Bizhawk emulator with correct arguements
# path: path to find hook.lua
# port: Port to connect to
def launchEmuhawk(path, port):
    """
    Launch the Bizhawk emulator with correct arguements.
    Do not call this directly.
    """
    socketIp = " --socket_ip=127.0.0.1"
    socketPort = " --socket_port=" + str(port)
    luaPath = " --Lua=" + path
    os.system("cd C:/BizHawk-2.4.2 && EmuHawk.exe" + socketIp + socketPort + luaPath)


# Request handler for the TCP server
# self.rfile: A file-like object that stores requests from the client
# self.wfile: A file-like object that sends data back to the client
# memory: A string of memory addresses that the hook should read from
class TCPHandler(socketserver.StreamRequestHandler):
    """
    Request handler for the TCP server.
    Do not instantiate this directly.
    """
    def handle(self):
        global memoryAddresses, advance, serverOutput, memoryReceived

        try:
            for line in self.rfile:
                message = str(line.strip(), "utf-8")

                if message.startswith("SETUP:"):
                    self.wfile.write((memoryAddresses).encode("utf-8"))

                elif message.startswith("MEMORY:"):
                    data = message[7:].split(";")
                    for e, k in enumerate(returnedMemory.keys()):
                        returnedMemory[k].append(data[e])
                    memoryReceived = True

                if advance and memoryReceived:
                    self.wfile.write(("INPUT:" + serverOutput).encode("utf-8"))
                    advance = False
                    memoryReceived = False
                    serverOutput = "None"

        except ConnectionResetError:
            raise ConnectionResetError("Connection with hook.lua lost")


def launchAndCreateServer(hookPath, address):
    """
    Launch the Bizhawk emulator with the hopefully correct arguements
    and then connect socket server to the emulator.

    :param hookPath: The path used by Bizhawk to find hook.lua
    :type hookPath: str

    :param address: The the connection address used for communicating.
    :type address: ConnectionAddress
    """
    emuThread = threading.Thread(target=launchEmuhawk, args=(hookPath, address.port))
    emuThread.start()

    server = socketserver.TCPServer((address.host, address.port), TCPHandler)
    commThread = threading.Thread(target=server.serve_forever)
    commThread.start()


def setControllerInput(a=False, b=False, up=False, down=False, right=False, left=False):
    """
    Set and controller inputs to be executed in the emulator.
    All inputs are set to False be default.
    All inputs are only executed for a single frame.
    Must call advanceFrame to send inputs to hook.lua

    :param a: State of the A button
    :type a: bool

    :param b: State of the B button
    :type b: bool

    :param up: State of the Up button on the control pad
    :type up: bool

    :param down: State of the Down button on the control pad
    :type down: bool

    :param right: State of the Right button on the control pad
    :type right: bool

    :param left: State of the Left button on the control pad
    :type left: bool
    """
    global controllerInput

    boolToString = {False : "false", True : "true"}
    controllerInput = boolToString[a] + ";" + boolToString[b] + ";" + \
                      boolToString[up] + ";" + boolToString[down] + ";" + \
                      boolToString[right] + ";" + boolToString[left] + ";"


def advanceFrame():
    """
    Advance the emulator to the next frame.
    This will send any set controller inputs to hook.lua.
    """
    global serverOutput, advance, controllerInput

    if controllerInput:
        serverOutput = controllerInput
    
    controllerInput = None
    advance = True


# Use for debugging
if __name__ == "__main__":
    serverAddress = ConnectionAddress("localhost", 8080)
    addMemoryAddress("addr1", "0009")
    addMemoryAddress("addr2", "0024")
    addMemoryAddress("addr3", "000A")
    launchAndCreateServer(
        "C:/Users/Conno/VSCodeProjects/birds-eye/src/hook.lua",
        serverAddress
    )
