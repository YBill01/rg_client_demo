public interface IState
{
	void OnEnter();
	void OnUpdate();
	void OnLeave();
	void MonoThreadEnter();
	void MonoThreadUpdate();
	void MonoThreadLeave();
}
