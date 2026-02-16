global using Luminance.Common.Utilities;
global using static System.MathF;
global using static Microsoft.Xna.Framework.MathHelper;
global using LumUtils = Luminance.Common.Utilities.Utilities;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using System.IO;
using CalamityMod.ILEditing;
using CalamityMod.NPCs;
using CalamityMod.Systems;
using InfernumMode.Assets.BossTextures;
using InfernumMode.Assets.Effects;
using InfernumMode.Common.Graphics.Primitives;
using InfernumMode.Content.BossBars;
using InfernumMode.Content.UI;
using InfernumMode.Content.WorldGeneration;
using InfernumMode.Core.Balancing;
using InfernumMode.Core.GlobalInstances.Systems;
using InfernumMode.Core.Netcode;
using InfernumMode.Core.OverridingSystem;
using Luminance.Core.ModCalls;
using Terraria.ID;
using System.Collections.Generic;
using CalamityMod.World;
using InfernumMode.Assets.Sounds;
using InfernumMode.Core.Netcode.Packets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.Localization;
using static CalamityMod.Systems.DifficultyModeSystem;

namespace InfernumLegenAndMas
{
    public class InfernumMasterFtW : Mod
    {
        public class InfernumMasterDifficulty : DifficultyMode
        {
            public override bool Enabled
            {
                get => WorldSaveSystem.InfernumModeEnabled && CalamityWorld.death;
                set
                {
                    WorldSaveSystem.InfernumModeEnabled = value;
                    if (value)
                    {
                        if (Main.getGoodWorld)
                        {
                            if (!Main.GameModeInfo.IsJourneyMode)
                            {
                                Main.GameMode = value == true ? GameModeID.Expert : GameModeID.Normal;
                            }
                            else
                            {
                                DifficultyModeSystem.AlignJourneyDifficultySlider();
                            }
                            CalamityWorld.death = value;
                        }
                        else
                        {
                            CalamityWorld.death = value;
                            if (value && !Main.GameModeInfo.IsJourneyMode)
                                Main.GameMode = BackBoneGameModeID;

                        }
                    }
                }
            }

            public override Asset<Texture2D> Texture => _texture ??= ModContent.Request<Texture2D>("InfernumLegenAndMas/UI/InfernumMasIcon");
            public override Asset<Texture2D> OutlineTexture => _outlineTexture ??= ModContent.Request<Texture2D>("InfernumLegenAndMas/UI/InfernumMasIcon_Outline");

            public override Asset<Texture2D> TextureDisabled => _textureDisabled ??= ModContent.Request<Texture2D>("InfernumLegenAndMas/UI/InfernumMasIcon_Off");

            public override SoundStyle ActivationSound => InfernumSoundRegistry.ModeToggleLaugh;

            public override int BackBoneGameModeID => Main.getGoodWorld ? GameModeID.Expert : GameModeID.Master;

            public override float DifficultyScale => 0.1f;

            public override LocalizedText Name => Language.GetText("Mods.InfernumMLMode.DifficultyUI.Name");

            public override Color ChatTextColor => Color.DarkRed;

            public override LocalizedText ShortDescription => Language.GetText("Mods.InfernumMLMode.DifficultyUI.ShortDescription");

            public override LocalizedText ExpandedDescription => Language.GetText("Mods.InfernumMLMode.DifficultyUI.ExpandedDescription");


            public override int[] FavoredDifficultyAtTier(int tier)
            {
                DifficultyMode[] tierList = DifficultyModeSystem.DifficultyTiers[tier];
                List<int> difficulties = new List<int>();

                for (int i = 0; i < tierList.Length; i++)
                {
                    if (tierList[i] is MasterDifficulty || tierList[i] is DeathDifficulty)
                        difficulties.Add(i);
                }

                if (difficulties.Count <= 0)
                    difficulties.Add(0);


                return difficulties.ToArray();
            }
        }
        public class InfernumLegendaryDifficulty : DifficultyMode
        {
            public override bool Enabled
            {
                get => WorldSaveSystem.InfernumModeEnabled && Main.getGoodWorld ? CalamityWorld.death && CalamityWorld.LegendaryMode : false;
                set
                {
                    WorldSaveSystem.InfernumModeEnabled = value;
                    if (value)
                    {
                        if (Main.getGoodWorld)
                        {
                            if (!Main.GameModeInfo.IsJourneyMode)
                            {
                                Main.GameMode = value == true ? GameModeID.Master : GameModeID.Expert;
                            }
                            else
                            {
                                DifficultyModeSystem.AlignJourneyDifficultySlider();
                            }
                            CalamityWorld.revenge = value;
                            CalamityWorld.death = value;
                        }
                    }
                }
            }

            public override Asset<Texture2D> Texture => _texture ??= ModContent.Request<Texture2D>("InfernumLegenAndMas/UI/InfernumLegIcon");
            public override Asset<Texture2D> OutlineTexture => _outlineTexture ??= ModContent.Request<Texture2D>("InfernumLegenAndMas/UI/InfernumLegIcon_Outline");

            public override Asset<Texture2D> TextureDisabled => _textureDisabled ??= ModContent.Request<Texture2D>("InfernumLegenAndMas/UI/InfernumLegIcon_Off");

            public override SoundStyle ActivationSound => InfernumSoundRegistry.ModeToggleLaugh;

            public override int BackBoneGameModeID => GameModeID.Master;

            public override float DifficultyScale => 0.1f;

            public override LocalizedText Name => Language.GetText("Mods.InfernumMLMode.DifficultyUI1.Name");

            public override Color ChatTextColor => Color.DarkRed;

            public override LocalizedText ShortDescription => Language.GetText("Mods.InfernumMLMode.DifficultyUI1.ShortDescription");

            public override LocalizedText ExpandedDescription => Language.GetText("Mods.InfernumMLMode.DifficultyUI1.ExpandedDescription");
            public override FTWDisplayMode GetForTheWorthyDisplay => FTWDisplayMode.OnlyForTheWorthy;

            public override int[] FavoredDifficultyAtTier(int tier)
            {
                DifficultyMode[] tierList = DifficultyModeSystem.DifficultyTiers[tier];
                List<int> difficulties = new List<int>();

                for (int i = 0; i < tierList.Length; i++)
                {
                    if (tierList[i] is LegendaryDifficulty || tierList[i] is MaliceDifficulty)
                        difficulties.Add(i);
                }

                if (difficulties.Count <= 0)
                    difficulties.Add(0);


                return difficulties.ToArray();
            }
        }
        public override void Load()
        {
            InfernumMode.Core.GlobalInstances.Systems.DifficultyManagementSystem.DisableDifficultyModes = false;
            InfernumMasterDifficulty difficulty = new();
            DifficultyModeSystem.Difficulties.Add(difficulty);
            InfernumLegendaryDifficulty difficulty1 = new();
            DifficultyModeSystem.Difficulties.Add(difficulty1);
            DifficultyModeSystem.CalculateDifficultyData();
        }
        public override void Unload()
        {
            InfernumMode.Core.GlobalInstances.Systems.DifficultyManagementSystem.DisableDifficultyModes = true;
        }
    }
}