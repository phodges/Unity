/*
Copyright (c) 2013, Peter Hodges
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer. 
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies, 
either expressed or implied, of the FreeBSD Project.
*/

using UnityEngine;
using System.Collections;

namespace GP {

    /// <summary>
    /// 
    /// Key mappins from http://wiki.unity3d.com/index.php?title=Xbox360Controller
    /// </summary>
    [SupportedPad(GamePadXboxPC.XboxPC)]
    public class GamePadXboxPC : GamePad {
        public const string XboxPC = "Controller (XBOX 360 For Windows)";
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
