using ECS = Terraria_JJK.EC.ComponentExtensions;

namespace Terraria_JJK.Components.OnHit;

[EC.Component]
public struct Shoot
{
	public (int[] Types, bool Random) Queue;
	public int Count;
	public System.Func<FNA.Vector2> Velocity;
	public System.Func<FNA.Vector2> RelativePosition;
}

internal class Shoot_Projectile : TML.GlobalProjectile
{
	public override void OnHitPlayer(Terraria.Projectile projectile, Terraria.Player target, Terraria.Player.HurtInfo info) {
		base.OnHitPlayer(projectile, target, info);
	}

	public override void OnHitNPC(Terraria.Projectile projectile, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone) {
		var query = ECS.Get<Shoot>(projectile);
		if (query is Shoot data) {
			var types = data.Queue.Types;
			for (int i = 0; i < data.Count; i++)
				Terraria.Projectile.NewProjectile(
					projectile.GetSource_FromThis(),
					data.RelativePosition() + target.Center,
					data.Velocity(),
					data.Queue.Random ? Terraria.Utils.NextFromList(Terraria.Main.rand, types) : types[i % types.Length],
					projectile.damage,
					projectile.knockBack,
					projectile.owner
				);
		}
	}
}