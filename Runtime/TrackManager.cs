using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aurora.Timeline
{
    public class TrackManager: MonoBehaviour
    {
        private static TrackManager _instance;

        public static TrackManager Instance
        {
            get
            {
                if (_instance is null)
                {
                    // 为_instance赋值之后, 返回
                    _instance = FindObjectOfType<TrackManager>();

                    if (_instance is null)
                    {
                        GameObject obj = new GameObject("TrackManager");
                        _instance = obj.AddComponent<TrackManager>();
                        DontDestroyOnLoad(obj);
                    }
                }

                return _instance;
            }
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
