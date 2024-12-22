import birdseyelib as bird
import time

HOST = "127.0.0.1"
PORT = 8080

if __name__ == "__main__":
    client = bird.Client(HOST, PORT)
    framecount = bird.emulation.Framecount(client)
    external_tool = bird.ExternalTool(client)
    requests = bird.RequestBatch((framecount, external_tool))

    client.connect()
    print("Connecting to server at {} on port {}.".format(HOST, PORT))

    # Set the joypad to use, and set it to hold right
    joypad = bird.SNESJoypad()
    joypad_config = bird.controllerInput.JoypadConfig(client, joypad)
    controller_input = bird.controllerInput.ControllerInputs(client, joypad)
    controller_input.joypad.controls["Right"] = True
    controller_input.queue()

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
            requests.queue_all()

            external_tool.request_commandeer()
            if cnt == 0:
                external_tool.set_commandeer(True)

            # Send requests, parse responses, and advance the emulator to the next frame.
            client.advance_frame()

            print(
                "Frame:" + str(framecount.receive()) + ": " \
                + "Commandeer: " + str(external_tool.get_commandeer()) + ": " \
                + " ".join([
                    ":".join([button, str(state)]) for button, state in joypad.controls.items()
                ])
            )

            cnt += 1

            # After 1000 responses are received, break from main loop and end test.
            if cnt >= 1000:
                client.close()
                close_attempt = True
