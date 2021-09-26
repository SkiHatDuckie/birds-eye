-- socket timeout values are in seconds

local socket = require("socket")
local lanes = require("lanes")

-- create a TCP socket and bind it to the local host
local server = assert(socket.bind("*", 8080))
local ip, port = server:getsockname()

local hook = nil
local socket_err = nil


-- wait for a connection from hook
local connect = function()
  server:settimeout(10)
  hook, socket_err = server:accept()
  if socket_err then
    print("Session timed out: No client connected after 10 seconds.")
  else
    print("Hook connected!")
  end
end


-- check for console input in seperate thread to avoid interferance w/ sockets
local input_thread = lanes.gen(
  "io",
  function()
    local input = io.read("*l")
    return input
  end
)
local check_input = input_thread()


-- launch the BizHawk emulator in seperate thread
local emuhawk_thread = lanes.gen(
  "os",
  function()
    local hook_path = "C:/Users/Conno/VSCodeProjects/birds-eye/src/hook.lua"
    os.execute("cd C:/BizHawk-2.6.2 && EmuHawk.exe --socket_ip=127.0.0.1 --socket_port="..port.." --Lua="..hook_path)
  end
)


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
      launch:   launch emulator and establish connection with hook
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
launch:   launch emulator and establish connection with hook
quit:     disconnect socket and terminate process
]]
      )

    elseif input == "launch" then
      -- launch emulator
      print("Launching Emuhawk...")
      emuhawk_thread()
      -- wait for hook to connect
      print("Waiting for connection...")
      connect()
      if socket_err then break end

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

if hook then hook:close() end
server:close()
