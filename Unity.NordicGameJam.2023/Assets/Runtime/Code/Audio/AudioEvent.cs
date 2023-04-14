using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Event")]
public class AudioEvent : ScriptableObject
{
    [SerializeField] AudioClip[] clips;

    [SerializeField] FloatRange volume;

    [MinMaxRange(0, 2)]
    [SerializeField] FloatRange pitch;

    public void Play(AudioSource source)
    {
        if (clips.Length == 0) return;

        source.clip = RandomAudioClip;
        source.volume = volume.Random;
        source.pitch = pitch.Random;
        source.Play();
    }

    #region Internals

    private AudioClip RandomAudioClip => clips[Random.Range(0, clips.Length)];

    #endregion
}