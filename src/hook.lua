local send = false


-- check if key in table
local function tableContains(table, key)
    return table[key] ~= nil
end


-- main loop
while true do
    local userInput = input.get()

    -- check user input
    if tableContains(userInput, "Number0") then
        if send then
            send = false
        else
            send = true
        end
    end

    if send then
        -- returns the framerate
        comm.socketServerSend(emu.framecount().."\n")
    end

    -- next frame!
    emu.frameadvance()
end
