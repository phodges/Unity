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
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace GP {

	/// <summary>
	/// This editor tool is intended to make it easier to add axis data for a given game pad
	/// to Unity's input manager.
	/// </summary>
	public class GamePadEd : EditorWindow {

		private SerializedObject _inputManager;
		private SerializedProperty _axesArray;

		[MenuItem("GamePad/Configure")]
		static void ShowGamePadEd() {
			GamePadEd window = GamePadEd.GetWindow<GamePadEd>();
			window.Show ();
		}

		void OnEnable() {
			if (0 == _defaultAxes.Count) {
				CreateDefaultAxes();
            }
			if (null == _inputManager) {
				_inputManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
				_axesArray = _inputManager.FindProperty("m_Axes");
			}
		}

		void OnGUI() {
			string[] joysticks = Input.GetJoystickNames();
			int numSticks = null != joysticks ? joysticks.Length : 0;
			EditorGUILayout.LabelField(string.Format ("num sticks = {0}", numSticks));
			for (int i=0; i<numSticks; ++i) {
				UpdateAxisSetup(joysticks[i]);
			}
		}

		AxisData GetAxisData(int index) {
			AxisData data = null;
			SerializedProperty property = null;
			if (null != _axesArray && _axesArray.arraySize > index) {
				property = _axesArray.GetArrayElementAtIndex(index);
			}
			if (null != property) {
				data = new AxisData();
				data.ReadProperties(property);
			}
			return data;
		}

		void UpdateAxisSetup(string joystick) {
			if (AxesExist(joystick)) {
				EditorGUILayout.LabelField(string.Format ("'{0}' axes setup ok", joystick));
			} else if (IsSupportedJoystick(joystick)) {
				if (GUILayout.Button (string.Format ("Configure '{0}'", joystick))) {
					ConfigureAxes(joystick);
				}
			} else {
				EditorGUILayout.LabelField(string.Format ("'{0}' unsupported", joystick));
			}
		}

		bool AxisExists(string name) {
			bool exists = false;
			int totalAxes = _axesArray.arraySize;
			for (int i=0; i<totalAxes; ++i) {
				AxisData storedAxis = GetAxisData(i);
				if (storedAxis.Name == name) {
					exists = true;
					break;
				}
            }
			return exists;
		}

		bool AxesExist(string joystick) {
			bool ok = false;
			AxisData[] axes = null;
			if (_defaultAxes.TryGetValue(joystick, out axes)) {
				ok = true;
				foreach(AxisData axis in axes) {
					if (!AxisExists(axis.Name)) {
						ok = false;
						break;
					}
				}
			} else {
				ok = false;
			}
			return ok;
		}

		bool IsSupportedJoystick(string joystick) {
			return _defaultAxes.ContainsKey(joystick);
		}

		void ConfigureAxes(string joystick) {
			AxisData[] axes = null;
			if (_defaultAxes.TryGetValue(joystick, out axes)) {
				foreach(AxisData axis in axes) {
					if (!AxisExists(axis.Name)) {
						++_axesArray.arraySize;
						_inputManager.ApplyModifiedProperties();

						SerializedProperty toAdd = _axesArray.GetArrayElementAtIndex(_axesArray.arraySize - 1);
						axis.WriteProperties(toAdd);
						_inputManager.ApplyModifiedProperties();
					}
				}
			}
		}

		Dictionary<string, AxisData[]> _defaultAxes = new Dictionary<string, AxisData[]>();

		void CreateDefaultAxes() {
			CreateAxesPs3();
		}

		void CreateAxesPs3() {
			AxisData[] axes = new AxisData[2];

			AxisData rightX = axes[0] = new AxisData();
			rightX.Name = GamePadPS3.RightX;
			rightX.DescriptiveName = "PS3 right horizontal input";
			rightX.DeadZone = 0.19f;
			rightX.Sensitivity = 1.0f;
			rightX.Type = (int)AxisType.JoystickAxis;
			rightX.Axis = 2;
			rightX.JoystickNumber = 0;	// XXX: Capture from all connected

			AxisData rightY = axes[1] = new AxisData();
			rightY.Name = GamePadPS3.RightY;
			rightY.DescriptiveName = "PS3 right vertical input";
			rightY.DeadZone = 0.19f;
			rightY.Sensitivity = 1.0f;
			rightY.Type = (int)AxisType.JoystickAxis;
			rightY.Axis = 3;
			rightY.Invert = true;		// Apparently we need to flip the right stick to make it match the left.
            rightY.JoystickNumber = 0;

			_defaultAxes.Add(GamePadPS3.DualShockPs3, axes);
		}
	}

}
