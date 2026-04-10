global using static Terraria_JJK.EC.ComponentExtensions;
global using DaybreakHooks = Daybreak.Common.Features.Hooks;
global using FNA = Microsoft.Xna.Framework;
global using TML = Terraria.ModLoader;
using Locale = Terraria.Localization;

namespace Terraria_JJK;

public class Terraria_JJK : TML.Mod
{
	public static string AssetPath(string subpath) => $"{nameof(Terraria_JJK)}/Assets/{subpath}";

	public static string GetLocalization(string key) => Locale.Language.GetTextValue($"Mods.{nameof(Terraria_JJK)}.{key}");

	public static string GetLocalizationWith(string key, object data) => Locale.Language.GetTextValueWith($"Mods.{nameof(Terraria_JJK)}.{key}", data);

	public static Locale.NetworkText LocalizedNetworkText(string key) => Locale.NetworkText.FromLiteral(GetLocalization(key));

	public static Locale.NetworkText LocalizedNetworkTextWith(string key, object data) => Locale.NetworkText.FromLiteral(GetLocalizationWith(key, data));
}