using static System.Linq.Enumerable;
using AttributeExt = System.Reflection.CustomAttributeExtensions;

namespace Terraria_JJK.EC;

[System.AttributeUsage(System.AttributeTargets.Struct)]
public class ComponentAttribute : System.Attribute
{
	public System.Type[] Wraps = [];
}

public class ComponentLoader : TML.ModSystem
{
	public override void Load() {
		var types = Mod.Code.GetTypes().Where(type => AttributeExt.GetCustomAttribute<ComponentAttribute>(type) != null);
		var components = types.SelectMany(T => T.IsGenericType ? InstantiateGeneric(T) : [InstantiateComponents(T)]).ToArray();

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

	static ComponentTuple[] InstantiateGeneric(System.Type T) {
		if (AttributeExt.GetCustomAttribute<ComponentAttribute>(T) is not ComponentAttribute { Wraps: { Length: >= 1 } wrapped })
			return [];

		return wrapped.Select(t => {
			var wrappedType = T.MakeGenericType(t);
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(wrappedType.TypeHandle);
			return InstantiateComponents(wrappedType);
		}).ToArray();
	}
}

