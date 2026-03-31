namespace Terraria_JJK.Components;

[EC.Component]
public record struct ApplyBuff(int Type, int Duration) : Core.ITriggerable
{
	void Core.ITriggerable.Trigger(Terraria.Entity source, Terraria.Entity target, TargetType targetType) {
		if (targetType == TargetType.Self) {
			switch (source) {
				case Terraria.NPC npc: npc.AddBuff(Type, Duration); break;
				case Terraria.Player player: player.AddBuff(Type, Duration); break;
				case Terraria.Projectile projectile:
					if (projectile.friendly)
						Terraria.Main.player[projectile.owner].AddBuff(Type, Duration);
					if (projectile.hostile)
						Terraria.Main.npc[projectile.owner].AddBuff(Type, Duration);
					break;
				default: break;
			}
		}
	}
}
