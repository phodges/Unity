using UnityEngine;
using System.Collections.Generic;

using Attribute = System.Attribute;
using Assembly = System.Reflection.Assembly;
using Type = System.Type;

namespace GP {

    /// <summary>
    /// Convenience class providing access to available game pads.
    /// </summary>
    public static class GamePadFactory {

        private static Dictionary<SupportedPad, Type> _pads;

        public static void Initialise() {
            if (null == _pads) {
                _pads = new Dictionary<SupportedPad, Type>();

                Assembly assembly = Assembly.GetAssembly(typeof(GamePad));
                Type[] types = assembly.GetTypes();
                foreach (Type type in types) {
                    if (Attribute.IsDefined(type, typeof(SupportedPad))) {
                        object[] attrs = type.GetCustomAttributes(typeof(SupportedPad), false);
                        SupportedPad support = attrs[0] as SupportedPad;
                        _pads.Add(support, type);
                    }
                }
            }
        }

		public static void Terminate() {
			_pads = null;
		}

        public static List<Type> GetConnectedPads() {
            List<Type> connected = new List<Type>();

            foreach (var i in _pads) {
                if (IsConnected(i.Key.PadName)) {
                    connected.Add(i.Value);
                }
            }
            // We add the default pad to this list manually, so as to be sure it features last.
            connected.Add(typeof(GamePadStdAxes));

            return connected;
        }

        public static bool IsConnected(string joystickType) {
            bool connected = false;
            string[] joysticks = Input.GetJoystickNames();
            foreach (string joystick in joysticks) {
                if (joystick == joystickType) {
                    connected = true;
                    break;
                }
            }
            return connected;
        }
    }

}
