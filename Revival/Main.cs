using Exiled.API.Features;
using Player = Exiled.Events.Handlers.Player;

namespace Revival
{
    public class Main : Plugin<Config>
    {
        EventHandlers handlers;
        public static Main Instance { get; set; }
        public override void OnEnabled()
        {
            handlers = new EventHandlers();
            Player.Died += handlers.OnDying;
            Player.Spawning += handlers.OnRespawn;
            Player.ChangingItem += handlers.OnChangingItems;
            Player.Left += handlers.OnDisconnect;
            Instance = this;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Player.Died -= handlers.OnDying;
            Player.Spawning -= handlers.OnRespawn;
            Player.ChangingItem -= handlers.OnChangingItems;
            Player.Left -= handlers.OnDisconnect;
            handlers = null;
            base.OnDisabled();
        }
    }
}