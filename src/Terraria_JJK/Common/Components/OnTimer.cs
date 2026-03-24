namespace Terraria_JJK.Components;

[EntityComponent.Component]
public struct OnTimer<T> where T : struct
{
	static OnTimer() {
		DaybreakHooks.GlobalProjectileHooks.AI.Event += TickProjectile;
	}

	public T Inner;
	public int Timer;

	internal static void TickProjectile(DaybreakHooks.GlobalProjectileHooks.AI.Original orig, TML.GlobalProjectile self, Terraria.Projectile projectile) {
		orig(projectile);
		if (!EC.TryGet<OnTimer<T>>(projectile, out var data)) return;

		if (data.Timer > 0) {
			EC.Set(projectile, data with { Timer = data.Timer - 1 });
			return;
		}

		EC.Disable<OnTimer<T>>(projectile);
		EC.With(projectile, data.Inner);
	}
}