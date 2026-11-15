using BubbleFriend.Persistency;
using Sketch.Persistency;
using Sketch.Translation;
using System;
using System.Linq;
using TMPro;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BubbleFriend
{
    [AutoStaticsCleanup]
    public partial class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance { private set; get; }

        [SerializeField]
        private Animator _collapseAnim, _settingsAnim;

        [SerializeField]
        private TMP_Dropdown _languageDropdown;

        [SerializeField]
        private GameObject _soapFlavorsMenu;

        [SerializeField]
        private TMP_Text _deleteSavesText;

        [SerializeField]
        private Toggle _flashToggle;

        private bool _isCollapsed = false;
        private bool _isSettings = false;

        private bool _deleteSaveConfirm = false;

        private void Awake()
        {
            Instance = this;

            _settingsAnim.SetBool("IsCollapsed", false);
            ToggleFlavorMenu(false);
        }

        private void Start()
        {
            var lang = PersistencyManager<SaveData>.Instance.SaveData.CurrentLanguage;
            Translate.Instance.CurrentLanguage = lang;
            _languageDropdown.value = _languageDropdown.options.IndexOf(_languageDropdown.options.First(x => x.text == lang));
            _flashToggle.isOn = PersistencyManager<SaveData>.Instance.SaveData.FlashOnNotification;
        }

        public void UpdateLanguage(int value)
        {
            var lang = _languageDropdown.options[value].text;
            Translate.Instance.CurrentLanguage = lang;
            PersistencyManager<SaveData>.Instance.SaveData.CurrentLanguage = lang;
            PersistencyManager<SaveData>.Instance.Save();

            BubbleManager.Instance.UpdateDebugText();
        }

        public void ToggleCollapse()
        {
            _isCollapsed = !_isCollapsed;
            _collapseAnim.SetBool("IsCollapsed", _isCollapsed);

            if (_isCollapsed)
            {
                ToggleFlavorMenu(false);
            }
        }

        public void ToggleSettings()
        {
            if (!_isCollapsed) // Close care menu when opening settings
            {
                _isCollapsed = true;
                _collapseAnim.SetBool("IsCollapsed", true);
                ToggleFlavorMenu(false);
            }
            _isSettings = !_isSettings;
            _settingsAnim.SetBool("IsCollapsed", _isSettings);
            _deleteSaveConfirm = false;
            _deleteSavesText.text = Translate.Instance.Tr("reset_save");
        }

        public void DeleteSaves()
        {
            if (!_deleteSaveConfirm)
            {
                _deleteSaveConfirm = true;
                _deleteSavesText.text = Translate.Instance.Tr("confirm");
            }
            else
            {
                PersistencyManager<SaveData>.Instance.DeleteSave();
                SceneManager.LoadScene("Main");
            }
        }

        public void ToggleFlavorMenu(bool value)
        {
            _soapFlavorsMenu.SetActive(value);
        }

        public void ToggleAllowFlash(bool value)
        {
            SystemManager.Instance.AllowWindowFlash = value;
            PersistencyManager<SaveData>.Instance.SaveData.FlashOnNotification = value;
            PersistencyManager<SaveData>.Instance.Save();
        }
    }
}
