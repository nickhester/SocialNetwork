using UnityEngine;
using System.Collections;

public class Notification : MonoBehaviour
{
    [SerializeField] private GameObject quadPrefab;
    [SerializeField] private GameObject modalBackgroundPrefab;
    private GameObject modalBackgroundObject;
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

    public void DisplayNotification(Texture texture, string name, Vector2 screenPos, bool isModal, bool isDarkened)
    {
        if (texture == null)
        {
            Debug.LogError("Texture for notification not found");
            return;
        }

        if (isModal)
        {
            if (modalBackgroundObject == null)
            {
                modalBackgroundObject = Instantiate(modalBackgroundPrefab, new Vector3(0.0f, 0.0f, -2.3f), Quaternion.identity) as GameObject;
            }
            
            if (isDarkened)
            {
                modalBackgroundObject.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                modalBackgroundObject.GetComponent<Renderer>().enabled = false;
            }
        }

        activeNotification = Instantiate(quadPrefab, new Vector3(screenPos.x, screenPos.y, -2.4f), Quaternion.identity) as GameObject;
        activeNotification.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);

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
        if (modalBackgroundObject != null)
        {
            Destroy(modalBackgroundObject);
            modalBackgroundObject = null;
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
