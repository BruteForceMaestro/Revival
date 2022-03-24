using Exiled.API.Enums;
using Exiled.API.Interfaces;
using Revival.Enums;
using System.Collections.Generic;
using System.ComponentModel;

namespace Revival
{
    public class Config : IConfig
    {
        [Description("Indicates if plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Time (seconds) required for revival")]
        public int ReviveTime { get; set; } = 5;

        [Description("Items that can revive people")]
        public List<ItemType> Items { get; set; } = new List<ItemType>()
        {
            ItemType.Medkit,
            ItemType.Adrenaline,
            ItemType.SCP500
        };

        [Description("SCPs are revivable")]
        public bool SCPRevivable { get; set; } = false;

        [Description("Timeframe in which the player is revivable after death")]
        public int DeathTime { get; set; } = 15;

        [Description("Distance limit for reviving")]
        public float Distance { get; set; } = 1;

        [Description("Percent of HP on revival")]
        public int HP { get; set; } = 40;

        [Description("Hints")]
        public Dictionary<HintType, string> Hints { get; set; } = new Dictionary<HintType, string>()
        {
            [HintType.Begin] = "You begin reviving %revivee%",
            [HintType.InProgress] = "You are reviving %revivee%\nRevive progress: %progress%%",
            [HintType.SuccessReviver] = "You have revived %revivee%",
            [HintType.SuccessRevivee] = "You have been revived by %reviver%",
            [HintType.Generic] = "It's too late for them...",
            [HintType.WrongItem] = "You can't revive with this item!",
            [HintType.WrongDistance] = "They are too far away to be revived.",
            [HintType.NoRagdollFound] = "What am I trying to revive?",
            [HintType.SCP] = "SCPs are not revivable.",
            [HintType.AlreadyReviving] = "This player is already being revived."
        };

        [Description("Effects on revival")]
        public List<EffectType> Effects { get; set; } = new List<EffectType>()
        {
            EffectType.Disabled,
            EffectType.Bleeding
        };
    }
}