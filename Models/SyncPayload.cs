using System;
using System.Collections.Generic;

namespace VPS_LaBarata.API.Models
{
    public class SyncPayload
    {
        public VentaSync Venta { get; set; }
        public List<DetalleSync> Detalles { get; set; }
    }

    public class VentaSync
    {
        public string id_venta { get; set; }
        public int id_usuario { get; set; }
        public decimal total { get; set; }
        public string metodo_pago { get; set; }
        public decimal monto_recibido { get; set; }
        public decimal pago_efectivo { get; set; }
        public decimal pago_tarjeta { get; set; }
        public decimal pago_transferencia { get; set; }
        public string estado { get; set; }
        public DateTime fecha_hora { get; set; }
    }

    public class DetalleSync
    {
        public string id_venta { get; set; }
        public string codigo_barras { get; set; }
        public decimal cantidad { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal importe { get; set; }
    }
}
