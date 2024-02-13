import birdseyelib as bird
import time

HOST = "127.0.0.1"
PORT = 8080

if __name__ == "__main__":
    client = bird.Client(HOST, PORT)

    memory = bird.Memory(client)
    controller_input = bird.ControllerInput(client)
    emulation = bird.Emulation(client)
    external_tool = bird.ExternalTool(client)

    client.connect()
    print("Connecting to server at {} on port {}.".format(HOST, PORT))

    # Add some arbitrary addresses to read from.
    memory.add_address(0x0057)
    memory.add_address_range(0x0087, 0x008B)

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
            # Queueing requests to the external tool.
            memory.request_memory()
            controller_input.set_controller_input(right=True)
            emulation.request_framecount()
            emulation.request_board_name()
            # if cnt == 0:
            #     external_tool.set_commandeer(True)

            # Send requests, parse responses, and advance the emulator to the next frame.
            client.advance_frame()

            print(
                "Frame:" + str(emulation.get_framecount()) + ": " \
                + "Board:" + emulation.get_board_name() + ": " \
                + " ".join([
                    ":".join([str(addr), str(data)]) for addr, data in memory.get_memory().items()
                ])
            )

            cnt += 1

            # After 1000 responses are received, break from main loop and end test.
            if cnt >= 1000:
                client.close()
                close_attempt = True
