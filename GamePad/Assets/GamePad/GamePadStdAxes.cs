using UnityEngine;
using System.Collections;

namespace GP {

	public class GamePadStdAxes : GamePad {
		private static readonly string RightX = "Mouse X";
		private static readonly string RightY = "Mouse Y";
		private static readonly string ActionA = "Fire1";
		private static readonly string ActionB = "Fire2";
		private static readonly string ActionC = "Fire3";
		private static readonly string ActionD = "Jump";

		[AxisToButtonMapping(Button.ActionA, 2)]
		string _actionA;
		
		[AxisToButtonMapping(Button.ActionB, 3)]
		string _actionB;
		
		[AxisToButtonMapping(Button.ActionC, 4)]
		string _actionC;
		
		[AxisToButtonMapping(Button.ActionD, 5)]
		string _actionD;

		public override Vector2 GetRightStick() {
			return new Vector2(Input.GetAxis(RightX), Input.GetAxis(RightY));
		}
		
		protected override void BuildSupportedAxes() {
			SupportedAxes = new string[] {
				RightX, RightY,
				ActionA, ActionB, ActionC, ActionD
			};
		}
		
		public override string[] SupportedAxes { get; protected set; }
	}

}
