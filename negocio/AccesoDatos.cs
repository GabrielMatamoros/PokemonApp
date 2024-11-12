using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace negocio
{               // COMENTANDO
                //PARA PROBAR


    //Esta clase que vamos a crear a continuacion es para centralizar todas
                      //las acciones que queremos realizar hacia la base de datos en pequenas funciones.
    class AccesoDatos // Creo los atributos de mi clase
    {
        public SqlConnection conexion;        //Objetos necesarios para conectarme a la BD. 
        public SqlCommand comando;     
        public SqlDataReader lector;
        public SqlDataReader Lector
        {                                    //Esta property la creo para poder leer el lector desde la clse elemento negocio en el while.
            get { return lector; }
        } 
        
        public  AccesoDatos()                 //Constructor: Caracteristicas con las que nace mi clase de AccesoDatos
        {                                     // Al crear una instancia de esta clase automaticamente ya va a contar con estas caracteristicas.
            conexion = new SqlConnection("server=.\\SQLEXPRESS;database=POKEDEX_DB;integrated security=true");
            comando = new SqlCommand();
        }
        
        public void setearConsulta(string consulta)// Metodo pra realizar mi consulta a la BD/
        {
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = consulta;
        }
       
        public void ejecutarLectura()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                lector = comando.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ejecutarAccion()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void setearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor);
        }
        
        public void cerrarConexion()
        {
            if (lector!= null)
            {
                lector.Close();
                conexion.Close();
            }

        }
    }
}
