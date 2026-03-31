namespace Terraria_JJK.Components;

public interface IListenable;

[EC.Component(Wraps = typeof(IListenable))]
public record struct Broadcast<T>(T Data) : ITimeable where T : struct, IListenable
{
	static Broadcast() {
		DaybreakHooks.GlobalProjectileHooks.AI.Event += ProjectileBroadcast;
	}

	static void ProjectileBroadcast(DaybreakHooks.GlobalProjectileHooks.AI.Original orig, TML.GlobalProjectile self, Terraria.Projectile projectile) {
		if (!projectile.TryGet(out Broadcast<T> data)) return;

		ExecuteBroadcast(data.Data);
	}

	static void ExecuteBroadcast(T data) {
		foreach (var projectile in Terraria.Main.ActiveProjectiles) {
			if (!projectile.TryGet(out Listen<T> listener)) continue;

			listener.Action(projectile, data);
		}
	}
}

[EC.Component(Wraps = typeof(IListenable))]
public record struct Listen<T>(System.Action<Terraria.Entity, T> Action) : ITimeable where T : struct, IListenable;