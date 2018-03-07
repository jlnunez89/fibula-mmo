using System;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Combat
{
    internal class StandardAttackOperation : BaseAttackOperation
    {
        public StandardAttackOperation(ICombatActor hunter, ICombatActor prey)
            : base(hunter, prey)
        {

        }

        public override bool CanBeExecuted
        {
            get
            {
                if (Target == null || !base.CanBeExecuted)
                {
                    return false;
                }

                var locationDiff = Attacker.Location - Target.Location;

                return locationDiff.Z == 0 && Attacker.AutoAttackRange >= locationDiff.MaxValueIn2D;
            }
        }

        public override AttackType AttackType => AttackType.Physical; // TODO: wands, firesword, poison stuff, etc?

        public override TimeSpan ExhaustionCost => TimeSpan.FromSeconds(2);

        public override int MinimumDamage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MaximumDamage
        {
            get
            {
                throw new NotImplementedException();
            }
        }
                
        protected override int InternalExecute(out EffectT resultingEffect, out bool shielded, out bool armored, out TextColor colorText)
        {
            resultingEffect = EffectT.XBlood;
            colorText = TextColor.Red;
            shielded = false;
            armored = false;
            
            var rng = new Random((int)Attacker.ActorId);

            var val = rng.Next(4);

            switch (val)
            {
                default:
                    return 0;
                case 1:
                    break;
                case 2:
                    shielded = true;
                    break;
                case 3:
                    armored = true;
                    break;
            }

            return rng.Next(10) + 1;
        }
    }
}