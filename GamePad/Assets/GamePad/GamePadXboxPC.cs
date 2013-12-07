using UnityEngine;
using System.Collections;

namespace GP {

    /// <summary>
    /// 
    /// Key mappins from http://wiki.unity3d.com/index.php?title=Xbox360Controller
    /// </summary>
    public class GamePadXboxPC : GamePad {
        public static readonly string XboxPC = "Controller (XBOX 360 For Windows)";
        public static readonly string Trigger = XboxPC + "_Trigger";	// 3rd axis
        public static readonly string RightX = XboxPC + "_RightX";	// 4th axis
        public static readonly string RightY = XboxPC + "_RightY";	// 5th axis
        public static readonly string DpadX = XboxPC + "_DpadX";    // 6th axis
        public static readonly string DpadY = XboxPC + "_DpadY";    // 7th axis

        [ButtonMapping(Button.Select, 6)]	// Back
        string _select;

        [ButtonMapping(Button.Start, 7)]
        string _start;

        //The system button is reserved!
        //[ButtonMapping(Button.System, 15)]
        //string _system;

        [ButtonMapping(Button.L1, 4)]
        string _l1;

        [AxisToButtonMapping(Button.L2, 0)]
        string _l2;

        [ButtonMapping(Button.L3, 8)]
        string _l3;

        [ButtonMapping(Button.R1, 5)]
        string _r1;

        [AxisToButtonMapping(Button.R2, 0, true)]   // -ve range
        string _r2;

        [ButtonMapping(Button.R3, 9)]
        string _r3;

        [AxisToButtonMapping(Button.DPadUp, 4)]
        string _dpadUp;

        [AxisToButtonMapping(Button.DPadDown, 4, true)]
        string _dpadDown;

        [AxisToButtonMapping(Button.DPadLeft, 3, true)]
        string _dpadLeft;

        [AxisToButtonMapping(Button.DPadRight, 3)]
        string _dpadRight;

        [ButtonMapping(Button.ActionA, 0)] // A
        string _actionA;

        [ButtonMapping(Button.ActionB, 1)] // B
        string _actionB;

        [ButtonMapping(Button.ActionC, 2)] // X
        string _actionC;

        [ButtonMapping(Button.ActionD, 3)] // Y
        string _actionD;

        public override Vector2 GetRightStick() {
            return new Vector2(Input.GetAxis(SupportedAxes[1]), Input.GetAxis(SupportedAxes[2]));
        }

        protected override void BuildSupportedAxes() {
            SupportedAxes = new string[] {
                GetAxisName(Trigger),
				GetAxisName(RightX), GetAxisName(RightY),
				GetAxisName(DpadX), GetAxisName(DpadY)
			};
        }

        public override string[] SupportedAxes { get; protected set; }
    }

}
