﻿using System;
﻿using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AutomationHelper
{
    /// <summary>
    /// Captures global keyboard events
    /// </summary>
    public class KeyboardHook : GlobalHook
    {

        #region Events

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        public event KeyPressEventHandler KeyPress;

        #endregion

        #region Constructor

        public KeyboardHook()
        {
            HookType = WhKeyboardLl;
        }

        #endregion

        #region Methods

        protected override int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
        {
            var handled = false;

            if (nCode > -1 && (KeyDown != null || KeyUp != null || KeyPress != null))
            {

                var keyboardHookStruct =
                    (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                // Is Control being held down?
                var control = ((GetKeyState(VkLcontrol) & 0x80) != 0) ||
                               ((GetKeyState(VkRcontrol) & 0x80) != 0);

                // Is Shift being held down?
                var shift = ((GetKeyState(VkLshift) & 0x80) != 0) ||
                             ((GetKeyState(VkRshift) & 0x80) != 0);

                // Is Alt being held down?
                var alt = ((GetKeyState(VkLalt) & 0x80) != 0) ||
                           ((GetKeyState(VkRalt) & 0x80) != 0);

                // Is CapsLock on?
                var capslock = (GetKeyState(VkCapital) != 0);

                // Create event using keycode and control/shift/alt values found above
                var e = new KeyEventArgs(
                    (Keys)(
                        keyboardHookStruct.vkCode |
                        (control ? (int)Keys.Control : 0) |
                        (shift ? (int)Keys.Shift : 0) |
                        (alt ? (int)Keys.Alt : 0)
                        ));

                // Handle KeyDown and KeyUp events
                switch (wParam)
                {

                    case WmKeydown:
                    case WmSyskeydown:
                        if (KeyDown != null)
                        {
                            KeyDown(this, e);
                            handled = handled || e.Handled;
                        }
                        break;
                    case WmKeyup:
                    case WmSyskeyup:
                        if (KeyUp != null)
                        {
                            KeyUp(this, e);
                            handled = handled || e.Handled;
                        }
                        break;
                }

                // Handle KeyPress event
                if (wParam == WmKeydown &&
                   !handled &&
                   !e.SuppressKeyPress &&
                    KeyPress != null)
                {
                    var keyState = new byte[256];
                    var inBuffer = new byte[2];
                    GetKeyboardState(keyState);

                    if (ToAscii(keyboardHookStruct.vkCode,
                              keyboardHookStruct.scanCode,
                              keyState,
                              inBuffer,
                              keyboardHookStruct.flags) == 1)
                    {

                        var key = (char)inBuffer[0];
                        if ((capslock ^ shift) && Char.IsLetter(key))
                            key = Char.ToUpper(key);
                        var e2 = new KeyPressEventArgs(key);
                        KeyPress(this, e2);
                        handled = handled || e.Handled;
                    }
                }
            }

            return handled ? 1 : CallNextHookEx(HandleToHook, nCode, wParam, lParam);
        }

        #endregion
    }
}
