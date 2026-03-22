using Locale = Terraria.Localization;
using Input = Terraria.GameInput;
using Data = Terraria.DataStructures;
using TextureAsset = ReLogic.Content.Asset<Microsoft.Xna.Framework.Graphics.Texture2D>;
using UI = Terraria.UI;
using UIElements = Terraria.GameContent.UI.Elements;
using Collections = System.Collections.Generic;
using TextureRepo = Terraria.GameContent.TextureAssets;

namespace Terraria_JJK.Content;

public class MythicalBerries : TML.ModItem
{
	public override string Texture => $"{Mod.Name}/Assets/{nameof(MythicalBerries)}";

	public const int DamageBoostPercent = 30;

	public override void SetDefaults() {
		Item.Size = new FNA.Vector2(32f);
		Item.value = Terraria.Item.buyPrice(gold: 1, silver: 100);
		Item.accessory = true;
	}

	public override void UpdateEquip(Terraria.Player player) {
		var transformation = player.GetModPlayer<BerryTransformation>();
		transformation.Available = true;
		if (transformation.Activated)
			player.GetDamage(TML.DamageClass.Generic) += (float)DamageBoostPercent / 100;
	}

	public override void ModifyTooltips(Collections.List<TML.TooltipLine> tooltips) {
		int lineIndex = tooltips.FindIndex(tooltip => tooltip.Text.StartsWith("Press {0}"));

		if (lineIndex == -1) return;
		tooltips[lineIndex].Text = Humanizer.StringExtensions.FormatWith(
			tooltips[lineIndex].Text,
			System.Linq.Enumerable.FirstOrDefault(BerryTransformation.TransformKeybind.GetAssignedKeys(), "UNBOUND"),
			DamageBoostPercent * 2
		);
	}
}

public class BerryTransformation : TML.ModPlayer
{
	public int TimeLeft;
	public bool Activated;
	public bool Available;

	public const int Duration = 10 * 60;

	public static TML.ModKeybind TransformKeybind = null!;

	public override void ProcessTriggers(Input.TriggersSet triggersSet) {
		if (TransformKeybind.JustPressed && (Available && !Activated)) {
			Activated = true;
			TimeLeft = Duration;
		}
	}

	public override void ResetEffects() {
		Available = false;

		if (TimeLeft > 0 && Activated) TimeLeft--;
		else if (Activated) Player.KillMe(
			damageSource: new() {
				CustomReason = Locale.NetworkText.FromLiteral(Locale.Language.GetTextValue($"Mods.{Mod.Name}.PlayerDeathReason.Berry"))
			},
			dmg: 10_000,
			hitDirection: 1,
			pvp: false
		);
	}

	public override void UpdateDead() => (TimeLeft, Activated) = (0, false);

	public override void Load() => TransformKeybind = TML.KeybindLoader.RegisterKeybind(Mod, "BerryTransformation", FNA.Input.Keys.None);

	public override void Unload() => TransformKeybind = null!;
}

public class BerryUI
{
	[TML.Autoload(Side = TML.ModSide.Client)]
	public class Setup : TML.ModSystem
	{
		UI.UserInterface? ui;
		BerryUI.State? state;

		public override void Load() {
			ui = new UI.UserInterface();
			state = new BerryUI.State();
			ui.SetState(state);
		}

		public override void UpdateUI(FNA.GameTime gameTime) => ui?.Update(gameTime);

		public override void ModifyInterfaceLayers(Collections.List<UI.GameInterfaceLayer> layers) {
			int targetIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Resource Bars");
			if (targetIndex == -1) return;

			layers.Insert(
				targetIndex,
				new UI.LegacyGameInterfaceLayer(
					"Terraria JJK: Beast Berry Bar",
					Draw,
					UI.InterfaceScaleType.UI
				)
			);
		}

		bool Draw() {
			ui?.Draw(Terraria.Main.spriteBatch, new FNA.GameTime());
			return true;
		}
	}

	public class State : UI.UIState
	{
		UI.UIElement area = null!;
		UIElements.UIImage frameElement = null!;
		TextureAsset frame = null!;

		public override void OnInitialize() {
			frame = TML.ModContent.Request<FNA.Graphics.Texture2D>($"{nameof(Terraria_JJK)}/Assets/BerryTransformation_Frame", ReLogic.Content.AssetRequestMode.ImmediateLoad);

			area = new UI.UIElement();
			area.Width.Set(frame.Value.Width, 0f);
			area.Height.Set(frame.Value.Height, 0f);
			area.Left.Set(-frame.Value.Width / 2f, 0.5f);
			area.Top.Set(frame.Value.Height + Terraria.Main.LocalPlayer.Hitbox.Height, 0.5f);

			frameElement = new UIElements.UIImage(frame.Value);
			frameElement.Width.Set(frame.Value.Width, 0f);
			frameElement.Height.Set(frame.Value.Height, 0f);

			area.Append(frameElement);
			this.Append(area);
		}

		public override void Draw(FNA.Graphics.SpriteBatch spriteBatch) {
			if (!Terraria.Main.LocalPlayer.GetModPlayer<BerryTransformation>().Activated)
				return;

			base.Draw(spriteBatch);
		}

