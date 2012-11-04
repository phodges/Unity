using UnityEngine;
using System.Collections.Generic;
using Enum = System.Enum;

/*
Copyright (c) 2012, Peter Hodges
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

/// <summary>
/// A simple demonstration of how a set of 'roles' can be preallocated and reused,
/// using an intrusive arrangement of LinkedListNodes to handle update and removal
/// in constant time.
/// </summary>
public class Monitor : MonoBehaviour {

	/// <summary>
	/// These are the roles that we support.
	/// </summary>
	public enum Role {
		Foo = 0,
		Bar,
		Gnu
	}

	/// <summary>
	/// Maps a gameobject to a specific role. Contains some additional state that should be 
	/// reset on reuse.
	/// </summary>
	private class Element {
		private Role _role;
		private GameObject _item;
		private float _age;

		public Element(Role r) {
			_role = r;
		}

		public void Initialise(GameObject o) {
			_item = o;
			_age = 0f;
		}

		public void Update() {
			_age += Time.deltaTime;
		}

		public Role Role { get { return _role; } }
		public float Age { get { return _age; } }
		public GameObject Item { get { return _item; } }
		public LinkedListNode<Element> Node { get; set; }
	}

	// All elements, active or not, are stored here and allocated only once.
	private Element[] _allElements;
	// Only those elements which are currently active are entered into this list.
	private LinkedList<Element> _activeElements;

	// Use this for initialization
	void Start () {
		// We preallocate all of the elements and store them in a native array
		// for speed of access and to reduce memory allocation overhead during normal operation.
		int numRoles = Enum.GetNames(typeof(Role)).Length;
		_allElements = new Element[numRoles];
		for (int i = 0; i < numRoles; ++i) {
			Element toAdd = new Element((Role)i);
			toAdd.Node = new LinkedListNode<Element>(toAdd);
			_allElements[i] = toAdd;
		}
		_activeElements = new LinkedList<Element>();
	}

	// Update is called once per frame
	void Update() {
		foreach (Element e in _activeElements) {
			e.Update();
		}
	}

	/// <summary>
	/// Attempts to associate a given gameobject with a specific role.
	/// This is only permitted if the role is not already active.
	/// </summary>
	public void ActivateMonitor(Role role, GameObject o){
		// Only permit the role to be activated if it is not already active.
		Element toActivate = _allElements[(int)role];
		if (null == toActivate.Node.List) {
			// Reset any properties of the element, making it appear to be a new instance.
			toActivate.Initialise(o);
			// Then put it on the list of active elements.
			// Note that we did not allocate a new object here, nor did we perform an o(n)
			// search through a list. Addition to this active set requires no memory allocation
			// and is carried out in o(1) time.
			_activeElements.AddLast(toActivate.Node);
		}
	}

	/// <summary>
	/// Deactivates the association between a GameObject and a specific role.
	/// </summary>
	/// <param name="role"></param>
	/// <param name="g"></param>
	public void DeactivateMonitor(Role role, GameObject g) {
		Element toDeactivate = _allElements[(int)role];
		if (null != toDeactivate) {
			if (toDeactivate.Item == g) {
				// We could also trigger any desired end of role behaviours at this point.

				// Once again, o(1) time to remove these items from the active list and no
				// memory allocation or deallocation implied within the list itself.
				_activeElements.Remove(toDeactivate.Node);
			}
		}
	}

	/// <summary>
	/// A simple display method to demonstrate that the roles are functioning.
	/// </summary>
	void OnGUI() {
		foreach (Element active in _activeElements) {
			DrawElement(active);
		}
	}

	/// <summary>
	/// Displays text next to activated objects, labelling current xz position and
	/// using the age measure of the item to fade this text. The purpose behind the 
	/// fade is to demonstrate that state is being reset correctly when roles are recycled.
	/// </summary>
	/// <param name="element"></param>
	void DrawElement(Element element) {
		Vector3 pos = element.Item.transform.position;
		Vector3 at = camera.WorldToScreenPoint(pos + Vector3.up * 0.5f);
		Rect r = new Rect(at.x, at.y, 100f, 100f);
		Color c = Color.white;
		c.a = Mathf.Clamp01(element.Age);
		GUI.color = c;
		GUILayout.BeginArea(r);
		GUILayout.Label(string.Format("{0}@ {1:0.00}, {2:0.00}", element.Role, pos.x, pos.z));
		GUILayout.EndArea();
	}
}
