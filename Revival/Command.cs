using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using Mirror;
using Revival.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Revival
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal class ReviveCommand : ParentCommand
    {
        readonly Config cfg = Main.Instance.Config;
        public override string Command => "revive";

        public override string[] Aliases => new[] { "rev", "revival" };

        public override string Description => "Tries to revive the person you're looking at.";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = string.Empty; // i use hints, this command is intended to be binded.

            Player reviver = Player.Get(sender);
            Transform camTransform = reviver.CameraTransform;

            int ragdollLayerMask = 1 << 17; // select only ragdoll layer

            if (!Physics.Raycast(camTransform.position, camTransform.forward, out RaycastHit hit, cfg.Distance, ragdollLayerMask))
            {
                reviver.ShowHint(cfg.Hints[HintType.NoRagdollFound]);
                return false;
            }

            Ragdoll ragdoll = hit.transform.gameObject.GetComponentInParent<Ragdoll>();
            // Log.Debug($"Object name: {hit.transform.gameObject.name}\nLayer: {hit.transform.gameObject.layer}");
            if (ragdoll == null)
            {
                reviver.ShowHint(cfg.Hints[HintType.NoRagdollFound]);
                return false;
            }

            Player revivee = Player.Get(ragdoll.Info.OwnerHub);
            if (revivee == null || revivee.IsAlive || ragdoll.Info.ExistenceTime > cfg.DeathTime)
            {
                reviver.ShowHint(cfg.Hints[HintType.Generic]);
                return false;
            }
            if (revivee.CurrentItem == null || !cfg.Items.Contains(revivee.CurrentItem.Type))
            {
                reviver.ShowHint(cfg.Hints[HintType.WrongItem]);
                return false;
            }
            if (!cfg.SCPRevivable && revivee.IsScp)
            {
                reviver.ShowHint(cfg.Hints[HintType.SCP]);
                return false;
            }
            if (EventHandlers.RevPlayers[revivee.Id] || EventHandlers.RevPlayers[reviver.Id])
            {
                reviver.ShowHint(cfg.Hints[HintType.AlreadyReviving]);
                return false;
            }

            EventHandlers.RevPlayers[revivee.Id] = true;
            EventHandlers.RevPlayers[reviver.Id] = true;

            reviver.ShowHint(cfg.Hints[HintType.Begin]
                .Replace("%revivee%", revivee.Nickname));

            Timing.RunCoroutine(Revive(revivee, reviver, ragdoll));
            return true;
        }
        private IEnumerator<float> Revive(Player revivee, Player reviver, Ragdoll ragdoll)
        {
            bool success = true;
            for (int i = 0; i < cfg.ReviveTime; i++)
            {
                if (reviver == null || !reviver.IsAlive)
                {
                    success = false;
                    break;
                }
                if (revivee == null || revivee.IsAlive)
                {
                    reviver.ShowHint(cfg.Hints[HintType.Generic]);
                    
                    success = false;
                    break;
                }
                if (Vector3.Distance(reviver.Position, ragdoll.transform.position) > cfg.Distance)
                {
                    reviver.ShowHint(cfg.Hints[HintType.WrongDistance]);

                    success = false;
                    break;
                }
                int progress = (int)((double)i / cfg.ReviveTime * 100);

                reviver.ShowHint(cfg.Hints[HintType.InProgress]
                    .Replace("%revivee%", revivee.Nickname)
                    .Replace("%progress%", progress.ToString())
                );

                yield return Timing.WaitForSeconds(1);
            }
            if (!success)
            {
                EventHandlers.TryRemove(revivee.Id);
                EventHandlers.TryRemove(reviver.Id);
                yield break;
            }
            
            revivee.SetRole(ragdoll.Info.RoleType);
            yield return Timing.WaitForSeconds(0.15f);

            revivee.Position = ragdoll.Info.StartPosition;
            revivee.Health = revivee.MaxHealth * (cfg.HP / 100);
            foreach (EffectType effect in cfg.Effects)
            {
                revivee.EnableEffect(effect);
            }
            NetworkServer.Destroy(ragdoll.gameObject);

            reviver.RemoveItem(reviver.CurrentItem);
            
            revivee.ShowHint(cfg.Hints[HintType.SuccessRevivee]
                .Replace("%reviver%", reviver.Nickname));

            reviver.ShowHint(cfg.Hints[HintType.SuccessReviver]
                .Replace("%revivee%", revivee.Nickname));
        }
    }
}
