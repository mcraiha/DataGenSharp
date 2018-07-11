using System;
using System.Collections.Generic;

namespace DatagenSharp
{
    public static class OutputTypeHelper
    {
        public static (bool isSupported, string possibleError) CheckIfSupported()
        {
            return (isSupported: true, possibleError: "");
        }
    }
}