using PositionQueue = System.Collections.Generic.Queue<Microsoft.Xna.Framework.Vector3>;
using VertexData = Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture;
using Rendering = Daybreak.Common.Rendering;
using static Daybreak.Common.Rendering.SpriteBatchSnapshotExtensions;
using static Daybreak.Common.Rendering.SpriteBatchScopeExtensions;
using static Terraria.Utils;

namespace Terraria_JJK.Components;

[EC.Component]
public record struct Trail(int MaxPositions, PositionQueue Positions, float StartingWidth, FNA.Color Color)
{
	const FNA.Graphics.PrimitiveType Type = FNA.Graphics.PrimitiveType.TriangleStrip;

	[DaybreakHooks.GlobalProjectileHooks.AI]
	internal static void UpdateProjectile(Terraria.Projectile projectile) {
		if (!projectile.TryGet(out Trail data)) return;
		if (data.Positions == null) {
			projectile.Set(data with { Positions = [] });
			return;
		}

		data.Positions.Enqueue(new FNA.Vector3(projectile.Center, 0));
		if (data.Positions.Count > data.MaxPositions)
			data.Positions.Dequeue();
	}

	[DaybreakHooks.GlobalProjectileHooks.PreDraw]
	internal static void RenderProjectileTrail(Terraria.Projectile projectile, ref FNA.Color lightColor) {
		if (!projectile.TryGet(out Trail data)) return;

		RenderTrail(data, projectile);
	}

	static void RenderTrail(in Trail data, Terraria.Projectile projectile) {
		var shape_count = Core.Rendering.PrimitiveCount(Type, data.Positions?.Count ?? 0);
		if (shape_count < 1) return;

		var positions = data.Positions!.ToArray();
		var initial_count = positions.Length;
		System.Span<VertexData> vertices = stackalloc VertexData[(2 * initial_count) - 1];

		for (int i = 1; i < initial_count; i++) {
			var current = positions[^i];
			var next = positions[^(i + 1)];
			var width = data.StartingWidth * (1f - (i / (float)(initial_count - 1)));
			var normal = new FNA.Vector3(new FNA.Vector2(next.X - current.X, next.Y - current.Y).SafeNormalize(FNA.Vector2.Zero).RotatedBy(FNA.MathHelper.PiOver2), 0);
			vertices[(i * 2) - 1] = new VertexData(current + (normal * width), data.Color, FNA.Vector2.Zero);
			vertices[(i * 2) - 2] = new VertexData(current - (normal * width), data.Color, FNA.Vector2.UnitY);
		}

		vertices[^1] = new VertexData(positions[0], data.Color, new FNA.Vector2 { X = 1, Y = 0.5f });

		var matrix = Core.Rendering.GetMatrix();
		var snapshot = new Rendering.SpriteBatchSnapshot(Terraria.Main.spriteBatch);
		using (Terraria.Main.spriteBatch.Scope()) {
			Terraria.Main.spriteBatch.Begin(snapshot with {
				RasterizerState = FNA.Graphics.RasterizerState.CullNone,
			});

			var effect = Core.Rendering.DefaultEffect.Value;
			effect.Parameters["WorldViewProjection"].SetValue(matrix);
			effect.CurrentTechnique.Passes["Texture"].Apply();
			Terraria.Main.graphics.GraphicsDevice.DrawUserPrimitives(Type, vertices.ToArray(), 0, shape_count);
		}
	}
}