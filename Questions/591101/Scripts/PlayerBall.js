#pragma strict

var target : GameObject;

var speedX : float = 1.0f / 10.0f;
var speedScaleZ : float = 4.0f;
var motionScale : float = 10.0f;

var lineRender : LineRenderer;

function Start () {
	lineRender = GetComponent(LineRenderer);
}

function Update () {
	var position = transform.position;
	position.x = Mathf.Sin(speedX * Time.time) * motionScale;
	position.z = Mathf.Sin(speedX * speedScaleZ * Time.time) * motionScale;
	transform.position = position;
	
	var direction = Vector3(Mathf.Cos(speedX * Time.time), 0.0f, Mathf.Cos(speedX * speedScaleZ * Time.time) * speedScaleZ);
	transform.rotation = Quaternion.LookRotation(direction);
	
	if (null != target && null != lineRender) {
		lineRender.SetPosition(0, transform.position);
		lineRender.SetPosition(1, target.transform.position);
	}
}