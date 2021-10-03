local socket = require("socket")
local lanes = require("lanes")

local host, port = "localhost", 8080
local client = assert(socket.tcp())
local status = "not connected"


-- connect to hook
local function connect()
  client:connect(host, port);
end


-- check for console input in seperate thread to avoid interferance w/ sockets
local checkInput = lanes.gen(
  "io",
  function()
    local input = io.read("*l")
    return input
  end
)
local input_thread = checkInput()


-- cui startup text
print("=== Birds-Eye ===")
io.write("> ")

-- main loop
while true do
  -- check for user input is ready to be processed
  if input_thread.status == "done" then
    -- get input from thread
    local input = input_thread[1]

    --[[
      === Commands List ===
      connect:  connect to hook
      get ip:   get socket ip
      get port: get socket port
      help:     display commands list
      quit:     disconnect socket and terminate process
    ]]--
    if input == "connect" then
      if status ~= "connected" then
        connect()
        status = "connecting"
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
quit:     disconnect socket and terminate process
]]
      )

    elseif input == "quit" then
      break

    else
      print("Unknown command \""..input.."\": type \"help\" for a list of commands")
    end

    io.write("> ")

    -- regenerate thread to check for input again
    input_thread = checkInput()
  end


  if status == "connecting" then
    -- say hello to hook!
    client:send("hello!\n");
    status = "connected"
  end

  if status == "connected" then
    -- receive message from hook
    client:settimeout(3)
    local hook_msg, socket_err = client:receive()

    if hook_msg ~= nil then
      io.write("\n", "HOOK: ", hook_msg)
    end
  end
end

-- close client when finished
client:close()
