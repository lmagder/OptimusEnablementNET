using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;

namespace OptimusEnablementNET
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }

        static NvOptimusEnablementExporter exporter = new NvOptimusEnablementExporter();
    }

    class NvOptimusEnablementExporter
    {
        static uint NvOptimusEnablement = 1;

        const uint PAGE_READWRITE = 0x04;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        const uint GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT = 0x2;
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetModuleHandleEx(uint dwFlags, string lpModuleName, out IntPtr phModule);

        static NvOptimusEnablementExporter()
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            IntPtr myNativeModuleHandle = IntPtr.Zero;
            if (GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT, thisAssembly.ManifestModule.Name, out myNativeModuleHandle))
            {
                IntPtr nvOptimusEnablementExportAddress = GetProcAddress(myNativeModuleHandle, "NvOptimusEnablement");
                if (nvOptimusEnablementExportAddress != IntPtr.Zero)
                {
                    uint oldProtect = 0;
                    //make it writable 
                    if (VirtualProtect(nvOptimusEnablementExportAddress, 4, PAGE_READWRITE, out oldProtect))
                    {
                        unsafe
                        {
                            uint* dwordValuePtr = (uint*)nvOptimusEnablementExportAddress.ToPointer();
                            //overwrite code that will never be called with the dword the driver is looking for
                            *dwordValuePtr = NvOptimusEnablement;
                        }
                        VirtualProtect(nvOptimusEnablementExportAddress, 4, oldProtect, out oldProtect);
                    }
                }
                else
                {
                    Console.Error.WriteLine("You didn't hack the MSIL output!");
                }
            }

        }

        //This magic name is found by regex in the post-build step
        private static void NvOptimusEnablementExporter_DontCallThis()
        {

        }
    }
}
