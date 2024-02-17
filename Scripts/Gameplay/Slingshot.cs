using DG.Tweening;
using UnityEngine;

public class Slingshot : MonoBehaviour, ISubscriber
{
    public LineRenderer[] lineRenderers;
    public Transform[] stripPositions;
    public Transform center;
    public Transform idlePosition;

    public Vector3 currentPosition;

    public float maxLength;

    public float bottomBoundary;

    bool isMouseDown;

    public GameObject rockPrefab;

    public float rockPositionOffset;

    private Rigidbody2D rock;
    private Collider2D rockCollider;

    public float force;
    public GameObject LastRock;

    void Start()
    {
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        CreateRock();

        transform.localScale = Vector3.zero;
        transform.DOScale(0.5f, 0.4f).SetEase(Ease.OutBack).SetLink(gameObject);
    }

    void CreateRock()
    {
        rock = Instantiate(rockPrefab).GetComponent<Rigidbody2D>();
        rockCollider = rock.GetComponent<Collider2D>();
        rockCollider.enabled = false;

        rock.isKinematic = true;

        //ResetStrips();
    }

    void Update()
    {
        if (isMouseDown)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;

            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            currentPosition = center.position + Vector3.ClampMagnitude(currentPosition
                - center.position, maxLength);

            currentPosition = ClampBoundary(currentPosition);

            SetStrips(currentPosition);

            if (rockCollider)
            {
                rockCollider.enabled = true;
            }
        }
        else
        {
            ResetStrips();
        }
    }

    private void OnMouseDown()
    {
        if (GameState.Instance.CurrentState != GameState.State.InGame)
            return;

        AudioVibrationManager.Instance.PlaySound(AudioVibrationManager.Instance.SlingShot, Random.Range(0.85f, 1.1f), 0.7f);
        isMouseDown = true;
    }

    private void OnMouseUp()
    {
        if (GameState.Instance.CurrentState != GameState.State.InGame)
            return;

        isMouseDown = false;
        Shoot();
        currentPosition = idlePosition.position;
    }

    void Shoot()
    {
        AudioVibrationManager.Instance.PlaySound(AudioVibrationManager.Instance.ShotEmpty, Random.Range(0.85f, 1.1f));
        if (rock == null)
            return;

        AudioVibrationManager.Instance.PlaySound(AudioVibrationManager.Instance.ShotFull, Random.Range(0.85f, 1.1f));
        GameTimer.Instance.StartTimer();
        FindObjectOfType<HUD>().StartTimer();


        rock.isKinematic = false;
        Vector3 rockForce = (currentPosition - center.position) * force * -1;
        rock.velocity = rockForce;

        rock.GetComponent<Rock>().Release();
        LastRock = rock.gameObject;

        rock = null;
        rockCollider = null;

        //Invoke("CreateRock", 1);
    }
    public void ResetRock()
    {
        Invoke("CreateRock", 1.5f);
    }
    void ResetStrips()
    {
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
    }

    void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);

        if (rock)
        {
            Vector3 dir = position - center.position;
            rock.transform.position = position + dir.normalized * rockPositionOffset;
            rock.transform.right = -dir.normalized;
        }
    }

    Vector3 ClampBoundary(Vector3 vector)
    {
        vector.y = Mathf.Clamp(vector.y, bottomBoundary, 1000);
        return vector;
    }

    public void SubscribeAll()
    {
        GameState.Instance.ScoreAdded += ResetRock;
    }

    public void UnsubscribeAll()
    {
        GameState.Instance.ScoreAdded -= ResetRock;
    }
}