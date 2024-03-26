using LuvaApp.Helpers.AndroidPermissions;
using LuvaApp.Interfaces;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace LuvaApp.Helpers.BluetoothHelper
{
    public class BluetoothController : IBluetoothController
    {
        #region SINGLETON
        private static BluetoothController _instancia;
        
        private BluetoothController() { }

        public async static Task<BluetoothController> GetInstance()
        {
            if (_instancia == null)
            {
                _instancia = new BluetoothController();
                await EfetuarConexaoBluetooth();
            }

            return _instancia;
        }

        private static async Task EfetuarConexaoBluetooth()
        {
            await _instancia.AsyncRequestBluetoothPermissions();
            await _instancia.AsyncConnectToDeviceByName("LuvaController");
        }

        #endregion
        public IDevice? ConnectedDevice { get; set; }
        public IAdapter? Adapter { get; set; }

        public async Task<IEnumerable<IDevice>> AsyncGetDevices()
        {
            IEnumerable<IDevice> dispositivosEncontrados = null;

            await MainThread.InvokeOnMainThreadAsync(async() =>
            {
                try
                {
                    Adapter = CrossBluetoothLE.Current.Adapter;
                    await Adapter.StartScanningForDevicesAsync();
                    dispositivosEncontrados = Adapter.DiscoveredDevices.Where(device => device.Name != null);
                }
                catch
                {
                    throw new Exception("Erro ao obter dispositivos. O bluetooth está ligado?");
                }
            });

            return dispositivosEncontrados;
        }

        public async Task AsyncConnectToDeviceByName(string deviceName)
        {
            IEnumerable<IDevice> devicesFound = await AsyncGetDevices();
            ConnectedDevice = devicesFound.FirstOrDefault(device => device.Name == deviceName);

            if (ConnectedDevice == null)
                throw new Exception("Dispositivo não encontrado: " + deviceName);

            await Adapter!.ConnectToDeviceAsync(ConnectedDevice);
        }

        public void DisconnectFromDevice()
        {
            ConnectedDevice!.Dispose();
            ConnectedDevice = null;
        }

        public async Task AsyncRequestBluetoothPermissions()
        {
            await Permissions.RequestAsync<BluetoothPermissions>();
        }
    }
}
