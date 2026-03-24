namespace Terraria_JJK.Components;

[EntityComponent.Component]
public struct Buff
{
	public int Type;
	public int Duration;
	public bool TargetSelf;

	[DaybreakHooks.GlobalNPCHooks.AI]
	public static void ApplyToNPC(Terraria.NPC npc) {
		if (!EC.TryGet<Buff>(npc, out var data) || !data.TargetSelf) return;

		npc.AddBuff(data.Type, data.Duration);
		EC.Disable<Buff>(npc);
	}
}

[EntityComponent.Component(Wraps = [typeof(Buff)])]
public struct OnHit<T> where T : struct
{
	static OnHit() {
		DaybreakHooks.GlobalProjectileHooks.OnHitNPC.Event += OnHitNPC;
	}

	public T Inner;

	internal static void OnHitNPC(DaybreakHooks.GlobalProjectileHooks.OnHitNPC.Original orig, TML.GlobalProjectile self, Terraria.Projectile projectile, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone) {
		orig(projectile, target, hit, damageDone);
		if (!EC.TryGet<OnHit<T>>(projectile, out var data)) return;

		EC.With(target, data.Inner);
	}
}