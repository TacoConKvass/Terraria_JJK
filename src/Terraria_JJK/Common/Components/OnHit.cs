namespace Terraria_JJK.Components;

public enum TargetType : byte
{
	Self,
	Victim,
	None,
}

[EC.Component(Wraps = [
	typeof(Fade), typeof(ApplyBuff), typeof(Shoots)
])]
public record struct OnHit<T>(T Inner) where T : struct
{
	static OnHit() {
		DaybreakHooks.GlobalProjectileHooks.OnHitNPC.Event += OnProjectileHitNPC;
	}

	internal static void OnProjectileHitNPC(DaybreakHooks.GlobalProjectileHooks.OnHitNPC.Original orig, TML.GlobalProjectile self, Terraria.Projectile projectile, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone) {
		orig(projectile, target, hit, damageDone);

		if (!projectile.TryGet<OnHit<T>>(out var data)) return;
		TryTrigger(projectile, target, data.Inner);
	}

	static void TryTrigger(Terraria.Entity source, Terraria.Entity entity, T data) {
		if (data is not Core.ITriggerable i) return;
		i.Trigger(source, entity);
	}
}