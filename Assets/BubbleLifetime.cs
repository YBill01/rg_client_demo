using UnityEngine;

public class BubbleLifetime : MonoBehaviour
{
    public float Lifetime;
    [SerializeField]
    private bool startOnAwake;
    [SerializeField]
    private AnimationAutoDestroy animationAutoDestroy;
    private bool started;
    private float _leftTime;
    void Start()
    {
        if (startOnAwake)
        {
            InitLifetime();
        }
    }

    public void InitLifetime()
    {
        if (started) return;
        started = true;
        _leftTime = Lifetime;
    }

    private bool finished;
    void Update()
    {
        if (finished) return;
        if (!started) return;
        _leftTime -= Time.deltaTime;
        if (_leftTime > 0) return;
        finished = true;
        if (animationAutoDestroy != null)
            animationAutoDestroy.Animator.SetTrigger("Close");
    }
}
