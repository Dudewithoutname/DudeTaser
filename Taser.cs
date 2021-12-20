using System;
using System.Collections.Generic;
using System.Collections;
using Rocket.API;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned;
using Steamworks;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using UnityEngine;
using SDG.Unturned;

namespace DudeTaser
{
    public class Taser : RocketPlugin<TaserConfig>
    {
        public static Taser instance;
        public static List<CSteamID> TasedPlayers;

        protected override void Load()
        {
            instance = this;
            Logger.Log("#----------------------------------------#", ConsoleColor.Green);
            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded", ConsoleColor.Green);
            Logger.Log("Author: Dudewithoutname#3129", ConsoleColor.Green);
            Logger.Log("#----------------------------------------#", ConsoleColor.Green);

            TasedPlayers = new List<CSteamID>();


            UnturnedEvents.OnPlayerDamaged += OnPlayerDamage;
            
        }

        protected override void Unload()
        {
            instance = null;
            TasedPlayers = null;

            UnturnedEvents.OnPlayerDamaged -= OnPlayerDamage;

            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }

        private void OnPlayerDamage(UnturnedPlayer victim, ref EDeathCause cause, ref ELimb limb, ref UnturnedPlayer attacker, ref Vector3 direction, ref float damage, ref float times, ref bool canDamage)
        {
            if (cause == EDeathCause.GUN && attacker.Player.equipment.itemID == Configuration.Instance.TaserId  
                && attacker.Player.equipment.isEquipped && !TasedPlayers.Contains(attacker.CSteamID) && attacker.HasPermission(Configuration.Instance.TaserPermission))
            {
                victim.Player.equipment.dequip();
                victim.Player.movement.sendPluginSpeedMultiplier(0.1f);
                victim.Player.movement.sendPluginJumpMultiplier(0.1f);
                victim.Player.stance.stance = EPlayerStance.PRONE;
                victim.Player.stance.checkStance(EPlayerStance.PRONE);
                TasedPlayers.Add(victim.CSteamID);
                StartCoroutine(nameof(RemoveFromTased), victim);
                StartCoroutine(CheckTased(victim));
                damage = 0;
                canDamage = false;
                return;
            }
        }
        private IEnumerator CheckTased(UnturnedPlayer victim)
        {
            while (TasedPlayers.Contains(victim.CSteamID))
            {
                victim.Player.equipment.dequip();
                victim.Player.stance.stance = EPlayerStance.PRONE;
                victim.Player.stance.checkStance(EPlayerStance.PRONE);

                yield return new WaitForSeconds(0.3f);
            }
        }

        private IEnumerator RemoveFromTased(UnturnedPlayer victim)
        {
            yield return new WaitForSeconds(Configuration.Instance.TasedTime);

            victim.Player.movement.sendPluginSpeedMultiplier(1f);
            victim.Player.movement.sendPluginJumpMultiplier(1f);
            TasedPlayers.Remove(victim.CSteamID);
        }
    }
}
