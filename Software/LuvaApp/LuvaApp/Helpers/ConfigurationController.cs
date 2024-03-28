using LuvaApp.Models;
using System.Text.Json;

namespace LuvaApp.Helpers
{
    public abstract class ConfigurationController
    {
        public async static Task<ConfigurationModel> GetConfigurationAsync()
        {
            try
            {
                string configurationJson = await SecureStorage.Default.GetAsync(Constantes.CONFIGURATION_KEY_SECURE_STORAGE);

                if (configurationJson == null)
                    return await GetConfigurationDefault();

                var configurationModel = JsonSerializer.Deserialize<ConfigurationModel>(configurationJson);

                return configurationModel;
            }
            catch (Exception ex)
            {
                //TODO: Adicionar logs.
                return await GetConfigurationDefault();
            }
        }

        public async static Task SetConfigurationAsync(ConfigurationModel configuration)
        {
            var configurationJson = JsonSerializer.Serialize(configuration);

            await SecureStorage.Default.SetAsync(Constantes.CONFIGURATION_KEY_SECURE_STORAGE, configurationJson);
        }

        private async static Task<ConfigurationModel> GetConfigurationDefault()
        {
            ConfigurationModel configurationModelDefault = new ConfigurationModel();

            await SetConfigurationAsync(configurationModelDefault);

            return configurationModelDefault;
        }
    }
}
