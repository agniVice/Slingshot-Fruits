using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UserInterfaceMenu : MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _optionsPanel;

    [SerializeField] private TextMeshProUGUI _highscoreText;
    [SerializeField] private TextMeshProUGUI _localeText;

    [SerializeField] private Transform _star1;
    [SerializeField] private Transform _star2;
    [SerializeField] private Transform _star3;

    [SerializeField] private Slider _audioSlider;
    [SerializeField] private Slider _musicSlider;
    
    private void Start()
    {
        OpenMenu();
        UpdateLanguage();

        _highscoreText.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
    }
    public void OpenMenu()
    {
        _star1.localScale = Vector3.zero;
        _star2.localScale = Vector3.zero;
        _star3.localScale = Vector3.zero;

        _star1.DOScale(1, 0.2f).SetEase(Ease.OutBack).SetLink(_star1.gameObject).SetDelay(0.2f);
        _star2.DOScale(1, 0.4f).SetEase(Ease.OutBack).SetLink(_star2.gameObject).SetDelay(0.4f);
        _star3.DOScale(1, 0.6f).SetEase(Ease.OutBack).SetLink(_star3.gameObject).SetDelay(0.6f);

        _menuPanel.transform.localScale = Vector3.zero;
        _menuPanel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetLink(_menuPanel);
        _menuPanel.SetActive(true);
        _optionsPanel.SetActive(false);

        AudioVibrationManager.Instance.Save();
    }
    public void OpenOptions()
    {
        _optionsPanel.transform.localScale = Vector3.zero;
        _optionsPanel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetLink(_optionsPanel);
        _menuPanel.SetActive(false);
        _optionsPanel.SetActive(true);

        _audioSlider.value = AudioVibrationManager.Instance.Audio;
        _musicSlider.value = AudioVibrationManager.Instance.Music;
    }
    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://doc-hosting.flycricket.io/slingshot-fruits-privacy-policy/5b693db0-0314-484f-8894-cba24a462ca2/privacy");
        Debug.Log("Privacy Policy opened");
    }
    public void ChangeAudio()
    {
        AudioVibrationManager.Instance.ChangeAudio(_audioSlider.value);
    }
    public void ChangeMusic()
    {
        AudioVibrationManager.Instance.ChangeMusic(_musicSlider.value);
    }
    public void OnExitButtonClicked()
    {
        Application.Quit();
    }
    public void OnPlayButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Gameplay");
    }
    public void OnSettingsButtonClicked()
    {
        OpenOptions();
    }
    public void OnReturnToMenuButtonCliked()
    {
        OpenMenu();
    }
    public void OnPrivacyPolicyButtonClicked()
    {
        OpenPrivacyPolicy();
    }
    public void ToggleLanguage()
    {
        LanguageManager.Instance.ToggleLanguange();
        UpdateLanguage();
    }
    private void UpdateLanguage()
    {
        if ((int)LanguageManager.Instance.CurrentLanguage == 0)
            _localeText.text = "EN";
        else if ((int)LanguageManager.Instance.CurrentLanguage == 1)
            _localeText.text = "RU";
    }
}
