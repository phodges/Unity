#pragma strict

var player : GameObject;

var lineRender : LineRenderer;

var lineLength : float = 2.0f;

function Start () {
	lineRender = GetComponent(LineRenderer);


}

function Update () {
	if (null != player && null != lineRender) {
		var approaching : boolean = false;
		
		// Test whether the vector from the player to the target has a component along 
		// its direction of travel.
		var separation : Vector3 = transform.position - player.transform.position;
		if (0.0f < Vector3.Dot(separation, player.transform.forward)) {
			approaching = true;
		}
		
		// We only draw something if the player is not moving away from the target.
		if (approaching) {
			// Create a line starting at the target and lying along the direction of the separation vector
			var tangent : Vector3 = Vector3.Normalize(separation) * lineLength;
			
			var start : Vector3 = transform.position;
			var end : Vector3 = start + tangent;
			
			lineRender.SetPosition(0, start);
			lineRender.SetPosition(1, end);
			lineRender.enabled = true;
		} else {
			lineRender.enabled = false;
		}
	}
}