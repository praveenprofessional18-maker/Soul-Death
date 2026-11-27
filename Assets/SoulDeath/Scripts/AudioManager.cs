using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField, Range(0f, 1f)] private float volume = 0.32f;

    private AudioSource source;

    public void Configure(AudioClip clip)
    {
        music = clip;
    }

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.loop = true;
        source.playOnAwake = false;
        source.clip = music;
        source.volume = volume;
        if (music != null)
            source.Play();
    }
}
