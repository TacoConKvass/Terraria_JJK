namespace Terraria_JJK.Components;

public enum TargetType : byte
{
	Self,
	Victim,
}

[EC.Component(Wraps = [
	typeof(Fade), typeof(ApplyBuff), typeof(Shoots), typeof(Trail)
])]
public record struct OnHit<T>(T Inner, TargetType Target = TargetType.Victim) where T : struct
{
	static OnHit() {
		DaybreakHooks.GlobalProjectileHooks.OnHitNPC.Event += OnProjectileHitNPC;
	}

	internal static void OnProjectileHitNPC(DaybreakHooks.GlobalProjectileHooks.OnHitNPC.Original orig, TML.GlobalProjectile self, Terraria.Projectile projectile, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone) {
		orig(projectile, target, hit, damageDone);

		if (!projectile.TryGet<OnHit<T>>(out var data)) return;
		TryTrigger(projectile, target, data);
	}

	static void TryTrigger(Terraria.Entity source, Terraria.Entity entity, OnHit<T> data) {
		if (data.Inner is Core.ITriggerable i) {
			i.Trigger(source, entity);
			return;
		}

		if (data.Target == TargetType.Victim) {
			_ = entity switch {
				Terraria.NPC npc => npc.With(data.Inner),
				Terraria.Player player => player.With(data.Inner),
				_ => data.Inner,
			};
			return;
		}

		_ = source switch {
			Terraria.NPC npc => npc.With(data.Inner),
			Terraria.Item item => item.With(data.Inner),
			Terraria.Player player => player.With(data.Inner),
			Terraria.Projectile projectile => projectile.With(data.Inner),
			_ => data.Inner,
		};
	}
}