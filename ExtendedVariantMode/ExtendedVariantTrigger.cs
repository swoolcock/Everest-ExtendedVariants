﻿using Celeste;
using Celeste.Mod.Entities;
using ExtendedVariants.Module;
using Microsoft.Xna.Framework;

namespace ExtendedVariants {
    [CustomEntity("ExtendedVariantTrigger", "ExtendedVariantMode/ExtendedVariantTrigger")]
    public class ExtendedVariantTrigger : Trigger {
        private ExtendedVariantsModule.Variant variantChange;
        private int newValue;
        private bool revertOnLeave;
        private bool revertOnDeath;
        private int oldValueToRevertOnLeave;

        public ExtendedVariantTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            // parse the trigger parameters
            variantChange = data.Enum("variantChange", ExtendedVariantsModule.Variant.Gravity);
            newValue = data.Int("newValue", 10);
            revertOnLeave = data.Bool("revertOnLeave", false);
            revertOnDeath = data.Bool("revertOnDeath", true);

            if (!data.Bool("enable", true)) {
                // "disabling" a variant is actually just resetting its value to default
                newValue = GetDefaultValueForVariant(variantChange);
            }

            // failsafe
            oldValueToRevertOnLeave = newValue;
        }

        public static int GetDefaultValueForVariant(ExtendedVariantsModule.Variant variant) {
            switch (variant) {
                case ExtendedVariantsModule.Variant.ChaserCount: return 1;
                case ExtendedVariantsModule.Variant.AffectExistingChasers: return 0;
                case ExtendedVariantsModule.Variant.HiccupStrength: return 10;
                case ExtendedVariantsModule.Variant.RefillJumpsOnDashRefill: return 0;
                case ExtendedVariantsModule.Variant.SnowballDelay: return 8;
                case ExtendedVariantsModule.Variant.BadelineLag: return 0;
                case ExtendedVariantsModule.Variant.DelayBetweenBadelines: return 4;
                case ExtendedVariantsModule.Variant.OshiroCount: return 1;
                case ExtendedVariantsModule.Variant.ReverseOshiroCount: return 0;
                case ExtendedVariantsModule.Variant.DisableOshiroSlowdown: return 0;
                case ExtendedVariantsModule.Variant.DisableSeekerSlowdown: return 0;
                case ExtendedVariantsModule.Variant.BadelineAttackPattern: return 0;
                case ExtendedVariantsModule.Variant.ChangePatternsOfExistingBosses: return 0;
                case ExtendedVariantsModule.Variant.FirstBadelineSpawnRandom: return 0;
                case ExtendedVariantsModule.Variant.BadelineBossCount: return 1;
                case ExtendedVariantsModule.Variant.BadelineBossNodeCount: return 1;
                case ExtendedVariantsModule.Variant.RisingLavaSpeed: return 10;
                case ExtendedVariantsModule.Variant.AllowThrowingTheoOffscreen: return 0;
                case ExtendedVariantsModule.Variant.AllowLeavingTheoBehind: return 0;

                default: return ExtendedVariantsModule.Instance.VariantHandlers[variant].GetDefaultValue();
            }
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);

            int oldValue = ExtendedVariantsModule.Instance.TriggerManager.OnEnteredInTrigger(variantChange, newValue, revertOnLeave, isFade: false, revertOnDeath);

            if (revertOnLeave) {
                oldValueToRevertOnLeave = oldValue;
            }
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);

            if (revertOnLeave) {
                ExtendedVariantsModule.Instance.TriggerManager.OnExitedRevertOnLeaveTrigger(variantChange, oldValueToRevertOnLeave);
            }
        }
    }
}
