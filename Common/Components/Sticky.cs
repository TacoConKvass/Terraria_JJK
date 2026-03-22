namespace Terraria_JJK.Components;

[EntityComponent.Component]
public struct Sticky
{
	public float TicksOfDamagePerSecond;
}

[EntityComponent.Component]
public struct StuckTo
{
	public Terraria.Entity Target;
	public System.Func<FNA.Vector2> WithOffset;
}

file class Sticky_Impl
{
	private sealed class SetStickyProjectileValues : TML.GlobalProjectile
	{
		public override void SetDefaults(Terraria.Projectile entity) {
			if (!EC.TryGet<Sticky>(entity, out var data)) return;
			entity.penetrate = -1;
			entity.usesLocalNPCImmunity = true;
			entity.localNPCHitCooldown = (int)(60 / data.TicksOfDamagePerSecond);
		}
	}

	[DaybreakHooks.GlobalProjectileHooks.OnHitNPC]
	internal static void StickToTargetNPC(Terraria.Projectile projectile, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone) {
		if (!EC.Enabled<Sticky>(projectile)) return;

		EC.Disable<Sticky>(projectile);

		var offset = projectile.Center - target.Center;
		EC.With(projectile, new StuckTo {
			Target = target,
			WithOffset = () => offset
		});
	}

	[DaybreakHooks.GlobalProjectileHooks.AI]
	internal static void MoveStuckProjectile(Terraria.Projectile projectile) {
		if (!EC.TryGet<StuckTo>(projectile, out var data)) return;
		if (!data.Target.active) {
			projectile.Kill();
			return;
		}
		projectile.Center = data.Target.Center + (data.WithOffset?.Invoke() ?? FNA.Vector2.Zero);
	}
}