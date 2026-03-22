namespace Terraria_JJK.Components;

[EntityComponent.Component]
public struct RightClickable
{
	public System.Action<Terraria.Player> Effect;
}

internal class RightClickable_Impl
{
	[DaybreakHooks.GlobalItemHooks.CanRightClick]
	internal static bool Item_RightClickable(DaybreakHooks.GlobalItemHooks.CanRightClick.Original orig, Terraria.Item item) {
		return EC.Enabled<RightClickable>(item) ? true : orig(item);
	}

	[DaybreakHooks.GlobalItemHooks.RightClick]
	internal static void Item_TriggerEffect(Terraria.Item item, Terraria.Player player) {
		if (!EC.TryGet<RightClickable>(item, out var data)) return;
		data.Effect(player);
	}
}