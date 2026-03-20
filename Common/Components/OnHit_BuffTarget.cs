using ECS = Terraria_JJK.EC.ComponentExtensions;

namespace Terraria_JJK.Components.OnHit;

[EC.Component]
public struct BuffTarget
{
	public int Type;
	public int Time;
}

internal class BuffTarget_Projectile : TML.GlobalProjectile
{
	public override void OnHitNPC(Terraria.Projectile projectile, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone) {
		var query = ECS.Get<BuffTarget>(projectile);
		if (query is BuffTarget data)
			target.AddBuff(data.Type, data.Time);
	}
}