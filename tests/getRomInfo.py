import birdseyelib as bird
import time

HOST = "127.0.0.1"
PORT = 8080

if __name__ == "__main__":
    client = bird.Client(HOST, PORT)
    emulation = bird.Emulation(client)

    client.connect()
    print("Connecting to server at {} on port {}.".format(HOST, PORT))

    if not client.is_connected():
        print("Could not connect to external tool :[")
        exit(1)
    
    # NOTE: Certain data will not be available depending on the system being emulated.
    emulation.request_board_name()
    emulation.request_display_type()

    client.advance_frame()

    print("Board: {:16}".format(emulation.get_board_name()))
    print("Display Type: {:16}".format(emulation.get_display_type()))

    client.close()