import birdseye as bird
import time

# External tool connects to this address and port 
# TODO: Add way in external tool to set port and address
HOST = "127.0.0.1"
PORT = 8081


if __name__ == "__main__":
    client = bird.Client(HOST, PORT)

    memory = bird.Memory()
    controller_input = bird.ControllerInput()
    emulation = bird.Emulation()
    external_tool = bird.ExternalTool()
    bizhawk_objects = [memory, controller_input, emulation, external_tool]

    client.connect()
    print("Conencting to server at {} on port {}.".format(HOST, PORT))

    # Add some arbitrary addresses to read from.
    memory.add_address(0x0002)
    memory.add_address(0x0032)
    memory.add_address_range(0x0150, 0x015A)

    close_attempt = False
    if not client.is_connected():
            print("Could not connect to external tool :[")
            close_attempt = True

    while not close_attempt:
        cnt = 0

        # If connection lost, attempt to reconnect every 10 seconds.
        while not client.is_connected():
            print("Connection lost! Attempting to reconnect in 10 seconds...")
            time.sleep(10)
            client.connect()

        while client.is_connected():
            # Sending requests to the external tool.
            memory.request_memory()
            controller_input.set_controller_input(right=True)
            emulation.request_framecount()
            if cnt == 0:
                external_tool.set_commandeer(True)
            client.send_requests(bizhawk_objects)

            # Processing responses from external tool.
            responses = client.get_responses()
            memory.process_responses(responses)
            emulation.process_responses(responses)

            print("Frame:" + str(emulation.framecount) + ": " \
                  + " ".join([data for data in memory.get_memory()])
            )

            cnt += 1

            # After 1000 responses are received, break from main loop and end test.
            if cnt >= 1000:
                client.close()
                close_attempt = True
