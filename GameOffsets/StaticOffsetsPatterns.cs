﻿namespace GameOffsets
{
    public struct StaticOffsetsPatterns
    {
        public static Pattern[] patterns =
        {
            ///<HowToFindIt>
            /// 1: Open CheatEngine and Attach to POE Game
            /// 2: Search for String: "InGameState", type: UTF-16 (use case insensitive if you don't find anything in the first try)
            /// 3: On all of them, "Find out what accesses this address"
            /// 4: Highlight one of the instruction
            /// 5: Go through all the registers (e.g. RAX, RBX, RCX) and one of them would be the HashNode Address which points to the InGameState Address.
            /// 5.1: To know that this is a HashNode Address make sure that Address + 0x20 points to the "InGameState" key (string)
            /// 6: Open HashNode Address in the "Dissect data/structure" window of CheatEngine program.
            /// 7: @ 0x08 is the Root HashNode. Copy that value (copy xxxx in i.e. p-> xxxxxx)
            /// 7.1: To validate that it's a real Root, 0x019 (byte) would be 1.
            /// 8: Pointer scan the value ( which is an address ) you got from step-7 with following configs
            ///     Maximum Offset Value: 1024 is enough
            ///     Maximum Level: 2 is enough, if you don't find anything increase to 3.
            ///     "Allow stack addresses of the first threads": false/unchecked
            ///     Rest: doesn't matter
            /// 9: Copy the base address and put it in your "Add address manually" button this is your InGameState Address.
            /// 10: Do "Find out what accesses this address" and make a pattern out of that function. (pick the one which has smallest offset)
            /// 
            /// ==Function start==to==10 lines after== 
            /// PathOfExile_x64.exe+149650 - 40 57                 - push rdi
            /// PathOfExile_x64.exe+149652 - 48 83 EC 40           - sub rsp,40 { 64 }
            /// PathOfExile_x64.exe+149656 - 48 C7 44 24 20 FEFFFFFF - mov qword ptr [rsp+20],FFFFFFFFFFFFFFFE { -2 }
            /// PathOfExile_x64.exe+14965F - 48 89 5C 24 58        - mov [rsp+58],rbx
            /// PathOfExile_x64.exe+149664 - 48 89 6C 24 60        - mov [rsp+60],rbp
            /// PathOfExile_x64.exe+149669 - 48 89 74 24 68        - mov [rsp+68],rsi
            /// PathOfExile_x64.exe+14966E - 48 8B F9              - mov rdi,rcx
            /// PathOfExile_x64.exe+149671 - 33 ED                 - xor ebp,ebp
            /// PathOfExile_x64.exe+149673 - 48 39 2D C680F001     - cmp [PathOfExile_x64.exe+2051740],rbp { (1F8B94DED50) } <------- Game State Address
            /// PathOfExile_x64.exe+14967A - 0F85 5F010000         - jne PathOfExile_x64.exe+1497DF
            /// PathOfExile_x64.exe+149680 - 65 48 8B 04 25 58000000  - mov rax,gs:[00000058] { 88 }
            /// PathOfExile_x64.exe+149689 - B9 28000000           - mov ecx,00000028 { 40 }
            /// PathOfExile_x64.exe+14968E - 48 8B 00              - mov rax,[rax]
            /// PathOfExile_x64.exe+149691 - 45 33 C9              - xor r9d,r9d
            /// PathOfExile_x64.exe+149694 - BA 68020000           - mov edx,00000268 { 616 }
            /// PathOfExile_x64.exe+149699 - 44 8D 45 10           - lea r8d,[rbp+10]
            /// PathOfExile_x64.exe+14969D - 0FB7 0C 01            - movzx ecx,word ptr [rcx+rax]
            /// PathOfExile_x64.exe+1496A1 - E8 EA1A0501           - call PathOfExile_x64.exe+119B190
            /// PathOfExile_x64.exe+1496A6 - 48 8B D0              - mov rdx,rax
            ///</HowToFindIt>
            new Pattern
            (
                "Game States",
                "48 83 EC ?? 48 C7 44 24 ?? ?? ?? ?? ?? 48 89 9C 24 ?? ?? ?? ?? 48 8B F9 33 ED ?? ?? ?? ^"
            ),

            ///<HowToFindIt>
            /// NOTE: This is not a good one, it might break one day.
            /// 1: Open CheatEngine and Attach to POE Game
            /// 2: Search for String: "Mods.dat", type: UTF-16 (case sensitive is fine), Writable: false/unchecked.
            /// 3: "Find out what accesses this address"
            /// 4: Highlight one of the instruction (make sure the highlighted instruction is in POE memory, not Kernel/Windows-Lib memory).
            /// 5: For each static (green) address on the register RAX, RBX and etc do the following
            /// 6: Find the value 1 in float format. It must be either before or on that green address. If not, go to step-4 and choose next instruction.
            /// 8: Valid address found in step-6, do "Find out what accesses this address" on it.
            /// 9: On this instruction do "Find out what addresses this instruction accesses"
            /// 10: Go to the game, change area/zone 2 or 3 times.
            /// 11: Go back to CheatEngine and sort all the addresses you got and pick the one which is lowest address.
            /// 12: On the step-9 instruction do "Break and trace instruction" 
            /// 12.1: Put (RSI)==0xAddress-of-step8 in the start-condition.
            /// 13 From here, find the function which loads that address.
            /// ==Function start==to==10 lines after== 
            /// PathOfExile_x64.exe+E89360 - 48 8B C4              - mov rax,rsp
            /// PathOfExile_x64.exe+E89363 - 56                    - push rsi
            /// PathOfExile_x64.exe+E89364 - 57                    - push rdi
            /// PathOfExile_x64.exe+E89365 - 41 56                 - push r14
            /// PathOfExile_x64.exe+E89367 - 48 83 EC 60           - sub rsp,60
            /// PathOfExile_x64.exe+E8936B - 48 C7 40 B8 FEFFFFFF  - mov qword ptr [rax-48],FFFFFFFFFFFFFFFE
            /// PathOfExile_x64.exe+E89373 - 48 89 58 10           - mov [rax+10],rbx
            /// PathOfExile_x64.exe+E89377 - 48 89 68 18           - mov [rax+18],rbp
            /// PathOfExile_x64.exe+E8937B - 0F29 70 D8            - movaps [rax-28],xmm6
            /// PathOfExile_x64.exe+E8937F - 0F29 78 C8            - movaps [rax-38],xmm7
            /// PathOfExile_x64.exe+E89383 - BE 70000000           - mov esi,00000070
            /// PathOfExile_x64.exe+E89388 - 65 48 8B 04 25 58000000  - mov rax,gs:[00000058]
            /// PathOfExile_x64.exe+E89391 - 48 8B 08              - mov rcx,[rax]
            /// PathOfExile_x64.exe+E89394 - 4C 8D 35 750D6901     - lea r14,[PathOfExile_x64.exe + 251A110] <---- here's the address (i.e. array-start-ptr)
            /// PathOfExile_x64.exe+E8939B - 8B 04 0E              - mov eax,[rsi + rcx]
            /// PathOfExile_x64.exe+E8939E - 39 05 640D6901        - cmp[PathOfExile_x64.exe + 251A108],eax
            /// PathOfExile_x64.exe+E893A4 - 0F8E A6010000         - jng PathOfExile_x64.exe+E89550
            /// PathOfExile_x64.exe+E893AA - 48 8D 0D 570D6901     - lea rcx, [PathOfExile_x64.exe+251A108]
            /// PathOfExile_x64.exe+E893B1 - E8 D6758F00           - call PathOfExile_x64.exe+178098C
            /// PathOfExile_x64.exe+E893B6 - 83 3D 4B0D6901 FF     - cmp dword ptr[PathOfExile_x64.exe + 251A108],-01
            /// PathOfExile_x64.exe+E893BD - 0F85 8D010000         - jne PathOfExile_x64.exe+E89550
            /// PathOfExile_x64.exe+E893C3 - 48 8D 05 A6F3FFFF     - lea rax, [PathOfExile_x64.exe+E88770]
            /// PathOfExile_x64.exe+E893CA - 48 89 44 24 20        - mov[rsp + 20], rax
            /// PathOfExile_x64.exe+E893CF - 4C 8D 0D 4A030000     - lea r9, [PathOfExile_x64.exe+E89720]
            /// PathOfExile_x64.exe+E893D6 - BA 40000000           - mov edx,00000040
            /// PathOfExile_x64.exe+E893DB - 44 8D 42 40           - lea r8d, [rdx+40]
            /// PathOfExile_x64.exe+E893DF - 49 8B CE              - mov rcx, r14
            /// PathOfExile_x64.exe+E893E2 - E8 19788F00           - call PathOfExile_x64.exe+1780C00
            /// PathOfExile_x64.exe+E893E7 - 90                    - nop
            /// PathOfExile_x64.exe+E893E8 - 48 8D 3D 212D6901     - lea rdi, [PathOfExile_x64.exe+251C110] <---- For calculating array length (i.e. array-end-ptr)
            ///</HowToFindIt>
        new Pattern
            (
                "File Root",
                "4C ?? ?? ^ ?? ?? ?? ?? 8B ?? ?? 39 ?? ?? ?? ?? ?? 0F"
            ),

            /// <HowToFindIt>
            /// This one is really simple/old school CE formula.
            /// The only purpose of this Counter is to figure out the files-loaded-in-current-area.
            /// 1: Open CheatEngine and Attach to POE Game
            /// 2: Search for 4 bytes, Search type should be "UnknownInitialValue"
            /// 3: Now Change the Area again and again and on every area change do a "Next Scan" with "Increased Value"
            /// 4: U will find 2 green addresses at the end of that search list.
            /// 5: Pick the smaller one and create pattern for it.
            /// 5.1: Normally pattern are created by "What accesses this address/value"
            /// 5.2: but in this case the pattern at "What writes to this address/value" is more unique.
            ///
            /// NOTE: Reason you picked the smaller one in step-5 is because
            ///       the bigger one is some other number which increments by 3
            ///       every time you move from Charactor Select screen to In Game screen.
            ///       Feel free to test this, just to be sure that the Address you are picking
            ///       is the correct one.
            /// </HowToFindIt>
            new Pattern
            (
                "AreaChangeCounter",
                "E8 ?? ?? ?? ?? E8 ?? ?? ?? ?? FF 05 ^"
            ),
        };
    }
}