﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Models.Factory
{
    /// <summary>
    /// Represents a spawn point of a certain resource
    /// </summary>
    class SourceStep : FlowStep
    {
        public SourceStep(ItemAmount amount)
            : base(amount)
        {

        }

        public override string ToString()
        {
            return String.Format("Source<{0}>", Item);
        }
    }
}