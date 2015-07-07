using UnityEngine;
using System.Collections;

public class Finger : MonoBehaviour {

    [SerializeField] private GameObject fingerObject;
    private GameObject fingerInstance;
    private Vector2 fingerOrigin;

    private Vector2 currentOrigin;
    private Vector2 currentTarget;

    [SerializeField] private float moveSpeed;

	void Start ()
    {
        fingerOrigin = new Vector2(0.0f, -10.0f);
        fingerInstance = Instantiate(fingerObject, new Vector3(fingerOrigin.x, fingerOrigin.y, transform.position.z), Quaternion.identity) as GameObject;

        SendFinger(new Vector2(0.0f, 0.0f));
	}

    public void SendFinger(Vector2 target)
    {
        currentOrigin = fingerInstance.transform.position;
        currentTarget = target;
        StartCoroutine("LerpToPosition");
    }

    public void SendFingerAway()
    {
        SendFinger(fingerOrigin);
    }

    IEnumerator LerpToPosition()
    {
        float progress = 0.0f;
        while (progress < 1.0f)
        {
            progress += Time.deltaTime * moveSpeed;
            fingerInstance.transform.position = Vector2.Lerp(currentOrigin, currentTarget, Mathf.Sin(progress * (Mathf.PI / 2.0f)));

            yield return null;
        }
    }
}
