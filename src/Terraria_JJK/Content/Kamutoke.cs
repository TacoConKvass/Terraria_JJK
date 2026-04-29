using static Terraria.Utils;

namespace Terraria_JJK.Content;

public class Kamutoke : TML.ModItem
{
	public override string Texture => Terraria_JJK.AssetPath($"Items/{nameof(Kamutoke)}");

	const float LightningSpeed = 1f;

	public override void SetDefaults() {
		Item.Size = new FNA.Vector2 { X = 58, Y = 58 };
		Item.damage = 10;
		Item.useStyle = Terraria.ID.ItemUseStyleID.Swing;
		Item.DamageType = TML.DamageClass.Ranged;

		Item.With(new Components.Shoots {
			Type = KamutokeLightning.ID,
			Delay = 60,
			Velocity = static (orig) => orig * LightningSpeed,
			RelativePosition = static () => FNA.Vector2.Zero,
		});
	}
}

public class KamutokeLightning : TML.ModProjectile
{
	public override string Texture => Terraria_JJK.AssetPath("Extra/Beam");

	public static int ID => TML.ModContent.ProjectileType<KamutokeLightning>();

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 20, Y = 20 };
		Projectile.friendly = true;
		Projectile.extraUpdates = 20;
		Projectile.timeLeft = 25 * Projectile.extraUpdates;

		Projectile.With(new Components.RotateWithVelocity { });
		Projectile.With(new Components.OnTimer<Components.SpawnDust> {
			Timer = 3,
			Resettable = true,
			Inner = new Components.SpawnDust {
				Type = Terraria.ID.DustID.WitherLightning,
				RelativePosition = () => FNA.Vector2.Zero,
				Velocity = () => FNA.Vector2.Zero,
				Count = 1,
				Alpha = 128,
				AffectedByGravity = false,
				Perfect = true,
			}
		});
		Projectile.With(new Components.OnTimer<Components.SpawnProjectile> {
			Timer = 30,
			Inner = new Components.SpawnProjectile {
				Type = ID,
				Velocity = ChildVelocity,
				Damage = Projectile.damage,
				Callback = ChildCallback
			},
		});
		Projectile.With(new Components.OnTimer<Components.ChangeVelocity> {
			Timer = Projectile.extraUpdates * 2,
			Resettable = true,
			Inner = new Components.ChangeVelocity {
				Change = ChangeVelocity
			}
		});
	}

	FNA.Vector2 ChildVelocity() => Projectile.velocity.RotatedBy(10 * (1 + Projectile.ai[1]) * FNA.MathHelper.TwoPi / 360f);

	static float[] angles = [-1f, 0.5f, 1f];
	FNA.Vector2 ChangeVelocity(FNA.Vector2 old) {
		if (Terraria.Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient)
			Projectile.ai[2] = angles[System.Math.Clamp((int)Projectile.ai[2] * -1, -1, 1) + 1] * Terraria.Main.rand.NextFloat(30, 45);
		return old.RotatedBy(Projectile.ai[2] * FNA.MathHelper.TwoPi / 360f);
	}


	void ChildCallback(Terraria.Projectile child) {
		if (Projectile.ai[1] > -1) {
			Projectile.ai[1] = -2;
			if (Terraria.Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient)
				Projectile.With(new Components.OnTimer<Components.SpawnProjectile> {
					Timer = 15,
					Inner = new Components.SpawnProjectile {
						Type = ID,
						Velocity = ChildVelocity,
						Damage = Projectile.damage,
						Callback = ChildCallback
					},
				});
		}
		if (Projectile.ai[0] > 1) {
			child.Disable<Components.OnTimer<Components.SpawnProjectile>>();
			return;
		}
		Projectile.ai[0]++;
		child.ai[0] = Projectile.ai[0];
		_ = child.TryGet(out Components.OnTimer<Components.SpawnProjectile> data);
		child.Set(data with {
			Timer = (int)(data.Timer * Terraria.Main.rand.NextFloat(0.75f, 3f))
		});
	}

	public override bool PreDraw(ref FNA.Color lightColor) => false;
}