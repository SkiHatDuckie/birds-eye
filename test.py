import birdseye as bird

# External tool connects to this address and port 
# TODO: Add way in external tool to set port and address
HOST = "127.0.0.1"
PORT = 8080


if __name__ == "__main__":
    client = bird.Client(HOST, PORT)

    # Add some arbitrary addresses to read from.
    # All addresses must be added before calling bird.client.connect().
    client.add_address(0x000E)
    client.add_address(0x001D)
    client.add_address_range(0x0100, 0x010A)

    client.connect()
    print("Conencting to server at {} on port {}.".format(HOST, PORT))

    close_attempt = False

    if not client.is_connected():
            print("Could not connect to external tool :[")
            close_attempt = True

    while not close_attempt:
        cnt = 0

        while not client.is_connected():
            print("Connection lost! Attempting to reconnect...")
            client.connect()

        # Will attempt to get and print the memory it receives 200 times
        # before disconnecting from the external tool
        while client.is_connected():
            memory = client.get_memory()
            print([data for data in memory])

            client.set_controller_input(right=True)
            
            # Must be called in order to send requests to the external tool 
            # and receive data collected by the external tool
            client.send_requests()

            cnt += 1

            if cnt >= 200:
                client.disconnect()
                close_attempt = True
