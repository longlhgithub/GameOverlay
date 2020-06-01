﻿namespace GameOffsets
{
    using System;
    using System.Runtime.InteropServices;
    using GameOffsets.Native;

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct GameStateStaticObject
    {
        [FieldOffset(0x00)] public IntPtr GameState;
        //public fixed byte pad_0008[64];
        // 51+ StdVectors containing the content.ggpk files
        //public StdVector FilesPtr;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct GameStateObject
    {
        [FieldOffset(0x48)] public StdMap States;
    }
}