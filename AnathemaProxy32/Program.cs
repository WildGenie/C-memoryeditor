﻿using AnathenaProxy;
using System;

namespace AnathenaProxy32
{
    /// <summary>
    /// Program to handle operations that are required to be run in 32 bit mode.
    /// This is needed when Ana is running in 64 bit and editing a 32 bit application
    /// </summary>
    class Program
    {
        private static ProxyService ProxyService;

        static void Main(String[] Args)
        {
            if (Args.Length < 1)
                return;

            Console.WriteLine("Initialized Anathena 32-bit helper process with channel name: " + Args[0]);
            ProxyService = new ProxyService(Args[0]);
        }

    } // End class

} // End namespace