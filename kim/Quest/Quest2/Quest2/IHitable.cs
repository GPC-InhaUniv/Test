﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest2
{
    interface IHitable
    {
        void Hit(int maxDamage, Random random);
    }
}