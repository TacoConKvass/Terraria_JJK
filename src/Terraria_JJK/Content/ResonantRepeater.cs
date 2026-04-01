using TextureAsset = ReLogic.Content.Asset<Microsoft.Xna.Framework.Graphics.Texture2D>;
using static Terraria.Utils;

namespace Terraria_JJK.Content;

public class ResonantRepeater : TML.ModItem
{
	public override string Texture => Terraria_JJK.AssetPath + $"Items/{nameof(ResonantRepeater)}";

	public override void SetDefaults() {
		Item.Size = new FNA.Vector2 { X = 50, Y = 20 };
		Item.damage = 20;
		Item.DamageType = TML.DamageClass.Ranged;
		Item.useStyle = Terraria.ID.ItemUseStyleID.Shoot;
		Item.useAmmo = Terraria.ID.AmmoID.Arrow;
		Item.UseSound = Terraria.ID.SoundID.Item5;

		Item.With(new Components.Shoots {
			Type = ResonantNail.ID,
			Delay = 30,
			Velocity = static (orig) => orig * 9.5f
		});
		Item.With(new Components.RightClickable {
			When = static (Terraria.Entity entity) => (entity as Terraria.Player)?.ownedProjectileCounts[StrawDoll.ID] == 0,
			Effect = TriggerDoll
		});
	}

	public override FNA.Vector2? HoldoutOffset() => FNA.Vector2.UnitX * -9;

	public override bool CanShoot(Terraria.Player player) => player.altFunctionUse != 2;

	bool? TriggerDoll(Terraria.Player player) {
		Terraria.Projectile.NewProjectile(
			player.GetSource_FromThis(),
			player.Center,
			FNA.Vector2.Zero,
			StrawDoll.ID,
			Damage: 0,
			KnockBack: 0,
			Owner: player.whoAmI
		);

		return true;
	}
}

public class ResonantNail : TML.ModProjectile
{
	public override string Texture => Terraria_JJK.AssetPath + $"Projectiles/{nameof(ResonantNail)}";

	public static int ID => TML.ModContent.ProjectileType<ResonantNail>();
	static TextureAsset trail_texture = null!;

	public override void SetStaticDefaults() {
		trail_texture = TML.ModContent.Request<FNA.Graphics.Texture2D>(Terraria_JJK.AssetPath + "Extra/HollowBeam");
	}

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 10, Y = 10 };
		Projectile.timeLeft = 20 * 60;
		Projectile.friendly = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60; // Damage every second;
		Projectile.penetrate = -1;

		Projectile.With(new Components.OnHit<Components.StickTo> {
			Inner = new(),
			Target = Components.TargetType.Self
		});
		Projectile.With(new Components.RotateWithVelocity {
			AdditionalRotation = FNA.MathHelper.PiOver2
		});
		Projectile.With(new Components.Trail {
			MaxPositions = 9,
			Color = static (_) => FNA.Color.Cyan,
			Width = static (progress) => 7f * (1f - progress),
			Texture = trail_texture?.Value
		});
		Projectile.With(new Components.OnHit<Components.Trail> {
			Inner = new() { MaxPositions = 0, },
			Target = Components.TargetType.Self
		});
		Projectile.With(new Components.Listen<StrawDoll.Explode> {
			Action = static (entity, data) => {
				var Projectile = entity as Terraria.Projectile;
				if (Projectile is null || Projectile.owner != data.Owner || !Projectile.Enabled<Components.StickTo>()) return;

				Terraria.Projectile.NewProjectile(
					Projectile.GetSource_FromThis(),
					Projectile.Center, FNA.Vector2.Zero,
					Terraria.ID.ProjectileID.Flames,
					Projectile.damage * 2,
					Projectile.knockBack,
					Projectile.owner
				);

				Projectile.Kill();
			}
		});
	}
}

public class StrawDoll : TML.ModProjectile
{
	public override string Texture => Terraria_JJK.AssetPath + $"Projectiles/{nameof(StrawDoll)}";

	public static int ID => TML.ModContent.ProjectileType<StrawDoll>();

	[EC.Component]
	public record struct Explode(int Owner) : Components.IListenable;

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 22, Y = 30 };
		Projectile.timeLeft = 5 * 60;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.tileCollide = false;

		Projectile.With(new Components.OnTimer<Components.SpawnDust> {
			Inner = new Components.SpawnDust {
				Type = Terraria.ID.DustID.HallowSpray,
				Count = 15,
				Velocity = static () => Terraria.Main.rand.NextVector2Unit() * 3f,
				RelativePosition = static () => FNA.Vector2.Zero,
				Callback = static () => { },
			},
			Timer = Projectile.timeLeft - 1
		});
	}

	public override void OnSpawn(Terraria.DataStructures.IEntitySource source) {
		Projectile.With(new Components.StickTo {
			Target = Terraria.Main.player[Projectile.owner],
			WithOffset = GetOffset,
		});
		Projectile.With(new Components.OnTimer<Components.Broadcast<StrawDoll.Explode>> {
			Inner = new() { Data = new() { Owner = Projectile.owner } },
			Timer = Projectile.timeLeft - 1
		});
		Projectile.With(new Components.Listen<StrawDoll.Explode> {
			Action = static (entity, data) => {
				Terraria.Main.player[data.Owner].Hurt(new Terraria.Player.HurtInfo {
					Damage = 100,
					DamageSource = new() {
						CustomReason = Terraria_JJK.LocalizedNetworkText("PlayerDeathReason.StrawDoll"),
					}
				});
			}
		});
	}

	public FNA.Vector2 GetOffset() {
		Projectile.ai[0] = System.Math.Clamp(Projectile.ai[0] + 1, 0, 60);
		return -Projectile.ai[0] * FNA.Vector2.UnitY;
	}
}