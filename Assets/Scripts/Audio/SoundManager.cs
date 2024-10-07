using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // Singleton instance of the SoundManager
    public GameSound[] gameSounds; // Array of game sounds

    [SerializeField] private AudioSource soundFXObject; // Audio source prefab for sound effects

    private Dictionary<string, List<AudioSource>> activeSounds; // Dictionary to track active sounds

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            activeSounds = new Dictionary<string, List<AudioSource>>();
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (GameSound gs in gameSounds)
        {
            gs.source = gameObject.AddComponent<AudioSource>();
            gs.source.clip = gs.clip;
            gs.source.volume = gs.volume;
            gs.source.pitch = gs.pitch;
            gs.source.loop = gs.loop;
            gs.source.spatialBlend = gs.spatialBlend;

            // Initialize the activeSounds dictionary
            if (!activeSounds.ContainsKey(gs.name))
            {
                activeSounds[gs.name] = new List<AudioSource>();
            }
        }
    }

    // Play a sound by name
    public void PlaySound(string name, Transform spawnTransform = null, bool allowMultiple = false)
    {
        GameSound gs = System.Array.Find(gameSounds, sound => sound.name == name);
        if (gs == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        Debug.Log("Playing Sound: " + name);

        if (!allowMultiple && gs.source != null && gs.source.isPlaying)
        {
            gs.source.Stop();
        }

        if (spawnTransform != null)
        {
            // Create a new AudioSource instance for spatial/3D sound
            AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
            audioSource.clip = gs.clip;
            audioSource.volume = gs.volume;
            audioSource.pitch = gs.pitch;
            audioSource.loop = gs.loop;
            audioSource.spatialBlend = gs.spatialBlend;
            audioSource.Play();

            if (!gs.loop)
            {
                Destroy(audioSource.gameObject, gs.clip.length);
            }

            if (!activeSounds.ContainsKey(name))
            {
                activeSounds[name] = new List<AudioSource>();
            }
            activeSounds[name].Add(audioSource); // Track this instance
        }
        else
        {
            if (gs.source != null)
            {
                gs.source.Play();
            }
        }
    }

    // Fade out a sound by name
    public void FadeOutSound(string name, float fadeDuration)
    {
        if (!activeSounds.ContainsKey(name))
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        Debug.Log("Fading out Sound: " + name);

        // Fade out all instances of the sound
        foreach (AudioSource source in activeSounds[name])
        {
            if (source != null && source.isPlaying)
            {
                StartCoroutine(FadeOutCoroutine(source, fadeDuration));
            }
        }
    }

    // Coroutine to fade out the volume of an AudioSource
    private IEnumerator FadeOutCoroutine(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop(); // Stop the sound after fade-out
        audioSource.volume = startVolume; // Reset the volume for future use
    }

    // Stop a sound by name
    public void StopSound(string name)
    {
        if (!activeSounds.ContainsKey(name))
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        Debug.Log("Stopping Sound: " + name);

        // Stop all instances of the sound
        for (int i = activeSounds[name].Count - 1; i >= 0; i--)
        {
            AudioSource source = activeSounds[name][i];
            if (source != null && source.isPlaying)
            {
                source.Stop();
            }

            // Remove destroyed or inactive sources from the list
            if (source == null || !source.isPlaying)
            {
                activeSounds[name].RemoveAt(i);
            }
        }
    }

    // Play a random sound from an array of sound names
    public void PlayRandomSound(string[] names, Transform spawnTransform = null)
    {
        int rand = Random.Range(0, names.Length);
        PlaySound(names[rand], spawnTransform);
    }
}

