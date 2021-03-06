﻿using Celeste;
using ExtendedVariants.Module;
using ExtendedVariants.Variants;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace ExtendedVariants.Entities {
    /// <summary>
    /// An indicator for extra jumps (dots above Madeline's head).
    /// </summary>
    class JumpIndicator : Entity {
        private ExtendedVariantsSettings settings;

        public JumpIndicator() {
            Depth = -20000; // appear on top of most things, including (most importantly) fg tiles
            AddTag(Tags.Persistent); // this entity isn't bound to the screen it was spawned in, keep it when transitioning to another level.

            settings = ExtendedVariantsModule.Settings;
        }

        public override void Update() {
            base.Update();

            if (!settings.MasterSwitch) {
                // extended variants were turned off, the jump indicator should kick itself out.
                RemoveSelf();
            }
        }

        public override void Render() {
            base.Render();

            Player player = Scene.Tracker.GetEntity<Player>();
            if (player != null) {
                MTexture jumpIndicator = GFX.Game["ExtendedVariantMode/jumpindicator"];

                // draw no indicator in the case of infinite jumps.
                int jumpIndicatorsToDraw = settings.JumpCount == 6 ? 0 : JumpCount.GetJumpBuffer();

                int lines = 1 + (jumpIndicatorsToDraw - 1) / 5;

                for (int line = 0; line < lines; line++) {
                    int jumpIndicatorsToDrawOnLine = Math.Min(jumpIndicatorsToDraw, 5);
                    int totalWidth = jumpIndicatorsToDrawOnLine * 6 - 2;
                    for (int i = 0; i < jumpIndicatorsToDrawOnLine; i++) {
                        jumpIndicator.DrawJustified(player.Center + new Vector2(-totalWidth / 2 + i * 6, -15f - line * 6), new Vector2(0f, 0.5f));
                    }
                    jumpIndicatorsToDraw -= jumpIndicatorsToDrawOnLine;
                }
            }
        }
    }
}
