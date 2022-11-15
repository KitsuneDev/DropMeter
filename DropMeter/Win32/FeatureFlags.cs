using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropMeter.Win32
{
    public enum InputForwardMode
    {
        off,
        mouse,
        mousekeyboard,
    }
    internal class FeatureFlags
    {
        internal static InputForwardMode InputMode = InputForwardMode.mousekeyboard;
        internal static bool ForwardWhenUnfocused = false;
    }
}
