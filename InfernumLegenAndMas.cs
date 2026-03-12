using System.Collections.Generic;
using CalamityMod.Systems;
using CalamityMod.World;
using InfernumMode.Assets.Sounds;
using InfernumMode.Core.GlobalInstances.Systems;
using InfernumMode.Core.Netcode;
using InfernumMode.Core.Netcode.Packets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using InfernumMode.Content.UI;
using static CalamityMod.Systems.DifficultyModeSystem;
using System.Reflection;

namespace InfernumLegenAndMas
{
    public class CompatibilitySystem : ModSystem
    {
        public static FieldInfo InfernalDiff;
        private static bool _Init;
        public override void OnModLoad()
        {
            Init();
        }
        public override void PreUpdateWorld()
        {
            if (!_Init)
            {
                Init();
            }
        }
        private static void Init()
        {
            if (_Init)
            {
                return;
            }
            if (ModLoader.TryGetMod("InfernalEclipseAPI", out Mod inf))
            {
                InfernalDiff = inf.Code.GetType("InfernalEclipseAPI.Core.World.InfernalWorld")?.GetField("RagnarokModeEnabled", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            }
            _Init = true;
        }
        public static void RagnarokEnable(bool enabled)
        {
            if (!_Init || InfernalDiff == null)
            {
                return;
            }
            else
            {
                InfernalDiff.SetValue(null, enabled);
            }
        }
        public static bool IsRagnarokEnable()
        {
            if (!_Init || InfernalDiff == null)
                return false;
            try
            {
                return (bool)InfernalDiff.GetValue(null);
            }
            catch
            {
                return false;
            }
        }
    }
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
                        CalamityWorld.death = value;
                        if (!Main.GameModeInfo.IsJourneyMode)
                        {
                            Main.GameMode = BackBoneGameModeID;
                        }
                        else
                        {
                            AlignJourneyDifficultySlider();
                        }
                    }
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        PacketManager.SendPacket<InfernumModeActivityPacket>();

                }
            }

            public override Asset<Texture2D> Texture => _texture ??= ModContent.Request<Texture2D>("InfernumLegenAndMas/UI/InfernumMasIcon");
            public override Asset<Texture2D> OutlineTexture => _outlineTexture ??= ModContent.Request<Texture2D>("InfernumLegenAndMas/UI/InfernumMasIcon_Outline");

            public override Asset<Texture2D> TextureDisabled => _textureDisabled ??= ModContent.Request<Texture2D>("InfernumLegenAndMas/UI/InfernumMasIcon_Off");

            public override SoundStyle ActivationSound => InfernumSoundRegistry.ModeToggleLaugh;

            public override int BackBoneGameModeID => GameModeID.Master;


            public override float DifficultyScale => 0.5f;

            public override LocalizedText Name => Language.GetText("Mods.InfernumMLMode.DifficultyUI.Name");

            public override Color ChatTextColor => Color.DarkRed;

            public override LocalizedText ShortDescription => Language.GetText("Mods.InfernumMLMode.DifficultyUI.ShortDescription");

            public override LocalizedText ExpandedDescription => Language.GetText("Mods.InfernumMLMode.DifficultyUI.ExpandedDescription");


            public override int[] FavoredDifficultyAtTier(int tier)
            {
                DifficultyMode[] tierList = DifficultyTiers[tier];

                List<int> difficulties = new List<int>();

                for (int i = 0; i < tierList.Length; i++)
                {
                    if (tierList[i] is InfernumDifficulty && !Main.getGoodWorld)
                        difficulties.Add(i);
                    if (Main.getGoodWorld && (tierList[i] is InfernumDifficulty || tierList[i] is DeathDifficulty))
                        difficulties.Add(i);
                }

                if (difficulties.Count <= 0)
                    difficulties.Add(0);

                return difficulties.ToArray();
            }
            public override bool IsBasedOn(DifficultyMode mode)
            {
                if (!Main.getGoodWorld)
                {
                    return mode is DeathDifficulty;
                }
                else
                {
                    return mode is MaliceDifficulty;
                }
            }
        }
        public class InfernumLastDifficulty : DifficultyMode
        {
            public override bool Enabled
            {
                get => CompatibilitySystem.IsRagnarokEnable() && WorldSaveSystem.InfernumModeEnabled && Main.getGoodWorld ? CalamityWorld.death && CalamityWorld.LegendaryMode : false;
                set
                {
                    CompatibilitySystem.RagnarokEnable(value);
                    WorldSaveSystem.InfernumModeEnabled = value;
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
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        PacketManager.SendPacket<InfernumModeActivityPacket>();
                }
            }

            public override Asset<Texture2D> Texture => _texture ??= ModContent.Request<Texture2D>("InfernumLegenAndMas/UI/RagnarokLegIcon");
            public override Asset<Texture2D> OutlineTexture => _outlineTexture ??= ModContent.Request<Texture2D>("InfernumLegenAndMas/UI/RagnarokLegIcon_Outline");

            public override Asset<Texture2D> TextureDisabled => _textureDisabled ??= ModContent.Request<Texture2D>("InfernumLegenAndMas/UI/RagnarokLegIcon_Off");

            public override SoundStyle ActivationSound => InfernumSoundRegistry.ModeToggleLaugh;

            public override int BackBoneGameModeID => GameModeID.Master;


            public override float DifficultyScale => 0.5f;

            public override LocalizedText Name => Language.GetText("Mods.InfernumMLMode.DifficultyUI1.Name");

            public override Color ChatTextColor => Color.DarkRed;

            public override LocalizedText ShortDescription => Language.GetText("Mods.InfernumMLMode.DifficultyUI1.ShortDescription");

            public override LocalizedText ExpandedDescription => Language.GetText("Mods.InfernumMLMode.DifficultyUI1.ExpandedDescription");
            public override FTWDisplayMode GetForTheWorthyDisplay => FTWDisplayMode.OnlyForTheWorthy;

            public override int[] FavoredDifficultyAtTier(int tier)
            {
                DifficultyMode[] tierList = DifficultyTiers[tier];

                List<int> difficulties = new List<int>();

                for (int i = 0; i < tierList.Length; i++)
                {
                    if (Main.getGoodWorld && (tierList[i] is InfernumDifficulty || tierList[i] is DeathDifficulty))
                        difficulties.Add(i);
                }

                if (difficulties.Count <= 0)
                    difficulties.Add(0);

                return difficulties.ToArray();
            }
            public override bool IsBasedOn(DifficultyMode mode)
            {
                return mode is MaliceDifficulty;
            }
        }
        public override void Load()
        {
            InfernumMode.Core.GlobalInstances.Systems.DifficultyManagementSystem.DisableDifficultyModes = false;
            InfernumMasterDifficulty difficulty = new();
            if (!ModLoader.HasMod("InfernalEclipseAPI"))
            {
                DifficultyModeSystem.Difficulties.Add(difficulty);
            }
            else
            {
                InfernumLastDifficulty dif = new();
                DifficultyModeSystem.Difficulties.Add(dif);
            }
            DifficultyModeSystem.CalculateDifficultyData();
        }
        public override void Unload()
        {
            InfernumMode.Core.GlobalInstances.Systems.DifficultyManagementSystem.DisableDifficultyModes = true;
        }
    }
}