﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Logging.Enums
{
    public enum MethodActionLogTypes
    {
        Starting,
        Ending,
        Failed,
        Completed,
        Aborted,
        Suspended,
        Resumed
    }
}
