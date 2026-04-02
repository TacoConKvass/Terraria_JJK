using static Terraria.Utils;

namespace Terraria_JJK.Core.Rendering;

public class Helpers
{
	public static ReLogic.Content.Asset<FNA.Graphics.Effect> DefaultEffect = null!;

	[DaybreakHooks.ModSystemHooks.OnModLoad]
	static void SetEffect() {
		DefaultEffect = TML.ModContent.Request<FNA.Graphics.Effect>(Terraria_JJK.AssetPath + "Effects/BasicEffect");
	}

	public static int PrimitiveCount(FNA.Graphics.PrimitiveType type, int count) => type switch {
		FNA.Graphics.PrimitiveType.TriangleStrip => count - 2,
		FNA.Graphics.PrimitiveType.LineList => count / 2,
		FNA.Graphics.PrimitiveType.TriangleList => count / 3,
		FNA.Graphics.PrimitiveType.LineStrip => count - 1,
		_ => 0
	};

	public static FNA.Vector3 FromWorldToScreenUniform(FNA.Vector3 vector) {
		var translation = new FNA.Vector3(Terraria.Main.screenPosition + Terraria.Main.ScreenSize.ToVector2() / 2, 0);
		var screen = new FNA.Vector3(new FNA.Vector2(Terraria.Main.ScreenSize.X, -Terraria.Main.ScreenSize.Y), 0);
		return (vector - translation) * 2 / screen;
	}


	public static FNA.Matrix GetMatrix() {
		var viewport = Terraria.Main.graphics.GraphicsDevice.Viewport;
		FNA.Matrix world = FNA.Matrix.CreateTranslation(-new FNA.Vector3(Terraria.Main.screenPosition, 0));
		FNA.Matrix view = Terraria.Main.GameViewMatrix.TransformationMatrix;
		FNA.Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 1, out var projection);
		return world * view * projection;
	}
}