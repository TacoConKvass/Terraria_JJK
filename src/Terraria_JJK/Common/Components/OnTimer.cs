namespace Terraria_JJK.Components;

public interface ITimeable { }

[EC.Component(Wraps = typeof(ITimeable))]
public struct OnTimer<T> where T : struct, ITimeable
{
	static OnTimer() {
		DaybreakHooks.GlobalProjectileHooks.AI.Event += TickProjectile;
	}

	public T Inner;
	public int Timer;

	internal static void TickProjectile(DaybreakHooks.GlobalProjectileHooks.AI.Original orig, TML.GlobalProjectile self, Terraria.Projectile projectile) {
		orig(projectile);
		if (!projectile.TryGet<OnTimer<T>>(out var data)) return;

		if (data.Timer > 0) {
			projectile.Set(data with { Timer = data.Timer - 1 });
			return;
		}

		projectile.Disable<OnTimer<T>>();
		projectile.With(data.Inner);
	}
}