using UnityEngine;

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
/// Performs periodic raycasts in order to detect objects in front of the main camera, out to a
/// specified range and limited to the members of the required layers.
/// 
/// On detection of an object, an "OnLookEnter" message is sent to it. Similarly, when focus leaves an object
/// an "OnLookExit" message is sent.
/// </summary>
public class Selector : MonoBehaviour {

	/// <summary>
	/// Select which layers should be tested against in raycasting.
	/// </summary>
	public LayerMask layersIntersected = Physics.kDefaultRaycastLayers;
	/// <summary>
	/// Sets the range out from the main camera that raycasts should extend.
	/// </summary>
	public float range = 30f;	

	/// <summary>
	/// Track which object, if any, was last detected by raycasts.
	/// </summary>
	private GameObject lastHit = null;
	
	/// <summary>
	/// Performs a raycast each frame, dispatching appropriate messages to objects as they
	/// pass into and out of focus.
	/// It follows that in order to receive these messages, such objects should have a collision
	/// volume attached.
	/// </summary>
	void Update () {
		Vector2 centreScreen = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
		Ray ray = Camera.main.ScreenPointToRay(centreScreen);
		RaycastHit hit;

		// Use layermask to reduce the set of tested objects.
		if (Physics.Raycast(ray, out hit, range, layersIntersected)) {
			GameObject struck = hit.collider.gameObject;
			if (struck != lastHit) {
				// If tested layers were organised in such a way that it would be guaranteed that 
				// detected objects would always be capable of responding to the "OnLook" messages
				// then the message requirement could be made stricter.
				if (null != lastHit) {
					lastHit.SendMessage("OnLookExit", SendMessageOptions.DontRequireReceiver);
				}
				struck.SendMessage("OnLookEnter", SendMessageOptions.DontRequireReceiver);
				lastHit = struck;
			}
		} else {
			if (null != lastHit) {
				lastHit.SendMessage("OnLookExit", SendMessageOptions.DontRequireReceiver);
				lastHit = null;
			}
		}
	}

	#region Editor methods

	void OnDrawGizmos() {
		DrawRange();
	}

	void OnDrawGizmosSelected() {
		DrawRange();
	}

	/// <summary>
	/// Shows a ray starting from this objects location out to the limit of its raycasting range.
	/// If this script happens to be attached to the main camera then the scene view will show which
	/// object ought to be caught by the raycast test in Update.
	/// </summary>
	void DrawRange() {
		Vector3 to = transform.position + transform.forward * range;
		Gizmos.DrawLine(transform.position, to);
	}

	#endregion
}
