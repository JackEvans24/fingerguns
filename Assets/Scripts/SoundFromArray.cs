using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundFromArray : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;

    private AudioSource source;

    private void Awake()
    {
        this.source = GetComponent<AudioSource>();
    }

    public void Play() => StartCoroutine(Enumerated_Play());

    private IEnumerator Enumerated_Play()
    {
        var clip = clips[Random.Range(0, clips.Length)];
        source.PlayOneShot(clip);

        yield return new WaitForSeconds(clip.length);
    }
}