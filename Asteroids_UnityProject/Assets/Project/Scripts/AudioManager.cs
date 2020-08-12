using UnityEngine;

namespace KyleConibear
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance => instance;

        [SerializeField] private AudioSource musicAudioSource = null;
        [SerializeField] private AudioSource sfxAudioSource = null;

        public enum AudioChannel
        {
            Music,
            SFX
        }

        private void Awake()
        {
            // Create singleton
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
        }

        public void PlayAudio(AudioChannel audioChannel, AudioClip audioClip)
        {
            switch (audioChannel)
            {
                case AudioChannel.Music:
                this.musicAudioSource.clip = audioClip;
                this.musicAudioSource.Play();
                break;

                case AudioChannel.SFX:
                this.sfxAudioSource.clip = audioClip;
                this.sfxAudioSource.Play();
                break;
            }
        }
    }
}