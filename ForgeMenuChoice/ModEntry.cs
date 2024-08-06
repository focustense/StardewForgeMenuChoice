using ForgeMenuChoice.HarmonyPatches;
using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI.Events;

namespace ForgeMenuChoice;

/// <inheritdoc/>
internal sealed class ModEntry : Mod
{
    /// <summary>
    /// Gets the logger for this file.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    /// <summary>
    /// Gets the translation helper for this mod.
    /// </summary>
    internal static ITranslationHelper TranslationHelper { get; private set; } = null!;

    /// <summary>
    /// Gets the configuration class for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; } = null!;

    /// <summary>
    /// Gets the input helper for this mod.
    /// </summary>
    internal static IInputHelper InputHelper { get; private set; } = null!;

    /// <summary>
    /// Gets a delegate that checks to see if the forge instance is Casey's NewForgeMenu or not.
    /// </summary>
    internal static Func<object, bool>? IsSpaceForge { get; private set; } = null;

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;

        I18n.Init(helper.Translation);

        TranslationHelper = helper.Translation;
        InputHelper = helper.Input;

        AssetLoader.Initialize(helper.GameContent);
        Config = helper.ReadConfig<ModConfig>();

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        helper.Events.Player.Warped += this.Player_Warped;
        helper.Events.Content.AssetRequested += static (_, e) => AssetLoader.OnLoadAsset(e);
        helper.Events.Content.LocaleChanged += this.OnLocaleChanged;
        helper.Events.Content.AssetsInvalidated += static (_, e) => AssetLoader.Refresh(e.NamesWithoutLocale);

        helper.Events.Input.ButtonsChanged += static (_, e) => ForgeMenuPatches.ApplyButtonPresses(e);
    }

    /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
    /// <remarks>We must wait until GameLaunched to patch in order to patch Spacecore.</remarks>
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));

        var gmcm = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (gmcm is not null)
        {
            ConfigMenu.Register(
                gmcm,
                this.ModManifest,
                this.Helper.Translation,
                static () => Config,
                static () => Config = new ModConfig(),
                () => this.Helper.WriteConfig(Config));
        }
    }

    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            harmony.PatchAll(typeof(ModEntry).Assembly);
        }
        catch (Exception ex)
        {
            ModMonitor.Log(string.Format("Mod crashed while applying harmony patches:\n\n{0}", ex), LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

    #region assets

    private void Player_Warped(object? sender, WarpedEventArgs e)
    {
        if (e.IsLocalPlayer)
        {
            AssetLoader.Refresh();
        }
    }

    private void OnLocaleChanged(object? sender, LocaleChangedEventArgs e)
    {
        var content = this.Helper.GameContent;
        content.InvalidateCache(AssetLoader.ENCHANTMENT_NAMES_LOCATION);
        if (content.CurrentLocaleConstant != LocalizedContentManager.LanguageCode.en)
        {
            content.InvalidateCache($"{AssetLoader.ENCHANTMENT_NAMES_LOCATION}.{content.CurrentLocale}");
        }
        AssetLoader.Refresh();
    }

    #endregion
}
