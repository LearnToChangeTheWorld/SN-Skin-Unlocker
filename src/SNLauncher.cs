using System;
using System.IO;
using System.Reflection;
using System.Text;

[assembly: AssemblyTitle("SN Skin Unlocker")]
[assembly: AssemblyDescription("SN 换肤 · 免卡密版")]
[assembly: AssemblyProduct("SN Skin Unlocker")]
[assembly: AssemblyCompany("gkyovo")]
[assembly: AssemblyCopyright("Copyright (c) 2026 gkyovo")]
[assembly: AssemblyTrademark("gkyovo")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace SnSkinUnlocker
{
    /// <summary>
    /// SN 换肤 免卡密启动器
    /// 用内置载荷密钥驱动原版授权壳内核：跳过卡密与服务器，
    /// 保留「释放客户端 → 注入 → 内存补丁」完整流程。
    /// </summary>
    internal static class Program
    {
        private const BindingFlags ALL = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        private const string AUTHOR = "gkyovo";
        private const string VERSION = "v1.0.0";

        // 从登录应答解出的载荷密钥（可用同目录 payload_key.txt 覆盖）
        private const string KEY_HEX = "0335a940305f58c223647093cd8c322929e0766b90385ab93c0147da8bc49b16";

        [STAThread]
        private static void Main(string[] args)
        {
            Banner();
            try
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                string dllPath = Path.Combine(dir, "snarl_inner.dll");
                if (!File.Exists(dllPath))
                {
                    Error("缺少 snarl_inner.dll（必须与本程序放在同一目录）");
                    return;
                }

                byte[] key = HexToBytes(KEY_HEX);
                string keyFile = Path.Combine(dir, "payload_key.txt");
                if (File.Exists(keyFile))
                {
                    byte[] k2 = HexToBytes(File.ReadAllText(keyFile).Trim());
                    if (k2 != null && k2.Length == 32) key = k2;
                }
                if (key == null || key.Length != 32)
                {
                    Error("载荷密钥无效（应为 64 位十六进制）");
                    return;
                }

                Assembly asm = Assembly.Load(File.ReadAllBytes(dllPath));
                Type prog = asm.GetType("Program");
                if (prog == null)
                {
                    Error("程序集结构异常，无法继续");
                    return;
                }

                prog.GetField("Key", ALL).SetValue(null, key);
                prog.GetField("Expire", ALL).SetValue(null, "永久");

                MethodInfo main = prog.GetMethod("Main", ALL);
                if (main == null)
                {
                    Error("未找到入口方法");
                    return;
                }

                Info("密钥已注入，正在启动主流程 ...");
                Info("请在弹窗中选择特效模式，并保持本窗口不要关闭。");
                Console.WriteLine();
                main.Invoke(null, null);
            }
            catch (TargetInvocationException tie)
            {
                Error(tie.InnerException != null ? tie.InnerException.Message : tie.Message);
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private static void Banner()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================");
            Console.WriteLine("   SN 换肤 · 免卡密版   " + VERSION);
            Console.WriteLine("   作者: " + AUTHOR);
            Console.WriteLine("================================================");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void Info(string s)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[*] " + s);
            Console.ResetColor();
        }

        private static void Error(string s)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[!] " + s);
            Console.ResetColor();
            Pause();
        }

        private static byte[] HexToBytes(string s)
        {
            if (string.IsNullOrEmpty(s)) return null;
            s = s.Trim().Replace("-", "").Replace(" ", "");
            if (s.Length != 64) return null;
            byte[] b = new byte[32];
            for (int i = 0; i < 32; i++)
                b[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
            return b;
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.Write("按回车退出 ...");
            try { Console.ReadLine(); } catch { }
        }
    }
}
