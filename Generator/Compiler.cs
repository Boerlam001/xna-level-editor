using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace XleGenerator
{
    public class Compiler
    {
        bool is64BitProcess;

        public bool Is64BitProcess
        {
            get { return is64BitProcess; }
            set { is64BitProcess = value; }
        }

        bool is64BitOperatingSystem;

        public bool Is64BitOperatingSystem
        {
            get { return is64BitOperatingSystem; }
            set { is64BitOperatingSystem = value; }
        }

        string dotNetVersion;

        public string DotNetVersion
        {
            get { return dotNetVersion; }
            set { dotNetVersion = value; }
        }

        string xnaDir;

        public string XnaDir
        {
            get { return xnaDir; }
            set { xnaDir = value; }
        }

        string dotNetDir;

        public string DotNetDir
        {
            get { return dotNetDir; }
            set { dotNetDir = value; }
        }

        public Compiler()
        {
            is64BitProcess = (IntPtr.Size == 8);
            is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();
            dotNetVersion = System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion();
            if (is64BitOperatingSystem)
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);
                key = key.OpenSubKey(@"SOFTWARE\\Wow6432Node\\Microsoft\\.NETFramework\\" + dotNetVersion + "\\AssemblyFoldersEx\\Xna Framework for x86 (v4.0)\\");
                xnaDir = (string)key.GetValue("");
            }
            else
            {
                xnaDir = (string)Microsoft.Win32.Registry.LocalMachine.GetValue(@"SOFTWARE\\Microsoft\\.NETFramework\\" + dotNetVersion + "\\AssemblyFoldersEx\\Xna Framework for x86 (v4.0)\\");
            }
            dotNetDir = Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFrameworkReferenceAssemblies(Microsoft.Build.Utilities.TargetDotNetFrameworkVersion.Version40);
        }
        
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }

        public string Compile()
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters ps = new CompilerParameters();
            
            //System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            ps.ReferencedAssemblies.AddRange(new string[] {
                xnaDir + "Microsoft.Xna.Framework.dll",
                xnaDir + "Microsoft.Xna.Framework.Game.dll",
                xnaDir + "Microsoft.Xna.Framework.GamerServices.dll",
                xnaDir + "Microsoft.Xna.Framework.Graphics.dll"
            });
            CompilerResults cr = codeProvider.CompileAssemblyFromFile(ps, new string[] { "D:\\Users\\m.rap\\Documents\\Visual Studio 2010\\Projects\\XnaLevelEditor\\Model\\BaseObject.cs" });
            StringBuilder sb = new StringBuilder();
            if (cr.Errors.Count > 0)
            {
                foreach (CompilerError er in cr.Errors)
                {
                    sb.AppendLine(er.ErrorText);
                }
                return sb.ToString();
            }

            object[] obj = new object[] {cr.CompiledAssembly.GetCustomAttributes(false)};
            //ICodeParser codeParser = codeProvider.CreateParser();
            //TextReader textReader = File.OpenText("D:\\Users\\m.rap\\Documents\\Visual Studio 2010\\Projects\\XnaLevelEditor\\Model\\BaseObject.cs");
            //CodeCompileUnit unit = codeProvider.Parse(textReader);
            foreach (CodeAttributeDeclaration attr in obj)
            {
                sb.AppendLine(attr.Name);
            }
            return sb.ToString();
        }
    }
}
