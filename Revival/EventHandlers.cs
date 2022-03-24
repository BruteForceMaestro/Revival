using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.Events.EventArgs;

namespace Revival
{
    internal class EventHandlers
    {
        public static Dictionary<int, bool> RevPlayers = new Dictionary<int, bool>(); // int - player's id. bool - in process of reviving
        public void OnDying(DiedEventArgs ev)
        {
            if (ev.Target != null)
            {
                RevPlayers[ev.Target.Id] = false;
            }
        }
        public void OnRespawn(SpawningEventArgs ev)
        {
            TryRemove(ev.Player.Id);
        }
        public void OnChangingItems(ChangingItemEventArgs ev)
        {
            if (RevPlayers.TryGetValue(ev.Player.Id, out var rev) && rev)
            {
                ev.IsAllowed = false;
            }
        }
        public void OnDisconnect(LeftEventArgs ev)
        {
            TryRemove(ev.Player.Id);
        }
        public static void TryRemove(int id)
        {
            if (RevPlayers.ContainsKey(id))
            {
                RevPlayers.Remove(id);
            }
        }
    }
}
