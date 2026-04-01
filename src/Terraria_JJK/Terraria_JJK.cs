global using TML = Terraria.ModLoader;
global using FNA = Microsoft.Xna.Framework;
global using DaybreakHooks = Daybreak.Common.Features.Hooks;
global using static Terraria_JJK.EC.ComponentExtensions;

using Locale = Terraria.Localization;

namespace Terraria_JJK;

public class Terraria_JJK : TML.Mod
{
	public const string AssetPath = $"{nameof(Terraria_JJK)}/Assets/";

	public static string GetLocalization(string key) => Locale.Language.GetTextValue($"Mods.{nameof(Terraria_JJK)}.{key}");

	public static string GetLocalizationWith(string key, object data) => Locale.Language.GetTextValueWith($"Mods.{nameof(Terraria_JJK)}.{key}", data);

	public static Locale.NetworkText LocalizedNetworkText(string key) => Locale.NetworkText.FromLiteral(GetLocalization(key));

	public static Locale.NetworkText LocalizedNetworkTextWith(string key, object data) => Locale.NetworkText.FromLiteral(GetLocalizationWith(key, data));
}