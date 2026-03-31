namespace Terraria_JJK.Core;

public interface ITriggerable
{
	public void Trigger(Terraria.Entity source, Terraria.Entity target, Components.TargetType targetType);

	public static void Default<T>(Terraria.Entity source, Terraria.Entity target, Components.TargetType targetType, T data) where T : struct {
		if (targetType == Components.TargetType.Self) {
			source.With(data);
			return;
		}

		target.With(data);
	}
}