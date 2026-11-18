using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VPS_LaBarata.API.Models;
using VPS_LaBarata.API.Security;
// Espacio de nombres de tu ApiKeyAttribute

namespace VPS_LaBarata.API.Controllers
{
    [Route("api/sincronizacion")]
    [ApiController]
    [ApiKey] // El candado de seguridad
    public class SyncController : ControllerBase
    {
        private readonly string _vpsConnectionString;

        public SyncController(IConfiguration configuration)
        {
            // Lee "PostgresVPS" de tu appsettings.json
            _vpsConnectionString = configuration.GetConnectionString("PostgresVPS");
        }

        [HttpPost("recibir-venta")]
        public IActionResult RecibirVenta([FromBody] SyncPayload payload)
        {
            if (payload == null || payload.Venta == null)
                return BadRequest(new { exito = false, mensaje = "Payload vacío" });

            using (var db = new NpgsqlConnection(_vpsConnectionString))
            {
                db.Open();

                // 1. IDEMPOTENCIA: Verificar si el UUID ya existe en PostgreSQL
                bool existe = db.ExecuteScalar<bool>("SELECT COUNT(1) > 0 FROM Ventas WHERE id_venta = @Id", new { Id = payload.Venta.id_venta });

                if (existe) return Ok(new { exito = true, mensaje = "Venta ya sincronizada previamente." });

                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        // 2. Insertar Venta
                        string sqlVenta = @"INSERT INTO Ventas 
                                            (id_venta, id_usuario, total, metodo_pago, monto_recibido, pago_efectivo, pago_tarjeta, pago_transferencia, estado, fecha_hora) 
                                            VALUES 
                                            (@id_venta, @id_usuario, @total, @metodo_pago, @monto_recibido, @pago_efectivo, @pago_tarjeta, @pago_transferencia, @estado, @fecha_hora)";

                        db.Execute(sqlVenta, payload.Venta, tran);

                        // 3. Insertar Detalles
                        if (payload.Detalles != null && payload.Detalles.Count > 0)
                        {
                            string sqlDetalle = @"INSERT INTO DetalleVentas 
                                                  (id_venta, codigo_barras, cantidad, precio_unitario, importe) 
                                                  VALUES 
                                                  (@id_venta, @codigo_barras, @cantidad, @precio_unitario, @importe)";

                            db.Execute(sqlDetalle, payload.Detalles, tran);
                        }

                        tran.Commit();
                        return Ok(new { exito = true });
                    }
                    catch (System.Exception ex)
                    {
                        tran.Rollback();
                        return StatusCode(500, new { exito = false, mensaje = ex.Message });
                    }
                }
            }
        }
    }
}
