﻿using Celeste;
using ExtendedVariants.Module;
using System;
using System.Collections.Generic;

namespace ExtendedVariants.UI {
    /// <summary>
    /// The randomizer options submenu. Parameters = none.
    /// </summary>
    public class OuiRandomizerOptions : AbstractSubmenu {

        public OuiRandomizerOptions() : base("MODOPTIONS_EXTENDEDVARIANTS_RANDOMIZERTITLE", "MODOPTIONS_EXTENDEDVARIANTS_RANDOMIZER") { }
        
        /// <summary>
        /// List of options shown for Change Variants Interval.
        /// </summary>
        private static int[] changeVariantsIntervalScale = new int[] {
            0, 1, 2, 5, 10, 15, 30, 60
        };

        /// <summary>
        /// Finds out the index of an interval in the changeVariantsIntervalScale table.
        /// If it is not present, will return the previous option.
        /// (For example, 26s will return the index for 15s.)
        /// </summary>
        /// <param name="option">The interval</param>
        /// <returns>The index of the interval in the changeVariantsIntervalScale table</returns>
        private static int indexFromChangeVariantsInterval(int option) {
            for (int index = 0; index < changeVariantsIntervalScale.Length - 1; index++) {
                if (changeVariantsIntervalScale[index + 1] > option) {
                    return index;
                }
            }

            return changeVariantsIntervalScale.Length - 1;
        }
        

        private static int[] vanillafyScale = new int[] {
            0, 15, 30, 60, 120, 300, 600
        };

        private static int indexFromVanillafyScale(int option) {
            for (int index = 0; index < vanillafyScale.Length - 1; index++) {
                if (vanillafyScale[index + 1] > option) {
                    return index;
                }
            }

            return vanillafyScale.Length - 1;
        }

        private class OptionItems {
            public HashSet<TextMenu.Item> VanillaVariantOptions = new HashSet<TextMenu.Item>();
            public HashSet<TextMenu.Item> ExtendedVariantOptions = new HashSet<TextMenu.Item>();
            public TextMenu.Option<int> VanillafyOption;
        }

