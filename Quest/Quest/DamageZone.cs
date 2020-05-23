using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Quest
{
    /// <summary>
    /// A collider with some damage properties
    /// </summary>
    public class DamageZone
    {
        float damage;
        /// <summary>
        /// X representing the ID and Y the timer
        /// </summary>
        List<Point> cooldownIDs = new List<Point>();
        public Collider collider;
        /// <summary>
        /// The amount of frames the zone exists for
        /// </summary>
        public int lifeSpan;
        /// <summary>
        /// The amount of frames to add to the timer for every ID after being hit
        /// </summary>
        int cooldown;
        List<Agent.Tag> ignoreTags = new List<Agent.Tag>();
        public int ID;


        public DamageZone(Collider collider, float damage, int cooldown, int lifeSpan)
        {
            this.collider = collider;
            this.damage = damage;
            this.cooldown = cooldown;
            this.lifeSpan = lifeSpan;
            this.ID = Game1.NextIDDamageZone;
        }
        public DamageZone(Collider collider, float damage, int cooldown, int lifeSpan, List<Agent.Tag> tagsToIgnore)
        {
            this.collider = collider;
            this.damage = damage;
            this.cooldown = cooldown;
            this.lifeSpan = lifeSpan;
            this.ignoreTags = tagsToIgnore;
            this.ID = Game1.NextIDDamageZone;
        }

        public void IgnoreTag(Agent.Tag tag)
        {
            if(!ignoreTags.Contains(tag))
                ignoreTags.Add(tag);
        }

        public void IncludeTag(Agent.Tag tag)
        {
            for (int i = 0; i < ignoreTags.Count; i++)
            {
                if(ignoreTags[i] == tag)
                {
                    ignoreTags.RemoveAt(i);
                    return;
                }
            }
        }

        public void Update()
        {
            lifeSpan--;
            if (lifeSpan > 0)
            {
                for (int i = 0; i < cooldownIDs.Count; i++)
                {
                    if (cooldownIDs[i].Y <= 0)
                    {
                        cooldownIDs.RemoveAt(i);
                        i--;
                    }
                    else
                        cooldownIDs[i] = new Point(cooldownIDs[i].X, cooldownIDs[i].Y);
                }
            }
        }

        /// <summary>
        /// Returns the proposed damage to take
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public float TakeDamage(int id)
        {
            if(!cooldownIDs.ContainsX(id))
            {
                return damage;
            }
            else
            {
                StartCooldown(id);
            }

            return 0;
        }

        /// <summary>
        /// Starts a cooldown with the zone's coolDownTimer
        /// </summary>
        /// <param name="id"></param>
        public void StartCooldown(int id)
        {
            if(cooldown > 0)
                cooldownIDs.Add(new Point(id, cooldown));
        }

        internal bool IsTagIgnored(Agent.Tag tag)
        {
            return ignoreTags.Contains(tag);
        }
    }
}
