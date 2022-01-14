import socket;
from threading import Thread;


class ThreadWithReturnValue(Thread):
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
    # Instantiate a socket client object that will be used to
    # communicate with the BirdsEye server located at a set
    # port and address.
    # Use the BirdsEye external tool to get and set the port and
    # address the server should listen in to.

    # :param ip: Socket address
    # :type ip: str

    # :param port: Socket port
    # :type port: int
    def __init__(self, ip, port):
        self.ip = ip
        self.port = port
        self.client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.connectionStatus = -1
    
    # Attempts to send a connection request to the BirdsEye socket server.
    def connect(self):
        self.connectionStatus = self.client.connect_ex((self.ip, self.port))
    
    # Returns true if client is connected to a socket server object.
    def isConnected(self):
        if self.connectionStatus == 0:
            return True
        else:
            return False


# Use for debugging purposes only
if __name__ == "__main__":
    client = SocketClient("127.0.0.1", 8080)
    commThread = ThreadWithReturnValue(target=client.connect())
    commThread.start()
    while True:
        if client.isConnected():
            commThread.join()
