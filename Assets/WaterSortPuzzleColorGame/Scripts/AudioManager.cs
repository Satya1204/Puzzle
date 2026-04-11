using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSortPuzzleGame
{

    public class AudioManager : Singleton<AudioManager>
    {
        // Start is called before the first frame update
        [SerializeField] public AudioSource _musicSource;
        [SerializeField] public AudioSource _SFXSource;

        public AudioClip background;
        public AudioClip touch;
        public AudioClip water;
        public AudioClip confetti;
        public AudioClip purchaseComplete;

        public void OnEnable()
        {
            EventManager.ChangeMusicSetting += ChangeMusicSetting;

        }
        public void OnDisable()
        {
            EventManager.ChangeMusicSetting -= ChangeMusicSetting;
        }
        private void Start()
        {
            ChangeMusicSetting();
        }
        public void ChangeMusicSetting()
        {
            if (GameManager.IsMusicEnable)
            {
                _musicSource.clip = background;
                _musicSource.Play();
            }
            else
            {
                _musicSource.Stop();
            }

        }
        public void PlaySFX(AudioClip playAudio)
        {
            if (GameManager.IsSoundEnable)
            {
                _SFXSource.PlayOneShot(playAudio);
            }
        }
        public void WaterPourSFX(AudioClip playAudio)
        {
            _SFXSource.clip = playAudio;
            if (GameManager.IsSoundEnable)
            {
                _SFXSource.Play();
            }

        }
        public void StopSFX()
        {
            _SFXSource.Stop();
        }

    }
}