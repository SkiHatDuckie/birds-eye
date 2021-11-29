local send = false  -- if hook should send data to server


-- check if key in table
local function tableContains(table, key)
    return table[key] ~= nil
end


local function main()
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
end


main()
