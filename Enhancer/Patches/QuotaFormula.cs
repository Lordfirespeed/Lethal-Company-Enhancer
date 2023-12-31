using HarmonyLib;

namespace Enhancer.Patches;

public class QuotaFormula : IPatch
{
    [HarmonyPatch(typeof(StartOfRound), "Start")]
    [HarmonyPrefix]
    public static void StartOfRoundShipStartPre()
    {
        Plugin.Logger.LogInfo("Setting quota formula variables...");
        var quotaSettings = TimeOfDay.Instance.quotaVariables;
        quotaSettings.startingQuota = Plugin.BoundConfig.StartingQuota.Value;
        // vanilla 'increase steepness' is actually 'increase shallowness', so we reciprocate (1/x) the value
        quotaSettings.increaseSteepness = 1f / Plugin.BoundConfig.QuotaIncreaseSteepness.Value;
        quotaSettings.baseIncrease = Plugin.BoundConfig.QuotaBaseIncrease.Value;
        quotaSettings.randomizerMultiplier = Plugin.BoundConfig.QuotaIncreaseRandomFactor.Value;
    }
}