using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenBasedAuthSample.Services
{
    public static class EncrptedHelper
    {

        public static string Replace(string chiperText)
        {
            return chiperText.Replace($"{ConfigurationEncryptionTypes.ChiperPrefix}:", "");
        }
    }
}
