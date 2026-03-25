namespace Terraria_JJK.EC;

public static class ComponentExtensions
{
	public static NPCComponent<T> GetComponent<T>(this Terraria.NPC npc) where T : struct => npc.GetGlobalNPC<NPCComponent<T>>();
	public static ItemComponent<T> GetComponent<T>(this Terraria.Item item) where T : struct => item.GetGlobalItem<ItemComponent<T>>();
	public static PlayerComponent<T> GetComponent<T>(this Terraria.Player player) where T : struct => player.GetModPlayer<PlayerComponent<T>>();
	public static ProjectileComponent<T> GetComponent<T>(this Terraria.Projectile projectile) where T : struct => projectile.GetGlobalProjectile<ProjectileComponent<T>>();

	public static void Enable<T>(this Terraria.NPC npc) where T : struct => SetEnabled<T>(npc, true);
	public static void Enable<T>(this Terraria.Item item) where T : struct => SetEnabled<T>(item, true);
	public static void Enable<T>(this Terraria.Player player) where T : struct => SetEnabled<T>(player, true);
	public static void Enable<T>(this Terraria.Projectile projectile) where T : struct => SetEnabled<T>(projectile, true);

	public static void Disable<T>(this Terraria.NPC npc) where T : struct => SetEnabled<T>(npc, false);
	public static void Disable<T>(this Terraria.Item item) where T : struct => SetEnabled<T>(item, false);
	public static void Disable<T>(this Terraria.Player player) where T : struct => SetEnabled<T>(player, false);
	public static void Disable<T>(this Terraria.Projectile projectile) where T : struct => SetEnabled<T>(projectile, false);

	public static void SetEnabled<T>(this Terraria.NPC npc, bool value) where T : struct => npc.GetComponent<T>().Enabled = value;
	public static void SetEnabled<T>(this Terraria.Item item, bool value) where T : struct => item.GetComponent<T>().Enabled = value;
	public static void SetEnabled<T>(this Terraria.Player player, bool value) where T : struct => player.GetComponent<T>().Enabled = value;
	public static void SetEnabled<T>(this Terraria.Projectile projectile, bool value) where T : struct => projectile.GetComponent<T>().Enabled = value;

	public static bool Enabled<T>(this Terraria.NPC npc) where T : struct => npc.GetComponent<T>().Enabled;
	public static bool Enabled<T>(this Terraria.Item item) where T : struct => item.GetComponent<T>().Enabled;
	public static bool Enabled<T>(this Terraria.Player player) where T : struct => player.GetComponent<T>().Enabled;
	public static bool Enabled<T>(this Terraria.Projectile projectile) where T : struct => projectile.GetComponent<T>().Enabled;

	public static void Set<T>(this Terraria.NPC npc, T data) where T : struct => npc.GetComponent<T>().Data = data;
	public static void Set<T>(this Terraria.Item item, T data) where T : struct => item.GetComponent<T>().Data = data;
	public static void Set<T>(this Terraria.Player player, T data) where T : struct => player.GetComponent<T>().Data = data;
	public static void Set<T>(this Terraria.Projectile projectile, T data) where T : struct => projectile.GetComponent<T>().Data = data;

	public static T With<T>(this Terraria.NPC npc, T data) where T : struct {
		Enable<T>(npc);
		Set(npc, data);
		return data;
	}
	public static T With<T>(this Terraria.Item item, T data) where T : struct {
		Enable<T>(item);
		Set(item, data);
		return data;
	}
	public static T With<T>(this Terraria.Player player, T data) where T : struct {
		Enable<T>(player);
		Set(player, data);
		return data;
	}
	public static T With<T>(this Terraria.Projectile projectile, T data) where T : struct {
		Enable<T>(projectile);
		Set(projectile, data);
		return data;
	}

	public static bool TryGet<T>(this Terraria.NPC npc, out T data) where T : struct {
		var component = npc.GetComponent<T>();
		data = component.Data;
		return component.Enabled;
	}

	public static bool TryGet<T>(this Terraria.Item item, out T data) where T : struct {
		var component = item.GetComponent<T>();
		data = component.Data;
		return component.Enabled;
	}

	public static bool TryGet<T>(this Terraria.Player player, out T data) where T : struct {
		var component = player.GetComponent<T>();
		data = component.Data;
		return component.Enabled;
	}

	public static bool TryGet<T>(this Terraria.Projectile projectile, out T data) where T : struct {
		var component = projectile.GetComponent<T>();
		data = component.Data;
		return component.Enabled;
	}
}
