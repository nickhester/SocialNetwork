using UnityEngine;
using System.Collections;

public class PersonMovement : MonoBehaviour {

    private Vector3 originalPos;
	private Vector3 startingPos;
    private const float startingPositionScaler = 3.0f;
	private float positionScaler;
    private float progress = 0.0f;

	private bool hasMovedIn = false;
	private bool isMovingOut = false;

    private const float moveSpeed = 1.6f;
    private const float bounceStartPoint = 0.65f;
    private const float bounceDistance = 0.1f;
	
	// Use this for initialization
	void Awake () {
		originalPos = transform.position;
		startingPos = transform.position * positionScaler;
		transform.position = startingPos;
	}

    void Start ()
    {
        positionScaler = startingPositionScaler;
    }
	
	// Update is called once per frame
	void Update () {
		if (!hasMovedIn)
        {
            MoveIn ();
        }
		
        if (isMovingOut)
        {
            positionScaler += moveSpeed * (startingPositionScaler - 1.0f) * Time.deltaTime;
        }

        if (!isMovingOut && progress >= 1.0f)
        {
            positionScaler = 1.0f;
            hasMovedIn = true;
        }
		
        transform.position = originalPos * positionScaler;
	}

	void MoveIn()
	{
        progress += moveSpeed * Time.deltaTime;

        if (progress < bounceStartPoint)
        {
            positionScaler = startingPositionScaler - ((progress * 2.0f) / bounceStartPoint);
        }
        else
        {
            // some crazy math to get a bounce-back after hitting the bounceStartPoint
            // localProgress is a 0 to 1 value between the "bounceStartPoint" and the end of the movement
            float localProgress = (progress - bounceStartPoint) * (1.0f / (1.0f - bounceStartPoint));
            // this does a little mini-arc bounce
            positionScaler = -(Mathf.Sin(localProgress * Mathf.PI) / (1.0f / bounceDistance)) + 1.0f;
        }
        
	}

	public void MoveOut()
	{
		isMovingOut = true;
	}

    public Vector3 GetTargetPosition()
    {
        return originalPos;
    }
}
