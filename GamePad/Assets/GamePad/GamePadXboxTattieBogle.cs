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
	/// Allows for the use of an Xbox360 controller on OSX, using the driver
	/// created by Tattie Bogle
	/// http://tattiebogle.net/index.php/ProjectRoot/Xbox360Controller/OsxDriver
	/// </summary>
	public class GamePadXboxTattieBogle : GamePad {
		public static readonly string XboxTattieBogle = ""; // XXX: InputManager retrieves no name. This is weird and should be investigated.
		public static readonly string RightX = "TattieBogle_RightX";	// 3rd axis
		public static readonly string RightY = "TattieBogle_RightY";	// 4th axis
		public static readonly string LeftTrigger = "TattieBogle_LeftTrigger";	// 5th axis
		public static readonly string RightTrigger = "TattieBogle_RightTrigger"; // 6th axis

		[ButtonMapping(Button.Select, 10)]	// Back
		string _select;
		
		[ButtonMapping(Button.Start, 9)]
		string _start;
		
		[ButtonMapping(Button.System, 15)]
		string _system;
		
		[ButtonMapping(Button.L1, 13)]
		string _l1;

		[AxisToButtonMapping(Button.L2, 2)]
		string _l2;
		
		[ButtonMapping(Button.L3, 11)]
		string _l3;
		
		[ButtonMapping(Button.R1, 14)]
		string _r1;

		[AxisToButtonMapping(Button.R2, 3)]
		string _r2;
		
		[ButtonMapping(Button.R3, 12)]
		string _r3;
		
		[ButtonMapping(Button.DPadUp, 5)]
		string _dpadUp;
		
		[ButtonMapping(Button.DPadDown, 6)]
		string _dpadDown;
		
		[ButtonMapping(Button.DPadLeft, 7)]
		string _dpadLeft;
		
		[ButtonMapping(Button.DPadRight, 8)]
		string _dpadRight;
		
		[ButtonMapping(Button.ActionA, 16)] // A
		string _actionA;
		
		[ButtonMapping(Button.ActionB, 17)] // B
		string _actionB;
		
		[ButtonMapping(Button.ActionC, 18)] // X
		string _actionC;
		
		[ButtonMapping(Button.ActionD, 19)] // Y
		string _actionD;

		public override Vector2 GetRightStick() {
			return new Vector2(Input.GetAxis(SupportedAxes[0]), Input.GetAxis(SupportedAxes[1]));
		}
		
		protected override void BuildSupportedAxes() {
			SupportedAxes = new string[] {
				GetAxisName(RightX), GetAxisName(RightY),
				GetAxisName(LeftTrigger), GetAxisName(RightTrigger)
			};
		}
		
		public override string[] SupportedAxes { get; protected set; }

	}
}
