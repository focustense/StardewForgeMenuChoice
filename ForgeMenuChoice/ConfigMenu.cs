using GenericModConfigMenu;

namespace ForgeMenuChoice;

/// <summary>
/// Helper for registering the mod's config menu with GMCM.
/// </summary>
internal class ConfigMenu
{
    /// <summary>
    /// Registers the mod's config menu page(s) with GMCM.
    /// </summary>
    /// <param name="gmcm">The GMCM API.</param>
    /// <param name="mod">The mod manifest.</param>
    /// <param name="translations">Translation helper for the current mod.</param>
    /// <param name="config">Function to retrieve the current configuration data.</param>
    /// <param name="reset">Delegate to reset/recreate the configuration data.</param>
    /// <param name="save">Delegate to save the current configuration data.</param>
    public static void Register(
        IGenericModConfigMenuApi gmcm,
        IManifest mod,
        ITranslationHelper translations,
        Func<ModConfig> config,
        Action reset,
        Action save)
    {
        gmcm.Register(mod, reset, save);
        gmcm.AddParagraph(mod, I18n.ModDescription);
        gmcm.AddTextOption(
            mod,
            name: I18n.TooltipBehavior_Title,
            tooltip: I18n.TooltipBehavior_Description,
            getValue: () => config().TooltipBehavior.ToString(),
            setValue: value => config().TooltipBehavior = Enum.Parse<TooltipBehavior>(value),
            allowedValues: Enum.GetValues<TooltipBehavior>().Select(e => e.ToString()).ToArray(),
            formatAllowedValue: value => translations.Get($"config.TooltipBehavior.{value}"));
        gmcm.AddBoolOption(
            mod,
            name: I18n.EnableTooltipAutogeneration_Title,
            tooltip: I18n.EnableTooltipAutogeneration_Description,
            getValue: () => config().EnableTooltipAutogeneration,
            setValue: value => config().EnableTooltipAutogeneration = value);
        gmcm.AddKeybindList(
            mod,
            name: I18n.LeftArrow_Title,
            tooltip: I18n.LeftArrow_Description,
            getValue: () => config().LeftArrow,
            setValue: value => config().LeftArrow = value);
        gmcm.AddKeybindList(
            mod,
            name: I18n.RightArrow_Title,
            tooltip: I18n.RightArrow_Description,
            getValue: () => config().RightArrow,
            setValue: value => config().RightArrow = value);
        gmcm.AddBoolOption(
            mod,
            name: I18n.OverrideInnateEnchantments_Title,
            tooltip: I18n.OverrideInnateEnchantments_Description,
            getValue: () => config().OverrideInnateEnchantments,
            setValue: value => config().OverrideInnateEnchantments = value);
    }
}
