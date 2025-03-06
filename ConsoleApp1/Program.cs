
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;

class Program
{
    // Windows API 선언
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

    [DllImport("user32.dll")]
    static extern IntPtr GetKeyboardLayout(uint idThread);

    [DllImport("user32.dll")]
    static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

    [DllImport("user32.dll")]
    static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
    const uint KLF_ACTIVATE = 0x00000001;

    // 현재 입력기 확인
    static int GetInputMethod()
    {
        IntPtr hwnd = GetForegroundWindow();
        if (hwnd != IntPtr.Zero)
        {
            uint threadID = GetWindowThreadProcessId(hwnd, IntPtr.Zero);
            IntPtr layout = GetKeyboardLayout(threadID);
            return layout.ToInt32() & 0xFFFF;
        }
        return 0;
    }

    // 입력기 변경 (강제 적용)
    static void SwitchInputMethod(int locale)
    {
        IntPtr hwnd = GetForegroundWindow();
        string localeHex = locale.ToString("X8"); // 16진수 문자열 변환 (8자리)
        IntPtr hkl = LoadKeyboardLayout(localeHex, KLF_ACTIVATE); // IME 강제 로드
        if (hkl != IntPtr.Zero)
        {
            SendMessage(hwnd, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, hkl); // SendMessage로 전환
        }
        else
        {
            Console.WriteLine($"입력기 변경 실패: {localeHex}");
        }
    }

    // 현재 설치된 IME 목록 조회 (Windows Registry 접근)
    static Dictionary<int, string> GetInstalledIMEs()
    {
        Dictionary<int, string> imeList = new Dictionary<int, string>();
        string registryPath = @"SYSTEM\CurrentControlSet\Control\Keyboard Layouts";

        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
        {
            if (key != null)
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using (RegistryKey subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey != null)
                        {
                            string layoutText = subKey.GetValue("Layout Text") as string;
                            if (!string.IsNullOrEmpty(layoutText))
                            {
                                int layoutID = int.Parse(subKeyName, NumberStyles.HexNumber);
                                imeList[layoutID] = layoutText;
                            }
                        }
                    }
                }
            }
        }
        return imeList;
    }

    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            // 현재 입력기 확인
            int imID = GetInputMethod();
            Console.WriteLine($"현재 입력기: {imID:X} (0x{imID:X})");
        }
        else if (args[0] == "-l")
        {
            // 설치된 입력기 목록 출력
            Dictionary<int, string> imeList = GetInstalledIMEs();
            Console.WriteLine("설치된 입력기 목록:");
            foreach (var ime in imeList)
            {
                Console.WriteLine($"0x{ime.Key:X}: {ime.Value}");
            }
        }
        else
        {
            // 특정 입력기로 변경
            if (int.TryParse(args[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int locale))
            {
                SwitchInputMethod(locale);
                Console.WriteLine($"입력기 변경 완료: 0x{locale:X}");
            }
            else
            {
                Console.WriteLine("잘못된 입력기 ID입니다. 16진수 값으로 입력하세요.");
            }
        }
    }
}
