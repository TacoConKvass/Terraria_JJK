namespace Terraria_JJK.Content;

public class Seraph : TML.ModItem
{
	public override string Texture => Terraria_JJK.AssetPath + $"Items/{nameof(Seraph)}";

	public override void SetDefaults() {
		Item.Size = new FNA.Vector2 { X = 60, Y = 60 };
		Item.damage = 10;
		Item.DamageType = TML.DamageClass.Ranged;

		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.useStyle = Terraria.ID.ItemUseStyleID.Swing;

		Item.With(new Components.Shoots {
			Type = SeraphSpear.ID,
			Count = 1,
			Delay = 30,
			Velocity = static (orig) => orig * 10f,
			RelativePosition = static () => FNA.Vector2.Zero,
		});
	}
}

public class SeraphSpear : TML.ModProjectile
{
	public override string Texture => Terraria_JJK.AssetPath + $"Projectiles/{nameof(SeraphSpear)}";

	public static int ID => TML.ModContent.ProjectileType<SeraphSpear>();

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 60, Y = 60 };
		Projectile.timeLeft = 9 * 60;
		Projectile.friendly = true;

		Projectile.With(new Components.OnHit<Components.ApplyBuff> {
			Inner = new() { Type = Terraria.ID.BuffID.Weak, Duration = 9 * 60 },
		});
	}
}