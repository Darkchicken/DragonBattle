#pragma strict

var isFlying			: boolean			= false;
var flyingHeight		: float				= 50.0;
var groundHeight		: float				= 0.0;
var minFlyHeight		: float				= 10.0;

function Update(){
	if (isFlying)
	{
		if (transform.position.y < minFlyHeight)
			transform.position.y = flyingHeight;
	}
}

function StartFlying(){
	isFlying				= true;
	transform.position.y	= flyingHeight;
	transform.position.x	= 0;
	transform.position.z	= 0;
}

function StartGround(){
	isFlying				= false;
	transform.position.y	= groundHeight;
	transform.position.x	= 0;
	transform.position.z	= 0;
}