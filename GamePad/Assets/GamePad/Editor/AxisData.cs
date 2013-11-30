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

using System;
using System.Reflection;

namespace GP {

	public enum AxisType {
		KeyOrMouseButton = 0,
		MouseMovement = 1,
		JoystickAxis = 2
    }
    
    
    [AttributeUsage(AttributeTargets.Property)]
	public class AxisDataAttr : Attribute {

		public AxisDataAttr(string name, Type t) {
			Name = name;
			Type = t;
        }

		public string Name { get; private set; }
		public Type Type { get; private set; }
	}

	/// <summary>
	/// AxisData is used to read and write data from entries in Unity's input manager.
	/// It uses reflection to handle the retrieval of data, mapping the names used in the
	/// various properties into something more pleasing to the author.
	/// </summary>
	public class AxisData {

		[AxisDataAttr("m_Name", typeof(string))]
		public string Name { get; set; }

		[AxisDataAttr("descriptiveName", typeof(string))]
		public string DescriptiveName { get; set; }

		[AxisDataAttr("descriptiveNegativeName", typeof(string))]
		public string DescriptiveNegativeName { get; set; }

		[AxisDataAttr("negativeButton", typeof(string))]
		public string NegativeButton { get; set; }

		[AxisDataAttr("positiveButton", typeof(string))]
		public string PositiveButton { get; set; }

		[AxisDataAttr("altNegativeButton", typeof(string))]
		public string AltNegativeButton { get; set; }

		[AxisDataAttr("altPositiveButton", typeof(string))]
		public string AltPositiveButton { get; set; }

		[AxisDataAttr("gravity", typeof(float))]
		public float Gravity { get; set; }

		[AxisDataAttr("dead", typeof(float))]
		public float DeadZone { get; set; }

		[AxisDataAttr("sensitivity", typeof(float))]
		public float Sensitivity { get; set; }

		[AxisDataAttr("snap", typeof(bool))]
		public bool Snap { get; set; }

		[AxisDataAttr("invert", typeof(bool))]
		public bool Invert { get; set; }

		[AxisDataAttr("type", typeof(int))]
		public int Type { get; set; }

		[AxisDataAttr("axis", typeof(int))]
		public int Axis { get; set; }

		[AxisDataAttr("joyNum", typeof(int))]
		public int JoystickNumber { get; set; }

		public void ReadProperties(SerializedProperty axis) {
			PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach(PropertyInfo property in properties) {
				object[] attrs = property.GetCustomAttributes(typeof(AxisDataAttr), false);
				foreach(object a in attrs) {
					AxisDataAttr attr = a as AxisDataAttr;
					if (null != attr) {
						string serialName = attr.Name;
						SerializedProperty serialProperty = GetChildProperty(axis, serialName);
						if (null != serialProperty) {
							if (typeof(string) == attr.Type) {
								property.SetValue(this, serialProperty.stringValue, null);
							} else if (typeof(int) == attr.Type) {
								property.SetValue(this, serialProperty.intValue, null);
							} else if (typeof(float) == attr.Type) {
								property.SetValue(this, serialProperty.floatValue, null);
							} else if (typeof(bool) == attr.Type) {
								property.SetValue(this, serialProperty.boolValue, null);
							}
						}
                    }
                }
			}
		}

		public void WriteProperties(SerializedProperty axis) {
			PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach(PropertyInfo property in properties) {
				object[] attrs = property.GetCustomAttributes(typeof(AxisDataAttr), false);
				foreach(object a in attrs) {
					AxisDataAttr attr = a as AxisDataAttr;
                    if (null != attr) {
						string serialName = attr.Name;
						SerializedProperty serialProperty = GetChildProperty(axis, serialName);
						if (null != serialProperty) {
							if (typeof(string) == attr.Type) {
								serialProperty.stringValue = (string)property.GetValue(this, null);
							} else if (typeof(int) == attr.Type) {
								serialProperty.intValue = (int)property.GetValue(this, null);
							} else if (typeof(float) == attr.Type) {
								serialProperty.floatValue = (float)property.GetValue(this, null);
                            } else if (typeof(bool) == attr.Type) {
								serialProperty.boolValue = (bool)property.GetValue(this, null);
                            }
                        }
					}
				}
			}
		}

		private SerializedProperty GetChildProperty(SerializedProperty parent, string name) {
			SerializedProperty result = null;
			SerializedProperty child = parent.Copy();
			child.Next (true);
			do {
				if (child.name == name) {
					result = child;
                    break;
                }
            } while (child.Next (false));
            return result;
		}
	}

}
