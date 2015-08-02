using UnityEngine;
using System.Collections;

public class AudioToggle : MonoBehaviour {

	[SerializeField] private Material matOn;
    [SerializeField] private Material matOff;

	private bool isAudioOn = true;
    private enum typeOfAudio
	{
		music,
		sfx
	};
	[SerializeField] private typeOfAudio audioType;
    private bool isFirstCreation = true;

    void Awake()
    {
        GameObject foundSelf = GameObject.Find(gameObject.name);

        if (foundSelf != null && foundSelf != gameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            InputManager.Instance.OnClick += OnClick;
        }
    }

    void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            int id = gameObject.GetInstanceID();
            InputManager.Instance.OnClick -= OnClick;
        }
    }

    private void OnClick(GameObject go)
    {
        if (go == gameObject)
        {
            GameObject networkManagerObject = GameObject.FindGameObjectWithTag("networkManager");
			GameManager gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            if (isAudioOn)
            {
                if (audioType == typeOfAudio.sfx)
				{
					SaveGame.SetAudioOn_sfx(false);
					MetricsLogger.Instance.LogCustomEvent("Settings", "TurnOffSFX", gm.GetCurrentDay() + "-" + gm.GetCurrentAppointment());
				}
                else
				{
					SaveGame.SetAudioOn_music(false);
					MetricsLogger.Instance.LogCustomEvent("Settings", "TurnOffMusic", gm.GetCurrentDay() + "-" + gm.GetCurrentAppointment());
				}

                gameObject.GetComponent<Renderer>().material = matOff;
                isAudioOn = false;

                if (audioType == typeOfAudio.music)
                {
                    AudioSource audio = GameObject.Find("Music Player").GetComponent<AudioSource>();
                    audio.enabled = false;
                }
            }
            else
            {
                if (audioType == typeOfAudio.sfx)
				{
					SaveGame.SetAudioOn_sfx(true);
					MetricsLogger.Instance.LogCustomEvent("Settings", "TurnOnSFX", gm.GetCurrentDay() + "-" + gm.GetCurrentAppointment());
				}
                else
				{
					SaveGame.SetAudioOn_music(true);
					MetricsLogger.Instance.LogCustomEvent("Settings", "TurnOnMusic", gm.GetCurrentDay() + "-" + gm.GetCurrentAppointment());
				}
                gameObject.GetComponent<Renderer>().material = matOn;
                isAudioOn = true;

                if (audioType == typeOfAudio.music)
                {
                    AudioSource m_audio = GameObject.Find("Music Player").GetComponent<AudioSource>();
                    m_audio.enabled = true;
                    m_audio.Play();
                }
            }
            // push bool to network manager
            if ((audioType == typeOfAudio.sfx) && (networkManagerObject != null))
            {
                networkManagerObject.GetComponent<NetworkManager>().isAudioOn_sfx = isAudioOn;
            }
        }
    }

	void Start ()
    {
		if (audioType == typeOfAudio.sfx)
		{
			// TODO: figure out why this isn't working!!!
			if (SaveGame.GetAudioOn_sfx())
			{
				gameObject.GetComponent<Renderer>().material = matOn;
				isAudioOn = true;
			}
			else
			{
				gameObject.GetComponent<Renderer>().material = matOff;
				isAudioOn = false;
			}
		}
		else if (audioType == typeOfAudio.music)
		{
			if (SaveGame.GetAudioOn_music())
			{
				gameObject.GetComponent<Renderer>().material = matOn;
				isAudioOn = true;
			}
			else
			{
				gameObject.GetComponent<Renderer>().material = matOff;
				isAudioOn = false;
			}
		}

        isFirstCreation = false;
	}

	void OnLevelWasLoaded(int level)
	{
        if (!isFirstCreation)
        {
            InputManager.Instance.OnClick += OnClick;
        }

        if (level == 0)
        {
            Destroy(gameObject);
        }
	}
}
