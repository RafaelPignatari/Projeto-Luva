using LuvaApp.Helpers.BluetoothHelper;
using LuvaApp.Models;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System.Text;

namespace LuvaApp.Helpers
{
    public class RecepcaoController
    {
        public static List<string> ListaRecepcaoCopy
        {
            get {
                try
                {
                    return _listaRecepcao.Select(item => item.Replace(';', ',')).ToList();
                } catch{ return new List<string>(); } 
            }
        }

        private static List<string> _listaRecepcao;

        #region SINGLETON
        private static RecepcaoController _instancia;

        // Private constructor to prevent instance creation
        private RecepcaoController() { }

        public static RecepcaoController Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new RecepcaoController();
                }

                if (_listaRecepcao == null)
                {
                    _listaRecepcao = new List<string>();
                }

                return _instancia;
            }
        }
        #endregion

        public async Task IniciaRecepcao(BluetoothController bluetoothController)
        {
            IService luvaService = await bluetoothController.ConnectedDevice.GetServiceAsync(Guid.Parse("00000015-0000-1000-8000-00805F9B34FB"));
            ICharacteristic luvaCharacteristic = (await luvaService.GetCharacteristicsAsync())[0];

            _listaRecepcao = new List<string>();
            luvaCharacteristic.ValueUpdated -= ValueUpdatedHandler;
            luvaCharacteristic.ValueUpdated += ValueUpdatedHandler;

            await luvaCharacteristic.StartUpdatesAsync();
        }

        public async Task<string> GetValues(BluetoothController bluetoothController)
        {
            await RecepcaoController.Instancia.IniciaRecepcao(bluetoothController);

            List<string> valuesCapturedList = new List<string>();
            while (true)
            {
                valuesCapturedList = RecepcaoController.ListaRecepcaoCopy.ToList();
                if (valuesCapturedList.Count == 6)
                    break;
            }

            string values = string.Join(",", valuesCapturedList).Replace(",,", ",");
            return values;
        }

        public SensoresModel ObtemUltimoValorRecebido()
        {
            var penultimoValor = _listaRecepcao[_listaRecepcao.Count - 2];
            var retorno = new SensoresModel(penultimoValor);

            return retorno;
        }

        private void ValueUpdatedHandler(object? sender, CharacteristicUpdatedEventArgs e)
        {
            var bytes = e.Characteristic.Value;
            var valor = Encoding.UTF8.GetString(bytes);
            AdicionaValorALista(valor);
        }

        private void AdicionaValorALista(string valor)
        {
            if (valor.First() == '0')
            {
                valor = valor.Substring(1); //Removemos o primeiro caractere, referente ao index do pacote.
                _listaRecepcao.Add(valor);
            }
            else
            {
                valor = valor.Substring(1); //Removemos o primeiro caractere, referente ao index do pacote.
                try
                {
                    var ultimoValorAdicionado = _listaRecepcao.Last();
                    var indexUltimo = _listaRecepcao.IndexOf(ultimoValorAdicionado);

                    ultimoValorAdicionado += valor;
                    _listaRecepcao[indexUltimo] = ultimoValorAdicionado;
                }
                catch
                {
                    return;
                }                
            }

            RemoveValorDaLista();
        }

        private void RemoveValorDaLista()
        {
            if (_listaRecepcao.Count > 6)
            {
                _listaRecepcao.RemoveAt(0);
            }
        }        
    }
}
