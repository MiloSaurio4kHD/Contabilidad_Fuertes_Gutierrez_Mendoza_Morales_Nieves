using System;
using System.Collections.Generic;

namespace Contabilidad.Models
{
    /// <summary>
    /// Contrato comun entre un asiento del Libro Diario y un ajuste del Libro de Ajustes,
    /// para que ambos puedan mostrarse con la misma grilla (LibroContableDataGridViewBase).
    /// </summary>
    public interface IAsientoLibro
    {
        string NumeroDisplay { get; }
        DateTime Fecha { get; }
        string Glosa { get; }
        List<DetalleAsiento> Detalles { get; }
        decimal TotalDebe { get; }
        decimal TotalHaber { get; }
        bool EstaBalanceado { get; }
    }
}
