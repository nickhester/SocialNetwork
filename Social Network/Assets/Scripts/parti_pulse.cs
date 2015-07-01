using UnityEngine;
using System.Collections;

public class Parti_Pulse : MonoBehaviour {

	public Vector3 startingPos;
	public Vector3 endingPos;
	public float timeToMove;
	public bool isGreenAction;
	public bool changesColor;

	private ParticleSystem myParti;
	private Color currentColor;
	private float countDown;

	public Color posColor;
	public Color negColor;

	// Use this for initialization
	void Start () {
		myParti = GetComponent<ParticleSystem>();

		if (isGreenAction) { currentColor = posColor; }
		else { currentColor = negColor; }
		myParti.startColor = currentColor;

		countDown = timeToMove;
	}
	
	// Update is called once per frame
	void Update () {
		countDown -= Time.deltaTime;
		if (countDown > 0)
		{
			transform.Translate(((endingPos - startingPos) * Time.deltaTime) / timeToMove);
		}

		if (countDown < timeToMove * 0.6f && countDown > timeToMove * 0.4f)
		{
			if (changesColor)
			{
				currentColor = Color.black;
			}
		}
		else if (countDown < timeToMove * 0.4f)
		{
			if (changesColor)
			{
				if (isGreenAction)
				{
					currentColor = negColor;
				}
				else
				{
					currentColor = posColor;
				}
			}
		}

		if (countDown <= 0)
		{
			TurnOffParticleEmission();
		}
		myParti.startColor = currentColor;

	}

	void TurnOffParticleEmission()
	{
		myParti.emissionRate = 0;
	}
}
