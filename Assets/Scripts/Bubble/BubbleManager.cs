using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace BubbleFriend
{
    public enum Need
    {
        Clean,
        Feed,
        Play
    }

    public enum Flavor
    {
        Lemon,
        Coconut,
        Cherry
    }

    [System.Serializable]
    public class NeedInfo
    {
        public int TimeToDepleat;
        [Range(0f, 1f)] public float DownModProba;
    }

    [System.Serializable]
    public class AgeInfo
    {
        public Sprite Sprite;
        public float AccelerationRate;
    }

    [AutoStaticsCleanup]
    public partial class BubbleManager : MonoBehaviour
    {
        public static BubbleManager Instance { private set; get; }

        [SerializeField]
        private Dictionary<Need, NeedInfo> _needInfo = new();
        [SerializeField, Tooltip("Debug only, do not modify")]
        private Dictionary<Need, int> _needs = new();

        private Flavor _currentTaste;

        public float CleaningNeed => 1f - (_needs[Need.Clean] / (float)_needInfo[Need.Clean].TimeToDepleat);

        private float _checkTimer = 1f;
        private int _happiness;

        private void Awake()
        {
            Instance = this;

            _needs = new();
            foreach (var ni in _needInfo)
            {
                _needs.Add(ni.Key, ni.Value.TimeToDepleat);
            }

            _currentTaste = GetRandomFlavor();
            StartCoroutine(ChangeTasteCoroutine());
        }

        private IEnumerator ChangeTasteCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(600, 2000));
                _currentTaste = GetRandomFlavor();
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

        public void UpdateHappiness(int modifier)
        {
            _happiness = Mathf.Clamp(_happiness + modifier, -10, 10);
        }

        private Flavor GetRandomFlavor()
        {
            var flavors = System.Enum.GetValues(typeof(Flavor)).Cast<Flavor>().ToArray();
            return flavors[Random.Range(0, flavors.Length)];
        }

        public void ClearNeed(Need need, float percent)
        {
            var max = _needInfo[need].TimeToDepleat;
            _needs[need] = Mathf.RoundToInt(Mathf.Clamp(_needs[need] + (max * percent), 0f, max));
        }
    }
}
