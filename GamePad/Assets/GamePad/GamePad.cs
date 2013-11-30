﻿/*
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
using System;
using System.Reflection;

namespace GP {

	public enum Button {
		Select,
		Start,
		System,
		L1, L2, L3,
		R1, R2, R3,
		DPadUp, DPadDown, DPadLeft, DPadRight,
		ActionA, ActionB, ActionC, ActionD
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class ButtonMapping : Attribute {
		
		public ButtonMapping(Button role, int id) {
			Role = role;
			Id = id;
		}
		
		public Button Role { get; private set; }
		public int Id { get; private set; }
	}

	/// <summary>
	/// The abstract game pad class provides method to query stick and button status.
	/// Reflection is used to gather button IDs from derived classes, caching key code 
	/// strings used to query Unity's Input class.
	/// </summary>
	public abstract class GamePad : MonoBehaviour {

		private static readonly string LeftStickHorizontalName = "Horizontal";
		private static readonly string LeftStickVerticalName = "Vertical";

		private int _joystickNumber = 0; // TODO: Support more than one pad.

		private Dictionary<Button, string> _keycodes = new Dictionary<Button, string>();

		void Start() {
			CompileInputNames();
		}

		public Vector2 GetLeftStick() {
			return new Vector2(Input.GetAxis(LeftStickHorizontalName), Input.GetAxis(LeftStickVerticalName));
		}

		public abstract Vector2 GetRightStick();

		public bool GetButtonHeld(Button button) {
			bool held = false;
			string keycode;
			if (_keycodes.TryGetValue(button, out keycode)) {
				held = Input.GetKey(keycode);
			}
			return held;
		}

		public bool GetButtonUp(Button button) {
			bool up = false;
			string keycode;
			if (_keycodes.TryGetValue(button, out keycode)) {
				up = Input.GetKeyUp(keycode);
			}
			return up;
		}

		public bool GetButtonDown(Button button) {
			bool down = false;
			string keycode;
			if (_keycodes.TryGetValue(button, out keycode)) {
				down = Input.GetKeyDown(keycode);
			}
			return down;
		}

		public static bool IsConnected(string joystickType) {
			bool connected = false;
			string[] joysticks = Input.GetJoystickNames();
			foreach(string joystick in joysticks) {
				if (joystick == joystickType) {
					connected = true;
					break;
				}
			}
			return connected;
		}

		private void CompileInputNames() {
			FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach(FieldInfo field in fields) {
				object[] attrs = field.GetCustomAttributes(typeof(ButtonMapping), false);
				foreach(object a in attrs) {
					ButtonMapping attr = a as ButtonMapping;
					string fmt;
					if (0 < _joystickNumber) {
						fmt = string.Format ("joysick {0} button {1}", _joystickNumber, attr.Id);
					} else {
						fmt = string.Format ("joystick button {0}", attr.Id);
					}
					field.SetValue(this, fmt);
					_keycodes.Add(attr.Role, fmt);
				}
			}
		}
	}

}