namespace Terraria_JJK.Content;

public class VolcanicImmolator : TML.ModItem
{
	public override string Texture => Terraria_JJK.AssetPath + $"Items/{nameof(VolcanicImmolator)}";

	public override void SetDefaults() {
		Item.Size = new FNA.Vector2 { X = 70, Y = 70 };
		Item.DamageType = TML.DamageClass.Ranged;
		Item.damage = 20;

		Item.useStyle = Terraria.ID.ItemUseStyleID.Shoot;

		Item.useAmmo = Terraria.ID.AmmoID.Arrow;
		EC.With(Item, new Components.Shoots {
			Type = CalderaArrow.ID,
			Delay = 35,
			Velocity = static (orig) => orig * 5f
		});
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
		Projectile.Size = new FNA.Vector2 { X = 12, Y = 12 };
		Projectile.timeLeft = 15 * 60; // 15 seconds
		Projectile.friendly = true;
		Projectile.aiStyle = Terraria.ID.ProjAIStyleID.Arrow;

		EC.With(Projectile, new Components.OnHit.BuffTarget {
			Type = Terraria.ID.BuffID.CursedInferno,
			Time = Duration
		});
		EC.With(Projectile, new Components.OnHit.Shoot {
			Type = Terraria.ID.ProjectileID.CursedArrow,
			Count = SpawnedFlames,
			RelativePosition = static () => FNA.Vector2.Zero,
			Velocity = static () => Terraria.Utils.NextVector2Unit(Terraria.Main.rand) * FlameSpeed
		});
	}
}