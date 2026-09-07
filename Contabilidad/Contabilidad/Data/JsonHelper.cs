using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Contabilidad.Data
{
    internal static class JsonHelper
    {
        /// <summary>
        /// JavaScriptSerializer.DeserializeObject devuelve un arreglo JSON como
        /// object[] (no como ArrayList), por lo que se normaliza aqui a List&lt;object&gt;.
        /// </summary>
        public static List<object> ParseArray(string json)
        {
            var serializer = new JavaScriptSerializer();
            if (string.IsNullOrWhiteSpace(json)) return new List<object>();
            var resultado = serializer.DeserializeObject(json);

            var comoArreglo = resultado as object[];
            if (comoArreglo != null) return comoArreglo.ToList();

            var comoLista = resultado as List<object>;
            if (comoLista != null) return comoLista;

            return new List<object>();
        }

        public static string ToJson(List<Dictionary<string, object>> datos)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(datos);
        }

        public static string ObtenerTexto(Dictionary<string, object> dict, string clave)
        {
            object valor;
            if (dict.TryGetValue(clave, out valor) && valor != null) return valor.ToString();
            return null;
        }

        public static int ObtenerEntero(Dictionary<string, object> dict, string clave, int porDefecto = 0)
        {
            object valor;
            if (dict.TryGetValue(clave, out valor) && valor != null) return System.Convert.ToInt32(valor);
            return porDefecto;
        }

        public static bool ObtenerBooleano(Dictionary<string, object> dict, string clave, bool porDefecto = false)
        {
            object valor;
            if (dict.TryGetValue(clave, out valor) && valor != null) return System.Convert.ToBoolean(valor);
            return porDefecto;
        }

        public static decimal ObtenerDecimal(Dictionary<string, object> dict, string clave)
        {
            object valor;
            if (dict.TryGetValue(clave, out valor) && valor != null) return System.Convert.ToDecimal(valor);
            return 0m;
        }
    }
}
