using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    // event handler
    public delegate void OnClickEvent(GameObject go);
    public event OnClickEvent OnClick;
    // exclusive event handler
    public delegate void OnClickEvent_Exclusive(GameObject go);
    public event OnClickEvent_Exclusive OnClick_Exclusive;
    // singleton
    private static InputManager instance;
    // constructor
    private InputManager() { }
    // instance
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(InputManager)) as InputManager;
            }
            return instance;
        }
    }
    private GameObject objectMousedDownOn;
    private bool isSendingExclusiveEvents = false;
    private bool isIgnoringUserInput = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                objectMousedDownOn = hit.transform.gameObject;
            }
        }
        else if (Input.GetMouseButtonUp(0) && !isIgnoringUserInput)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (objectMousedDownOn != null && hit.transform.gameObject == objectMousedDownOn)
                {
                    SendMouseClick(hit.transform.gameObject);
                }
            }
        }
    }

    public void SendMouseClick(GameObject go)
    {
        // notify of the event
        if (isSendingExclusiveEvents)
        {
            OnClick_Exclusive(go);
        }
        else
        {
            OnClick(go);
        }
    }

    public void RequestExclusiveControl()
    {
        isSendingExclusiveEvents = true;
    }

    public void EndExclusiveControl()
    {
        isSendingExclusiveEvents = false;
    }

    public void ManuallySendEvent(GameObject obj)
    {
        OnClick(obj);
    }

    public void IgnoreUserInput()
    {
        isIgnoringUserInput = true;
    }

    public void ResumeUserInput()
    {
        isIgnoringUserInput = false;
    }
}
