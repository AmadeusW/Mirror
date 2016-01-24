using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Mirror.GUI.Controllers
{
    static class SettingsController
    {
        public static dynamic Settings { get; private set; }

        public static async Task LoadSettings()
        {
            var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var sampleFile = await packageFolder.GetFileAsync("settings.json");
            var rawSettings = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            Settings = JsonConvert.DeserializeObject(rawSettings);
        }
    }
}
