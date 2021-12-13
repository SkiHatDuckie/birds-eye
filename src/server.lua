local socket = require("socket")
local lanes = require("lanes")

local host, port = "localhost", 8080 -- Host and port of socket


-- Get data from config.txt, including hook path and memory
-- addresses that hook needs to read
-- RETURN: Hook path, String of memory addresses
local function getConfigData()
    local handle = io.open("config.txt")
    local path = handle:read("*l")
    local mem = handle:read("*l")
    handle:close()

    return path, mem
end


-- Accept connection from hook
-- server: Socket server object
-- RETURN: Socket client object (nil if not found)
local function connectToHook(server)
    server:settimeout(10)
    local client, err = server:accept()

    if err == "timeout" then
        print("Could not connect to hook after 10 seconds")
    end

    return client
end


-- Receive message from client
-- RETURN: Message from client and socket error (nil if none)
local function getResponse(client)
    client:settimeout(1)
    local msg, err = client:receive()

    return msg, err
end


-- Send a message to hook
-- client: Socket client object
-- msg: Message to send
local function sendMessage(client, msg)
    client:send(msg.."\n")
end


-- Launch the BizHawk emulator in seperate thread
-- path: Path to find hook.lua
local launchEmuhawk = lanes.gen(
    "os",
    function(path)
        local socketIp = " --socket_ip=127.0.0.1"
        local socketPort = " --socket_port="..port
        os.execute("cd C:/BizHawk-2.4.2 && EmuHawk.exe"..socketIp..socketPort.." --Lua="..path)
    end
)


-- Check for console input in seperate thread to avoid interferance w/ sockets
-- RETURN: User input
local checkInput = lanes.gen(
  "io",
  function()
    local input = io.read("*l")
    return input
  end
)


-- Main
local function main()
    local server = assert(socket.bind("*", 8080))
    local client = nil
    local emulatorLaunched = false
    local inputThread = checkInput()
    local hookPath, memoryAddresses = getConfigData()
    local setup = false

    -- Startup
    print("=== Birds-Eye ===")
    io.write("> ")

    --  Main loop
    while true do
        -- Check if user input is ready to be processed
        if inputThread.status == "done" then
            local input = inputThread[1]

            -- Process user input
            if input == "get ip" then
                print(host)
            elseif input == "get port" then
                print(port)
            elseif input == "help" then
                print("=== Commands List ===")
                print("get ip:   get socket ip")
                print("get port: get socket port")
                print("help:     display commands list")
                print("launch:   launch Bizhawk emulator and connect hook")
                print("quit:     disconnect socket and terminate process")
            elseif input == "launch" then
                if not emulatorLaunched then
                    launchEmuhawk(hookPath)
                    emulatorLaunched = true
                end
                if not client then
                    client = connectToHook(server)
                end
            elseif input == "quit" then
                break
            else
                print("Unknown command \""..input.."\": type \"help\" for a list of commands")
            end

            -- Regenerate thread to check for input again
            inputThread = checkInput()

            -- Start new line
            io.write("> ")
        end

        -- Receive and send messages between server and hook
        if client then
            local msg, err = getResponse(client)

            if err == "closed" then
                print("Connection to hook was lost")
                client:close()
                client = nil
                emulatorLaunched = false
            elseif msg then
                print("HOOK: "..msg)
                if msg == "Hook has been connected!" then
                    setup = true
                end
            end

            if setup then
                sendMessage(client, memoryAddresses)
                setup = false
            end
        end
    end

    -- Close server and client object when finished
    if client then
        client:close()
    end
    server:close()
end


main()
