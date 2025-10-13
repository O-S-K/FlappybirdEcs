using UnityEngine;

namespace FlappyECS
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource bgmSource;

        [Header("Clips")]
        public AudioClip jumpClip;
        public AudioClip hitClip;
        public AudioClip dieClip;
        public AudioClip scoreClip;
        public AudioClip buttonClip;
        

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip != null) sfxSource.PlayOneShot(clip, volume);
        }

        public void PlayDie() => PlaySFX(dieClip);
        public void PlayJump() => PlaySFX(jumpClip);
        public void PlayHit() => PlaySFX(hitClip);
        public void PlayScore() => PlaySFX(scoreClip);
        public void PlayButton() => PlaySFX(buttonClip);

        public void PlayBGM(bool play)
        {
            if (play && !bgmSource.isPlaying) bgmSource.Play();
            else if (!play) bgmSource.Stop();
        }
    }
}