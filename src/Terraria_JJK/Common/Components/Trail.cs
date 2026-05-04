using static Daybreak.Common.Rendering.SpriteBatchScopeExtensions;
using static Daybreak.Common.Rendering.SpriteBatchSnapshotExtensions;
using static Terraria.Utils;
using PositionQueue = System.Collections.Generic.Queue<Microsoft.Xna.Framework.Vector3>;
using Rendering = Daybreak.Common.Rendering;
using VertexData = Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture;

namespace Terraria_JJK.Components;

public enum TextureMode
{
	ApplyLightColor = 1,
	ApplyLightBrightness = 2,
	UseActualColor = 4,
}

[EC.Component]
public record struct Trail(
	int MaxPositions,
	PositionQueue Positions,
	int? Delay,
	System.Func<float, float> Width,
	System.Func<float, FNA.Color> Color,
	FNA.Graphics.Texture2D? Texture,
	int FadeSpeed,
	TextureMode TextureMode
) : ITriggerable
{
	const FNA.Graphics.PrimitiveType Type = FNA.Graphics.PrimitiveType.TriangleStrip;

	[EC.Component]
	public struct AddPosition : ITimeable;

	[DaybreakHooks.GlobalProjectileHooks.AI]
	internal static void UpdateProjectile(Terraria.Projectile projectile) {
		if (!projectile.TryGet(out Trail data)) return;

		if (data.MaxPositions == 0 && data.Positions?.Count == 0) {
			projectile.Disable<Trail>();
			return;
		}

		if (data.Positions == null) {
			projectile.Set(data with { Positions = [] });
			return;
		}

		var add_position = projectile.Enabled<Trail.AddPosition>();
		if (data.Delay.HasValue) {
			projectile.Disable<AddPosition>();
		}

		if (data.Delay == null || add_position) {
			data.Positions.Enqueue(new FNA.Vector3(projectile.Center, 0));
			for (int i = 0; i < 2 && (data.Positions.Count > data.MaxPositions); i++) // Try removing excessive positions
				data.Positions.Dequeue();
		}

		if (data.Delay.HasValue && !projectile.Enabled<OnTimer<Trail.AddPosition>>())
			projectile.With(new OnTimer<Trail.AddPosition> {
				Timer = data.Delay.Value,
				Inner = default,
			});
	}

	[DaybreakHooks.GlobalProjectileHooks.PreDraw]
	internal static void RenderProjectileTrail(Terraria.Projectile projectile, ref FNA.Color lightColor) {
		if (!projectile.TryGet(out Trail data)) return;

		RenderTrail(data);
	}

	internal static void RenderTrail(in Trail data) {
		var shape_count = data.Positions?.Count ?? 0;
		if (shape_count < 1) return;

		var positions = data.Positions!.ToArray();
		var initial_count = positions.Length;
		System.Span<VertexData> vertices = stackalloc VertexData[(2 * initial_count) - 1];

		for (int i = 1; i < initial_count; i++) {
			var current = positions[^i];
			var next = positions[^(i + 1)];
			var progress = (i - 1) / (float)(initial_count - 1);
			var width = System.MathF.Abs(data.Width(progress));
			FNA.Color color;

			if (data.TextureMode.HasFlag(TextureMode.UseActualColor))
				color = FNA.Color.White;
			else color = data.Color(progress);

			// Lighting interaction
			if (data.TextureMode.HasFlag(TextureMode.ApplyLightColor))
				color = color.MultiplyRGB(Terraria.Lighting.GetColor((int)current.X / 16, (int)current.Y / 16));
			if (data.TextureMode.HasFlag(TextureMode.ApplyLightBrightness))
				color *= Terraria.Lighting.GetColor((int)current.X / 16, (int)current.Y / 16).A;

			var normal = new FNA.Vector3(new FNA.Vector2(next.X - current.X, next.Y - current.Y).SafeNormalize(FNA.Vector2.Zero).RotatedBy(FNA.MathHelper.PiOver2), 0);
			vertices[(i * 2) - 2] = new VertexData(current + (normal * width), color, FNA.Vector2.Zero);
			vertices[(i * 2) - 1] = new VertexData(current - (normal * width), color, FNA.Vector2.UnitY);
		}

		vertices[^1] = new VertexData(positions[0], data.Color(1f), new FNA.Vector2 { X = 1, Y = 0.5f });

		var matrix = Core.Rendering.Helpers.GetMatrix();
		var snapshot = new Rendering.SpriteBatchSnapshot(Terraria.Main.spriteBatch);

		using (Terraria.Main.spriteBatch.Scope()) {
			Terraria.Main.spriteBatch.Begin(snapshot with {
				RasterizerState = FNA.Graphics.RasterizerState.CullNone,
			});

			var effect = Core.Rendering.Helpers.DefaultEffect.Value;
			effect.Parameters["WorldViewProjection"].SetValue(matrix);
			effect.Parameters["inputTexture"].SetValue(data.Texture ?? Terraria.GameContent.TextureAssets.MagicPixel.Value);
			effect.CurrentTechnique.Passes["Texture"].Apply();
			Terraria.Main.graphics.GraphicsDevice.DrawUserPrimitives(
				Type, vertices.ToArray(), 0,
				Core.Rendering.Helpers.PrimitiveCount(Type, vertices.Length)
			);
		}
	}

	void ITriggerable.Trigger(Terraria.Entity source, Terraria.Entity target, TargetType targetType) {
		var data = this;
		if (targetType == TargetType.Self && source.TryGet(out Trail selfData)) data = selfData;
		else if (target.TryGet(out Trail targetData)) data = targetData;

		ITriggerable.Default(source, target, targetType, this with {
			Positions = Positions ?? data.Positions,
			Width = Width ?? data.Width,
			Color = Color ?? data.Color,
			Texture = Texture ?? data.Texture
		});
	}

	// Dead trail rendering
	[DaybreakHooks.ModSystemHooks.OnModLoad]
	static void SetupDeadRendering() => Terraria.On_Main.DrawProjectiles += DrawDead;

	static System.Collections.Generic.List<Trail> dead = [];
	static System.Collections.Generic.List<int> timers = [];
	static System.Collections.Generic.Queue<int> toDelete = [];

	private static void DrawDead(Terraria.On_Main.orig_DrawProjectiles orig, Terraria.Main self) {
		for (int i = 0; i < dead.Count; i++) {
			var trail = dead[i];
			RenderTrail(trail);
			if (timers[i] < 0) timers[i]++;
			if (timers[i] >= 0) {
				for (int j = 0; j < timers[i] && (trail.Positions.Count > 0); j++)
					trail.Positions.Dequeue();
				timers[i] = trail.FadeSpeed;
			}
			if (trail.Positions.Count == 0) {
				timers.RemoveAt(i);
				dead.RemoveAt(i);
			}
		}
		orig(self);
	}

	[DaybreakHooks.GlobalProjectileHooks.PreKill]
	static void AddToDead(Terraria.Projectile projectile, int timeLeft) {
		if (projectile.TryGet(out Trail data) && data.FadeSpeed != 0) {
			data.Positions.Enqueue(new FNA.Vector3(projectile.Center, 0));
			dead.Add(data);
			timers.Add(data.FadeSpeed);
		}
	}
}