        protected override void addOptionsToMenu(TextMenu menu, bool inGame, object[] parameters) {
            OptionItems items = new OptionItems();

            // Add the general settings
            menu.Add(new TextMenu.SubHeader(Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_RANDOMIZER_GENERALSETTINGS")));
            menu.Add(new TextMenu.Slider(Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_CHANGEVARIANTSINTERVAL"),
                i => {
                    if (i == 0) return Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_ONSCREENTRANSITION");
                    return $"{Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_EVERY")} {changeVariantsIntervalScale[i]}s";
                }, 0, changeVariantsIntervalScale.Length - 1, indexFromChangeVariantsInterval(ExtendedVariantsModule.Settings.ChangeVariantsInterval))
                .Change(i => {
                    ExtendedVariantsModule.Settings.ChangeVariantsInterval = changeVariantsIntervalScale[i];
                    refreshOptionMenuEnabledStatus(items);
                    ExtendedVariantsModule.Instance.Randomizer.UpdateCountersFromSettings();
                }));

            menu.Add(new TextMenu.Slider(Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_RANDOMIZER_VARIANTSET"),
                i => Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_" + new string[] { "OFF", "VANILLA", "EXTENDED", "BOTH" }[i]), 1, 3, ExtendedVariantsModule.Settings.VariantSet)
                .Change(i => {
                    ExtendedVariantsModule.Settings.VariantSet = i;
                    refreshOptionMenuEnabledStatus(items);
                }));

            TextMenu.Option<int> maxEnabledVariants = new TextMenu.Slider(
                Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_RANDOMIZER_MAXENABLEDVARIANTS" + (ExtendedVariantsModule.Settings.RerollMode ? "_REROLL" : "")),
                i => i.ToString(), 0, ExtendedVariantsModule.Instance.VariantHandlers.Count + 13, ExtendedVariantsModule.Settings.MaxEnabledVariants)
                .Change(newValue => ExtendedVariantsModule.Settings.MaxEnabledVariants = newValue);

            menu.Add(new TextMenu.OnOff(Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_RANDOMIZER_REROLLMODE"), ExtendedVariantsModule.Settings.RerollMode)
                .Change(newValue => {
                    ExtendedVariantsModule.Settings.RerollMode = newValue;
                    maxEnabledVariants.Label = Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_RANDOMIZER_MAXENABLEDVARIANTS" + (newValue ? "_REROLL" : ""));
                }));

            menu.Add(maxEnabledVariants);

            menu.Add(items.VanillafyOption = new TextMenu.Slider(Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_RANDOMIZER_VANILLAFY"), i => {
                if (i == 0) return Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_DISABLED");
                i = vanillafyScale[i];
                if(i < 60) return $"{i.ToString()}s";
                return $"{(i / 60).ToString()} min";
            }, 0, vanillafyScale.Length - 1, indexFromVanillafyScale(ExtendedVariantsModule.Settings.Vanillafy))
                .Change(newValue => {
                    ExtendedVariantsModule.Settings.Vanillafy = vanillafyScale[newValue];
                    ExtendedVariantsModule.Instance.Randomizer.UpdateCountersFromSettings();
                }));

            menu.Add(new TextMenu.OnOff(Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_RANDOMIZER_DISPLAYONSCREEN"), ExtendedVariantsModule.Settings.DisplayEnabledVariantsToScreen)
                .Change(newValue => ExtendedVariantsModule.Settings.DisplayEnabledVariantsToScreen = newValue));

            // build the toggles to individually enable or disable all vanilla variants
            menu.Add(new TextMenu.SubHeader(Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_RANDOMIZER_ENABLED_VANILLA")));
            items.VanillaVariantOptions.Add(addToggleOptionToMenu(menu, VanillaVariant.GameSpeed));
            items.VanillaVariantOptions.Add(addToggleOptionToMenu(menu, VanillaVariant.MirrorMode));
            items.VanillaVariantOptions.Add(addToggleOptionToMenu(menu, VanillaVariant.ThreeSixtyDashing));
            items.VanillaVariantOptions.Add(addToggleOptionToMenu(menu, VanillaVariant.InvisibleMotion));
            items.VanillaVariantOptions.Add(addToggleOptionToMenu(menu, VanillaVariant.NoGrabbing));
            items.VanillaVariantOptions.Add(addToggleOptionToMenu(menu, VanillaVariant.LowFriction));
            items.VanillaVariantOptions.Add(addToggleOptionToMenu(menu, VanillaVariant.SuperDashing));
            items.VanillaVariantOptions.Add(addToggleOptionToMenu(menu, VanillaVariant.Hiccups));
            items.VanillaVariantOptions.Add(addToggleOptionToMenu(menu, VanillaVariant.PlayAsBadeline));

            menu.Add(new TextMenu.SubHeader(Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_RANDOMIZER_ENABLED_VANILLA_ASSISTS")));
            items.VanillaVariantOptions.Add(addToggleOptionToMenu(menu, VanillaVariant.InfiniteStamina));
            items.VanillaVariantOptions.Add(addToggleOptionToMenu(menu, VanillaVariant.DashMode));
            items.VanillaVariantOptions.Add(addToggleOptionToMenu(menu, VanillaVariant.Invincible));
            items.VanillaVariantOptions.Add(addToggleOptionToMenu(menu, VanillaVariant.DashAssist));

            // and do the same with extended ones
            menu.Add(new TextMenu.SubHeader(Dialog.Clean("MODOPTIONS_EXTENDEDVARIANTS_RANDOMIZER_ENABLED_EXTENDED")));
            foreach(ExtendedVariantsModule.Variant variant in ExtendedVariantsModule.Instance.VariantHandlers.Keys) {
                items.ExtendedVariantOptions.Add(addToggleOptionToMenu(menu, variant));
            }

            refreshOptionMenuEnabledStatus(items);
        }

        private static void refreshOptionMenuEnabledStatus(OptionItems items) {
            foreach (TextMenu.Item item in items.VanillaVariantOptions) item.Disabled = (ExtendedVariantsModule.Settings.VariantSet % 2 == 0);
            foreach (TextMenu.Item item in items.ExtendedVariantOptions) item.Disabled = (ExtendedVariantsModule.Settings.VariantSet / 2 == 0);
            items.VanillafyOption.Disabled = ExtendedVariantsModule.Settings.ChangeVariantsInterval != 0;

            if(ExtendedVariantsModule.Settings.ChangeVariantsInterval != 0 && ExtendedVariantsModule.Settings.Vanillafy != 0) {
                // vanillafy is impossible, so set its value to 0
                ExtendedVariantsModule.Settings.Vanillafy = 0;
                items.VanillafyOption.PreviousIndex = items.VanillafyOption.Index;
                items.VanillafyOption.Index = 0;
                items.VanillafyOption.ValueWiggler.Start();
            }
        }

        private static TextMenu.Item addToggleOptionToMenu(TextMenu menu, VanillaVariant variant) {
            return addToggleOptionToMenu(menu, variant.Name, variant.Label);
        }

        private static TextMenu.Item addToggleOptionToMenu(TextMenu menu, ExtendedVariantsModule.Variant variant) {
            return addToggleOptionToMenu(menu, variant.ToString(), "MODOPTIONS_EXTENDEDVARIANTS_" + variant.ToString().ToUpperInvariant());
        }

        private static TextMenu.Item addToggleOptionToMenu(TextMenu menu, string keyName, string label) {
            TextMenu.Option<bool> toggle = new TextMenuExt.OnOff(Dialog.Clean(label),
                ExtendedVariantsModule.Settings.RandomizerEnabledVariants.TryGetValue(keyName, out bool val) ? val : true, false)
                .Change(newValue => ExtendedVariantsModule.Settings.RandomizerEnabledVariants[keyName] = newValue);
            menu.Add(toggle);
            return toggle;
        }

        protected override void gotoMenu(Overworld overworld) {
            overworld.Goto<OuiRandomizerOptions>();
        }
    }
}
