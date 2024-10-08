﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
using StardewValley.Menus;

namespace ForgeMenuChoice;

/// <summary>
/// Menu to allow choice of enchantments at the forge.
/// </summary>
internal sealed class ForgeSelectionMenu : IClickableMenu
{
    // These numbers are used for controllers in theory.
    private const int RegionBackButton = 101345;
    private const int RegionForwardButton = 1024323;

    private const int Width = 400; // px
    private const int Height = 188; // px

    private readonly int inherentWidth;

    private readonly bool shouldShowTooltip;
    private readonly List<BaseEnchantment> options = new();

    private ClickableTextureComponent backButton;
    private ClickableTextureComponent forwardButton;

    private Rectangle hoverrect;

    private int index = 0;

    private bool isHovered = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForgeSelectionMenu"/> class.
    /// </summary>
    /// <param name="options">A list of possible enchantments.</param>
    /// <param name="tool">The tool instance this menu was created for.</param>
    /// <param name="isInnate">Whether or not this menu is for innate enchantments.</param>
    internal ForgeSelectionMenu(List<BaseEnchantment> options, Tool tool, bool isInnate)
        : base(GetXPosFromViewport(Game1.uiViewport.Width), GetYPosFromViewport(Game1.viewport.Height), Width, Height)
    {
        this.options = options;
        this.Tool = tool;
        this.IsInnate = isInnate;

        this.inherentWidth = (int)MathF.Ceiling(Game1.dialogueFont.MeasureString("Matador de Insetos").X) + 12;

        this.backButton = this.GetBackButton();
        this.forwardButton = this.GetForwardButton();

        this.hoverrect = this.GetHoverRect();

        this.shouldShowTooltip = ModEntry.Config.TooltipBehavior == TooltipBehavior.On
            || (ModEntry.Config.TooltipBehavior == TooltipBehavior.Immersive && Utility.HasAnyPlayerSeenSecretNote(1008));
    }

    /// <summary>
    /// Gets the tool instance this menu was created for.
    /// </summary>
    internal Tool Tool { get; init; }

    /// <summary>
    /// Gets a value indicating whether whether or not this is an innate enchantment.
    /// </summary>
    internal bool IsInnate { get; init; }

    /// <summary>
    /// Gets the currently selected enchantment.
    /// </summary>
    internal BaseEnchantment CurrentSelectedOption => this.options[this.Index % this.options.Count];

    private static Texture2D Graphics => AssetLoader.UIElement;

    private static IDictionary<string, string> TooltipData => AssetLoader.TooltipData;

    /// <summary>
    /// Gets the display name of the currently selected enchantment.
    /// </summary>
    private string CurrentSelectedTranslatedOption => this.CurrentSelectedOption.GetDisplayName();

    private int Index
    {
        get
        {
            return this.index;
        }

        set
        {
            if (value < 0)
            {
                value += this.options.Count;
            }
            this.index = value % this.options.Count;
        }
    }

    /// <summary>
    /// Processes a right click. (In this case, same as left click).
    /// </summary>
    /// <param name="x">X location.</param>
    /// <param name="y">Y location.</param>
    /// <param name="playSound">Whether or not to play sounds.</param>
    public override void receiveRightClick(int x, int y, bool playSound = true)
        => this.receiveLeftClick(x, y, playSound);

