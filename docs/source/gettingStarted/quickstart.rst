Quickstart
==========
Here is a basic example of a working birds-eye python script::

    import birdseyelib as bird

    HOST = "127.0.0.1"
    PORT = 8080

    if __name__ == "__main__":
        client = bird.Client(HOST, PORT)

        memory = bird.Memory(client)
        emulation = bird.Emulation(client)
        external_tool = bird.ExternalTool(client)

        # This will block until a connection is established.
        client.connect()
        print("Connecting to server at {} on port {}.".format(HOST, PORT))

        # Add some arbitrary addresses to read from.
        memory.add_address(0x0057)
        memory.add_address_range(0x0087, 0x008B)

        while client.is_connected():
            # Queueing requests to the external tool.
            memory.request_memory()
            emulation.request_framecount()

            # Send requests, parse responses, and advance the emulator to the next frame.
            client.advance_frame()

            print(
                "Frame:" \
                + str(emulation.get_framecount()) + ": " \
                + " ".join([
                    ":".join([str(addr), str(data)]) for addr, data in memory.get_memory().items()
                ])
            )

        print("Could not connect to external tool :[")