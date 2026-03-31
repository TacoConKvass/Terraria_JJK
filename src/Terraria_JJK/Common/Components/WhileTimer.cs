namespace Terraria_JJK.Components;

[EC.Component(Wraps = typeof(ITimeable))]
public struct WhileTimer<T> where T : struct, ITimeable
{
	static WhileTimer() {
		DaybreakHooks.GlobalProjectileHooks.AI.Event += TickProjectile;
	}

	public T Inner;
	public int Timer;

	internal static void TickProjectile(DaybreakHooks.GlobalProjectileHooks.AI.Original orig, TML.GlobalProjectile self, Terraria.Projectile projectile) {
		orig(projectile);
		if (!projectile.TryGet<WhileTimer<T>>(out var data)) return;

		if (data.Timer > 0) {
			projectile.Set(data with { Timer = data.Timer - 1 });
			if (!projectile.Enabled<T>()) projectile.With(data.Inner);
			return;
		}

		projectile.Disable<WhileTimer<T>>();
		projectile.Disable<T>();
	}
}