import birdseyelib as bird

HOST = "127.0.0.1"
PORT = 8080

if __name__ == "__main__":
    client = bird.Client(HOST, PORT)
    board_name = bird.emulation.BoardName(client)
    display_type = bird.emulation.DisplayType(client)
    requests = bird.RequestBatch((board_name, display_type))

    client.connect()
    print("Connecting to server at {} on port {}.".format(HOST, PORT))

    if not client.is_connected():
        print("Could not connect to external tool :[")
        exit(1)
    
    # NOTE: Certain data will not be available depending on the system being emulated.
    requests.queue_all()

    client.advance_frame()

    print("Board: {:16}".format(board_name.receive()))
    print("Display Type: {:16}".format(display_type.receive()))

    client.close()