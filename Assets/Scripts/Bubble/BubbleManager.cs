using System.Collections.Generic;
using UnityEngine;

namespace BubbleFriend
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
        public static BubbleManager Instance { private set; get; }

        [SerializeField]
        private Dictionary<Need, NeedInfo> _needInfo = new();
        [SerializeField, Tooltip("Debug only, do not modify")]
        private Dictionary<Need, int> _needs = new();

        public float CleaningNeed => _needs[Need.Clean] / (float)_needInfo[Need.Clean].TimeToDepleat;

        private float _checkTimer = 1f;

        private void Awake()
        {
            Instance = this;

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
