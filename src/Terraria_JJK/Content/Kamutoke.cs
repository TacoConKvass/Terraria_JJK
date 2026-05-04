using static Terraria.Utils;
using ArmMode = Terraria_JJK.Components.SpecialUseStyle.ArmMode;

namespace Terraria_JJK.Content;

public class Kamutoke : TML.ModItem
{
	public override string Texture => Terraria_JJK.AssetPath($"Items/{nameof(Kamutoke)}");

	const float LightningSpeed = 2f;

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
		Item.With(new Components.SpecialUseStyle {
			Rotation = static (player) => {
				var gravity_adjust = new FNA.Vector2 { X = 1, Y = player.gravDir };
				return (player.MountedCenter.DirectionTo(Terraria.Main.MouseWorld) * gravity_adjust).ToRotation();
			},
			Location = static (player, rotation) => rotation.ToRotationVector2() * -1 * Core.Const.TileSize * 0.5f,
			Diagonal = true,
			CompositeArmMode = ArmMode.Front | ArmMode.Front,
		});
	}
}

public class KamutokeLightning : TML.ModProjectile
{
	public override string Texture => Terraria_JJK.AssetPath("Extra/Beam");

	public static int ID => TML.ModContent.ProjectileType<KamutokeLightning>();

	const int TicksOfMovementPerStraight = 20;
	const int Turns = 15;

	ref float LevelInHierarchy(Terraria.Projectile projectile) => ref projectile.ai[0];
	ref float SpawnedChildrenCount => ref Projectile.ai[1];
	ref float TurnDirection => ref Projectile.ai[2];

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 16, Y = 16 };
		Projectile.friendly = true;
		Projectile.extraUpdates = TicksOfMovementPerStraight;
		Projectile.timeLeft = Turns * TicksOfMovementPerStraight;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
		Projectile.penetrate = 1;

		Projectile.With(new Components.Trail {
			MaxPositions = Projectile.timeLeft,
			Width = static (progress) => progress > 0 && progress < 1 ? 2 : 0,
			Color = static (_) => FNA.Color.Aqua,
			Delay = TicksOfMovementPerStraight,
			FadeSpeed = 1,
		});
		Projectile.With(new Components.Trail.AddPosition { }); // Force first position to be added
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
			Timer = TicksOfMovementPerStraight,
			Resettable = true,
			Inner = new Components.ChangeVelocity {
				Change = ChangeVelocity
			}
		});
	}

	FNA.Vector2 ChildVelocity() => Projectile.velocity.RotatedBy(10 * LevelInHierarchy(Projectile) * FNA.MathHelper.TwoPi / 360f);

	static float[] angles = [-1f, 0.5f, 1f];

	FNA.Vector2 ChangeVelocity(FNA.Vector2 old) {
		foreach (var npc in Terraria.Main.ActiveNPCs) {
			if (!npc.friendly && Projectile.Center.DistanceSQ(npc.Center) < 64 * 64)
				return Projectile.DirectionTo(npc.Center);
		}

		if (Terraria.Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient)
			TurnDirection = angles[System.Math.Clamp((int)TurnDirection * -1, -1, 1) + 1] * Terraria.Main.rand.NextFloat(30, 45);

		return old.RotatedBy(TurnDirection * FNA.MathHelper.TwoPi / 360f);
	}

	void ChildCallback(Terraria.Projectile child) {
		if (SpawnedChildrenCount < 2) {
			SpawnedChildrenCount++;
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

		child.timeLeft = Projectile.timeLeft;
		LevelInHierarchy(child) = LevelInHierarchy(Projectile) + 1;
		if (LevelInHierarchy(Projectile) >= 1) {
			child.Disable<Components.OnTimer<Components.SpawnProjectile>>();
			return;
		}

		child.TryGet(out Components.OnTimer<Components.SpawnProjectile> data);
		child.Set(data with {
			Timer = (int)(data.Timer * Terraria.Main.rand.NextFloat(0.75f, 3f))
		});
	}

	public override bool PreDraw(ref FNA.Color lightColor) => false;
}
