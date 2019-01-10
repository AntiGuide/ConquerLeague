using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    /// <summary>Static reference to the only occurence of this object. Used because Unity doesn't let you set info for static components</summary>
    public static SoundController FSSoundController;

    public AudioSource AudioSource1;
    public AudioSource AudioSourceBGM;
    public AudioClip[] Clips;
    public AudioClip[] BGMClips;
    public float startVolumeBGM = 0.05f;
    public float inGameVolumeBGM = 0.2f;

    [SerializeField]
    private Slider audioSlider;

    private int aktTrackNumber;

    public enum Sounds
    {
        PLAYER_DESTRUCTION = 0,
        VICTORIOUS,
        DEFEATED,
        COUNTDOWN,
        BUY_WARTRUCKS,
        CANTBUY_WARTRUCKS,
        MG_SHOT,
        TOWER_MG,
        WARTRUCK_DESTRUCTION,
        TURRET_DESTRUCTION,
        GOAL,
    }

    public static void StopLoopingSound(ref AudioSource inpAudioSource) {
        UnityEngine.Object.Destroy(inpAudioSource);
    }

    public void StartSound(Sounds sound, float volume = 1f) {
        this.AudioSource1.PlayOneShot(this.Clips[(int)sound], volume);
    }

    public IEnumerator StartSound(Sounds sound, AudioClip sound2, float volume = 1f) {
        this.AudioSource1.PlayOneShot(this.Clips[(int)sound], volume);
        yield return new WaitForSecondsRealtime(this.Clips[(int)sound].length + 0.5f);
        this.AudioSource1.PlayOneShot(sound2, volume);
    }

    public AudioSource StartLoopingSound(Sounds sound, float volume) {
        var retAudioSource = this.gameObject.AddComponent<AudioSource>();
        retAudioSource.loop = true;
        retAudioSource.clip = this.Clips[(int)sound];
        retAudioSource.volume = volume;
        retAudioSource.Play();
        this.StartCoroutine(StopLoopingSoundDelayed(retAudioSource, 1f));
        return retAudioSource;
    }

    private static IEnumerator StopLoopingSoundDelayed(AudioSource retAudioSource, float delay) {
        yield return new WaitForSeconds(delay);
        StopLoopingSound(ref retAudioSource);
    }

    private void Start() {
        if (FSSoundController != null) {
            Application.Quit();
        }

        FSSoundController = this;
        this.AudioSourceBGM.volume = this.startVolumeBGM;
        this.AudioSourceBGM.loop = false;
        aktTrackNumber = -1;
    }

    private void Update() {
        if (!AudioSourceBGM.isPlaying) {
            aktTrackNumber = ++aktTrackNumber > this.BGMClips.Length - 1 ? 0 : aktTrackNumber;
            this.AudioSourceBGM.clip = this.BGMClips[aktTrackNumber];
            this.AudioSourceBGM.Play();
        }
    }

    public void AudioSlider()
    {
        AudioSourceBGM.volume = audioSlider.value/5;
    }
}