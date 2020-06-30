using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace FileShareRepositoryMover.Services
{
    class Helper
    {
        public static string StringLoader(string resourceName)
        {
            string data = null;
            using (Stream stringStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stringStream != null)
                {
                    data = new StreamReader(stringStream).ReadToEnd();
                }
            }
            return data;
        }
    }
}
