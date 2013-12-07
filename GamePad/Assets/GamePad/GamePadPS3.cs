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
	/// Contains both strings used in setting up axes and various button key codes.
	/// </summary>
    [SupportedPad(GamePadPS3.DualShockPs3)]
	public class GamePadPS3 : GamePad {
		public const string DualShockPs3 = "Sony PLAYSTATION(R)3 Controller";
        public const string RightX = DualShockPs3 + "_RightX";
		public const string RightY = DualShockPs3 + "_RightY";

		[ButtonMapping(Button.Select, 0)]
		string _select;

		[ButtonMapping(Button.Start, 3)]
		string _start;

		[ButtonMapping(Button.System, 16)]
		string _system;

		[ButtonMapping(Button.L1, 10)]
		string _l1;

		[ButtonMapping(Button.L2, 8)]
		string _l2;

		[ButtonMapping(Button.L3, 1)]
		string _l3;

		[ButtonMapping(Button.R1, 11)]
		string _r1;

		[ButtonMapping(Button.R2, 9)]
		string _r2;

		[ButtonMapping(Button.R3, 2)]
		string _r3;

		[ButtonMapping(Button.DPadUp, 4)]
		string _dpadUp;

		[ButtonMapping(Button.DPadDown, 6)]
		string _dpadDown;

		[ButtonMapping(Button.DPadLeft, 7)]
		string _dpadLeft;

		[ButtonMapping(Button.DPadRight, 5)]
		string _dpadRight;

		[ButtonMapping(Button.ActionA, 14)] // Cross
		string _actionA;

		[ButtonMapping(Button.ActionB, 13)] // Circle
		string _actionB;

		[ButtonMapping(Button.ActionC, 15)] // Square
		string _actionC;

		[ButtonMapping(Button.ActionD, 12)] // Triangle
		string _actionD;

		public override Vector2 GetRightStick() {
			return new Vector2(Input.GetAxis(SupportedAxes[0]), Input.GetAxis(SupportedAxes[1]));
		}

		protected override void BuildSupportedAxes() {
			SupportedAxes = new string[] {
				GetAxisName(RightX), GetAxisName(RightY)
			};
		}

		public override string[] SupportedAxes { get; protected set; }
	}
}
