using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aurora.Timeline
{
    internal class TrackManager: SingletonBehaviour<TrackManager>
    {
        // private static TrackManager _instance;
        //
        // public static TrackManager Instance
        // {
        //     get
        //     {
        //         if (_instance is null)
        //         {
        //             _instance = FindObjectOfType<TrackManager>();
        //
        //             if (_instance is null)
        //             {
        //                 GameObject obj = new GameObject("TrackManager");
        //                 _instance = obj.AddComponent<TrackManager>();
        //                 DontDestroyOnLoad(obj);
        //             }
        //         }
        //
        //         return _instance;
        //     }
        // }



        public List<Track> tracks = new List<Track>();
        public float _rate = 1f;

        private void Update()
        {
            foreach (var track in tracks)
            {
                track.Update(Time.deltaTime * _rate, Time.timeScale * _rate);
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
