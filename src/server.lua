local socket = require("socket")
local lanes = require("lanes")

-- create a TCP socket and bind it to the local host
local server = assert(socket.bind("*", 8080))
local ip, port = server:getsockname()

print("Connected to localhost on port "..port)

-- launch the BizHawk emulator in seperate thread
lanes.gen("os", function()
    os.execute("cd C:/BizHawk && EmuHawk.exe --socket_ip=127.0.0.1 --socket_port="..port.." --Lua=C:/Users/Conno/VSCodeProjects/birds-eye/src/bizhawkClient.lua") 
  end
)()

-- main loop
while true do
  -- wait for a connection from any client
  server:settimeout(10)
  local client, acceptErr = server:accept()
  if acceptErr then
    print("Session timed out: No client connected after 10 seconds.")
    break
  end

  -- receive the line
  local line, recvErr = client:receive()

  -- if there was no error, send it back to the client
  if not recvErr then client:send(line .. "\n") end

  -- done with client, close the object
  client:close()
end

server:close()
