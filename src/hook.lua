-- this script will be executed in BizHawk lua console
local socket = require("socket")

-- create a TCP socket and bind it to the local host
local server = assert(socket.bind("*", 8080))
-- local ip, port = server:getsockname()

-- hook object and socket status
local client = nil
local socket_err = nil
local client_msg = ""


-- accept connection from client (if any)
local function findConnections()
    server:settimeout(3)
    client, socket_err = server:accept()

    if socket_err == "timeout" then
        print("Could not find client after 3 seconds")
        socket_err = nil
    end
end


--  main loop
while true do
    -- receive and send messages to client
    if client == nil then
        findConnections()
    end

    -- receive message from client
    if client then
        client:settimeout(3)
        client_msg, socket_err = client:receive()
        print(client_msg)

        if socket_err == "timeout" then
            print("No message from client after 3 seconds")
            socket_err = nil

        elseif client_msg then
            -- send message back to client
            client:send(client_msg.."\n")
        end
    end
end

-- close server and client object when finished
if client then client:close() end
server:close()
