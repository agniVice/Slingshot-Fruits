using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public bool IsProtected = false;
    public bool IsRandomSprite = false;

    public List<Sprite> Sprites;

    private SpriteRenderer _spriteRenderer;

    private bool _isCollided = false;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (IsProtected)
        {
            _spriteRenderer.DOFade(0.2f, 0.2f).SetLink(gameObject);
        }
        if(IsRandomSprite && !IsProtected)
        {
            if(Sprites.Count != 0)
                _spriteRenderer.sprite = Sprites[Random.Range(0, Sprites.Count)];
        }

        gameObject.AddComponent<PolygonCollider2D>();

        Activate();
    }
    public void Spawn(Vector2 position)
    {
        _spriteRenderer.DOFade(0, 0.6f).SetLink(gameObject).OnKill(() => { transform.position = position; });
        _spriteRenderer.DOFade(1, 0.6f).SetLink(gameObject).SetDelay(0.6f);
    }
    public void Activate()
    { 
        IsProtected = false;
        _spriteRenderer.color = new Color32(255, 255, 255, 255);
        gameObject.layer = LayerMask.NameToLayer("FruitEnabled");
    }
    public void Deactivate() 
    {
        IsProtected = true;
        _spriteRenderer.color = new Color32(255, 255, 255, 100);
        gameObject.layer = LayerMask.NameToLayer("FruitDisabled");
    }
    private void DestroyMe()
    {
        _spriteRenderer.DOFade(0, 0.3f).SetLink(gameObject);
        _spriteRenderer.DOFade(1, 0.3f).SetLink(gameObject).SetDelay(0.3f);
        _spriteRenderer.DOFade(0f, 0.3f).SetLink(gameObject).SetDelay(0.6f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!_isCollided)
        {
            if (collision.gameObject.CompareTag("Rock"))
            {
                _isCollided = true;

                collision.gameObject.tag = "Player";
                collision.gameObject.GetComponent<Rock>().DestroyMe();

                AudioVibrationManager.Instance.PlaySound(AudioVibrationManager.Instance.Win, Random.Range(1f, 1.1f));

                DestroyMe();
                GameTimer.Instance.ResetTimer();
                GameState.Instance.AddScore();
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Trigger"))
        {
            if (collision.gameObject.GetComponent<Trigger>().Acvtivate)
                Activate();
            else
                Deactivate();
        }
    }
}