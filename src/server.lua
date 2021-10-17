local socket = require("socket")
local lanes = require("lanes")

-- create a TCP socket and bind it to the local host
local server = assert(socket.bind("*", 8080))
local host, port = "localhost", 8080

-- hook object and socket status
local client = nil
local socket_err = nil
local client_msg = ""
local status = "not connected"

-- emulator status
local emulator_launched = false

-- cui status
local processing_input = false


-- accept connection from client (if any)
local function findConnections()
    server:settimeout(3)
    client, socket_err = server:accept()

    if socket_err == "timeout" then
        print("Could not find hook after 3 seconds")
        socket_err = nil
    else
        status = "connected"
        processing_input = false
    end
end


-- launch the BizHawk emulator in seperate thread
local launchEmuhawk = lanes.gen(
    "os",
    function()
        local hook_path = "C:/Users/Conno/VSCodeProjects/birds-eye/src/hook.lua"
        local socket_ip = " --socket_ip=127.0.0.1"
        local socket_port = " --socket_port="..port
        os.execute("cd C:/BizHawk-2.6.2 && EmuHawk.exe"..socket_ip..socket_port.." --Lua="..hook_path)
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
local input_thread = checkInput()


-- process user input
-----------------------------------------------------
-- === Commands List ===
-- connect:  connect to hook
-- get ip:   get socket ip
-- get port: get socket port
-- help:     display commands list
-- launch:   launch Bizhawk emulator
-- quit:     disconnect socket and terminate process
-----------------------------------------------------
local function processInput(input)
    if input == "connect" then
        if status ~= "connected" then
            status = "connecting"
            processing_input = true

        else
            print("Already connected to hook")
        end

    elseif input == "get ip" then
        print(host)

    elseif input == "get port" then
        print(port)

    elseif input == "help" then
        io.write(
[[
=== Commands List ===
connect:  connect to hook
get ip:   get socket ip
get port: get socket port
help:     display commands list
launch:   launch Bizhawk emulator
quit:     disconnect socket and terminate process
]]
        )

    elseif input == "launch" then
        if not emulator_launched then
            launchEmuhawk()

        else
            print("Emulator already launched")
        end

    elseif input == "quit" then
        return "break"

    else
        print("Unknown command \""..input.."\": type \"help\" for a list of commands")
    end
end


-- cui startup text
print("=== Birds-Eye ===")
io.write("> ")

--  main loop
while true do
    -- check for user input is ready to be processed
    if input_thread.status == "done" and not processing_input then
        -- get input from thread
        local output = processInput(input_thread[1])

        if output == "break" then break end

        -- regenerate thread to check for input again
        input_thread = checkInput()

        -- start new line
        if not processing_input then io.write("> ") end
    end

    -- establish connection with client
    if status == "connecting" then
        findConnections()
    end

    -- receive message from client
    if client then
        client:settimeout(1)
        client_msg, socket_err = client:receive()

        if socket_err == "timeout" then
            print("No message from client after 3 seconds")
            socket_err = nil

        else
            print("HOOK: "..client_msg)
        end
    end
end

-- close server and client object when finished
if client then client:close() end
server:close()
