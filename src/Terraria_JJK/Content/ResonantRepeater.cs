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

		EC.With(Item, new Components.Shoots {
			Type = ResonantNail.ID,
			Delay = 30,
			Velocity = static (orig) => orig * 7.5f
		});
		EC.With(Item, new Components.RightClickable { Effect = TriggerDoll });
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
}

public class ResonantNail : TML.ModProjectile
{
	public override string Texture => Terraria_JJK.AssetPath + $"Projectiles/{nameof(ResonantNail)}";

	public static int ID => TML.ModContent.ProjectileType<ResonantNail>();

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 10, Y = 10 };
		Projectile.timeLeft = 20 * 60;
		Projectile.friendly = true;

		EC.With(Projectile, new Components.Sticky { TicksOfDamagePerSecond = 1 });
		EC.With(Projectile, new Components.RotateWithVelocity {
			AdditionalRotation = FNA.MathHelper.PiOver2
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

		EC.With(Projectile, new Components.StuckTo {
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