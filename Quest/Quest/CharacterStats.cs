using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest
{
    public class CharacterStats
    {
        public class ActiveEffect
        {
            public struct Affector
            {
                public float value;
                public bool multiply;

                public Affector(float value, bool multiply)
                {
                    this.value = value;
                    this.multiply = multiply;
                }
            }

            public bool Completed
            {
                get { return currentTime >= timer; }
            }

            float currentTime = 0;
            float timer;
            public Affector[] affectors = new Affector[Enum.GetNames(typeof(ValueType)).Length];

            public ActiveEffect(List<KeyValuePair<ValueType, Affector>> affectors, float timer)
            {
                this.timer = timer;
                for (int i = 0; i < this.affectors.Length; i++)
                {
                    this.affectors[i] = new Affector(0f, false);
                }
                for (int i = 0; i < affectors.Count; i++)
                {
                    this.affectors[(int)affectors[i].Key] = affectors[i].Value;
                }
            }

            public void Update(float timeToAdd)
            {
                currentTime += timeToAdd;
            }
        }

        public static CharacterStats Default
        {
            get
            {
                List<KeyValuePair<ValueType, float>> pairs = new List<KeyValuePair<ValueType, float>>();

                pairs.Add(new KeyValuePair<ValueType, float>(ValueType.Health, 100));
                pairs.Add(new KeyValuePair<ValueType, float>(ValueType.Mana, 100));
                pairs.Add(new KeyValuePair<ValueType, float>(ValueType.Energy, 100));

                pairs.Add(new KeyValuePair<ValueType, float>(ValueType.Strength, 10));
                pairs.Add(new KeyValuePair<ValueType, float>(ValueType.Intellect, 10));
                pairs.Add(new KeyValuePair<ValueType, float>(ValueType.Agility, 10));

                pairs.Add(new KeyValuePair<ValueType, float>(ValueType.HealthRegeneration, .1f));
                pairs.Add(new KeyValuePair<ValueType, float>(ValueType.ManaRegeneration, .1f));
                pairs.Add(new KeyValuePair<ValueType, float>(ValueType.EnergyRegeneration, .1f));

                return new CharacterStats(pairs);
            }
        }

        public enum ValueType
        {
        Health,
        Mana,
        Energy,

        HealthRegeneration,
        ManaRegeneration,
        EnergyRegeneration,

        Strength,
        Intellect,
        Agility
        }

        float[] values = new float[Enum.GetNames(typeof(ValueType)).Length];

        public float currentHealth;
        public float currentMana;
        public float currentEnergy;

        float[] activeEffectsMultiplication = new float[Enum.GetNames(typeof(ValueType)).Length];
        float[] activeEffectsAddition = new float[Enum.GetNames(typeof(ValueType)).Length];

        List<ActiveEffect> activeEffects = new List<ActiveEffect>();

        public void Update()
        {
            for (int i = 0; i < values.Length; i++)
            {
                activeEffectsMultiplication[i] = 1;
                activeEffectsAddition[i] = 0;
            }

            for (int i = 0; i < activeEffects.Count; i++)
            {
                for (int j = 0; j < activeEffects[i].affectors.Length; j++)
                {
                    if (activeEffects[i].affectors[j].multiply)
                        activeEffectsMultiplication[i] *= activeEffects[i].affectors[j].value;
                    else
                        activeEffectsAddition[i] += activeEffects[i].affectors[j].value;
                }

                activeEffects[i].Update(1f);

                if (activeEffects[i].Completed)
                {
                    activeEffects.RemoveAt(i);
                    i--;
                }
            }

            if (currentHealth < GetValue(ValueType.Health))
                currentHealth += GetValue(ValueType.HealthRegeneration);
            else
                currentHealth = GetValue(ValueType.Health);

            if (currentMana < GetValue(ValueType.Mana))
                currentMana += GetValue(ValueType.ManaRegeneration);
            else
                currentMana = GetValue(ValueType.Mana);

            if (currentEnergy < GetValue(ValueType.Energy))
                currentEnergy += GetValue(ValueType.EnergyRegeneration);
            else
                currentEnergy = GetValue(ValueType.Energy);
        }

        public void SetValue(ValueType valueType, float value)
        {
            values[(int)valueType] = value;
        }

        public float GetValue(ValueType valueType)
        {
            float ret = values[(int)valueType];

            ret = (ret + activeEffectsAddition[(int)valueType]) * activeEffectsMultiplication[(int)valueType];

            return ret;
        }

        public CharacterStats(List<KeyValuePair<ValueType, float>> keyValuePairs)
        {
            for (int i = 0; i < values.Length; i++)
            {
                activeEffectsMultiplication[i] = 1;
                activeEffectsAddition[i] = 0;
            }
            for (int i = 0; i < keyValuePairs.Count; i++)
            {
                SetValue(keyValuePairs[i].Key, keyValuePairs[i].Value);
            }

            currentHealth = GetValue(ValueType.Health);
            currentMana = GetValue(ValueType.Mana);
            currentEnergy = GetValue(ValueType.Energy);
        }

        public void AddActiveEffect(ActiveEffect effect)
        {
            activeEffects.Add(effect);
        }

        public void ClearActiveEffects()
        {
            activeEffects.Clear();
        }
    }
}
