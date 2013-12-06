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

		[System.Flags]
		private enum JoystickPorts {
			AllJoysticks = 1 << 0,
			Joystick1 = 1 << 1,
			Joystick2 = 1 << 2,
			Joystick3 = 1 << 3,
			Joystick4 = 1 << 4,
			Joystick5 = 1 << 5,
			Joystick6 = 1 << 6,
			Joystick7 = 1 << 7,
			Joystick8 = 1 << 8,
			Joystick9 = 1 << 9,
			Joystick10 = 1 << 10,
			Joystick11 = 1 << 11
		};

		private Dictionary<System.Type, JoystickPorts> _portMasks = new Dictionary<System.Type, JoystickPorts>();


		[MenuItem("Window/GamePad/Configure")]
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
			InitialisePortSettings();
		}

		void OnGUI() {
			var keyCollection = _portMasks.Keys;
			System.Type[] controllers = new System.Type[keyCollection.Count];
			keyCollection.CopyTo(controllers, 0);
			foreach(var controller in controllers) {
				UpdatePortSettings(controller);
			}
		}

		void UpdatePortSettings(System.Type controllerType) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(controllerType.Name);
			JoystickPorts currentPorts = _portMasks[controllerType];
			JoystickPorts updatedPorts = (JoystickPorts)EditorGUILayout.EnumMaskField((System.Enum)currentPorts);

			JoystickPorts added = updatedPorts & ~currentPorts;
			JoystickPorts removed = currentPorts & ~updatedPorts;

			JoystickPorts lowMask = (JoystickPorts)(1 << 0);
			int i;
			if (0 != added) {
				i = 0;
				while (0 != added) {
					if (0 != (added&lowMask)) {
						AddAxis(controllerType, i);
					}
					added = (JoystickPorts) ((int)added >> 1);
					++i;
				}
			}
			if (0 != removed) {
				i = 0;
				while (0 != removed) {
					if (0 != (removed&lowMask)) {
						RemoveAxis(controllerType, i);
					}
					removed = (JoystickPorts) ((int)removed >> 1);
					++i;
				}
			}

			_portMasks[controllerType] = updatedPorts;

			EditorGUILayout.EndHorizontal();
		}

		void AddAxis(System.Type controllerType, int joystickNumber) {
			AxisData[] sourceAxes;
			if (_defaultAxes.TryGetValue(controllerType, out sourceAxes)) {
				foreach(AxisData source in sourceAxes) {
					AxisData copy = source.Clone();
					copy.Name += string.Format ("@{0}", joystickNumber);
					copy.JoystickNumber = joystickNumber;

					_axesArray.arraySize++;
					_inputManager.ApplyModifiedProperties();

					SerializedProperty added = _axesArray.GetArrayElementAtIndex(_axesArray.arraySize - 1);
					copy.WriteProperties(added);
					_inputManager.ApplyModifiedProperties();
				}
			}
		}

		void RemoveAxis(System.Type controllerType, int joystickNumber) {
			AxisData[] sourceAxes;
			if (_defaultAxes.TryGetValue(controllerType, out sourceAxes)) {
				int numAxes = _axesArray.arraySize;
				while (0 < numAxes--) {
					AxisData entry = GetAxisData(numAxes);

					bool match = false;

					foreach(AxisData source in sourceAxes) {
						if (entry.Name.StartsWith(source.Name) && entry.JoystickNumber == joystickNumber) {
							match = true;
							break;
						}
					}

					if (match) {
						_axesArray.DeleteArrayElementAtIndex(numAxes);
						_inputManager.ApplyModifiedProperties();
					}
				}
			}
		}

		void InitialisePortSettings() {
			_portMasks.Clear();
			_portMasks.Add (typeof(GamePadPS3), 0);
			_portMasks.Add (typeof(GamePadXboxTattieBogle), 0);

			if (null != _axesArray) {
				int numAxes = _axesArray.arraySize;
				for (int i=0; i<numAxes; ++i) {
					AxisData axis = GetAxisData(i);
					System.Type controller = FindControllerForAxis(axis.Name);
					if (null != controller) {
						_portMasks[controller] |= (JoystickPorts)(1 << axis.JoystickNumber);
					}
				}
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

		System.Type FindControllerForAxis(string name) {
			System.Type type = null;
			foreach(var kv in _defaultAxes) {
				AxisData[] axes = kv.Value;
				foreach(AxisData axis in axes) {
					if (name.StartsWith(axis.Name)) {
						type = kv.Key;
						break;
					}
				}
			}
			return type;
		}

		Dictionary<System.Type, AxisData[]> _defaultAxes = new Dictionary<System.Type, AxisData[]>();

		void CreateDefaultAxes() {
			CreateAxesPs3();
			CreateAxesTattieBogle();
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
			rightX.JoystickNumber = 0;	// Capture from all connected

			AxisData rightY = axes[1] = new AxisData();
			rightY.Name = GamePadPS3.RightY;
			rightY.DescriptiveName = "PS3 right vertical input";
			rightY.DeadZone = 0.19f;
			rightY.Sensitivity = 1.0f;
			rightY.Type = (int)AxisType.JoystickAxis;
			rightY.Axis = 3;
			rightY.Invert = true;		// Apparently we need to flip the right stick to make it match the left.
            rightY.JoystickNumber = 0;

			_defaultAxes.Add(typeof(GamePadPS3), axes);
		}

		void CreateAxesTattieBogle() {
			AxisData[] axes = new AxisData[4];

			AxisData rightX = axes[0] = new AxisData();
			rightX.Name = GamePadXboxTattieBogle.RightX;
			rightX.DescriptiveName = "Xbox (TB) right horizontal input";
			rightX.DeadZone = 0.19f;
			rightX.Sensitivity = 1.0f;
			rightX.Type = (int)AxisType.JoystickAxis;
			rightX.Axis = 2;
            rightX.JoystickNumber = 0;

			AxisData rightY = axes[1] = new AxisData();
			rightY.Name = GamePadXboxTattieBogle.RightY;
			rightY.DescriptiveName = "Xbox (TB) right vertical input";
			rightY.DeadZone = 0.19f;
			rightY.Sensitivity = 1.0f;
			rightY.Type = (int)AxisType.JoystickAxis;
			rightY.Axis = 3;
            rightY.JoystickNumber = 0;

			AxisData triggerL = axes[2] = new AxisData();
			triggerL.Name = GamePadXboxTattieBogle.LeftTrigger;
			triggerL.DescriptiveName = "Xbox (TB) left trigger";
			triggerL.DeadZone = 0.19f;
			triggerL.Sensitivity = 1.0f;
			triggerL.Type = (int)AxisType.JoystickAxis;
			triggerL.Axis = 4;
			triggerL.JoystickNumber = 0;

			AxisData triggerR = axes[3] = new AxisData();
			triggerR.Name = GamePadXboxTattieBogle.RightTrigger;
			triggerR.DescriptiveName = "Xbox (TB) right trigger";
			triggerR.DeadZone = 0.19f;
			triggerR.Sensitivity = 1.0f;
			triggerR.Type = (int)AxisType.JoystickAxis;
			triggerR.Axis = 5;
			rightY.Invert = true;	// An inversion is required here also.
			triggerR.JoystickNumber = 0;
            
            _defaultAxes.Add(typeof(GamePadXboxTattieBogle), axes);
        }
    }
    
}
