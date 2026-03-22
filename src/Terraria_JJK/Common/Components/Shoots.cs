namespace Terraria_JJK.Components;

[EntityComponent.Component]
public struct Shoots
{
	public Shoots() {
		Type = Terraria.ID.ProjectileID.PurificationPowder;
		Count = 1;
		Velocity = static (orig) => orig;
		RelativePosition = static () => FNA.Vector2.Zero;
	}

	public int Type;
	public (int[] Types, bool Random)? Queue;
	public int Count;
	public System.Func<FNA.Vector2, FNA.Vector2> Velocity;
	public System.Func<FNA.Vector2> RelativePosition;
	public int Delay;
	public System.Action? Callback;
}

file class Shoots_Impl
{
	private class SetShootsItemValues : TML.GlobalItem
	{
		public override void SetDefaults(Terraria.Item entity) {
			if (!EC.TryGet<Shoots>(entity, out var data)) return;

			entity.shoot = data.Type;
			entity.useTime = data.Delay;
			entity.useAnimation = data.Delay;
			entity.shootSpeed = 1f;
		}
	}

	[DaybreakHooks.GlobalItemHooks.Shoot]
	internal static bool ShootItem(DaybreakHooks.GlobalItemHooks.Shoot.Original orig, Terraria.Item item, Terraria.Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, FNA.Vector2 position, FNA.Vector2 velocity, int type, int damage, float knockback) {
		if (!EC.TryGet<Shoots>(item, out var data)) return orig(item, player, source, position, velocity, type, damage, knockback);

		type = data.Type;
		for (int i = 0; i < data.Count; i++) {
			if (data.Queue is (int[] Types, bool Random) queue) {
				type = queue.Random ? Terraria.Utils.NextFromList(Terraria.Main.rand, queue.Types) : queue.Types[i % data.Count];
			}

			Terraria.Projectile.NewProjectile(
				source,
				position + data.RelativePosition(),
				data.Velocity(Terraria.Utils.SafeNormalize(velocity, FNA.Vector2.Zero)),
				type,
				damage,
				knockback,
				player.whoAmI
			);
		}

		data.Callback?.Invoke();
		return false;
	}
}