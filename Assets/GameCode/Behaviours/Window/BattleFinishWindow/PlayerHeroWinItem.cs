using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class PlayerHeroWinItem : MonoBehaviour
{
    public byte stars;
    public int exp;
    public ushort hero;
    public bool winner;
    public byte side;
    public GameObject glow;
    public GameObject shield;
    public Image heroImage;
    public TextMeshProUGUI heroName;
    [SerializeField]
    private Animator starsAnimator;
    void Start()
    {
        starsAnimator = (starsAnimator == null)?GetComponentInChildren<Animator>(): starsAnimator;
    }

    void Update()
    {
        
    }

    public void ResetView()
    {
        starsAnimator.SetInteger("Stars", stars);

        if (!winner) return;
        glow.SetActive(true);

        var tsd = shield.transform.DOPunchScale(new Vector3(0.05f, 0.05f), 1.5f, 1, 1f);
        tsd.SetDelay(4);
        tsd.onComplete += PunchComplete;
    }

    private void PunchComplete()
    {
        var tsd = shield.transform.DOPunchScale(new Vector3(0.05f, 0.05f), 1.5f, 1, 1f);
        tsd.SetDelay(4);
        tsd.onComplete += PunchComplete;
    }
}
