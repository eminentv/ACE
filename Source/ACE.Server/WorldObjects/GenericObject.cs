using System;

using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Entity.Models;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.WorldObjects
{
    public class GenericObject : WorldObject
    {
        /// <summary>
        /// A new biota be created taking all of its values from weenie.
        /// </summary>
        public GenericObject(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
        {
            SetEphemeralValues();
        }

        /// <summary>
        /// Restore a WorldObject from the database.
        /// </summary>
        public GenericObject(Biota biota) : base(biota)
        {
            SetEphemeralValues();
        }

        private void SetEphemeralValues()
        {
            //StackSize = null;
            //StackUnitEncumbrance = null;
            //StackUnitValue = null;
            //MaxStackSize = null;

            // Linkable Item Generator (linkitemgen2minutes) fix
            if (WeenieClassId == 4142)
            {
                MaxGeneratedObjects = 0;
                InitGeneratedObjects = 0;
            }
        }

        public override void ActOnUse(WorldObject activator)
        {
            if (!(activator is Player player))
                return;

            if (UseSound > 0)
                player.Session.Network.EnqueueSend(new GameMessageSound(player.Guid, UseSound));

            if(WeenieClassId == (uint)Factories.Enum.WeenieClassName.explorationMarker)
            {
                short landblockId = (short)(CurrentLandblock.Id.Raw >> 16);
                if (player.Exploration1LandblockId == landblockId && player.Exploration1MarkerProgressTracker > 0)
                {
                    player.Exploration1MarkerProgressTracker--;
                    var msg = $"{player.Exploration1MarkerProgressTracker:N0} marker{(player.Exploration1MarkerProgressTracker != 1 ? "s" : "")} remaining.";
                    player.EarnXP((int)(((-player.Level ?? -1) - 1000) * (PropertyManager.GetDouble("exploration_bonus_xp").Item + 0.5)), XpType.Exploration, null, null, 0, null, ShareType.None, msg);
                }
                else if (player.Exploration2LandblockId == landblockId && player.Exploration2MarkerProgressTracker > 0)
                {
                    player.Exploration2MarkerProgressTracker--;
                    var msg = $"{player.Exploration2MarkerProgressTracker:N0} marker{(player.Exploration2MarkerProgressTracker != 1 ? "s" : "")} remaining.";
                    player.EarnXP((int)(((-player.Level ?? -1) - 1000) * (PropertyManager.GetDouble("exploration_bonus_xp").Item + 0.5)), XpType.Exploration, null, null, 0, null, ShareType.None, msg);
                }
                else if (player.Exploration3LandblockId == landblockId && player.Exploration3MarkerProgressTracker > 0)
                {
                    player.Exploration3MarkerProgressTracker--;
                    var msg = $"{player.Exploration3MarkerProgressTracker:N0} marker{(player.Exploration3MarkerProgressTracker != 1 ? "s" : "")} remaining.";
                    player.EarnXP((int)(((-player.Level ?? -1) - 1000) * (PropertyManager.GetDouble("exploration_bonus_xp").Item + 0.5)), XpType.Exploration, null, null, 0, null, ShareType.None, msg);

                }
                else
                    player.Session.Network.EnqueueSend(new GameMessageSystemChat("You currently do not have any exploration assignments for this location.", ChatMessageType.Broadcast));

                CurrentLandblock.SpawnExplorationMarker();
                Destroy();
            }
        }
    }
}
