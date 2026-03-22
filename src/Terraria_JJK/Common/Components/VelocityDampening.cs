namespace Terraria_JJK.Components;

[EntityComponent.Component]
public record struct VelocityDampening(float Factor);

file class VelocityDampening_Impl
{
	[DaybreakHooks.GlobalProjectileHooks.AI]
	internal static void ApplyToProjectile(Terraria.Projectile projectile) {
		if (!EC.TryGet<VelocityDampening>(projectile, out var data)) return;

		projectile.velocity *= 1f - data.Factor;
	}
}