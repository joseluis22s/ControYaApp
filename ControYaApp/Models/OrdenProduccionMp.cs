using System.Text.Json.Serialization;
using ControYaApp.ViewModels.Base;
using SQLite;

namespace ControYaApp.Models
{
    public class OrdenProduccionMp : BaseViewModel
    {
        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [JsonIgnore]
        [Ignore]
        public decimal Saldo => ((decimal)Cantidad) - Notificado;

        private bool _isSelected;
        [JsonIgnore]
        [Ignore]
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public int IdMaterialProduccion { get; set; }

        public string Centro { get; set; }

        public string CodigoProduccion { get; set; }

        public int Orden { get; set; }

        public string CodigoMaterial { get; set; }

        public string Material { get; set; }

        public string CodigoBodega { get; set; }

        public string CodigoProducto { get; set; }

        public string Producto { get; set; }

        public string CodigoUnidad { get; set; }

        public decimal Cantidad { get; set; }


        private decimal _notificado;
        public decimal Notificado
        {
            get => _notificado;
            set
            {
                if (SetProperty(ref _notificado, value))
                {
                    OnPropertyChanged(nameof(Saldo));

                }
            }
        }

        public string CodigoUsuarioAprobar { get; set; }




    }
}
