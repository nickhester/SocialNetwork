using UnityEngine;
using System.Collections;

public class Notification : MonoBehaviour
{
    [SerializeField] private GameObject quadPrefab;
    private GameObject activeNotification;
    public Texture tempTexture;
    private bool currentNotificationIsModal;
    private string currentNotificationName;
    private Vector3 currentNotificationOriginalScale;
    private float currentPulseCounter = 0.0f;
    [SerializeField] private float pulseSpeed;
    [SerializeField] private float pulseAmount;

    void Awake()
    {
        InputManager.Instance.OnClick += OnClick;
    }

    /*
    void Start()
    {
        DisplayNotification(tempTexture, "temp", Vector2.zero, 10.0f, 10.0f, true);
    }
    */

    void OnClick(GameObject go)
    {
        if (activeNotification != null && go == activeNotification && currentNotificationIsModal)
        {
            // what to do when clicking the (modal) notification
        }
    }

    void Update()
    {
        if (IsNotificationActive())
        {
            currentPulseCounter += Time.deltaTime;
            float pulseScaler = (Mathf.Sin(currentPulseCounter * pulseSpeed) * pulseAmount) + 1.0f;
            activeNotification.transform.localScale = new Vector3(currentNotificationOriginalScale.x * pulseScaler, currentNotificationOriginalScale.y * pulseScaler, currentNotificationOriginalScale.z);
        }
    }

    public void DisplayNotification(Texture texture, string name, Vector2 screenPos, float scaleX, float scaleY, bool isModal)
    {
        activeNotification = Instantiate(quadPrefab, new Vector3(screenPos.x, screenPos.y, -0.5f), Quaternion.identity) as GameObject;
        activeNotification.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
        activeNotification.transform.localScale = new Vector3(scaleX, scaleY, activeNotification.transform.localScale.z);

        currentNotificationOriginalScale = activeNotification.transform.localScale;
        currentNotificationIsModal = isModal;
        currentNotificationName = name;
    }

    public void RemoveNotification()
    {
        if (activeNotification != null)
        {
            Destroy(activeNotification);
        }
        currentPulseCounter = 0.0f;
    }

    public bool IsNotificationActive()
    {
        return (activeNotification != null);
    }

    public string GetNotificationName()
    {
        if (IsNotificationActive())
        {
            return currentNotificationName;
        }
        return "";
    }
}
