using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioSource music;
    public static AudioClip defaultTrack;
    [SerializeField] GameObject musicObject;
    // Start is called before the first frame update
    void Start()
    {
        music = musicObject.GetComponent<AudioSource>();
        defaultTrack = music.clip;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
