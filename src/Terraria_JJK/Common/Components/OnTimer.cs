namespace Terraria_JJK.Components;

public interface ITimeable { }

[EC.Component(Wraps = typeof(ITimeable))]
public struct OnTimer<T> where T : struct, ITimeable
{
	static OnTimer() {
		DaybreakHooks.GlobalProjectileHooks.AI.Event += TickProjectile;
	}

	public bool Resettable;
	public T Inner;
	public int Timer;
	private int? orig_timer;

	internal static void TickProjectile(DaybreakHooks.GlobalProjectileHooks.AI.Original orig, TML.GlobalProjectile self, Terraria.Projectile projectile) {
		orig(projectile);
		if (!projectile.TryGet<OnTimer<T>>(out var data)) return;

		if (data.Timer > 0) {
			projectile.Set(data with {
				Timer = data.Timer - 1,
				orig_timer = data.orig_timer ?? data.Timer
			});
			return;
		}

		if (!data.Resettable) projectile.Disable<OnTimer<T>>();
		else {
			projectile.Set(data with { Timer = data.orig_timer!.Value });
		}
		projectile.With(data.Inner);
	}
}