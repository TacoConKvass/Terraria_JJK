namespace Terraria_JJK.Components;

[EC.Component]
public record struct CantDamage
{
	[DaybreakHooks.GlobalProjectileHooks.AI]
	static void UpdateProjectileTimer(Terraria.Projectile projectile) {
		if (!projectile.TryGet(out CantDamage data)) return;
	}

	[DaybreakHooks.GlobalProjectileHooks.CanHitNPC]
	static bool? ProjectileCanHitNPC(DaybreakHooks.GlobalProjectileHooks.CanHitNPC.Original orig, Terraria.Projectile projectile, Terraria.NPC target) {
		return projectile.Enabled<CantDamage>() ? false : orig(projectile, target);
	}
}