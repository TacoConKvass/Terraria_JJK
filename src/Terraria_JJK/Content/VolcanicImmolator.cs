using static Terraria.Utils;

namespace Terraria_JJK.Content;

public class VolcanicImmolator : TML.ModItem
{
	public override string Texture => Terraria_JJK.AssetPath + $"Items/{nameof(VolcanicImmolator)}";

	public override void SetDefaults() {
		Item.Size = new FNA.Vector2 { X = 32, Y = 70 };
		Item.DamageType = TML.DamageClass.Ranged;
		Item.damage = 20;

		Item.useStyle = Terraria.ID.ItemUseStyleID.Shoot;

		Item.useAmmo = Terraria.ID.AmmoID.Arrow;
		Item.With(new Components.Shoots {
			Type = CalderaArrow.ID,
			Delay = 40,
			Velocity = static (orig) => orig * 8f
		});
	}
}

public class CalderaArrow : TML.ModProjectile
{
	public static int ID => TML.ModContent.ProjectileType<CalderaArrow>();

	public override string Texture => $"Terraria/Images/{nameof(Terraria.Projectile)}_{Terraria.ID.ProjectileID.FireArrow}";

	const int Duration = 10 * 60; // 10 seconds
	const int SpawnedFlames = 5;
	const float FlameSpeed = 6f;

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 12, Y = 12 };
		Projectile.timeLeft = 15 * 60; // 15 seconds
		Projectile.friendly = true;
		Projectile.aiStyle = Terraria.ID.ProjAIStyleID.Arrow;

		Projectile.With(new Components.OnHit<Components.ApplyBuff> {
			Inner = new() {
				Type = Terraria.ID.BuffID.CursedInferno,
				Duration = Duration,
			},
			Target = Components.TargetType.Victim
		});
		Projectile.With(new Components.OnHit<Components.Shoots> {
			Inner = new() {
				Type = VolcanicFlame.ID,
				Count = SpawnedFlames,
				RelativePosition = static () => FNA.Vector2.Zero,
				Velocity = static (_) => Terraria.Utils.NextVector2Unit(Terraria.Main.rand) * FlameSpeed
			},
			Target = Components.TargetType.None,
		});
	}
}

public class VolcanicFlame : TML.ModProjectile
{
	// public override string Texture => Terraria_JJK.AssetPath + $"Projectiles/{nameof(ResonantNail)}";
	public override string Texture => $"Terraria/Images/Item_{Terraria.ID.ItemID.LivingFireBlock}";

	public static int ID => TML.ModContent.ProjectileType<VolcanicFlame>();

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 10, Y = 10 };
		Projectile.timeLeft = (int)(2.5f * 60);
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;

		Projectile.With(new Components.OnTimer<Components.DampenVelocity> {
			Inner = new() {
				Factor = 0.05f,
				MinVelocity = 0.1f,
			},
			Timer = 60 / 10 // Trigger after a tenth of a second
		});
		Projectile.With(new Components.OnTimer<Components.Fade> {
			Inner = new() {
				Starting = 1f,
				Final = 0.1f,
				Duration = (int)(1.5f * 60), // A second and a half,
			},
			Timer = 60, // After a second
		});
	}
}