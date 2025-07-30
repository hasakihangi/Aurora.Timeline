using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aurora.Timeline
{
    public class TrackManager: MonoBehaviour
    {
        public static TrackManager Instance {get; private set;}
        public void Init()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            Instance = this;
        }

        public List<Track> tracks = new List<Track>();
        public float m_Rate = 1f;

        private void Update()
        {
            foreach (var track in tracks)
            {
                track.Update(Time.deltaTime * m_Rate, Time.timeScale * m_Rate);
            }
        }

        public void Register(Track track)
        {
            if (track == null)
                return;

            tracks.Add(track);
        }

        public void Unregister(Track track)
        {
            if (track == null)
                return;

            tracks.Remove(track);
        }
    }
}
