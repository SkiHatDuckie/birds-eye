# TODO: Add exception handling
import socketserver
import threading
import os

# Host and port of socket
HOST, PORT = "localhost", 8080


# Get data from config.txt
# Data returned (in order):
# lineIndex: The line from config.txt requested to be returned
def getConfigData(lineIndex):
    with open("devconfig.txt", "r") as f:
        for i, line in enumerate(f):
            if i == lineIndex:
                return line


# Launch the BizHawk emulator in seperate thread
# path: Path to find hook.lua
def launchEmuhawk(path):
    print("Launching emulator")
    socketIp = " --socket_ip=127.0.0.1"
    socketPort = " --socket_port=" + str(PORT)
    os.system("cd C:/BizHawk-2.4.2 && EmuHawk.exe" + socketIp + socketPort + " --Lua=" + path)


# Request handler for the TCP server
# self.rfile: A file-like object that stores requests from the client
# self.wfile: A file-like object that sends data back to the client
class TCPHandler(socketserver.StreamRequestHandler):
    def handle(self):
        memory = getConfigData(1)

        print("Connected to hook")
        for line in self.rfile:
            message = str(line.strip(), "utf-8")

            if message.startswith("SETUP:"):
                self.wfile.write(memory.encode())
            elif message.startswith("MEMORY:"):
                print("HOOK: " + message[7:])


# Main
def main():
    hookPath = getConfigData(0)

    print("=== Birds-Eye ===")

    emuThread = threading.Thread(target=launchEmuhawk, args=(hookPath,))
    emuThread.start()

    server = socketserver.TCPServer((HOST, PORT), TCPHandler)
    server.serve_forever()


if __name__ == "__main__":
    main()
