local socket = require("socket")
local host, port = "127.0.0.1", 42242

-- create a TCP socket
local tcp = assert(socket.tcp())

-- connect to server at specified host and port
tcp:connect(host, port);

-- send message to server
tcp:send("hello world\n");

while true do
    -- check connection status
    local s, status, partial = tcp:receive()
    print(s or partial)
    if status == "closed" then break end
end

tcp:close()
