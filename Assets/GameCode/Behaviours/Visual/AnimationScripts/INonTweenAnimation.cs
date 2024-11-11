using UnityEngine;

namespace Legacy.Visual.NonTweenAnimations
{
	public interface INonTweenAnimation
	{
		GameObject target { get; set; }
		bool Fade { get; set; }
		bool IsZero { get; }
		void Update();
	}
}