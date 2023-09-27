Quickstart
==========
Here is a basic example of a working birds-eye python script::

    import birdseyelib as bird

    HOST = "127.0.0.1"
    PORT = 8080

    if __name__ == "__main__":
        client = bird.Client(HOST, PORT)

        memory = bird.Memory()
        controller_input = bird.ControllerInput()
        emulation = bird.Emulation()
        bizhawk_objects = [memory, controller_input, emulation]

        client.connect()
        print("Conencting to server at {} on port {}.".format(HOST, PORT))

        # Add some arbitrary addresses to read from.
        memory.add_address(0x0057)
        memory.add_address_range(0x0087, 0x008B)

        while client.is_connected():
            # Sending requests to the external tool.
            memory.request_memory()
            controller_input.set_controller_input(right=True)
            emulation.request_framecount()
            client.send_requests(bizhawk_objects)

            # Processing responses from external tool.
            responses = client.get_responses()
            memory.process_responses(responses)
            emulation.process_responses(responses)

            print("Frame:" + str(emulation.framecount) + ": " \
                + " ".join([data for data in memory.get_memory()])
            )

        print("Could not connect to external tool :[")