﻿using System.Collections.Generic;
using UnityEngine;

namespace JSAM
{
    #region Effect Structs
    [System.Serializable]
    public struct AudioLowPassFilterObj
    {
        public bool enabled;
        /// <summary>
        /// Ranges from 10 to 22000
        /// </summary>
        public float cutoffFrequency;
        /// <summary>
        /// Ranges from 1 to 10
        /// </summary>
        public float lowpassResonanceQ;
    }

    [System.Serializable]
    public struct AudioHighPassFilterObj
    {
        public bool enabled;
        /// <summary>
        /// Ranges from 10 to 22000
        /// </summary>
        public float cutoffFrequency;
        /// <summary>
        /// Ranges from 1 to 10
        /// </summary>
        public float highpassResonanceQ;
    }

    [System.Serializable]
    public struct AudioDistortionObj
    {
        public bool enabled;
        /// <summary>
        /// Ranges from 0 to 1
        /// </summary>
        public float distortionLevel;
    }
    #endregion

    [CreateAssetMenu(fileName = "New Audio File", menuName = "AudioManager/New Audio File Object", order = 1)]
    public class AudioFileObject : MonoBehaviour, IComparer<AudioFileObject>
    {
        [Header("Attach audio file here to use")]
        [SerializeField]
        public AudioClip file;

        [Header("Attach audio files here to use")]
        [SerializeField]
        public List<AudioClip> files = new List<AudioClip>();

        [HideInInspector]
        public bool useLibrary;

        [HideInInspector]
        public string category = "";

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("The volume of this Audio File relative to the volume levels defined in the main AudioManager. Leave at 1 to keep unchanged. The lower the value, the quieter it will be during playback.")]
        public float relativeVolume = 1;

        [SerializeField]
        [Tooltip("If true, playback will be affected based on distance and direction from listener")]
        public bool spatialize;

        [Tooltip("If there are several sounds playing at once, sounds with higher priority will be culled by Unity's sound system later than sounds with lower priority. \"Music\" has the absolute highest priority and \"Spam\" has the lowest.")]
        [SerializeField]
        public Priority priority = Priority.Default;

        [Tooltip("Amount of random variance to the sound's frequency to be applied (both positive and negative) when this sound is played. Keep below 0.2 for best results.")]
        [SerializeField]
        [Range(0, 0.5f)]
        public float pitchShift = 0.05f;

        [Tooltip("Adds a delay in seconds before this sound is played. If the sound loops, delay is only added to when the sound is first played before the first loop.")]
        [SerializeField]
        public float delay = 0;

        [Tooltip("If true, will ignore the \"Time Scaled Sounds\" parameter in AudioManager and will keep playing the sound even when the Time Scale is set to 0")]
        [SerializeField]
        public bool ignoreTimeScale = false;

        [Tooltip("Add fade to your sound. Set the details of this fade using the FadeMode tools")]
        [SerializeField]
        public FadeMode fadeMode;
        [HideInInspector]
        [Tooltip("The percentage of time the sound takes to fade-in relative to it's total length.")]
        [SerializeField]
        public float fadeInDuration;

        [HideInInspector]
        [SerializeField]
        [Tooltip("The percentage of time the sound takes to fade-out relative to it's total length.")]
        public float fadeOutDuration;

        /// <summary>
        /// Don't touch this unless you're modifying AudioManager functionality
        /// </summary>
        [HideInInspector]
        public string safeName = "";

        [SerializeField, HideInInspector] public AudioLowPassFilterObj lowPassFilter;
        [SerializeField, HideInInspector] public AudioHighPassFilterObj highPassFilter;
        [SerializeField, HideInInspector] public AudioDistortionObj distortionFilter;

        public AudioClip GetFile()
        {
            return file;
        }

        public List<AudioClip> GetFiles()
        {
            return files;
        }

        public AudioClip GetFirstAvailableFile()
        {
            if (useLibrary)
            {
                foreach (AudioClip f in files)
                {
                    if (f != null) return f;
                }
            }
            else
            {
                return file;
            }
            return null;
        }

        public int GetFileCount()
        {
            if (!useLibrary) return (file == null) ? 0 : 1;
            int count = 0;
            foreach (AudioClip a in files)
            {
                if (a != null)
                {
                    count++;
                }
            }
            return count;
        }

        public bool IsLibraryEmpty()
        {
            if (!useLibrary)
            {
                return file == null;
            }
            else
            {
                foreach (AudioClip a in files)
                {
                    if (a != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool HasAudioClip(AudioClip a)
        {
            return file == a || files.Contains(a);
        }

        public bool UsingLibrary()
        {
            return useLibrary;
        }

#if UNITY_EDITOR
        public int Compare(AudioFileObject x, AudioFileObject y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            return x.safeName.CompareTo(y.safeName);
        }
    }
#endif
}