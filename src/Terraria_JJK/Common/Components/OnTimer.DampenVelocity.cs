namespace Terraria_JJK.Components.OnTimer;

[EntityComponent.Component]
public struct DampenVelocity
{
	public float Factor;
	public float Timer;
}

file class DampenVelocity_Impl
{
	[DaybreakHooks.GlobalProjectileHooks.AI]
	internal static void ApplyToProjectile(Terraria.Projectile projectile) {
		if (!EC.TryGet<DampenVelocity>(projectile, out var data)) return;

		if (data.Timer > 0) {
			EC.Set(projectile, data with { Timer = data.Timer - 1 });
			return;
		}

		EC.Disable<DampenVelocity>(projectile);
		EC.With(projectile, new VelocityDampening { Factor = data.Factor });
	}
}