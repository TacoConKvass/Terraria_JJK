namespace Terraria_JJK.Components;

[EC.Component]
public record struct Sticky(float TicksOfDamagePerSecond) : Core.ITriggerable
{
	private sealed class SetStickyProjectileValues : TML.GlobalProjectile
	{
		public override void SetDefaults(Terraria.Projectile projectile) {
			if (!projectile.TryGet<Sticky>(out var data)) return;
			projectile.penetrate = -1;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = (int)(60 / data.TicksOfDamagePerSecond);
		}
	}

	public void Trigger(Terraria.Entity source, Terraria.Entity target, TargetType targetType) {
		Core.ITriggerable.Default(source, target, targetType, new StuckTo {
			Target = target,
			WithOffset = () => source.Center - target.Center
		});

		source.Disable<Sticky>();
	}
}

[EC.Component]
public record struct StuckTo(Terraria.Entity Target, System.Func<FNA.Vector2>? WithOffset)
{
	[DaybreakHooks.GlobalProjectileHooks.AI]
	internal static void MoveStuckProjectile(Terraria.Projectile projectile) {
		if (!projectile.TryGet<StuckTo>(out var data)) return;
		if (!data.Target.active) {
			projectile.Kill();
			return;
		}
		projectile.Center = data.Target.Center + (data.WithOffset?.Invoke() ?? FNA.Vector2.Zero);
	}
}