-- check if key in table
local function tableContains(table, key)
    return table[key] ~= nil
end


local function main()
    local send = false                                 -- if hook should send data to server

    -- startup
    comm.socketServerSend("Hook has been connected!\n")
    -- local serverMsg = comm.socketServerResponse()      -- msg from server (TODO)


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
            -- returns the framerate (TEMPORARY)
            comm.socketServerSend(emu.framecount().."\n")
        end

        -- next frame!
        emu.frameadvance()
    end
end


main()
