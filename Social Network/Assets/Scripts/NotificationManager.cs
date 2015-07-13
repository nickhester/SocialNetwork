using UnityEngine;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private GameObject fingerPrefab;

    private Finger m_finger;
    private Notification m_notification;

    private Vector2 instructionDimensions = new Vector2(1516.0f, 2178.0f);
    private float instructionDimensionScale = 0.007f;

    private int currentSet = -1;
    private int currentIndexWithinSet = -1;

    void Awake()
    {
        InputManager.Instance.OnClick += OnClick;

        m_finger = GetComponent<Finger>();
        m_notification = GetComponent<Notification>();
    }

    private void OnClick(GameObject go)
    {
        if (go.tag == "notificationModal")
        {
            // remove notification or go to next
        }
        else
        {
            // if you're clicking anywhere, dismiss current notification
            EndNotification();

            // TODO: this needs to be expanded to move to the "next" notification if there is one
        }
    }

    void Start()
    {
        //DisplayNotificationSet(0);
    }

    public void DisplayNotificationSet(int _setIndex)
    {
        // trigger correct notifications

        // check to see if it's been seen
        if (false) //SaveGame.GetSeenInstruction(_setIndex))
        {
            return;
        }
        else
        {
            SaveGame.SetSeenInstruction(_setIndex, true);
        }

        if (currentSet != -1 && currentIndexWithinSet != -1)
        {
            Debug.LogError("Starting new notification, but old one was not terminated properly");
            return;
        }

        currentSet = _setIndex;
        ActivateNotification(currentSet, currentIndexWithinSet);
    }

    void ActivateNotification(int _setIndex, int _indexWithinSet)
    {
        currentIndexWithinSet++;

        if (_setIndex == 0)      // 1 - first greeting at calendar
        {
            if (_indexWithinSet == 0)
            {
                m_finger.SendFinger(new Vector2(-1.8f, 2.5f));
                Texture tex = Resources.Load<Texture>("textures/instructions/Instruction Paper_introduction");
                m_notification.DisplayNotification(tex, "introduction", new Vector2(0.0f, -4.0f), instructionDimensions.x * instructionDimensionScale, instructionDimensions.y * instructionDimensionScale, false);
            }
        }
        else
        {
            Debug.LogError("Attempting to view notification that doesn't exist");
        }
    }

    void EndNotification()
    {
        m_finger.SendFingerAway();
        m_notification.RemoveNotification();
        currentSet = -1;
        currentIndexWithinSet = -1;
    }
}
