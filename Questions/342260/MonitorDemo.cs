using UnityEngine;
using System.Collections;

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
/// Handles creation and destruction of objects, associating them with 
/// specific monitored roles. It is assumed that the object that this behaviour
/// is attached to will have access to a camera.
/// 
/// Use the B,F,G keys to create new objects and click on them with the mouse
/// to remove them from the scene.
/// </summary>
[RequireComponent(typeof(Monitor))]
public class MonitorDemo : MonoBehaviour {

	// All monitored objects are based on this prefab
	public GameObject prefab = null;

	// We require one of these to track the created objects
	private Monitor _monitor = null;

	// Use this for initialization
	void Start () {
		_monitor = GetComponent<Monitor>();	
	}
	
	// Update is called once per frame
	void Update () {
		// Make a new object of the desired type upon pressing a key
		if (Input.GetKeyDown(KeyCode.F)) {
			Make(Monitor.Role.Foo);
		}
		if (Input.GetKeyDown(KeyCode.B)) {
			Make(Monitor.Role.Bar);
		}
		if (Input.GetKeyDown(KeyCode.G)) {
			Make(Monitor.Role.Gnu);
		}

		// Click on objects to remove them from the scene
		if (Input.GetMouseButtonDown(0)) {
			Remove();
		}
	}

	/// <summary>
	/// Make a new object, associating it with a role.
	/// </summary>
	void Make(Monitor.Role role) {
		if (null != prefab) {
			Vector3 at = new Vector3(Random.value * 10f - 5f, 0.5f, Random.value * 10f - 5f);
			GameObject o = (GameObject)Instantiate(prefab, at, Quaternion.identity);
			MonitorTag tag = o.GetComponent<MonitorTag>();
			if (null != tag) {
				tag.role = role;
				_monitor.ActivateMonitor(role, o);
			}
		}
	}

	/// <summary>
	/// Click on objects in the scene to remove them.
	/// </summary>
	void Remove() {
		if (null != camera) {
			Ray r = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(r, out hit)) {
				GameObject g = hit.transform.gameObject;
				MonitorTag tag = g.GetComponent<MonitorTag>();
				if (null != tag) {
					_monitor.DeactivateMonitor(tag.role, g);
					Destroy(g);
				}
			}
		}
	}
}