		protected override void DrawSelf(FNA.Graphics.SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			var player = Terraria.Main.LocalPlayer.GetModPlayer<BerryTransformation>();
			var percentage = (float)player.TimeLeft / BerryTransformation.Duration;

			const int frameWidth = 4;
			const int frameHeight = 4;
			var rect = frameElement.GetDimensions().ToRectangle();
			rect = rect with {
				X = rect.X + frameWidth,
				Y = rect.Y + frameHeight,
				Width = rect.Width - (frameWidth * 2),
				Height = rect.Height - (frameHeight * 2)
			};

			int steps = (int)(rect.Width * percentage);
			for (int i = 0; i < steps; i++) {
				var color = FNA.Color.Lerp(FNA.Color.MediumPurple, FNA.Color.Purple, (float)i / rect.Width);
				spriteBatch.Draw(TextureRepo.MagicPixel.Value, rect with { X = rect.X + i, Width = 1 }, color);
			}
		}
	}
}

public class BerryLayers
{
	static FNA.Color GetColor(Terraria.Player player) => player.immune ? FNA.Color.White with { A = (byte)player.immuneAlpha } : FNA.Color.White;

	public class Head : TML.PlayerDrawLayer
	{
		public static TextureAsset? Texture;

		public override void Load() => Texture = TML.ModContent.Request<FNA.Graphics.Texture2D>($"{Mod.Name}/Assets/BerriesTransformation_Head");

		public override void Unload() => Texture = null!;

		public override bool GetDefaultVisibility(Data.PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<BerryTransformation>().Activated;

		public override Position GetDefaultPosition() => new BeforeParent(Data.PlayerDrawLayers.ArmOverItem);

		protected override void Draw(ref Data.PlayerDrawSet drawInfo) {
			var player = drawInfo.drawPlayer;
			if (player.invis) return;

			var color = BerryLayers.GetColor(player);

			// Taken from https://github.com/Mr-Plauge/MrPlagueRaces-1.4/ and tweaked a bit
			// Let's assume it's correct :D
			var headPosition = new FNA.Vector2(
				(int)(drawInfo.Position.X - Terraria.Main.screenPosition.X - (player.bodyFrame.Width / 2) + (player.width / 2)),
				(int)(drawInfo.Position.Y - Terraria.Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)
			) + player.headPosition + new FNA.Vector2(14 + (player.direction == 1 ? 5 : 5), 28);

			int frameHeight = Texture!.Value.Height / 20;
			int headFrame = player.legFrame.Y / player.legFrame.Height;
			var frame = new FNA.Rectangle { X = 0, Y = frameHeight * headFrame, Height = frameHeight, Width = Texture.Value.Width };
			Data.DrawData backArm = new Data.DrawData(Texture.Value, headPosition, frame, color, drawInfo.compositeBackArmRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
			drawInfo.DrawDataCache.Add(backArm);
		}
	}

	public class FrontArm : TML.PlayerDrawLayer
	{
		public static TextureAsset? Texture;

		public override void Load() => Texture = TML.ModContent.Request<FNA.Graphics.Texture2D>($"{Mod.Name}/Assets/BerriesTransformation_Body");

		public override void Unload() => Texture = null!;

		public override bool GetDefaultVisibility(Data.PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<BerryTransformation>().Activated;

		public override Position GetDefaultPosition() => new AfterParent(Data.PlayerDrawLayers.ArmOverItem);

		protected override void Draw(ref Data.PlayerDrawSet drawInfo) {
			var player = drawInfo.drawPlayer;
			if (player.invis) return;

			var color = BerryLayers.GetColor(player);

			// Taken from https://github.com/Mr-Plauge/MrPlagueRaces-1.4/
			// Let's assume it's correct :D
			var frontArmPosition = new FNA.Vector2(
				(int)(drawInfo.Position.X - Terraria.Main.screenPosition.X - (player.bodyFrame.Width / 2) + (player.width / 2)),
				(int)(drawInfo.Position.Y - Terraria.Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)
			)
			+ player.bodyPosition
			+ new FNA.Vector2(
				(int)(player.bodyFrame.Width / 2),
				(int)(player.bodyFrame.Height / 2)
			);
			// + new FNA.Vector2(2, (int)-2);
			var frontArm = new Data.DrawData(Texture!.Value, frontArmPosition, drawInfo.compFrontArmFrame, color, drawInfo.compositeFrontArmRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
			drawInfo.DrawDataCache.Add(frontArm);
		}
	}

	public class BackArm : TML.PlayerDrawLayer
	{
		public static TextureAsset Texture => FrontArm.Texture!;

		public override bool GetDefaultVisibility(Data.PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<BerryTransformation>().Activated;

		public override Position GetDefaultPosition() => new AfterParent(Data.PlayerDrawLayers.Skin);

		protected override void Draw(ref Data.PlayerDrawSet drawInfo) {
			var player = drawInfo.drawPlayer;
			if (player.invis) return;

			drawInfo.colorHair = FNA.Color.Transparent;
			var color = BerryLayers.GetColor(player);

			// Taken from https://github.com/Mr-Plauge/MrPlagueRaces-1.4/
			// Let's assume it's correct :D
			var backArmPosition = new FNA.Vector2(
				(int)(drawInfo.Position.X - Terraria.Main.screenPosition.X - (player.bodyFrame.Width / 2) + (player.width / 2)),
				(int)(drawInfo.Position.Y - Terraria.Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)
			)
			+ player.bodyPosition
			+ new FNA.Vector2(
				(int)(player.bodyFrame.Width / 2),
				(int)(player.bodyFrame.Height / 2)
			);
			var backArm = new Data.DrawData(Texture.Value, backArmPosition, drawInfo.compBackArmFrame, color, drawInfo.compositeBackArmRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
			drawInfo.DrawDataCache.Add(backArm);
		}
	}
}