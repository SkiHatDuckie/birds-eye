-- Check if key in table
-- t: Table to check
-- key: Key to look for
-- RETURN: True if key in table, false otherwise
local function tableContains(t, key)
    return t[key] ~= nil
end


-- Split a received message into a table strings
-- msg: msg to split apart
-- RETURN: Iterable of strings
local function splitMessage(msg)
    local strTable = {}
    local k = 1
    for v in string.gmatch(msg, "[^;]+") do
        strTable[k] = v
        k = k + 1
    end
    return strTable
end


-- Read the specified memory addresses and return all values as a string
-- addresses: Memory addresses to read
-- RETURN: Concatenated string of values at each address in order of
-- index in addresses
local function readMemory(addresses)
    local memory = ""
    for k, v in pairs(addresses) do
        if v ~= "\n" then
            v = tonumber(v, 16)
            memory = memory..mainmemory.readbyte(v)..";"
        end
    end

    return memory
end


-- Switch the state of a boolean to
-- what it currently isn't
-- (i.e. true -> false, false -> true)
-- bool: Boolean to switch
local function switchBoolean(bool)
    if bool then
        return false
    else
        return true
    end
end


-- Wait for input message from server
-- This could be a message with controller inputs, or a message
-- for no input (INPUT:None)
-- This is a blocking function
-- RETURN: Received input from server
local function waitForServerInput()
    local msg = comm.socketServerResponse()
    if string.sub(msg, 1, 6) == "INPUT:" then
        return string.sub(msg, 7)
    end
end


-- Update the controller inputs with the received
-- input from the server.
-- serverInput: Input from the server
local function updateControllerInput(serverInput)
    serverInput = splitMessage(serverInput)
    local stringToBoolean = {["true"]=true, ["false"]=false}

    if serverInput[0] ~= "None" then
        joypad.set({
            A = stringToBoolean[serverInput[1]],
            B = stringToBoolean[serverInput[2]],
            Up = stringToBoolean[serverInput[3]],
            Down = stringToBoolean[serverInput[4]],
            Right = stringToBoolean[serverInput[5]],
            Left = stringToBoolean[serverInput[6]]
        })
    end
end


-- Main
local function main()
    local send = false

    -- Startup
    comm.socketServerSend("SETUP:\n")
    local msg = comm.socketServerResponse()
    print("SETUP: "..msg)
    local memoryAddresses = splitMessage(msg)
    print("setup completed")

    -- Main loop
    while true do
        local userInput = input.get()

        -- Check user input
        if tableContains(userInput, "L") then
            send = switchBoolean(send)
        end

        if send then
            -- Returns the values stored in each memeory address
            comm.socketServerSend("MEMORY:"..readMemory(memoryAddresses).."\n")
        end

        -- Next frame!
        emu.frameadvance()
    end
end


main()
