﻿using PlanetWars.Models.MilitaryUnits.Contracts;
using PlanetWars.Models.Planets.Contracts;
using PlanetWars.Models.Weapons.Contracts;
using PlanetWars.Repositories;
using PlanetWars.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetWars.Models.Planets
{
    public class Planet : IPlanet
    {
        public Planet(string name, double budget)
        {
            this.Name = name;
            this.Budget = budget;
            this.weapons = new WeaponRepository();
            this.army = new UnitRepository();
        }
        private string name;

        public string Name
        {
            get { return name; }
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(ExceptionMessages.InvalidPlanetName);
                }
                name = value;
            }
        }


        private double budget;

        public double Budget
        {
            get { return budget; }
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException(ExceptionMessages.InvalidBudgetAmount);
                }
                budget = value;
            }
        }

        public double MilitaryPower => CalculateMilitaryPower();

        private UnitRepository army;
        private WeaponRepository weapons;

        public IReadOnlyCollection<IMilitaryUnit> Army => this.army.Models;

        public IReadOnlyCollection<IWeapon> Weapons => this.weapons.Models;

        public void AddUnit(IMilitaryUnit unit) => this.army.AddItem(unit);

        public void AddWeapon(IWeapon weapon) => this.weapons.AddItem(weapon);

        public string PlanetInfo()
        {
            string forces = this.Army.Any() ? string.Join(", ", this.Army.Select(u => u.GetType().Name)) : "No units";
            string weapons = this.Weapons.Any() ? string.Join(", ", this.Weapons.Select(u => u.GetType().Name)) : "No weapons";

            StringBuilder sb = new StringBuilder();
            sb
                .AppendLine($"Planet: {this.Name}")
                .AppendLine($"--Budget: {this.Budget} billion QUID")
                .AppendLine($"--Forces: {forces}")
                .AppendLine($"--Combat equipment: {weapons}")
                .AppendLine($"--Military Power: {this.MilitaryPower}");

            return sb.ToString();
        }

        public void Profit(double amount) => this.Budget += amount;

        public void Spend(double amount)
        {
            if (this.Budget < amount)
            {
                throw new InvalidOperationException(ExceptionMessages.UnsufficientBudget);
            }
            this.Budget -= amount;
        }

        public void TrainArmy()
        {
            foreach (IMilitaryUnit unit in Army)
            {
                unit.IncreaseEndurance();
            }
        }

        private double CalculateMilitaryPower()
        {
            double total =
                Weapons.Sum(w => w.DestructionLevel) +
                Army.Sum(u => u.EnduranceLevel);

            if (Army.Any(u => u.GetType().Name == "AnonymousImpactUnit"))
            {
                total *= 1.3;
            }

            if (Weapons.Any(w => w.GetType().Name == "NuclearWeapon"))
            {
                total *= 1.45;
            }

            return Math.Round(total, 3);
        }
    }
}
