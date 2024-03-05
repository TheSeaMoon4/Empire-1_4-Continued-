using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;


namespace FactionColonies
{
    public enum FCWorkLoad : Byte
    {
        Light, //Adds to overmax
        Medium, //Adds 1 to max
        Heavy //Adds 2 to max
    }

    public class FCPrisoner : ILoadReferenceable, IExposable
    {
        public Pawn prisoner;
        public SettlementFC settlement;
        public float unrest;
        public float health;
        public bool isReturning;
        public int loadID;
        public FCWorkLoad workload;
        

        public FCPrisoner () 
        {            
        }

        public FCPrisoner (Pawn pawn, SettlementFC settlement)
        {
            prisoner = pawn;
            this.settlement = settlement;
            unrest = 0;
            health = (float)Math.Round(prisoner.health.summaryHealth.SummaryHealthPercent * 100);
            isReturning = false;
            loadID = Find.World.GetComponent<FactionFC>().GetNextPrisonerID();
            pawn.guest.SetGuestStatus(FactionColonies.getPlayerColonyFaction(), GuestStatus.Prisoner);
        }

        
        public void ExposeData()
        {
            Scribe_Deep.Look(ref prisoner, "prisoner");
            Scribe_References.Look(ref settlement, "settlement");
            Scribe_Values.Look(ref unrest, "unrest");
            Scribe_Values.Look(ref health, "healthy");
            Scribe_Values.Look(ref isReturning, "isReturning");
            Scribe_Values.Look(ref loadID, "loadID");
            Scribe_Values.Look(ref workload, "workload");
        }


        public string GetUniqueLoadID()
        {
            return "FCPrisoner_" + loadID;
        }



        public bool AdjustHealth(int value)
        {
            health += value;
            if (health >= 100)
            {
                health = 100;
                if (prisoner != null && prisoner.health != null)
                    HealthUtility.HealNonPermanentInjuriesAndRestoreLegs(prisoner);
            }

            return checkDead();
        }

        public bool checkDead()
        {
            if (health <= 0)
            {
                settlement.prisonerList.Remove(this);
                Find.LetterStack.ReceiveLetter("PrisonerHasDiedLetter".Translate(), "PrisonerHasDied".Translate(prisoner.Name.ToString(), settlement.name), LetterDefOf.NeutralEvent);
                return true;
            }
            return false;
        }
    }
}
