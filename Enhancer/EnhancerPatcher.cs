using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using Enhancer.Features;
using Enhancer.FeatureInfo;
using HarmonyLib;
using UnityEngine;

namespace Enhancer;

public class EnhancerPatcher : MonoBehaviour
{
    internal static PluginInfo Info { get; set; } = null!;

    internal static ManualLogSource Logger { get; set; } = null!;

    internal static PluginConfig BoundConfig { get; set; } = null!;

    private static readonly IList<IFeatureInfo<IFeature>> Features = [
        new FeatureInfo<AlwaysShowTerminal> {
            Name = "Always show terminal",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.KeepConsoleEnabled.Value,
            ListenToConfigEntries = [BoundConfig.KeepConsoleEnabled],
            DelegateToModGuids = ["mom.llama.enhancer"],
        },
        new FeatureInfo<DaysPerQuota> {
            Name = "Days per quota",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.DaysPerQuotaAssignmentEnabled.Value && BoundConfig.QuotaFormulaEnabled.Value,
            ListenToConfigEntries = [BoundConfig.DaysPerQuotaAssignmentEnabled, BoundConfig.QuotaFormulaEnabled],
            DelegateToModGuids = ["mom.llama.enhancer", "Haha.DynamicDeadline"],
        },
        new FeatureInfo<DeathPenalty> {
            Name = "Death penalty",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.DeathPenaltyFormulaEnabled.Value,
            ListenToConfigEntries = [BoundConfig.DeathPenaltyFormulaEnabled],
        },
        new FeatureInfo<HangarDoorCloseDuration> {
            Name = "Hangar door close duration",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.DoorPowerDurationEnabled.Value,
            ListenToConfigEntries = [BoundConfig.DoorPowerDurationEnabled],
            DelegateToModGuids = ["mom.llama.enhancer"],
        },
        new FeatureInfo<HideClock> {
            Name = "Hide Clock",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.HideClockEnabled.Value,
            ListenToConfigEntries = [BoundConfig.HideClockEnabled, BoundConfig.HideClockOutside, BoundConfig.HideClockOnShip, BoundConfig.HideClockInFacility],
            DelegateToModGuids = ["atk.lethalcompany.shipclock"],
        },
        new FeatureInfo<ThreatScanCommand> {
            Name = "Threat scanner",
            EnabledCondition = () =>
                BoundConfig.Enabled.Value && BoundConfig.ThreatScanner.Value is not ThreatScannerMode.Disabled,
            ListenToConfigEntries = [BoundConfig.ThreatScanner],
            DelegateToModGuids = ["mom.llama.enhancer"],
        },
        new FeatureInfo<ItemProtection> {
            Name = "Item protection",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.ScrapProtectionEnabled.Value,
            ListenToConfigEntries = [BoundConfig.ScrapProtectionEnabled],
            DelegateToModGuids = ["mom.llama.enhancer"],
        },
        new FeatureInfo<MilitaryTime> {
            Name = "24-hour clock",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.MilitaryTime.Value,
            ListenToConfigEntries = [BoundConfig.MilitaryTime],
            DelegateToModGuids = ["com.zduniusz.lethalcompany.24hourclock"],
        },
        new FeatureInfo<CompanyBuyingFactorTweaks> {
            Name = "Company buying factor tweaks",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.CompanyBuyingFactorTweaksEnabled.Value,
            DelegateToModGuids = ["mom.llama.enhancer"],
        },
        new FeatureInfo<QuotaFormula> {
            Name = "Quota formula",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.QuotaFormulaEnabled.Value,
            ListenToConfigEntries = [BoundConfig.QuotaFormulaEnabled],
        },
        new FeatureInfo<RemoveSavedItemCap> {
            Name = "Remove Saved Item Cap",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.RemoveSavedItemCapEnabled.Value,
            ListenToConfigEntries = [BoundConfig.RemoveSavedItemCapEnabled],
        },
        new FeatureInfo<SavedItemCap> {
            Name = "Saved Item Cap",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.SavedItemCapEnabled.Value && !BoundConfig.RemoveSavedItemCapEnabled.Value,
            ListenToConfigEntries = [BoundConfig.SavedItemCapEnabled, BoundConfig.RemoveSavedItemCapEnabled],
            DelegateToModGuids = ["MoreItems"]
        },
        new FeatureInfo<ScrapTweaks> {
            Name = "Scrap Tweaks",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.ScrapTweaksEnabled.Value,
            ListenToConfigEntries = [BoundConfig.ScrapPlayercountScaling, BoundConfig.ScrapQuantityScalar, BoundConfig.ScrapValueScalar]
        },
        new FeatureInfo<StartingCredits> {
            Name = "Starting credits",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.StartingCreditsEnabled.Value,
            ListenToConfigEntries = [BoundConfig.StartingCreditsEnabled],
        },
        new FeatureInfo<PassiveIncome> {
            Name = "Passive income",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.PassiveIncomeEnabled.Value,
            ListenToConfigEntries = [BoundConfig.PassiveIncomeEnabled],
        },
        new FeatureInfo<FreeUnlockables> {
            Name = "Suit unlock",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.FreeUnlockablesEnabled.Value,
            ListenToConfigEntries = [BoundConfig.FreeUnlockablesEnabled],
            DelegateToModGuids = ["mom.llama.enhancer"],
        },
        new FeatureInfo<TimeSpeed> {
            Name = "Time speed",
            EnabledCondition = () => BoundConfig.Enabled.Value && BoundConfig.TimeSpeedEnabled.Value,
            ListenToConfigEntries = [BoundConfig.TimeSpeedEnabled],
            DelegateToModGuids = ["mom.llama.enhancer"],
        }
    ];

    private void Start()
    {
        FeatureInfoInitializers.HarmonyFactory =
            harmonyName => new Harmony($"{MyPluginInfo.PLUGIN_GUID}-{harmonyName}");
        FeatureInfoInitializers.LogSourceFactory =
            patchName => BepInEx.Logging.Logger.CreateLogSource($"{MyPluginInfo.PLUGIN_NAME}/{patchName}");

        Logger.LogInfo("Initialising patches...");
        Features.Do(patch => patch.Initialise());
        Logger.LogInfo("Done!");
    }

    private void OnDestroy()
    {
        Features.Do(patch => patch.Dispose());
    }
}
