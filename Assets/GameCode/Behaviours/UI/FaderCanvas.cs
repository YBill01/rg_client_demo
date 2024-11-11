using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FaderCanvas : MonoBehaviour
{
	public static FaderCanvas Instance { get; private set; }
	public float ChangeSpeed;

	[SerializeField]
	private Image faderImage;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
	}

	private bool _hide;
	private UnityEvent _hideEvent = new UnityEvent();
	private UnityEvent _unhideEvent = new UnityEvent();
	private UnityEvent _changeStateEvent = new UnityEvent();
	private UnityEvent _completeStateEvent = new UnityEvent();
	public bool Hide {
		get { return _hide; }
		set {			
			if (_hide == value) return;
			_hide = value;
			ChangeState();
		}
	}

	public UnityEvent HideEvent { get => _hideEvent;}
	public UnityEvent UnhideEvent { get => _unhideEvent;}
	public UnityEvent ChangeStateEvent { get => _changeStateEvent; }
	public UnityEvent CompleteStateEvent { get => _completeStateEvent; }

	private float _changeProgress = 0;
	private bool _stateComplete = true;
	private void ChangeState()
	{
		_changeStateEvent.Invoke();
		if(_hide)
		{
			_changeProgress = -0.7f;
			_hideEvent.Invoke();
		}
		else
		{
			_unhideEvent.Invoke();
		}
		_stateComplete = false;
		if (faderImage == null) return;
		faderImage.enabled = true;
	}

	private void Update()
	{
		if (_stateComplete) return;
		if (_hide)
		{
			UpdateHide();
		}
		else
		{
			UpdateUnhide();
		}
		UpdateFaderImage();
	}

	private void UpdateFaderImage()
	{
		if (faderImage == null) return;
		var c = faderImage.color;
		c.a = Mathf.Clamp(_changeProgress, 0, 1);
		faderImage.color = c;
	}

	private void UpdateHide()
	{
		_changeProgress += ChangeSpeed * Time.deltaTime;
		if (_changeProgress >= 1)
		{
			_changeProgress = 1;
			_stateComplete = true;
			_completeStateEvent.Invoke();
		}
	}

	private void UpdateUnhide()
	{
		_changeProgress -= ChangeSpeed * Time.deltaTime;
		if (_changeProgress <= 0)
		{
			_changeProgress = 0;
			_stateComplete = true;
			_completeStateEvent.Invoke();
			if (faderImage == null) return;
			faderImage.enabled = false;
		}
	}

	public void ForceClear()
	{
		_changeProgress = 0;
		_stateComplete = true;
		if (faderImage == null) return;
		var c = faderImage.color;
		c.a = 0;
		faderImage.color = c;
		faderImage.enabled = false;
	}
}
