using CsvHelper;

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
			Type = VolcanicFlame.ID,
			Count = SpawnedFlames,
			RelativePosition = static () => FNA.Vector2.Zero,
			Velocity = static () => Terraria.Utils.NextVector2Unit(Terraria.Main.rand) * FlameSpeed
		});
	}
}

public class VolcanicFlame : TML.ModProjectile
{
	public override string Texture => Terraria_JJK.AssetPath + $"Projectiles/{nameof(ResonantNail)}";

	public static int ID => TML.ModContent.ProjectileType<VolcanicFlame>();

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 10, Y = 10 };
		Projectile.timeLeft = 20 * 60;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;

		EC.With(Projectile, new Components.OnTimer.DampenVelocity {
			Factor = 0.05f,
			Timer = 0.1f * 60, // trigger after a tenth of a second
		});
	}
}