    /// <summary>
    /// Processes a left click.
    /// </summary>
    /// <param name="x">X location clicked.</param>
    /// <param name="y">Y location clicked.</param>
    /// <param name="playSound">Whether or not to play sounds.</param>
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        try
        {
            base.receiveLeftClick(x, y, playSound);
            if (this.backButton.containsPoint(x, y))
            {
                this.RetreatPosition(playSound);
            }
            else if (this.forwardButton.containsPoint(x, y))
            {
                this.AdvancePosition(playSound);
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while trying to process a left click on smol menu.\n\n{ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Handles a change in window size.
    /// </summary>
    /// <param name="oldBounds">Old size.</param>
    /// <param name="newBounds">New size.</param>
    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        try
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = GetXPosFromViewport(Game1.uiViewport.Width);
            this.yPositionOnScreen = GetYPosFromViewport(Game1.uiViewport.Height);
            this.backButton = this.GetBackButton();
            this.forwardButton = this.GetForwardButton();
            this.hoverrect = this.GetHoverRect();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in trying to adjust window size for smol menu\n{ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Handles hovering over menu elements.
    /// </summary>
    /// <param name="x">x location.</param>
    /// <param name="y">y location.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override void performHoverAction(int x, int y)
    {
        try
        {
            base.performHoverAction(x, y);
            if (this.hoverrect.Contains(x, y))
            {
                this.isHovered = true;
            }
            else
            {
                this.isHovered = false;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors handling hover on smol menu.\n{ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Draws the menu to the screen.
    /// </summary>
    /// <param name="b">The spritebatch to draw with.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override void draw(SpriteBatch b)
    {
        try
        {
            base.draw(b);
            int stringWidth = Math.Max(this.inherentWidth, (int)MathF.Ceiling(Game1.dialogueFont.MeasureString(this.CurrentSelectedTranslatedOption).X));
            drawTextureBox(
                b,
                texture: Graphics,
                sourceRect: new Rectangle(0, 0, 15, 15),
                x: this.xPositionOnScreen + ((Width - stringWidth - 64) / 2),
                y: this.yPositionOnScreen + (Height / 2) - 40,
                width: stringWidth + 64,
                height: 80,
                color: Color.White,
                scale: 4f
                );
            Vector2 thisOptionSizeOffset = Game1.dialogueFont.MeasureString(this.CurrentSelectedTranslatedOption);
            Utility.drawTextWithShadow(
                b,
                text: this.CurrentSelectedTranslatedOption,
                font: Game1.dialogueFont,
                position: new Vector2(
                    this.xPositionOnScreen + (Width / 2) - ((int)thisOptionSizeOffset.X / 2),
                    this.yPositionOnScreen + (Height / 2) - ((int)thisOptionSizeOffset.Y / 2)),
                color: Game1.textColor);
            this.backButton.draw(b);
            this.forwardButton.draw(b);
            this.backButton.scale = Math.Min(this.backButton.scale + 0.05f, this.backButton.baseScale);
            this.forwardButton.scale = Math.Min(this.forwardButton.scale + 0.05f, this.forwardButton.baseScale);
            if (this.shouldShowTooltip && this.isHovered && TooltipData.TryGetValue(this.CurrentSelectedOption.GetName(), out string? tooltip))
            {
                // The tooltip should be previously wrapped.
                IClickableMenu.drawHoverText(b, tooltip, Game1.smallFont);
            }
            this.drawMouse(b);
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into difficulties trying to draw smol menu!\n{ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Retreat a position on the menu.
    /// </summary>
    /// <param name="playSound">Whether or not to play a sound.</param>
    internal void RetreatPosition(bool playSound)
    {
        this.Index--;
        this.backButton.scale = this.backButton.baseScale - 1;
        if (playSound)
        {
            Game1.playSound("shwip");
        }
    }

    /// <summary>
    /// Advance a position on the menu.
    /// </summary>
    /// <param name="playSound">Whether or not to play a sound.</param>
    internal void AdvancePosition(bool playSound)
    {
        this.Index++;
        this.forwardButton.scale = this.forwardButton.baseScale - 1;
        if (playSound)
        {
            Game1.playSound("shwip");
        }
    }

    private static int GetXPosFromViewport(int uiViewportX)
        => Game1.activeClickableMenu is not null
            ? Game1.activeClickableMenu.xPositionOnScreen + 120
            : Math.Max(((uiViewportX - (int)(Width * Game1.options.baseUIScale)) / 2) - (int)(80 * Game1.options.baseUIScale), 0);

    private static int GetYPosFromViewport(int uiViewportY)
        => Game1.activeClickableMenu is not null
            ? Game1.activeClickableMenu.yPositionOnScreen - 40
            : Math.Max(((uiViewportY - (int)(Height * Game1.options.baseUIScale)) / 2) - (int)(360 * Game1.options.baseUIScale), 0);

    #region buttons

    private ClickableTextureComponent GetBackButton()
        => new(
            bounds: new Rectangle(this.xPositionOnScreen - 80, this.yPositionOnScreen + (Height / 2) - 22, 48, 44),
            texture: Graphics,
            sourceRect: new Rectangle(18, 3, 12, 11),
            scale: 4f)
        {
            myID = RegionBackButton,
            rightNeighborID = RegionForwardButton,
        };

    private ClickableTextureComponent GetForwardButton()
        => new(
            bounds: new Rectangle(this.xPositionOnScreen + Width + 36, this.yPositionOnScreen + (Height / 2) - 22, 48, 44),
            texture: Graphics,
            sourceRect: new Rectangle(34, 3, 12, 11),
            scale: 4f)
        {
            myID = RegionForwardButton,
            leftNeighborID = RegionBackButton,
        };

    private Rectangle GetHoverRect()
    {
        return new Rectangle(
                   x: this.xPositionOnScreen + ((Width - this.inherentWidth - 64) / 2),
                   y: this.yPositionOnScreen + (Height / 2) - 40,
                   width: this.inherentWidth + 64,
                   height: 80
                   );
    }

    #endregion
}