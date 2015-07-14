using UnityEngine;
using System.Collections;

public class Finger : MonoBehaviour {

    [SerializeField] private GameObject fingerObject;
    private GameObject fingerInstance;
    private Vector3 fingerOrigin;

    private bool fingerIsActive = false;
    private Vector3 currentOrigin;
    private Vector3 currentTarget;
    private bool isTraveling = false;
    private float hoverCounter = 0.0f;
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float hoverAmount;
    [SerializeField] private float moveSpeed;

	void Awake ()
    {
        fingerOrigin = new Vector3(0.0f, -10.0f, -2.5f);
        fingerInstance = Instantiate(fingerObject, fingerOrigin, Quaternion.identity) as GameObject;

        //SendFinger(Vector2.zero);
	}

    void Update()
    {
        if (!isTraveling && fingerIsActive)
        {
            hoverCounter += Time.deltaTime;
            fingerInstance.transform.position = new Vector3(currentTarget.x, (currentTarget.y + (Mathf.Sin(hoverCounter * hoverSpeed) * hoverAmount)), fingerInstance.transform.position.z);
        }
    }

    public void SendFinger(Vector2 target, bool isImmediate)
    {
        fingerIsActive = true;
        currentOrigin = fingerInstance.transform.position;
        currentTarget = new Vector3(target.x, target.y, fingerInstance.transform.position.z);
        if (isImmediate)
        {
            fingerInstance.transform.position = currentTarget;
        }
        else
        {
            StartCoroutine("LerpToPosition");
        }
        
    }

    public void SendFinger(Vector2 target)
    {
        SendFinger(target, false);
    }
    
    public void SendFingerAway(bool isImmediate)
    {
        SendFinger(fingerOrigin, isImmediate);
        fingerIsActive = false;
    }

    IEnumerator LerpToPosition()
    {
        isTraveling = true;
        float progress = 0.0f;
        while (progress < 1.0f)
        {
            progress += Time.deltaTime * moveSpeed;
            fingerInstance.transform.position = Vector3.Lerp(currentOrigin, currentTarget, Mathf.Sin(progress * (Mathf.PI / 2.0f)));

            yield return null;
        }
        isTraveling = false;
        hoverCounter = 0.0f;
    }
}
