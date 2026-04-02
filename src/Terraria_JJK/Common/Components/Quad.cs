using VertexData = Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture;
using SpriteBatchSnapshot = Daybreak.Common.Rendering.SpriteBatchSnapshot;
using static Terraria.Utils;
using static Daybreak.Common.Rendering.SpriteBatchScopeExtensions;
using static Daybreak.Common.Rendering.SpriteBatchSnapshotExtensions;

namespace Terraria_JJK.Components;

[EC.Component]
public record struct Quad(bool Replace, bool WithBackside, FNA.Vector3[] Corners, System.Func<FNA.Vector3> Rotations, FNA.Graphics.Texture2D? Texture)
{
	[DaybreakHooks.GlobalProjectileHooks.PreDraw]
	public static bool ProjectilePreDraw(DaybreakHooks.GlobalProjectileHooks.PreDraw.Original orig, Terraria.Projectile projectile, ref FNA.Color lightColor) {
		if (!projectile.TryGet(out Quad data)) return true;

		RenderQuad(projectile.Center, data);
		return !data.Replace;
	}

	static FNA.Vector2[] textureCoords = [
		FNA.Vector2.UnitX,
		FNA.Vector2.Zero,
		FNA.Vector2.One,
		FNA.Vector2.UnitY
	];

	static void RenderQuad(FNA.Vector2 center, in Quad data) {
		if (data.Corners.Length < 4) throw new System.InvalidOperationException("You must provide 4 corners for a quad");

		int length = data.WithBackside ? 6 : 4;
		System.Span<VertexData> vertices = stackalloc VertexData[length];

		var rotations = data.Rotations();
		var rotation_matrix = FNA.Matrix.CreateRotationY(rotations.Y) * FNA.Matrix.CreateRotationX(rotations.X) * FNA.Matrix.CreateRotationZ(rotations.Z);
		for (int i = 0; i < length; i++) {
			vertices[i] = new VertexData(new FNA.Vector3(center, 0) + FNA.Vector3.Transform(data.Corners[i % 4], rotation_matrix), FNA.Color.White, textureCoords[i % 4]);
		}

		var spriteBatch = Terraria.Main.spriteBatch;
		var graphicsDevice = Terraria.Main.graphics.GraphicsDevice;
		var snapshot = new SpriteBatchSnapshot(spriteBatch);

		using (spriteBatch.Scope()) {
			spriteBatch.Begin(snapshot with { RasterizerState = FNA.Graphics.RasterizerState.CullNone });

			var effect = Core.Rendering.Helpers.DefaultEffect.Value;
			effect.Parameters["WorldViewProjection"].SetValue(Core.Rendering.Helpers.GetMatrix());// * FNA.Matrix.CreateRotationY(data.YRotation()));
			effect.Parameters["inputTexture"].SetValue(data.Texture ?? Terraria.GameContent.TextureAssets.MagicPixel.Value);
			effect.CurrentTechnique.Passes["ColoredTexture"].Apply();

			graphicsDevice.DrawUserPrimitives(
				FNA.Graphics.PrimitiveType.TriangleStrip, vertices.ToArray(), 0,
				Core.Rendering.Helpers.PrimitiveCount(FNA.Graphics.PrimitiveType.TriangleStrip, vertices.Length)
			);
		}
	}

	public static FNA.Vector3[] CreateCorners(FNA.Vector2 corner, FNA.Vector2? corner_2 = null) {
		return [
			new FNA.Vector3(corner, 0),
			new FNA.Vector3(corner_2 ?? corner.RotatedBy(FNA.MathHelper.PiOver2), 0),
			new FNA.Vector3(corner_2?.RotatedBy(FNA.MathHelper.Pi) ?? corner.RotatedBy(-FNA.MathHelper.PiOver2), 0),
			new FNA.Vector3(corner.RotatedBy(FNA.MathHelper.Pi), 0),
		];
	}

	public static FNA.Vector3[] CreateRectCorners(FNA.Vector2 corner) {
		corner *= -1;
		return [
			new FNA.Vector3( corner.X,  corner.Y, 0),
			new FNA.Vector3(-corner.X,  corner.Y, 0),
			new FNA.Vector3( corner.X, -corner.Y, 0),
			new FNA.Vector3(-corner.X, -corner.Y, 0)
		];
	}
}