using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class Appointment : MonoBehaviour {

	public GameObject textObject;
	public GameObject myTextObject;
	public TextMesh myTextComponent;
	public Vector3 myLerpTarget;
	public bool isLerping = false;
	public GameObject mySpecialOverlay_FallToRed;
	public GameObject mySpecialOverlay_OneClick;
	public GameObject mySpecialOverlay_CantTouch;
	public GameObject mySpecialOverlay_NoLines;

	public validLevels myLevel;
	public int levelIndex;

	private string myDisplayText;
	public string myDisplayText_prop
	{
		get
		{
			return this.myDisplayText;
		}
		set
		{
			this.myDisplayText = value;
			myTextComponent.text = value;
		}
	}
	
	void Start ()
	{
			myLerpTarget = transform.position;
	}

	void Update () { 
		if (isLerping) { transform.position = Vector3.Lerp(transform.position, myLerpTarget, 0.1f); }
		// end lerp early
		if (Vector3.Distance(transform.position, myLerpTarget) < 0.1f) { transform.position = myLerpTarget; }
	}

	public void Initialize()
	{
		// Create text on appointment block
		myTextObject = Instantiate(textObject, transform.position, Quaternion.identity) as GameObject;
        myTextObject.transform.localScale = myTextObject.transform.localScale * 0.8f;
		myTextObject.transform.parent = gameObject.transform;
		myTextComponent = myTextObject.GetComponent<TextMesh>();
	}

	public void SetMySpecialOverlays()
	{
		if (myLevel.isFallToRed)
		{ mySpecialOverlay_FallToRed = InstantiateAndPositionOverlay(mySpecialOverlay_FallToRed); }
		if (myLevel.isOneClick)
		{ mySpecialOverlay_OneClick = InstantiateAndPositionOverlay(mySpecialOverlay_OneClick); }
		if (myLevel.isCantTouch)
		{ mySpecialOverlay_CantTouch = InstantiateAndPositionOverlay(mySpecialOverlay_CantTouch); }
		if (myLevel.isNoLines)
		{ mySpecialOverlay_NoLines = InstantiateAndPositionOverlay(mySpecialOverlay_NoLines); }
	}

	GameObject InstantiateAndPositionOverlay(GameObject _overlay)
	{
		Vector3 overlayPos = new Vector3(transform.position.x + 3.8f, transform.position.y, transform.position.z - 0.5f);
		Vector3 overlayScale = new Vector3(1, 1, 1);

		GameObject returnGO = Instantiate(_overlay, overlayPos, Quaternion.identity) as GameObject;

		returnGO.transform.localScale = overlayScale;
		returnGO.transform.parent = transform;

		return returnGO;
	}
}
