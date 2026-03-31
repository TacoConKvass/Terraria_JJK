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
		Item.UseSound = Terraria.ID.SoundID.Item5;

		Item.useAmmo = Terraria.ID.AmmoID.Arrow;
		Item.With(new Components.Shoots {
			Type = CalderaArrow.ID,
			Delay = 40,
			Velocity = static (orig) => orig * 10f
		});
	}
}

public class CalderaArrow : TML.ModProjectile
{
	public static int ID => TML.ModContent.ProjectileType<CalderaArrow>();

	public override string Texture => Terraria_JJK.AssetPath + $"Projectiles/{nameof(CalderaArrow)}";

	const int Duration = 10 * 60; // 10 seconds
	const int SpawnedFlames = 5;
	const float FlameSpeed = 4f;

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 12, Y = 12 };
		Projectile.timeLeft = 15 * 60; // 15 seconds
		Projectile.friendly = true;
		Projectile.aiStyle = Terraria.ID.ProjAIStyleID.Arrow;

		Projectile.With(new Components.RotateWithVelocity {
			AdditionalRotation = -FNA.MathHelper.PiOver2,
		});
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
				Velocity = static (_) => -FNA.Vector2.UnitY.RotatedBy(Terraria.Main.rand.NextFloat(-FNA.MathHelper.Pi / 6, FNA.MathHelper.Pi / 6)) * FlameSpeed
			},
		});
	}
}

public class VolcanicFlame : TML.ModProjectile
{
	public override string Texture => Terraria_JJK.AssetPath + $"Projectiles/{nameof(VolcanicFlame)}";

	public static int ID => TML.ModContent.ProjectileType<VolcanicFlame>();

	public override void SetStaticDefaults() {
		Terraria.Main.projFrames[Type] = 6;
	}

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 10, Y = 10 };
		Projectile.timeLeft = 3 * 60;
		Projectile.friendly = true;
		Projectile.aiStyle = Terraria.ID.ProjAIStyleID.Arrow;

		Projectile.With(new Components.OnHit<Components.Shoots> {
			Inner = new() {
				Type = Terraria.ID.ProjectileID.Flames,
				Count = 1,
				RelativePosition = static () => FNA.Vector2.Zero,
				Velocity = static (_) => FNA.Vector2.Zero,
			},
		});
		Projectile.With(new Components.OnTimer<Components.Fade> {
			Inner = new() {
				Starting = 1f,
				Final = 0.1f,
				Duration = 3 * 60, // A second and a half,
			},
			Timer = 60, // After a second
		});
		Projectile.With(new Components.RotateWithVelocity {
			AdditionalRotation = -FNA.MathHelper.PiOver2,
		});
		Projectile.With(new Components.WhileTimer<Components.CantDamage> {
			Timer = 60,
			Inner = default
		});
		Projectile.With(new Components.Animate { FrameDelay = 10 });
	}
}