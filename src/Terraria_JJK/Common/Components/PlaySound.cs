namespace Terraria_JJK.Components;

[EC.Component]
public record struct PlaySound(Terraria.Audio.SoundStyle Style, System.Func<Terraria.Entity, FNA.Vector2> Position) : ITimeable, ITriggerable
{
	[DaybreakHooks.GlobalProjectileHooks.AI]
	static void PlaySoundOnProjectileUpdate(Terraria.Projectile projectile) {
		if (!projectile.TryGet(out PlaySound data)) return;

		(data as ITriggerable).Trigger(projectile, null!, TargetType.Self);
		projectile.Disable<PlaySound>();
	}

	void ITriggerable.Trigger(Terraria.Entity source, Terraria.Entity target, TargetType targetType) {
		Terraria.Audio.SoundEngine.PlaySound(Style, Position?.Invoke(source) ?? source.Center);
	}
}