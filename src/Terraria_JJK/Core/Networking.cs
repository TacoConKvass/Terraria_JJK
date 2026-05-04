namespace Terraria_JJK.Networking;

public struct PacketResult
{
	public bool Success;
	public string? FailReason;

	public static readonly PacketResult Successful = new PacketResult {
		Success = true,
		FailReason = null
	};
	public static readonly PacketResult UnknownPacket = new PacketResult {
		Success = false,
		FailReason = "Unknown packet type"
	};
}

public enum PacketType : byte
{
	SyncSpecialUseStyle
}