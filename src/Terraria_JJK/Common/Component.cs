using Enumerable = System.Linq.Enumerable;
using AttributeExt = System.Reflection.CustomAttributeExtensions;

namespace Terraria_JJK.EntityComponent;

[System.AttributeUsage(System.AttributeTargets.Struct)]
public class ComponentAttribute : System.Attribute;

public class ComponentLoader : TML.ModSystem
{
	public override void Load() {
		var types = Enumerable.Where(Mod.Code.GetTypes(), type => AttributeExt.GetCustomAttribute<ComponentAttribute>(type) != null);
		var components = Enumerable.ToArray(
			Enumerable.Select(types, T => InstantiateComponents(T))
		);

		foreach (var entry in components) {
			entry.Deconstruct(out var npc, out var item, out var player, out var projectile);
			if ((npc, item, player, projectile) is not (TML.ILoadable, TML.ILoadable, TML.ILoadable, TML.ILoadable)) continue;
			Mod.AddContent(npc);
			Mod.AddContent(item);
			Mod.AddContent(player);
			Mod.AddContent(projectile);
		}
	}

	record struct ComponentTuple(TML.ILoadable? npc, TML.ILoadable? item, TML.ILoadable? player, TML.ILoadable? projectile);

	static TML.ILoadable? InstantiateWith(System.Type t, System.Type argument) => (TML.ILoadable?)System.Activator.CreateInstance(t.MakeGenericType(argument));
	static ComponentTuple InstantiateComponents(System.Type t) {
		return new(
			InstantiateWith(typeof(NPCComponent<>), t),
			InstantiateWith(typeof(ItemComponent<>), t),
			InstantiateWith(typeof(PlayerComponent<>), t),
			InstantiateWith(typeof(ProjectileComponent<>), t)
		);
	}
}

public class NPCComponent<TData> : TML.GlobalNPC where TData : struct
{
	public override string Name => $"NPCComponent.{typeof(TData).Name}";
	public override bool InstancePerEntity { get; } = true;
	internal TData Data;
	internal bool Enabled = false;
}

public class ItemComponent<TData> : TML.GlobalItem where TData : struct
{

	public override string Name => $"ItemComponent.{typeof(TData).Name}";
	public override bool InstancePerEntity { get; } = true;
	internal TData Data;
	internal bool Enabled = false;
}

public class PlayerComponent<TData> : TML.ModPlayer where TData : struct
{
	public override string Name => $"PlayerComponent.{typeof(TData).Name}";
	internal TData Data;
	internal bool Enabled = false;
}

public class ProjectileComponent<TData> : TML.GlobalProjectile where TData : struct
{
	public override string Name => $"ProjectileComponent.{typeof(TData).Name}";
	public override bool InstancePerEntity { get; } = true;
	internal TData Data;
	internal bool Enabled = false;
}

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

	public static void With<T>(this Terraria.NPC npc, T data) where T : struct {
		Enable<T>(npc);
		Set(npc, data);
	}
	public static void With<T>(this Terraria.Item item, T data) where T : struct {
		Enable<T>(item);
		Set(item, data);
	}
	public static void With<T>(this Terraria.Player player, T data) where T : struct {
		Enable<T>(player);
		Set(player, data);
	}
	public static void With<T>(this Terraria.Projectile projectile, T data) where T : struct {
		Enable<T>(projectile);
		Set(projectile, data);
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
