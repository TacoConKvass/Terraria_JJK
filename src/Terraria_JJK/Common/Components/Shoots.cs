namespace Terraria_JJK.Components;

[EC.Component]
public struct Shoots : Core.ITriggerable
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

	private class SetShootsItemValues : TML.GlobalItem
	{
		public override void SetDefaults(Terraria.Item item) {
			if (!item.TryGet<Shoots>(out var data)) return;

			item.shoot = data.Type;
			item.useTime = data.Delay;
			item.useAnimation = data.Delay;
			item.shootSpeed = 1f;
		}
	}

	void Core.ITriggerable.Trigger(Terraria.Entity source, Terraria.Entity target) {
		var (damage, knockBack, owner) = source switch {
			Terraria.Projectile p => (p.damage, p.knockBack, p.owner),
			Terraria.Item i => (i.damage, i.knockBack, -1),
			Terraria.Player player => (player.HeldItem.damage, player.HeldItem.knockBack, player.whoAmI),
			_ => (10, 1, -1)
		};
		for (int i = 0; i < Count; i++) {
			var type = Type;

			if (Queue is (int[] Types, bool Random) queue)
				type = queue.Random ? Terraria.Utils.NextFromList(Terraria.Main.rand, queue.Types) : queue.Types[i % queue.Types.Length];

			Terraria.Projectile.NewProjectile(
				target.GetSource_FromThis(),
				RelativePosition() + target.Center,
				Velocity(source.velocity),
				type,
				damage,
				knockBack,
				owner
			);
		}

		Callback?.Invoke();
		(source switch {
			Terraria.Projectile p => p,
			_ => null
		})?.Disable<Shoots>();
	}

	[DaybreakHooks.GlobalItemHooks.Shoot]
	internal static bool ShootItem(DaybreakHooks.GlobalItemHooks.Shoot.Original orig, Terraria.Item item, Terraria.Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, FNA.Vector2 position, FNA.Vector2 velocity, int type, int damage, float knockback) {
		if (!item.TryGet<Shoots>(out var data)) return orig(item, player, source, position, velocity, type, damage, knockback);

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

file class Shoots_Impl
{

}