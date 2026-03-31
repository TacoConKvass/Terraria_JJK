namespace Terraria_JJK.Components;

public enum TargetType : byte
{
	Self,
	Victim,
}

[EC.Component(Wraps = typeof(Core.ITriggerable))]
public record struct OnHit<T>(T Inner, TargetType Target = TargetType.Victim) where T : struct, Core.ITriggerable
{
	static OnHit() {
		DaybreakHooks.GlobalProjectileHooks.OnHitNPC.Event += OnProjectileHitNPC;
	}

	internal static void OnProjectileHitNPC(DaybreakHooks.GlobalProjectileHooks.OnHitNPC.Original orig, TML.GlobalProjectile self, Terraria.Projectile projectile, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone) {
		orig(projectile, target, hit, damageDone);

		if (!projectile.TryGet<OnHit<T>>(out var data)) return;
		data.Inner.Trigger(projectile, target, data.Target);
	}
}