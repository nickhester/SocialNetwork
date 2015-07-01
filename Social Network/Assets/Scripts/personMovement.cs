using UnityEngine;
using System.Collections;

public class PersonMovement : MonoBehaviour {

	public Vector3 originalPos;
	private Vector3 startingPos;
	public Vector3 startDistance;
	public float lerpSpeed;
	private float countdownMultiplier = 3.0f;
	private float moveSpeed = 3.0f;

	private bool hasMovedIn = false;
	private bool isMovingOut = false;
	
	// Use this for initialization
	void Start () {
		originalPos = transform.position;
		startingPos = transform.position * countdownMultiplier;
		transform.position = startingPos;
	}
	
	// Update is called once per frame
	void Update () {
		if (!hasMovedIn) { MoveIn (); }
		if (isMovingOut) { countdownMultiplier += moveSpeed * Time.deltaTime;; }
		if (countdownMultiplier <= 1) { countdownMultiplier = 1; hasMovedIn = true; }
		transform.position = originalPos * countdownMultiplier;
	}

	void MoveIn()
	{
		countdownMultiplier -= moveSpeed * Time.deltaTime;
	}

	public void MoveOut()
	{
		isMovingOut = true;
	}
}
