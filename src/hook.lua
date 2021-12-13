-- Check if key in table
-- table: Table to check
-- key: Key to look for
-- RETURN: True if key in table, false otherwise
local function tableContains(table, key)
    return table[key] ~= nil
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
        v = tonumber(v, 16)
        memory = memory..mainmemory.readbyte(v)..";"
    end

    return memory
end


-- Main
local function main()
    local send = false

    -- Startup
    comm.socketServerSend("Hook has been connected!\n")
    local msg = comm.socketServerResponse()
    local memoryAddresses = splitMessage(msg)
    print("setup completed")

    -- Main loop
    while true do
        local userInput = input.get()

        -- Check user input
        if tableContains(userInput, "L") then
            if send then
                send = false
            else
                send = true
            end
        end

        if send then
            -- Returns the values stored in each memeory address
            comm.socketServerSend(readMemory(memoryAddresses).."\n")
        end

        -- Next frame!
        emu.frameadvance()
    end
end


main()
