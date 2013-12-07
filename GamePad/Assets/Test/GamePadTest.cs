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
using System.Collections.Generic;

namespace GP {

	/// <summary>
	/// A simple test class used to verify game pad features.
	/// To use, create a new scene and attach this component to any object. A single test will be made
	/// when you run your scene for connection of a pad, so be sure to have it plugged in beforehand.
	/// Use the '-' and '=' buttons on the keyboard to cycle through pages of game pad state data.
	/// 
	/// Remember to configure your game pad in Unity's input manager before running this code.
	/// Once compiled, you may use the tool provided (Window --> GamePad --> Configure) found in the window menus.
	/// </summary>
	public class GamePadTest : MonoBehaviour {

		public int _joystickNumber = 0;

		private GamePad _pad;

		private int _page = 0;
		private const int _numPages = 5;

		private struct State {
			public bool _unsupported;
			public float _lastDown;
			public float _lastUp;
			public bool _isHeld;
		}
		
		private Dictionary<Button, State> _states = new Dictionary<Button, State>();

		// Use this for initialization
		void Start () {
            GamePadFactory.Initialise();
            List<System.Type> connected = GamePadFactory.GetConnectedPads();
            _pad = gameObject.AddComponent(connected[0]) as GamePad;
			_pad.Initialise(_joystickNumber);
		}
		
		// Update is called once per frame
		void Update () {
			if (Input.GetKeyUp(KeyCode.Minus)) {
				_page = (_page - 1 + _numPages) % _numPages;
			}
			if (Input.GetKeyUp(KeyCode.Equals)) {
                _page = (_page + 1) % _numPages;
            }

			if (null != _pad) {
				CollectButtonState(Button.DPadUp);
				CollectButtonState(Button.DPadDown);
				CollectButtonState(Button.DPadLeft);
				CollectButtonState(Button.DPadRight);
				CollectButtonState(Button.L1);
				CollectButtonState(Button.L2);
				CollectButtonState(Button.L3);
				CollectButtonState(Button.R1);
				CollectButtonState(Button.R2);
				CollectButtonState(Button.R3);
				CollectButtonState(Button.ActionA);
				CollectButtonState(Button.ActionB);
				CollectButtonState(Button.ActionC);
				CollectButtonState(Button.ActionD);
				CollectButtonState(Button.Select);
				CollectButtonState(Button.Start);
				CollectButtonState(Button.System);
			}
		}

		void OnGUI() {
			if (null == _pad) {
				return;
			}

			float margin = 30f;
			float width = Screen.width - margin * 2f;
			float height = Screen.height - margin * 2f;
			Rect r = new Rect(margin, margin, width, height);

			GUILayout.BeginArea(r);

			switch (_page) {
			case 0:
				GUILayout.TextField(string.Format ("LEFT = {0}", _pad.GetLeftStick()));
				GUILayout.TextField(string.Format ("RIGHT = {0}", _pad.GetRightStick()));

				ReportButtonState(Button.DPadUp);
				ReportButtonState(Button.DPadDown);
				ReportButtonState(Button.DPadLeft);
				ReportButtonState(Button.DPadRight);
				break;

			case 1:
				ReportButtonState(Button.L1);
				ReportButtonState(Button.L2);
				ReportButtonState(Button.L3);
				
				ReportButtonState(Button.R1);
				ReportButtonState(Button.R2);
	            ReportButtonState(Button.R3);
				break;

			case 2:
				ReportButtonState(Button.ActionA);
				ReportButtonState(Button.ActionB);
				ReportButtonState(Button.ActionC);
				ReportButtonState(Button.ActionD);
				break;

			case 3:
				ReportButtonState(Button.Select);
				ReportButtonState(Button.Start);
				ReportButtonState(Button.System);
				break;
                case 4:
                ListConnectedJoysticks();
                break;
			}

			GUILayout.EndArea();
		}

		void CollectButtonState(Button button) {
			State state = default(State);

			bool add = !_states.TryGetValue(button, out state);
			bool supported = _pad.IsSupported(button);

			if (supported) {
				if (_pad.GetButtonDown(button)) {
					state._lastDown = Time.time;
				}
				if (_pad.GetButtonUp (button)) {
					state._lastUp = Time.time;
				}
				state._isHeld = _pad.GetButtonHeld(button);
				state._unsupported = false;
			} else {
				state._unsupported = true;
			}

			if (add) {
				if (state._isHeld || !supported) {
					_states.Add (button, state);
				}
			} else {
				_states[button] = state;
			}
		}

		void ReportButtonState(Button button) {
			State state;
			if (_states.TryGetValue(button, out state)) {
				if (state._unsupported) {
					GUILayout.TextField(string.Format("{0}: unsupported", button));
				} else {
					GUILayout.TextField (string.Format ("{0}: {1} (last down = {2:0.00}), (last up = {3:0.00})",
				                                    button, state._isHeld, state._lastDown, state._lastUp));
				}
			} else {
				GUILayout.TextField (string.Format ("{0}: untouched", button));
			}
		}

        void ListConnectedJoysticks() {
            string[] joysticks = Input.GetJoystickNames();
            GUILayout.Label(string.Format("{0} connected devices", joysticks.Length));
            foreach (string j in joysticks) {
                GUILayout.Label(j);
            }
        }
	}

}
