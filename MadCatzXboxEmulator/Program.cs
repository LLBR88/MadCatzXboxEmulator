// See https://aka.ms/new-console-template for more information

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using HidSharp.Experimental;
using HidSharp.Reports;
using HidSharp.Reports.Input;
using HidSharp.Reports.Encodings;
using HidSharp.Utility;
using HidSharp;
using Nefarius.ViGEm.Client;
using System.Runtime.InteropServices;
using Nefarius.ViGEm.Client.Targets;
using System.Linq.Expressions;
using Nefarius.ViGEm.Client.Targets.Xbox360;


public class mEmulator
{
    private static System.Timers.Timer TimerRI;
    private static System.Timers.Timer TimerVX;
    static int ProID = 23907;
    static int VenID = 1848;
    static string UsageID = "GenericDesktopGamepad";
    static int[] btns = { 0, 1, 3, 4, 6, 7, 11, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
    static int[] vals = new int[22];
    static bool[,] hats = new bool[9, 4] { { false, false, false, false }, { true, false, false, false }, { true, false, true, false }, { false, false, true, false }, { false, true, true, false }, { false, true, false, false }, { false, true, false, true }, { false, false, false, true }, { true, false, false, true } };
    static bool mcFound;
    static bool mcActive;
    static string dPaths;
    static HidDeviceInputReceiver inputReceiver;
    static DeviceItemInputParser inputParser;
    static HidStream hidStream;
    static byte[] inputReportBuffer = new byte[16];

    static class mEmu
    {
        public static ViGEmClient client;
        public static HidDevice MCPad;
        public static IXbox360Controller MCV;
    }


    public static void Main()
    {
        mEmu.client = new ViGEmClient();
        Console.WriteLine("Starting Emulator...");
        mDeviceTimer();
        Console.ReadLine();
    }

    private static void mDeviceTimer()
    {
        // Create a timer with a two second interval.
        TimerRI = new System.Timers.Timer(3000);
        // Hook up the Elapsed event for the timer. 
        TimerRI.Elapsed += mDeviceScanner;
        TimerRI.AutoReset = true;
        TimerRI.Enabled = true;
        TimerRI.Start();
    }

    private static void mRITimer()
    {
        // Create a timer with a two second interval.
        TimerVX = new System.Timers.Timer(5);
        // Hook up the Elapsed event for the timer. 
        TimerVX.Elapsed += mUpdate;
        TimerVX.AutoReset = true;
        TimerVX.Enabled = true;
        TimerVX.Start();
    }

    private static void mDeviceScanner(Object source, ElapsedEventArgs e)
    {
        mcFound = false;
        var alldevs = HidSharp.DeviceList.Local.GetHidDevices().ToArray();
        foreach (HidSharp.HidDevice x in alldevs)
        {
            if (x == mEmu.MCPad) 
            { 
                mcFound = true;
            }
            else
            {
                try
                {
                    if (x.ProductID == ProID && x.VendorID == VenID)
                    {
                        ReportDescriptor reportDescriptor = x.GetReportDescriptor();
                        DeviceItem deviceItem = reportDescriptor.DeviceItems[0];
                        bool usageX = false;
                        foreach (var usge in deviceItem.Usages.GetAllValues())
                        {
                            usageX = false;
                            if (((Usage)usge).ToString() == UsageID) { usageX = true; break; }
                        }
                        if (usageX)
                        {
                            dPaths = x.DevicePath;
                            mEmu.MCPad = x;
                            mEmu.MCV = mEmu.client.CreateXbox360Controller();
                            mEmu.MCV.Connect();
                            Console.WriteLine("Pad created");
                            inputReceiver = reportDescriptor.CreateHidDeviceInputReceiver();
                            inputParser = deviceItem.CreateDeviceItemInputParser();
                            x.TryOpen(out hidStream);
                            hidStream.ReadTimeout = Timeout.Infinite;
                            inputReceiver.Start(hidStream);
                            mRITimer();
                            mcFound = true;
                        }
                    }
                }
                catch { }
            }
        }
        if (mcFound == false && mEmu.MCPad != null)
        {
            mEmu.MCPad = null;
            mEmu.MCV.Disconnect();
            TimerVX.Stop();
            Console.WriteLine("Pad disconnected");
        }
    }

    private static void mUpdate(Object source, ElapsedEventArgs e)
    {
        HidDevice q = mEmu.MCPad;
        if (q!= null)
        {
            if (!inputReceiver.IsRunning) { TimerVX.Stop(); } // Disconnected?

            Report report;
            while (inputReceiver.TryRead(inputReportBuffer, 0, out report))
            {
                // Parse the report if possible.
                // This will return false if (for example) the report applies to a different DeviceItem.
                if (inputParser.TryParseReport(inputReportBuffer, 0, report))
                {
                    foreach (int xB in btns)
                    {
                        vals[xB] = inputParser.GetValue(xB).GetLogicalValue();
                        SendCommand(mEmu.MCV, xB, vals[xB]);
                    }
                }
            }
        }

    }
    static void SendCommand(IXbox360Controller vpad, int vbut, int vval)
    {
        switch (vbut)
        {
            case 0:
                vpad.SetButtonState(Xbox360Button.A, buttobool(vval)); break;
            case 1:
                vpad.SetButtonState(Xbox360Button.B, buttobool(vval)); break;
            case 3:
                vpad.SetButtonState(Xbox360Button.X, buttobool(vval)); break;
            case 4:
                vpad.SetButtonState(Xbox360Button.Y, buttobool(vval)); break;
            case 6:
                vpad.SetButtonState(Xbox360Button.LeftShoulder, buttobool(vval)); break;
            case 7:
                vpad.SetButtonState(Xbox360Button.RightShoulder, buttobool(vval)); break;
            case 11:
                vpad.SetButtonState(Xbox360Button.Start, buttobool(vval)); break;
            case 13:
                vpad.SetButtonState(Xbox360Button.LeftThumb, buttobool(vval)); break;
            case 14:
                vpad.SetButtonState(Xbox360Button.RightThumb, buttobool(vval)); break;
            case 15:
                vpad.SetButtonState(Xbox360Button.Up, hattobut(vval, 0));
                vpad.SetButtonState(Xbox360Button.Down, hattobut(vval, 1));
                vpad.SetButtonState(Xbox360Button.Right, hattobut(vval, 2));
                vpad.SetButtonState(Xbox360Button.Left, hattobut(vval, 3)); break;
            case 16:
                vpad.SetAxisValue(Xbox360Axis.LeftThumbX, inttoaxis(vval)); break;
            case 17:
                vpad.SetAxisValue(Xbox360Axis.LeftThumbY, invtoaxis(vval)); break;
            case 18:
                vpad.SetAxisValue(Xbox360Axis.RightThumbX, inttoaxis(vval)); break;
            case 19:
                vpad.SetAxisValue(Xbox360Axis.RightThumbY, invtoaxis(vval)); break;
            case 20:
                vpad.SetSliderValue(Xbox360Slider.RightTrigger, inttotrigger(vval)); break;
            case 21:
                vpad.SetSliderValue(Xbox360Slider.LeftTrigger, inttotrigger(vval)); break;
        }
    }

    static bool buttobool(int butval)
    {
        if (butval == 1)
        {
            return true;
        }
        else if (butval == 0)
        {
            return false;
        }
        else { return false; }
    }

    static short inttoaxis(int axval)
    {
        short ax = (short)(axval - 32768);
        return ax;
    }

    static short invtoaxis(int axval)
    {
        short ax = (short)(-1 * (axval - 32767));
        return ax;
    }


    static byte inttotrigger(int trigval)
    {
        byte at = (byte)((255 * trigval) / 65536);
        return at;
    }

    static bool hattobut(int hatval, int dir)
    {
        return hats[hatval, dir];
    }
}
