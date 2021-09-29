-- socket timeout values are in seconds
local socket = require("socket")
local lanes = require("lanes")

-- create a TCP socket and bind it to the local host
local server = assert(socket.bind("*", 8080))
local ip, port = server:getsockname()

-- hook object and socket status
local hook = nil
local socket_err = nil


-- check for console input in seperate thread to avoid interferance w/ sockets
local input_thread = lanes.gen(
  "io",
  function()
    local input = io.read("*l")
    return input
  end
)
local check_input = input_thread()


-- startup
print("=== Birds-Eye ===")
io.write("> ")

-- main loop
while true do
  -- check for user input is ready to be processed
  if check_input.status == "done" then
    -- get input from thread
    local input = check_input[1]

    --[[
      === Commands List ===
      get ip:   get socket ip
      get port: get socket port
      help:     display commands list
      quit:     disconnect socket and terminate process
    ]]--
    if input == "get ip" then
      print(ip)

    elseif input == "get port" then
      print(port)

    elseif input == "help" then
      io.write(
[[
=== Commands List ===
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
    check_input = input_thread()
  end
end
