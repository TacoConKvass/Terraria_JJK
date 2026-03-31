namespace Terraria_JJK.Components;

[EC.Component]
public struct Fade : ITimeable
{
	public float Starting;
	public float Final;
	int duration;
	int max_duration;
	public int Duration {
		get => duration;
		set {
			if (duration == 0) max_duration = value;
			duration = value;
		}
	}
	public System.Action<Terraria.Entity>? Callback;

	[DaybreakHooks.GlobalProjectileHooks.AI]
	internal static void ApplyFadeToProjectile(Terraria.Projectile projectile) {
		if (!projectile.TryGet<Fade>(out var data)) return;

		if (data.Duration > 0) {
			projectile.alpha = (int)(255 * (Terraria.Utils.GetLerpValue(data.Starting, data.Final, data.duration / (float)data.max_duration)));
			projectile.Set(data with { Duration = data.Duration - 1 });
			return;
		}

		projectile.Disable<Fade>();
		data.Callback?.Invoke(projectile);
	}
}
