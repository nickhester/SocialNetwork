using UnityEngine;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    enum LevelType
    {
        Calendar,
        Clipboard,
        Appointment
    };

    [SerializeField] private LevelType levelType;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
