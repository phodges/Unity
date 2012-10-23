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
/// Smooth attraction and repelusion of one object to the transform referred to by the owner of this component.
/// The magnet's own transform is updated to maintain a constant offset from a reference target, rotating around this
/// target when in 'Steering' mode.
/// Attraction begins when mouse button 1 is held down, steering permitted once the attracted object is comes within
/// range of the magnet. When the mouse button is released, this object will be repelled back to its original position and
/// orientation.
/// </summary>
public class Magnet : MonoBehaviour {

  #region Editable properties
  public Transform _attract = null;
  public Transform _target = null;

  public Vector3 _targetOffset = new Vector3(-1f,2.5f,-5f);
  public float _controlSensitivity = 625f;

  public float _attractSpeed = 10f;
  public float _attractRotationSpeed = 360f;

  public float _repelSpeed = 10f;
  public float _repelRotationSpeed = 360f;

  #endregion // Editable properties

  enum Mode {
	  Idle,
	  Attracting,
	  Steering,
	  Repelling
  }
  Mode _mode;

	private Vector3 _attractInitialPos;
	private Quaternion _attractInitialRot;
	private float _angle;

	// Use this for initialization
	void Start () {
		_angle = transform.eulerAngles.y;
		_mode = Mode.Idle;
	}
	
	// Update is called once per frame
	void Update () {
		if (null == _target || null == _attract) {
	      // Nothing to do here
	      return;
	    }
		UpdateTransform();
		bool click = Input.GetMouseButton(1);
		switch(_mode) {
		case Mode.Idle: UpdateIdle(click); break;
		case Mode.Attracting: UpdateAttracting(click); break;
		case Mode.Steering: UpdateSteering(click); break;
		case Mode.Repelling: UpdateRepelling(click); break;
		}
	}

	void OnGUI()
	{
		Rect r = new Rect(20f,20f,200f,200f);
		GUILayout.BeginArea(r);
		GUILayout.Label(string.Format("Mode: {0}",_mode));
		GUILayout.Label(string.Format("Angle = {0:0.00}",_angle));
		GUILayout.EndArea();
	}

	void UpdateTransform()
	{
		transform.rotation = Quaternion.Euler(0f, _angle, 0f);
		transform.position = transform.rotation * _targetOffset + _target.position;
	}

	void UpdateIdle(bool click)
	{
		if (click) {
			_mode = Mode.Attracting;
			_attractInitialPos = _attract.transform.position;
			_attractInitialRot = _attract.transform.rotation;
		}
	}

	bool Move(Vector3 target, float speed)
	{
		bool close = false;
		float maxDistance = speed * Time.deltaTime;
		Vector3 r = target - _attract.transform.position;
		if (r.sqrMagnitude <= maxDistance * maxDistance) {
			_attract.transform.position = target;
			close = true;
		} else {
			r = r.normalized * maxDistance;
			_attract.transform.position += r;
		}
		return close;
	}

	bool Rotate(Quaternion target, float speed)
	{
		bool close = false;
		float maxAngle = speed * Time.deltaTime;
		float angle = Quaternion.Angle(target, _attract.transform.rotation);
		if (angle <= maxAngle) {
			_attract.transform.rotation = target;
			close = true;
		} else {
			_attract.transform.rotation = Quaternion.Slerp(_attract.transform.rotation, target, maxAngle / angle);
		}
		return close;
	}

	void UpdateAttracting(bool click)
	{
		if (click) {
			bool close = Move(transform.position, _attractSpeed);
			close &= Rotate(transform.rotation, _attractRotationSpeed);
			if (close) {
				_mode = Mode.Steering;
			}
		} else {
			_mode = Mode.Repelling;
		}
	}

	void UpdateSteering(bool click)
	{
		if (click) {
			_angle += Input.GetAxis("Mouse X") * _controlSensitivity * Time.deltaTime;
			_attract.transform.rotation = transform.rotation;
			_attract.transform.position = transform.position;
		} else {
			_mode = Mode.Repelling;
		}
	}

	void UpdateRepelling(bool click)
	{
		if (click) {
			_mode = Mode.Attracting;
		} else {
			bool finished = Move(_attractInitialPos,_repelSpeed);
			finished &= Rotate(_attractInitialRot, _repelRotationSpeed);
			if (finished) {
				_mode = Mode.Idle;
			}
		}
	}
}
