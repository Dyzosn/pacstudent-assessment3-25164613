using UnityEngine;

public class AudioController : MonoBehaviour
{
    // Music clips
    public AudioClip introMusic;
    public AudioClip normalStateMusic;

    // Option to let intro play fully instead of cutting at 3 seconds
    // I make this because of assignment specs concerns that say "when that audio clip either finishes or 3 seconds has elapsed (whichever is earliest)"
    // Default: follow specs (3 second cut), but you should try the uncutted version to hear the proper transition.
    public bool letIntroFinish = false;

    AudioSource audioSource;

    float startTime;
    bool switchedToNormal = false;

    void Start()
    {
        // Get the audio source
        audioSource = GetComponent<AudioSource>();

        // Record start time
        startTime = Time.time;

        // Start playing intro music
        if (introMusic != null)
        {
            audioSource.clip = introMusic;
            audioSource.loop = false;
            audioSource.Play();

            Debug.Log("Started intro music - duration: " + introMusic.length + " seconds");
            Debug.Log("Let intro finish: " + letIntroFinish);
        }
        else
        {
            Debug.LogError("Intro music not assigned!");
        }
    }

    void Update()
    {
        // Only check if we haven't switched yet
        if (!switchedToNormal && audioSource != null)
        {
            float timeElapsed = Time.time - startTime;

            if (letIntroFinish)
            {
                // Wait for intro to finish naturally
                if (!audioSource.isPlaying)
                {
                    Debug.Log("Intro finished naturally after " + timeElapsed + " seconds");
                    SwitchToNormalMusic();
                }
            }
            else
            {
                // Follow specs (cut at 3 seconds)
                if (timeElapsed >= 3.0f)
                {
                    Debug.Log("Cutting intro at 3 seconds");
                    SwitchToNormalMusic();
                }
            }
        }
    }

    void SwitchToNormalMusic()
    {
        if (normalStateMusic != null && !switchedToNormal)
        {
            // Stop current audio first
            audioSource.Stop();

            // Switch to normal music
            audioSource.clip = normalStateMusic;
            audioSource.loop = true;
            audioSource.Play();
            switchedToNormal = true;

            Debug.Log("Now playing normal state music");
        }
    }
}