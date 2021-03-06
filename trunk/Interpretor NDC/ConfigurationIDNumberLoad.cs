using System;
using System.Collections.Generic;
using System.Text;

namespace Interpretor_NDC
{
    class ConfigurationIDNumberLoad : CustomisationDataCommands
    {
        public string ConfigurationIDNumber;

        public ConfigurationIDNumberLoad(string str)
            : base(str)
        {
            Name = "Configuration ID Number Load";

            int sep1 = Utils.StrIndexOf((char)28, str, 3);

            ConfigurationIDNumber = str.Substring(sep1 + 1, 4);
            Trailer = str.Substring(sep1 + 1 + 4);
        }
    }
}
