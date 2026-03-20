using ECS = Terraria_JJK.EC.ComponentExtensions;

namespace Terraria_JJK.Content;

public class VolcanicImmolator : TML.ModItem
{
	public override string Texture => Terraria_JJK.AssetPath + $"Items/{nameof(VolcanicImmolator)}";

	public override void SetDefaults() {
		Item.Size = new FNA.Vector2 { X = 70, Y = 70 };
		Item.DamageType = TML.DamageClass.Ranged;
		Item.damage = 20;

		Item.shoot = CalderaArrow.ID;
		Item.shootSpeed = 10f;
		Item.useStyle = Terraria.ID.ItemUseStyleID.Shoot;
		Item.useTime = 10;
		Item.useAnimation = 10;

		Item.useAmmo = Terraria.ID.AmmoID.Arrow;
	}

	public override void ModifyShootStats(Terraria.Player player, ref FNA.Vector2 position, ref FNA.Vector2 velocity, ref int type, ref int damage, ref float knockback) {
		type = CalderaArrow.ID;
	}
}

public class CalderaArrow : TML.ModProjectile
{
	public static int ID => TML.ModContent.ProjectileType<CalderaArrow>();

	public override string Texture => $"Terraria/Images/{nameof(Terraria.Projectile)}_{Terraria.ID.ProjectileID.FireArrow}";

	const int Duration = 10 * 60; // 10 seconds
	const int SpawnedFlames = 5;
	const float FlameSpeed = 12f;

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 16, Y = 16 };
		Projectile.timeLeft = 15 * 60; // 15 seconds
		Projectile.friendly = true;
		Projectile.aiStyle = Terraria.ID.ProjAIStyleID.Arrow;

		ECS.Set(Projectile, new Components.OnHit.BuffTarget {
			Type = Terraria.ID.BuffID.CursedInferno,
			Time = Duration
		});
		ECS.Set(Projectile, new Components.OnHit.Shoot {
			Queue = (Types: [Terraria.ID.ProjectileID.CursedArrow], Random: false),
			Count = SpawnedFlames,
			RelativePosition = static () => FNA.Vector2.Zero,
			Velocity = static () => Terraria.Utils.NextVector2Unit(Terraria.Main.rand) * FlameSpeed
		});
	}
}