using System.Collections.Generic;
using Unity.Collections;
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
        public int TimeToDepleat;
        [Range(0f, 1f)] public float DownModProba;
    }

    public class BubbleManager : MonoBehaviour
    {
        [SerializeField]
        private Dictionary<Need, NeedInfo> _needInfo = new();
        [SerializeField, Tooltip("Debug only, do not modify")]
        private Dictionary<Need, float> _needs = new();

        private float _checkTimer = 1f;

        private void Awake()
        {
            _needs = new();
            foreach (var ni in _needInfo)
            {
                _needs.Add(ni.Key, ni.Value.TimeToDepleat);
            }
        }

        private void Update()
        {
            _checkTimer -= Time.deltaTime;
            if (_checkTimer <= 0f)
            {
                foreach (var ni in _needs)
                {
                    if (Random.value < _needInfo[ni.Key].DownModProba)
                    {
                        _needs[ni.Key]--;
                    }
                }
                _checkTimer = 1f;
            }
        }
    }
}
