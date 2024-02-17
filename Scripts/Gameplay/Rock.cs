using UnityEngine;
using DG.Tweening;

public class Rock : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private TrailRenderer _trailRenderer;
    private void Start()
    {
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _trailRenderer.enabled = false;
    }
    public void Release()
    {
        _trailRenderer.enabled = true;
    }
    public void DestroyMe()
    {
        GetComponent<Collider2D>().enabled = false;

        _trailRenderer.gameObject.transform.parent = null;
        _trailRenderer.time = 0.1f;
        _spriteRenderer.DOFade(0f, 1.5f).SetLink(gameObject).OnKill(KillMe);
    }
    private void KillMe()
    {
        Destroy(gameObject, 0.2f);
    }
}