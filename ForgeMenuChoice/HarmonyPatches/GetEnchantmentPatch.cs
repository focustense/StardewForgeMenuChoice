using HarmonyLib;
using StardewValley.Tools;
using System.Reflection;
using System.Reflection.Emit;

namespace ForgeMenuChoice.HarmonyPatches;

/// <summary>
/// Patch that overrides which enchantment is applied when forging.
/// This has to be a transpiler - I need only this call to GetEnchantmentFromItem to be different.
/// </summary>
[HarmonyPatch(typeof(Tool))]
internal static class GetEnchantmentPatch
{
    /// <summary>
    /// Function that substitutes in an enchantment.
    /// </summary>
    /// <param name="base_item">Tool.</param>
    /// <param name="item">Thing to enchant with.</param>
    /// <returns>Enchantment to substitute in.</returns>
    private static BaseEnchantment SubstituteEnchantment(Item base_item, Item item)
    {
        try
        {
            if (item.QualifiedItemId == "(O)74" && ForgeMenuPatches.CurrentSelection is not null)
            {
                BaseEnchantment output = ForgeMenuPatches.CurrentSelection;
                ForgeMenuPatches.TrashMenu();
                return output;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed in forcing selection of enchantment.\n\n{ex}", LogLevel.Error);
        }
        return BaseEnchantment.GetEnchantmentFromItem(base_item, item);
    }

    private static Item SubstituteInnateEnchantment(Item weapon, Random r, bool force, List<BaseEnchantment>? enchantsToReRoll = null)
    {
        if (weapon is not MeleeWeapon w || ForgeMenuPatches.CurrentSelection is not { } selection)
        {
            return MeleeWeapon.attemptAddRandomInnateEnchantment(weapon, r, force, enchantsToReRoll);
        }
        w.enchantments.Add(ForgeMenuPatches.CurrentSelection);
        ForgeMenuPatches.TrashMenu();

        return weapon;
    }

    [HarmonyPatch(nameof(Tool.Forge))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            MethodInfo getEnchantmentFromItemInfo = AccessTools.Method(typeof(BaseEnchantment), nameof(BaseEnchantment.GetEnchantmentFromItem));
            MethodInfo substituteEnchantmentInfo = AccessTools.Method(typeof(GetEnchantmentPatch), nameof(SubstituteEnchantment));
            MethodInfo attemptAddRandomInnateEnchantmentInfo = AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.attemptAddRandomInnateEnchantment));
            MethodInfo substituteInnateEnchantmentInfo = AccessTools.Method(typeof(GetEnchantmentPatch), nameof(SubstituteInnateEnchantment));

            return new CodeMatcher(instructions)
                .MatchStartForward(
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldarg_1),
                    new CodeMatch(OpCodes.Call, getEnchantmentFromItemInfo),
                    new CodeMatch(OpCodes.Stloc_0),
                    new CodeMatch(OpCodes.Ldloc_0))
                .Advance(2)
                .SetInstruction(new(OpCodes.Call, substituteEnchantmentInfo))
                .MatchStartForward(new CodeMatch(OpCodes.Call, attemptAddRandomInnateEnchantmentInfo))
                .SetInstruction(new(OpCodes.Call, substituteInnateEnchantmentInfo))
                .InstructionEnumeration();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors transpiling {original.FullDescription()} to use selection.\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}