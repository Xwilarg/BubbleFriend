using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public enum Need
    {
        Clean,
        Feed,
        Play
    }

    [System.Serializable]
    public class NeedInfo
    {
        public float TimeToDepleat;
        [Range(0f, 1f)] public float DownModProba;
    }

    public class BubbleManager : MonoBehaviour
    {
        [SerializeField]
        private Dictionary<Need, NeedInfo> _needInfo = new();
        private Dictionary<Need, float> _needs = new();

        private void Awake()
        {
            foreach (var ni in _needInfo)
            {
                _needs.Add(ni.Key, ni.Value.TimeToDepleat);
            }
        }

        private void Update()
        {
            foreach (var ni in _needs)
            {
                if (Random.value < _needInfo[ni.Key].DownModProba)
                {
                    _needs[ni.Key] -= Time.deltaTime;
                }
            }
        }
    }
}
