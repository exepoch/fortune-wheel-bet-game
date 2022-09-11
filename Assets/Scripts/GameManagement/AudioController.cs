using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameManagement
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource music;
        
        private bool _isSoundOn = true;
        private float _volume = 1;
        
        public void SetGeneralSounOn(bool val)
        {
            AudioListener.volume = val ? _volume : 0;
            _isSoundOn = val;
        }

        public void SetVolume(Slider slider)
        {
            _volume = slider.value;

            if (_isSoundOn)
                AudioListener.volume = _volume;
        }

        public void SetMusicVolume(Slider slider)
        {
            music.volume = slider.value;
        }
    }
}
