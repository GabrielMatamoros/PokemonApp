using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio;

namespace negocio
{
    public class PokemonNegocio
    {      //Lo que queremos es traernos una tabla de datos de nuestra BD POKEDEX_DB
        public  List<Pokemon> listar() //Para empezar vamos a crear un metodo "Listar"
        {                              //Este metodo va a retornar una lsta de Pokemons.
            List<Pokemon> lista = new List<Pokemon>(); //Una variable de tipo"lista" para poder guardar los datos que me traigo de la tabla.
                                                       //Mi variable "lista" es como un vector.
          
            //Luego debemos crear los objetos necesarios para interactuar con la Base de Datos
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;
            
            
            try
            { //Configuro mis objetos.
                conexion.ConnectionString = "server=.\\SQLEXPRESS;database=POKEDEX_DB;integrated security=true";//Aca especifico a donde me voy a conectar.
                comando.CommandType = System.Data.CommandType.Text; // En esta linea especificamos que la consulta que vamos a realizae es de tipo "texto"
                comando.CommandText = "select Numero, Nombre, P.Descripcion, UrlImagen, E.Descripcion Tipo, D.Descripcion Debilidad, P.IdTipo, P.IdDebilidad, P.Id from POKEMONS P, ELEMENTOS E, ELEMENTOS D  where E.Id = P.IdTipo And D.id = P.IdDebilidad And P.Activo = 1";//Se realiza consulta.
                comando.Connection = conexion;   //Le decimos a nuestro comando que se conecte en la cadena de conexion que se indico arriba
 
                conexion.Open();
                lector = comando.ExecuteReader();// Generamos una especie de tabla virtual con el metodo "ExecuteReader"

                while (lector.Read())            // La tabla generada la vamos a convertir en objetos
                {
                    Pokemon aux = new Pokemon(); // Creamos nuestro objeto de tipo Pokemon.
                                                 // Mi variable aux es una especie de puntero que se va a parara en cada celda y va a gaudar el dato que se encuente en dicha celda, el nombre, el numero, la descripcion...
                                                 // Luego estos datos los va a guardar en la lista.
                    aux.Id = (int)lector["Id"];
                    aux.Numero = (int)lector["Numero"];
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["Descripcion"];
                    
                    if(!(lector["UrlImagen"] is DBNull))//Si el tipo de dato en BD es "Null" No lo vamos a leer
                    aux.UrlImagen = (string)lector["UrlImagen"];
                   
                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)lector["Tipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)lector["Debilidad"];
                    
                    lista.Add(aux);
                } //Cuando mi ciclo while ya no encuentre que leer va salir del ciclo y va a cerrar la conexion.
                conexion.Close();
                return lista; //Retornamos la lista de Pokemons.
            }
            catch (Exception ex)
            {
             throw ex;
            }     
        }    
    
        public void agregar(Pokemon nuevo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("Insert into POKEMONS(Numero, Nombre, Descripcion, Activo, idTipo, idDebilidad, UrlImagen) Values("+nuevo.Numero +",'"+nuevo.Nombre+"','"+nuevo.Descripcion+"',1,@idTipo, @idDebilidad, @UrlImagen)");
                datos.setearParametro("@idTipo",nuevo.Tipo.Id);
                datos.setearParametro("@idDebilidad",nuevo.Tipo.Id);
                datos.setearParametro("@UrlImagen", nuevo.UrlImagen);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Pokemon poke)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("update POKEMONS set Numero = @numero, Nombre = @nombre, Descripcion = @desc, UrlImagen = @img, IdTipo = @idTipo, IdDebilidad = @idDebilidad where Id = @id ");
                datos.setearParametro("@numero", poke.Numero);
                datos.setearParametro("@nombre", poke.Nombre);
                datos.setearParametro("@desc", poke.Descripcion);
                datos.setearParametro("@img", poke.UrlImagen);
                datos.setearParametro("@idTipo", poke.Tipo.Id);
                datos.setearParametro("@idDebilidad", poke.Debilidad.Id);
                datos.setearParametro("@id", poke.Id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
                
        }
        
        public List<Pokemon> filtrar(string campo, string criterio, string filtro)
        {
            List<Pokemon> lista = new List<Pokemon>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string consulta = "select Numero, Nombre, P.Descripcion, UrlImagen, E.Descripcion Tipo, D.Descripcion Debilidad, P.IdTipo, P.IdDebilidad, P.Id from POKEMONS P, ELEMENTOS E, ELEMENTOS D  where E.Id = P.IdTipo And D.id = P.IdDebilidad And P.Activo = 1 And ";
                
                if(campo == "Numero")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "Numero >" + filtro;
                            break;
                        case "Menor a":
                            consulta += "Numero < " + filtro;
                            break;
                        default:
                            consulta += "Numero = " + filtro;
                            break;
                    }
                }
                else if(campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Empieza con ":
                            consulta += "Nombre like '"+filtro+"%'";
                            break;
                        case "Termina con":
                            consulta += "Nombre like '%"+filtro+"'";
                             break;
                        default:
                            consulta += "Nombre like '%"+filtro+"%'";
                            break;
                    }                        
                }
                else
                {
                    switch (criterio)
                    {
                        case "Empieza con":
                            consulta += "P.Descripcion like '"+filtro +"%'";
                            break;
                        case "Termina con":
                            consulta += "P.Descripcion like '%"+filtro+"'";
                            break;
                        default:
                            consulta += "P.Descripcion like '%"+filtro+"%'";
                            break;
                    }
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Pokemon aux = new Pokemon();             //Aca obtengo los datos de la tablita virtual que me genera el Execute Reader
                    aux.Id = (int)datos.Lector["Id"];       //Ya convertidos en objetos
                    aux.Numero = (int)datos.Lector["Numero"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    if (!(datos.Lector["UrlImagen"] is DBNull))
                    aux.UrlImagen = (string)datos.Lector["UrlImagen"];

                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)datos.Lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)datos.Lector["Descripcion"];

                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)datos.Lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)datos.Lector["Descripcion"];

                    lista.Add(aux);                   
                }
                    return lista; //Con alt + flechita para arriba o para abajo subo y bajo el texto.
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void eliminar(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("delete from POKEMONS where id = @id");
                datos.setearParametro("@id",id);
                datos.ejecutarAccion();
            }
            catch (Exception ex )
            {

                throw ex;
            }
        }
        public void eliminarLogico(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("update POKEMONS set Activo = 0 where id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
           

        }
    }
    
}
