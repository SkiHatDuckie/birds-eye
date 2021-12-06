local socket = require("socket")
local lanes = require("lanes")

local server = assert(socket.bind("*", 8080))  -- socket server object
local host, port = "localhost", 8080           -- host and port of socket
local client = nil                             -- socket client object
local socketErr = nil                          -- any errors with socket communication are stored here
local clientMsg = ""                           -- message from client
local emulatorLaunched = false                 -- if emulator is launched or not
local processingInput = false                  -- if still processing user input
local hookPath = ""


-- reads and stores the contents of the config.txt file
local function readConfig()
    local handle = io.open("config.txt")
    hookPath = handle:read("*l")
    handle:close()
end


-- accept connection from client (if any)
local function findConnections()
    server:settimeout(10)
    client, socketErr = server:accept()

    if socketErr == "timeout" then
        print("Could not find hook after 10 seconds")
        socketErr = nil
    else
        processingInput = false
    end
end


-- launch the BizHawk emulator in seperate thread
local launchEmuhawk = lanes.gen(
    "os",
    function()
        local socketIp = " --socket_ip=127.0.0.1"
        local socketPort = " --socket_port="..port
        os.execute("cd C:/BizHawk-2.6.2 && EmuHawk.exe"..socketIp..socketPort.." --Lua="..hookPath)
    end
)


-- check for console input in seperate thread to avoid interferance w/ sockets
local checkInput = lanes.gen(
  "io",
  function()
    local input = io.read("*l")
    return input
  end
)
local inputThread = checkInput()


-- process user input
-----------------------------------------------------
-- === Commands List ===
-- get ip:   get socket ip
-- get port: get socket port
-- help:     display commands list
-- launch:   launch Bizhawk emulator and connect hook
-- quit:     disconnect socket and terminate process
-----------------------------------------------------
local function processInput(input)
    if input == "get ip" then
        print(host)
    elseif input == "get port" then
        print(port)
    elseif input == "help" then
        io.write(
[[
=== Commands List ===
get ip:   get socket ip
get port: get socket port
help:     display commands list
launch:   launch Bizhawk emulator and connect hook
quit:     disconnect socket and terminate process
]]
        )
    elseif input == "launch" then
        if not emulatorLaunched then
            processingInput = true
            launchEmuhawk()
            findConnections()
        else
            print("Emulator already launched")
        end
    elseif input == "quit" then
        return "break"
    else
        print("Unknown command \""..input.."\": type \"help\" for a list of commands")
    end
end


local function main()
    -- startup
    print("=== Birds-Eye ===")
    io.write("> ")
    readConfig()

    --  main loop
    while true do
        -- check for user input is ready to be processed
        if inputThread.status == "done" and not processingInput then
            -- get input from thread
            local output = processInput(inputThread[1])

            if output == "break" then
                break
            end

            -- regenerate thread to check for input again
            inputThread = checkInput()

            -- start new line
            if not processingInput then
                io.write("> ")
            end
        end

        -- receive message from client
        if client then
            client:settimeout(1)
            clientMsg, socketErr = client:receive()

            if socketErr == "timeout" then
                print("No message from client after 3 seconds")
                socketErr = nil
            else
                print("HOOK: "..clientMsg)
            end
        end
    end

    -- close server and client object when finished
    if client then
        client:close()
    end
    server:close()
end


main()
