namespace Terraria_JJK.Components;

[EC.Component]
public record struct Animate(int FrameDelay) : ITimeable
{
	[DaybreakHooks.GlobalProjectileHooks.AI]
	static void AnimateProjectile(Terraria.Projectile projectile) {
		if (!projectile.TryGet(out Animate data)) return;

		projectile.frame++;
		projectile.frame %= Terraria.Main.projFrames[projectile.type];
		projectile.Disable<Animate>();
		projectile.With(new OnTimer<Animate>() { Timer = data.FrameDelay, Inner = data });
	}
}