namespace Terraria_JJK.Content;

public class Seraph : TML.ModItem
{
	public override string Texture => Terraria_JJK.AssetPath($"Items/{nameof(Seraph)}");

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
		});
	}
}

public class SeraphSpear : TML.ModProjectile
{
	public override string Texture => Terraria_JJK.AssetPath($"Projectiles/{nameof(SeraphSpear)}");

	public static int ID => TML.ModContent.ProjectileType<SeraphSpear>();

	public override void SetDefaults() {
		Projectile.Size = new FNA.Vector2 { X = 20, Y = 20 };
		Projectile.timeLeft = 9 * Core.Const.Second;
		Projectile.friendly = true;

		Projectile.With(new Components.OnHit<Components.ApplyBuff> {
			Inner = new() { Type = Buff.ID, Duration = 10 * Core.Const.Second },
			Target = Components.TargetType.Victim,
		});
		Projectile.With(new Components.RotateWithVelocity {
			AdditionalRotation = FNA.MathHelper.PiOver4
		});
		Projectile.With(new Components.DrawPositionAdjustment {
			Origin = new FNA.Vector2 { X = 55, Y = 5 },
		});
	}

	public class Buff : TML.ModBuff
	{
		public static int ID => TML.ModContent.BuffType<Buff>();

		public override string Texture => $"Terraria/Images/Buff_{Terraria.ID.BuffID.Weak}";

		public override void SetStaticDefaults() {
			Terraria.Main.debuff[Type] = true;
			Terraria.Main.buffNoTimeDisplay[Type] = true;
		}

		[DaybreakHooks.GlobalNPCHooks.ModifyIncomingHit]
		static void NPCIncreaseDamageTaken(Terraria.NPC npc, ref Terraria.NPC.HitModifiers modifiers) {
			if (!npc.HasBuff<Buff>()) return;

			modifiers.FinalDamage *= 1.2f;
		}
	}
}