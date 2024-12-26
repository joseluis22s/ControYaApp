using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControYaApp.Models
{
    public class PrdOrdenProduccionNotificado
    {
        // TODO: VERIFICAR SIS E NECESITA AGREGAR [Serializable]
        public string CodigoProduccion {  get; set; }
        public int Orden { get; set; }
        public string CodigoMaterial { get; set; }
        public string IdProduccion { get; set; }
        public DateTime Fecha { get; set; }
        public double Notificado { get; set; }
        public string CodigoUsuario { get; set; }
        public string CodigoTransaccion { get; set; }
        public int Documento { get; set; }
        public int FilaINV { get; set; }
        public int CodigoEmpleado { get; set; }
        public string Estado { get; set; }

    }
}
