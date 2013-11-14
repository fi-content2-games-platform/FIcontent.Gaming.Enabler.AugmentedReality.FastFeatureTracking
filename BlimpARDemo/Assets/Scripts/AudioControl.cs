using UnityEngine;
using System.Collections;

public class AudioControl : MonoBehaviour
{

    public float musicVolume = .5f;         // music volume
    public float effectsVolume = 1f;        // fx volume      
    public AudioClip rebelRailSound, emperrorRailSound, messageSound, alarmSound, victorySound;

    public static float EffectsVolume;
    public static AudioClip RebelRailSound;
    public static AudioClip EmperrorRailSound;
    public static AudioClip MessageSound;
    public static AudioClip AlarmSound;
    public static AudioClip VictorySound;

    private AudioSource musicAudioSource;   // audio source of the music

    private float lastFxVol;



    void Awake()
    {
        if (!rebelRailSound ||
            !emperrorRailSound ||
            !messageSound ||
            !alarmSound ||
            !victorySound)
            Debug.LogError("please assign all the sounds in the inspector", this);

        RebelRailSound = rebelRailSound;
        EmperrorRailSound = emperrorRailSound;
        MessageSound = messageSound;
        AlarmSound = alarmSound;
        VictorySound = victorySound;
    }

    void Start()
    {
        musicAudioSource = GetComponent<AudioSource>() as AudioSource;
    }

    /// <summary>
    /// Sets the volumes
    /// </summary>
    void Update()
    {

        EffectsVolume = effectsVolume;

        // music volume
        if (musicAudioSource)
            musicAudioSource.volume = musicVolume;

        if (lastFxVol != effectsVolume)
        {
            // update if changes
            var uas = GameObject.FindObjectsOfType(typeof(UnitAudioSource)) as UnitAudioSource[];
            foreach (var u in uas)
            {
                //u.volume = effectsVolume;
                u.SendMessage("SetVolume", SendMessageOptions.DontRequireReceiver);
            }

            lastFxVol = effectsVolume;
        }

    }
}
