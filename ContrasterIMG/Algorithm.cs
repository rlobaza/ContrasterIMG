using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ContrasterIMG
{
    internal static class AlgorithmCpp
    {
        [DllImport("ContrasterIMG.Algorithm.Cpp.dll", EntryPoint = "changeContrast", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ChangeContrast([In, Out] byte[] pixels, int img_width, int img_height, int no_threads, double contrast, ref int progress);
    }
}
