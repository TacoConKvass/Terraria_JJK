namespace Terraria_JJK.Components;

[EntityComponent.Component]
public struct RotateWithVelocity
{
	public float AdditionalRotation;
}

file class RotateWithVelocity_Impl
{
	[DaybreakHooks.GlobalProjectileHooks.AI]
	internal static void RotateProjectile(Terraria.Projectile projectile) {
		if (!EC.TryGet<RotateWithVelocity>(projectile, out var data)) return;

		projectile.rotation = Terraria.Utils.ToRotation(projectile.velocity) + data.AdditionalRotation;
	}


	[DaybreakHooks.GlobalNPCHooks.AI]
	internal static void RotateNPC() {
		// if (!EC.TryGet<RotateWithVelocity>(npc, out var data)) return;

		// npc.rotation = Terraria.Utils.ToRotation(npc.velocity) + data.AdditionalRotation;
	}
}