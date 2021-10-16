local socket = require("socket")
local lanes = require("lanes")

local host, port = "localhost", 8080
local client = assert(socket.tcp())
local status = "not connected"

local processing_input = false


-- connect to hook
local function connect()
  print("Connecting to hook...")
  client:connect(host, port);
  status = "connecting"
  processing_input = true
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


-- process user input
-----------------------------------------------------
-- === Commands List ===
-- connect:  connect to hook
-- get ip:   get socket ip
-- get port: get socket port
-- help:     display commands list
-- quit:     disconnect socket and terminate process
-----------------------------------------------------
local function processInput(input)
  if input == "connect" then
    if status ~= "connected" then
      connect()
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
    return "break"

  else
    print("Unknown command \""..input.."\": type \"help\" for a list of commands")
  end
end


-- cui startup text
print("=== Birds-Eye ===")
io.write("> ")

-- main loop
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

  if status == "connected" or status == "connecting" then
    -- receive message from hook
    client:settimeout(1)
    local hook_msg, socket_err = client:receive()

    if hook_msg == "hello!" then
      print("Connected!")
      io.write("> ")
      status = "connected"
      processing_input = false
    else
      print(hook_msg)  -- TODO: store received framecount data in table
    end

    -- send messages to hook
    if status ~= "connecting" then
      client:send("state?\n")
    else
      client:send("hello!\n")
    end
  end
end

-- close client when finished
if client then client:close() end
