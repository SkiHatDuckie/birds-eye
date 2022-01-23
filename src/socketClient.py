import socket;
from threading import Thread;


class ThreadWithReturnValue(Thread):
    """
    A Thread object that collects a return value when the thread is joined.
    """
    def __init__(self, group=None, target=None, name=None, args=(), kwargs={}):
        Thread.__init__(self, group, target, name, args, kwargs)
        self._return = None
    
    def run(self):
        if self._target is not None:
            self._return = self._target(*self._args, **self._kwargs)
    
    def join(self, *args):
        Thread.join(self, *args)
        return self._return


class SocketClient:
    """
    Instantiate a socket client object that will be used to
    communicate with the BirdsEye server located at a set
    port and address.
    Use the BirdsEye external tool to get and set the port and
    address the server should listen in to.

    :param ip: Socket address
    :type ip: str

    :param port: Socket port
    :type port: int
    """
    def __init__(self, ip, port):
        self.ip = ip
        self.port = port
        self.client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.connectionStatus = -1

    def connect(self):
        """
        Attempts to send a connection request to the BirdsEye socket server.
        """
        self.connectionStatus = self.client.connect_ex((self.ip, self.port))
    
    def isConnected(self):
        """
        Returns true if client is connected to a socket server object.
        """
        if self.connectionStatus == 0:
            return True
        else:
            return False

    def getMemory(self):
        """
        Get the latest memory data from the server. This will return the 
        latest value received. Returns '-' if there has been no data 
        received yet from an address, and a bytes object representing the received data otherwise.
        Precondition: client is connected to a socket.
        """
        self.client.sendall("MEMORY\n".encode())
        return self.client.recv(1024)

    def setControllerInput(self, a=False, b=False, up=False, down=False, right=False, left=False):
        """
        Set and controller inputs to be executed in the emulator.
        All inputs are set to False be default.
        The inputs are executed until a new controller input is sent.
        Precondition: client is connected to a socket.

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
        boolToString = {False : "false", True : "true"}
        controllerInput = boolToString[a] + ";" + boolToString[b] + ";" + \
                          boolToString[up] + ";" + boolToString[down] + ";" + \
                          boolToString[right] + ";" + boolToString[left] + ";"
        self.client.sendall(("INPUT;" + controllerInput + "\n").encode())
        self.client.recv(1024)


# Use for debugging and testing purposes only
if __name__ == "__main__":
    client = SocketClient("127.0.0.1", 8080)
    commThread = ThreadWithReturnValue(target=client.connect())
    commThread.start()
    while True:
        if client.isConnected():
            if commThread.is_alive():
                commThread.join()

            client.setControllerInput(right=True)
