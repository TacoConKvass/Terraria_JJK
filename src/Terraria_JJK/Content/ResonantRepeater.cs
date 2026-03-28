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
		Item.With(new Components.RightClickable { Effect = TriggerDoll });
	}

	void TriggerDoll(Terraria.Player player) {
		Terraria.Projectile.NewProjectile(
			Item.GetSource_FromThis(),
			player.Center,
			FNA.Vector2.Zero,
			StrawDoll.ID,
			Damage: 0,
			KnockBack: 0,
			Owner: player.whoAmI
		);
	}

	public override FNA.Vector2? HoldoutOffset() => FNA.Vector2.UnitX * -9;
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

		Projectile.With(new Components.Sticky { TicksOfDamagePerSecond = 1 });
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
	}
}

public class StrawDoll : TML.ModProjectile
{
	public override string Texture => Terraria_JJK.AssetPath + $"Projectiles/{nameof(StrawDoll)}";

	public static int ID => TML.ModContent.ProjectileType<StrawDoll>();

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 22, Y = 30 };
		Projectile.timeLeft = 5 * 60;

		Projectile.With(new Components.StuckTo {
			Target = Terraria.Main.player[Projectile.owner],
			WithOffset = GetOffset
		});
	}

	public FNA.Vector2 GetOffset() {
		var offset = System.Math.Clamp(Projectile.ai[0]--, -48, 16);
		if ((int)offset == 48) { }
		Projectile.ai[0] = offset;
		return offset * FNA.Vector2.UnitY;
	}
}