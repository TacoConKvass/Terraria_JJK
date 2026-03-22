namespace Terraria_JJK.Components.OnHit;

[EntityComponent.Component]
public struct Shoot
{
	public Shoot() {
		Type = Terraria.ID.ProjectileID.PurificationPowder;
		Count = 1;
		Velocity = static () => FNA.Vector2.Zero;
		RelativePosition = static () => FNA.Vector2.Zero;
	}

	public int Type;
	public (int[] Types, bool Random)? Queue;
	public int Count;
	public System.Func<FNA.Vector2> Velocity;
	public System.Func<FNA.Vector2> RelativePosition;
	public System.Action? Callback;
}

internal class Shoot_Impl
{
	[DaybreakHooks.GlobalProjectileHooks.OnHitNPC]
	internal static void Projectile_HitNPC(Terraria.Projectile projectile, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone) {
		if (!EC.TryGet<Shoot>(projectile, out var data)) return;

		for (int i = 0; i < data.Count; i++) {
			var type = data.Type;

			if (data.Queue is (int[] Types, bool Random) queue)
				type = queue.Random ? Terraria.Utils.NextFromList(Terraria.Main.rand, queue.Types) : queue.Types[i % queue.Types.Length];

			Terraria.Projectile.NewProjectile(
				projectile.GetSource_FromThis(),
				data.RelativePosition() + target.Center,
				data.Velocity(),
				type,
				projectile.damage,
				projectile.knockBack,
				projectile.owner
			);
		}

		data.Callback?.Invoke();
	}
}