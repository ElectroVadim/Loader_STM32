using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;
using bootloader_Csharp;

namespace bootloader_Csharp
{
      class Program
    {
        static void Main(string[] args)
        {
            FlashLoaderSTM32 Loader = new FlashLoaderSTM32(@"c:\\file.bin", "COM3", 9600);

            Loader.Loader_start();
        }
        
        
    }



}

