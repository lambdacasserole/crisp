﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crisp.Tokenizing
{
    internal enum TokenType
    {
        OpeningParenthesis,
        ClosingParenthesis,
        Symbol,
        Numeric,
        String,
        Dot,
    }
}
