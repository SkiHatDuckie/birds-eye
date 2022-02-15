import birdseye as bird

# External tool connects to this address and port 
# TODO: Add way in external tool to set port and address
HOST = "127.0.0.1"
PORT = 8080


def main():
    quit_attempt = False

    if not client.is_connected():
        print("Could not connect to external tool :[")
        quit_attempt = True

    while not quit_attempt:
        print(client.get_memory())


if __name__ == "__main__":
    client = bird.Client(HOST, PORT)

    # Add some arbitrary addresses to read from.
    # All addresses must be added before calling bird.comm.start().
    client.add_address(0x000E)
    client.add_address(0x001D)
    client.add_address_range(0x0100, 0x010A)
    
    client.connect()
    print("Conencting to server at {} on port {}.".format(HOST, PORT))

    main()