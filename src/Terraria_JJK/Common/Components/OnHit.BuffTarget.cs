namespace Terraria_JJK.Components.OnHit;

[EntityComponent.Component]
public struct BuffTarget
{
	public int Type;
	public int Time;
}

internal class BuffTarget_Impl
{
	[DaybreakHooks.GlobalProjectileHooks.OnHitNPC]
	internal static void Projectile_HitNPC(Terraria.Projectile projectile, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone) {
		if (EC.TryGet<BuffTarget>(projectile, out var data))
			target.AddBuff(data.Type, data.Time);
	}

	[DaybreakHooks.GlobalNPCHooks.OnHitPlayer]
	internal static void NPC_HitPlayer(Terraria.NPC npc, Terraria.Player target, Terraria.Player.HurtInfo hurtInfo) {
		if (EC.TryGet<BuffTarget>(npc, out var data))
			target.AddBuff(data.Type, data.Time);
	}

	[DaybreakHooks.GlobalItemHooks.OnHitNPC]
	internal static void Item_HitNPC(Terraria.Item item, Terraria.Player player, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone) {
		if (EC.TryGet<BuffTarget>(item, out var data))
			target.AddBuff(data.Type, data.Time);
	}
}