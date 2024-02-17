using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class HUD : MonoBehaviour, IInitializable, ISubscriber
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _coins;
    [SerializeField] private TextMeshProUGUI _time;
    [SerializeField] private TextMeshProUGUI _hitPoints;
    [SerializeField] private Transform _coin;
    [SerializeField] private Transform _coinHud;

    [SerializeField] private List<Transform> _transforms = new List<Transform>();

    private bool _isInitialized;
    private void OnEnable()
    {
        if (!_isInitialized)
            return;

        SubscribeAll();
    }
    private void OnDisable()
    {
        UnsubscribeAll();
    }
    private void FixedUpdate()
    {
        if(GameState.Instance.CurrentState == GameState.State.InGame)
        {
            _time.text = GameTimer.Instance.Timer.ToString("F0");
        }
    }
    public void Initialize()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;

        Show();

        _isInitialized = true;

        _time.gameObject.SetActive(false);
        _hitPoints.GetComponent<CanvasGroup>().alpha = 0f;
        _coin.transform.localScale = Vector3.zero;

        UpdateScore();
    }
    public void SubscribeAll()
    {
        GameState.Instance.GameFinished += Hide;
        GameState.Instance.GamePaused += Hide;
        GameState.Instance.GameUnpaused += Show;

        GameState.Instance.ScoreAdded += UpdateScore;
        GameState.Instance.ScoreAdded += ShowHitPoints;
    }
    public void UnsubscribeAll()
    {
        GameState.Instance.GameFinished -= Hide;
        GameState.Instance.GamePaused -= Hide;
        GameState.Instance.GameUnpaused -= Show;

        GameState.Instance.ScoreAdded -= UpdateScore;
        GameState.Instance.ScoreAdded -= ShowHitPoints;
    }
    private void UpdateScore()
    {
        _coins.text = PlayerPrefs.GetInt("Coins", 0).ToString();
    }
    private void ShowHitPoints()
    {
        _hitPoints.transform.position = FindObjectOfType<Slingshot>().LastRock.transform.position;
        _hitPoints.transform.localScale = Vector3.zero;
        _hitPoints.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).SetLink(_hitPoints.gameObject);
        _hitPoints.GetComponent<CanvasGroup>().DOFade(1, 0.3f).SetLink(_hitPoints.gameObject);
        _hitPoints.text = "+" + PlayerScore.Instance.LastHitPoint;
        _hitPoints.GetComponent<CanvasGroup>().DOFade(0, 0.3f).SetLink(_hitPoints.gameObject).SetDelay(0.6f);
        _hitPoints.transform.DOScale(0, 0.3f).SetEase(Ease.OutBack).SetLink(_hitPoints.gameObject).SetDelay(0.6f);

        _coin.transform.localScale = Vector3.zero;
        _coin.transform.position = _hitPoints.transform.position;

        _coin.DOScale(1, 1f).SetLink(_coin.gameObject).SetEase(Ease.OutBack);
        _coin.DOMove(_coinHud.position, 0.7f).SetEase(Ease.Linear).SetLink(_coin.gameObject).OnKill(() => { _coin.localScale = Vector3.zero; });
    }
    private void Show()
    {
        foreach (Transform transform in _transforms)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(1, Random.Range(0.2f, 0.7f)).SetEase(Ease.OutBack).SetLink(transform.gameObject);
        }

        _panel.SetActive(true);
    }
    private void Hide()
    {
        _panel.SetActive(false);
    }
    public void OnRestartButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Gameplay");
    }
    public void OnPauseButtonClicked()
    {
        GameState.Instance.PauseGame();
    }
    public void StartTimer()
    {
        _time.gameObject.SetActive(true);
    }
}