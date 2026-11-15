using BubbleFriend.Bubble;
using BubbleFriend.Persistency;
using Sketch.Persistency;
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
        [Range(0f, 1f)] public float GrowthRate;
    }

    [System.Serializable]
    public class FlavorSprite
    {
        public Sprite SoapSprite, BellySprite;
    }

    [AutoStaticsCleanup]
    public partial class BubbleManager : MonoBehaviour
    {
        public static BubbleManager Instance { private set; get; }

        [SerializeField]
        private Dictionary<Need, NeedInfo> _needInfo = new();

        [SerializeField]
        private AgeInfo[] _ageData;

        private SpriteRenderer _sr;

        public Flavor CurrentTaste { private set; get; }
        [SerializeField]
        private Dictionary<Flavor, FlavorSprite> _flavorSprites;

        [SerializeField]
        private BubbleController _player;

        [SerializeField]
        private Sprite _spriteHappy, _spriteUnhappy;

        public int NeedOnTen(Need need)
        {
            var perc = 1f - (PersistencyManager<SaveData>.Instance.SaveData.Needs[need] / (float)_needInfo[need].TimeToDepleat);
            return Mathf.Clamp(Mathf.RoundToInt(perc * 10f), 0, 10);
        }

        private float _checkTimer = 1f;
        private int _checkCount = 0;

        private void Awake()
        {
            Instance = this;

            _sr = GetComponent<SpriteRenderer>();

            if (PersistencyManager<SaveData>.Instance.SaveData.Needs == null)
            {
                PersistencyManager<SaveData>.Instance.SaveData.Needs = new();
                foreach (var ni in _needInfo)
                {
                    PersistencyManager<SaveData>.Instance.SaveData.Needs.Add(ni.Key, ni.Value.TimeToDepleat / 2);
                }
                PersistencyManager<SaveData>.Instance.SaveData.LastCheck = System.DateTime.UtcNow;
                PersistencyManager<SaveData>.Instance.Save();
            }
            else
            {
                _sr.sprite = _ageData[PersistencyManager<SaveData>.Instance.SaveData.GrowthIndex].Sprite;

                // TODO: Check for time elapsed and update needs
            }

            CurrentTaste = GetRandomFlavor();
            StartCoroutine(ChangeTasteCoroutine());
        }

        private IEnumerator ChangeTasteCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(600, 2000));
                if (
                    !TimeManager.Instance.IsNight ||
                    NeedOnTen(Need.Feed) >= 3
                )
                {
                    CurrentTaste = GetRandomFlavor();
                    _player.ShowThought(_flavorSprites[CurrentTaste].BellySprite);
                }
            }
        }

        private void Update()
        {

            _checkTimer -= Time.deltaTime;
            if (_checkTimer <= 0f)
            {
                _checkTimer = 1f;
                if (TimeManager.Instance.IsNight)
                {
                    return;
                }
                foreach (var ni in PersistencyManager<SaveData>.Instance.SaveData.Needs)
                {
                    if (Random.value < _needInfo[ni.Key].DownModProba)
                    {
                        PersistencyManager<SaveData>.Instance.SaveData.Needs[ni.Key]--;
                    }
                }
                _checkCount++;
                if (_checkCount == 120)
                {
                    PersistencyManager<SaveData>.Instance.SaveData.LastCheck = System.DateTime.UtcNow;
                    PersistencyManager<SaveData>.Instance.SaveData.Growth += _ageData[PersistencyManager<SaveData>.Instance.SaveData.GrowthIndex].GrowthRate;
                    PersistencyManager<SaveData>.Instance.Save();

                    CheckForGrowth();
                }
            }
        }

        public void UpdateHappiness(int modifier)
        {
            PersistencyManager<SaveData>.Instance.SaveData.Happiness = Mathf.Clamp(PersistencyManager<SaveData>.Instance.SaveData.Happiness + modifier, -10, 10);
            PersistencyManager<SaveData>.Instance.Save();

            CheckForGrowth();

            _player.ShowThought(modifier > 0 ? _spriteHappy : _spriteUnhappy);
        }

        public void CheckForGrowth()
        {
            if (
                PersistencyManager<SaveData>.Instance.SaveData.Growth >= 1f &&
                PersistencyManager<SaveData>.Instance.SaveData.Happiness >= 5 &&
                PersistencyManager<SaveData>.Instance.SaveData.GrowthIndex < _ageData.Length - 1
            )
            {
                PersistencyManager<SaveData>.Instance.SaveData.GrowthIndex++;
                PersistencyManager<SaveData>.Instance.SaveData.Growth = 0f;
                PersistencyManager<SaveData>.Instance.SaveData.Happiness = 0;
                PersistencyManager<SaveData>.Instance.Save();

                _sr.sprite = _ageData[PersistencyManager<SaveData>.Instance.SaveData.GrowthIndex].Sprite;
            }
        }

        private Flavor GetRandomFlavor()
        {
            var flavors = System.Enum.GetValues(typeof(Flavor)).Cast<Flavor>().ToArray();
            return flavors[Random.Range(0, flavors.Length)];
        }

        public void ClearNeed(Need need, float percent)
        {
            var max = _needInfo[need].TimeToDepleat;
            PersistencyManager<SaveData>.Instance.SaveData.Needs[need] = Mathf.RoundToInt(Mathf.Clamp(PersistencyManager<SaveData>.Instance.SaveData.Needs[need] + (max * percent), 0f, max));
            PersistencyManager<SaveData>.Instance.Save();
        }
    }
}
