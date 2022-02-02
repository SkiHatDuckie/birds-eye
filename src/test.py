from birdseye import SocketClient;

HOST = "127.0.0.1"
PORT = 8080

# This script is used for testing the features of Birds Eye python library
if __name__ == "__main__":
    client = SocketClient(HOST, PORT)
    client.connect()

    while True:
        if client.isConnected():
            print(client.getMemory())
            client.setControllerInput(right=True)
        else:
            break
    
    print("No server at {} on port {}.".format(HOST, PORT))