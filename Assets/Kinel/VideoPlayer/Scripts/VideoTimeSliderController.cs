using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace Kinel.VideoPlayer.Scripts
{
    public class VideoTimeSliderController : UdonSharpBehaviour
    {

        public KinelVideoScript kinelVideoPlayer;
        public Slider slider;
        public InitializeScript initializeSystemObject;

        private const int waitTimeSeconds = 1; // seconds

        private bool isDrag = false;
        private bool isWait = false;
        private int waitCount = 0;

        public void OnSliderDrag()
        {
            isDrag = true;
        }

        public void OnSliderDrop()
        {
            if (kinelVideoPlayer.IsStreamLocal())
            {
                return;
            }
            kinelVideoPlayer.SetVideoTime(slider.value);
            isWait = true;
        }
        
        public void FixedUpdate()
        {
            if (kinelVideoPlayer.IsStreamLocal())
            {
                return;
            }
           
            if (isWait)
            {
                waitCount++;
                if (waitCount >= initializeSystemObject.UserUpdateCount() * waitTimeSeconds)
                {
                    isDrag = false;
                    isWait = false;
                    waitCount = 0;
                }

                return;
            }

            if (!isDrag)
            {
                slider.value = kinelVideoPlayer.GetVideoPlayer().GetTime();
            }
            
        }

        public void SetSliderLength(float time)
        {
            this.slider.maxValue = time;
        }

        public bool IsDrag()
        {
            return isDrag;
        }

        public void Freeze()
        {
            slider.interactable = !kinelVideoPlayer.IsStreamLocal();
            if (kinelVideoPlayer.IsStreamLocal())
            {
                slider.value = slider.maxValue;
            }
        }
    }
}