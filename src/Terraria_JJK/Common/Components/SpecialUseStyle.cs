namespace Terraria_JJK.Components;

[EC.Component]
public record struct SpecialUseStyle
{

	internal record struct Net(float rotation, FNA.Vector2 relative_location, ArmMode arm_mode);

	[System.Flags]
	public enum ArmMode : byte
	{
		/// <summary> Whether the front composite arm should be affected </summary>
		Front = 1,
		/// <summary> Whether the back composite arm should be affected </summary>
		Back = 2,
		/// <summary> Whether the composite arm should be perpendicular to the held item</summary>
		Perpendicular = 4,
	}

	public System.Func<Terraria.Player, float>? Rotation;
	public System.Func<Terraria.Player, float, FNA.Vector2> Location;
	public bool Diagonal;
	public bool TurnOnUse;
	public ArmMode CompositeArmMode;

	Net net;

	static int ID;
	[DaybreakHooks.OnLoad]
	static void Load() => ID = TML.ItemLoader.RegisterUseStyle(Terraria_JJK.Instance, "SpecialUseStyle");

	class SetItemDefaults : TML.GlobalItem
	{
		public override void SetDefaults(Terraria.Item item) => item.useStyle = item.Enabled<SpecialUseStyle>() ? ID : item.useStyle;
	}

	[DaybreakHooks.GlobalItemHooks.UseStyle]
	static void UseStyle(Terraria.Item item, Terraria.Player player, FNA.Rectangle heldItemFrame) {
		if (!item.TryGet(out SpecialUseStyle data)) return;

		float rotation;
		FNA.Vector2 relative_location;
		ArmMode arm_mode;

		if (player.ItemAnimationJustStarted && player.whoAmI == Terraria.Main.myPlayer) {
			rotation = data.Rotation?.Invoke(player) ?? 0;
			relative_location = data.Location?.Invoke(player, rotation) ?? FNA.Vector2.Zero;
			arm_mode = data.CompositeArmMode;

			var player_data = player.With(data with {
				net = new Net() {
					rotation = rotation,
					relative_location = relative_location,
					arm_mode = arm_mode
				}
			});

			if (Terraria.Main.netMode == Terraria.ID.NetmodeID.MultiplayerClient) Sync(player.whoAmI, player_data.net);
		}
		else {
			rotation = player.TryGet(out SpecialUseStyle player_data) ? player_data.net.rotation : 0;
			relative_location = player_data.net.relative_location;
			arm_mode = player_data.net.arm_mode;
		}

		var rotation_vector = Terraria.Utils.ToRotationVector2(rotation);
		player.direction = Terraria.Utils.ToDirectionInt(rotation_vector.X >= 0);

		float angle = (player.direction, data.Diagonal) switch {
			( >= 0, true) => FNA.MathHelper.PiOver4,
			(_, true) => FNA.MathHelper.PiOver4 * 3,
			_ => 0
		};

		player.itemRotation = rotation + angle;
		player.itemLocation = player.MountedCenter + relative_location; // + rotation_vector * data.HoldDistance;

		float arm_angle;
		if (arm_mode.HasFlag(ArmMode.Perpendicular))
			arm_angle = player.direction >= 0 ? 0 : FNA.MathHelper.Pi;
		else arm_angle = -FNA.MathHelper.PiOver2;

		if (arm_mode.HasFlag(ArmMode.Front))
			player.SetCompositeArmFront(true, Terraria.Player.CompositeArmStretchAmount.Full, rotation + arm_angle);
		if (arm_mode.HasFlag(ArmMode.Back))
			player.SetCompositeArmBack(true, Terraria.Player.CompositeArmStretchAmount.Full, rotation + arm_angle);

		player.FlipItemLocationAndRotationForGravity();
	}

	[DaybreakHooks.GlobalItemHooks.UseItemFrame]
	static void UseItemFrame(Terraria.Item item, Terraria.Player player) {
		player.bodyFrame.Y = player.bodyFrame.Height;
	}

	internal static void Sync(int player, in Net data) {
		var packet = Terraria_JJK.Instance.GetPacket();
		packet.Write(data.rotation);
		packet.Write(data.relative_location.X);
		packet.Write(data.relative_location.Y);
		packet.Write((byte)data.arm_mode);
	}

	internal static Networking.PacketResult Receive(System.IO.BinaryReader reader, int whoAmI) {
		int player = reader.ReadByte();
		bool server = Terraria.Main.netMode == Terraria.ID.NetmodeID.Server;

		if (server) player = whoAmI;

		var data = Terraria.Main.player[player].With(new SpecialUseStyle {
			net = new Net() {
				rotation = reader.ReadSingle(),
				relative_location = new FNA.Vector2 {
					X = reader.ReadSingle(),
					Y = reader.ReadSingle()
				},
				arm_mode = (ArmMode)reader.ReadByte(),
			}
		});

		if (server) Sync(player, data.net);
		return Networking.PacketResult.Successful;
	}
};