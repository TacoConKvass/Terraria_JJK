using static Terraria.Utils;

namespace Terraria_JJK.Components;

[EC.Component]
public record struct SpawnDust(
	int Type,
	(int[] Types, bool Random)? Queue,
	int Count,
	System.Func<FNA.Vector2> Velocity,
	System.Func<FNA.Vector2> RelativePosition,
	FNA.Color Color,
	int Alpha,
	float? Scale,
	System.Action? Callback
) : Core.ITriggerable, ITimeable
{
	[DaybreakHooks.GlobalProjectileHooks.AI]
	internal static void ProjectileUpdate(Terraria.Projectile projectile) {
		if (!projectile.TryGet(out SpawnDust data)) return;

		(data as Core.ITriggerable).Trigger(projectile, null!, TargetType.Self);
	}

	void Core.ITriggerable.Trigger(Terraria.Entity source, Terraria.Entity target, TargetType targetType) {
		if (targetType == TargetType.Self) {
			ExecuteSpawns(source.Center, source.Hitbox);
			if (source is Terraria.NPC npc) npc.Disable<SpawnDust>();
			else if (source is Terraria.Item item) item.Disable<SpawnDust>();
			else if (source is Terraria.Player player) player.Disable<SpawnDust>();
			else if (source is Terraria.Projectile projectile) projectile.Disable<SpawnDust>();
			return;
		}

		ExecuteSpawns(target.Center, target.Hitbox);

		if (target is Terraria.NPC npc2) npc2.Disable<SpawnDust>();
		else if (target is Terraria.Item item) item.Disable<SpawnDust>();
		else if (target is Terraria.Player player) player.Disable<SpawnDust>();
		else if (target is Terraria.Projectile projectile) projectile.Disable<SpawnDust>();
	}

	void ExecuteSpawns(FNA.Vector2 center, FNA.Rectangle bounds) {
		for (int i = 0; i < Count; i++) {
			var type = Type;
			if (Queue is (int[] Types, bool Random))
				type = Random ? Terraria.Main.rand.NextFromList(Types) : Types[i % Types.Length];

			var velocity = Velocity();

			Terraria.Dust.NewDust(
				center + RelativePosition(),
				bounds.Width, bounds.Height,
				type, velocity.X, velocity.Y,
				Alpha: Alpha, newColor: Color, Scale: Scale ?? 1
			);
		}

		Callback?.Invoke();
	}
}