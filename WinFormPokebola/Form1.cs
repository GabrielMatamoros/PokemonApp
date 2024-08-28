using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;

namespace WinFormPokebola
{
    public partial class frmPokemon : Form
    {
        private List<Pokemon> listaPokemon; //Atributo privado de tipo lista.
        private List<Elemento> listaElemento;//Todas las variables de tipo "lista" son como vectores dinamicos.
        public frmPokemon()
        {
            InitializeComponent();
        }
        

        private void frmPokemon_Load(object sender, EventArgs e)
        {
            cargar();
            cboCampo.Items.Add("Numero");
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripcion");
        }

        private void ocultarColumnas()
        {
            dgvPokemons.Columns["UrlImagen"].Visible = false;
            dgvPokemons.Columns["Numero"].Visible = false;
            dgvPokemons.Columns["Id"].Visible = false;
        }
        
        private void dgvPokemons_SelectionChanged(object sender, EventArgs e) //Con este evento de mi dgv logramos que cada vez
        {                                                                     //que seleccionemos una fila cambie la imagen del pokemon.
            if (dgvPokemons.CurrentRow != null)

            {
                Pokemon seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem; //Esto es para evitar que cuando tenga un item null
                cargarImagen(seleccionado.UrlImagen);                                 // el programa no se rompa, ya que no se puede transformar algo nulo en un Pokemon obviamente.
            }    
        }

        private void cargar() {

            try
            {
                PokemonNegocio negocio = new PokemonNegocio();//Este pasaje que hacemos aca es porque ahora voy a necesitar el objeto listaPokemon
                listaPokemon = negocio.listar();              //tendre mas libertad de manipular los datos de la tabla que obtengo con el metodo listar()
                ElementoNegocio elem = new ElementoNegocio(); //listaPokemon es como un vector que guarda pokemons.
                listaElemento = elem.listar();
                dgvPokemons.DataSource = listaPokemon;        //DataSource recibe un origen de datos y lo modela en la tabla"dgvPokemons".
                ocultarColumnas();
                cargarImagen(listaPokemon[0].UrlImagen); // Por ejm, cuando cargue mi ventana, al ser como un vector 
                                                         // puedo obtener el primer elemento de la lista.
            }
            catch (Exception ex)
            {

               MessageBox.Show(ex.ToString());
            }
        }

        private void cargarImagen(string imagen) //Creamos esta funcion que recibe por parametro un string, en este caso una "url"
        {                                        //Y a traves de un try catch, en caso de que algun pokemon no cuente con ninguna "url"
            try                                  //En la base de datos, ira al catch y va a mostrar la "Url" que le asignamos, en este caso el de una imagen vacia.
            {
                pbxPokemon.Load(imagen); //Carga la imagen correspondiente a la url en el pbx
            }
            catch (Exception ex)
            {
                                 //"url de imagen vacia.
                pbxPokemon.Load("https://developers.elementor.com/docs/assets/img/elementor-placeholder-image.png");
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaPokemon alta = new frmAltaPokemon(); //Creo una instancia de mi clase "frmAltaPokemon" para poder usar el metodo de su clase "ShowDialog"
            alta.ShowDialog();
            cargar();                                  //Este metodo nos permite que al agregar un pokemon se actualice automaticamenete en mi 
        }                                              // Ventana sin tener que cerrarla y volverla a abrir.

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Pokemon seleccionado;
            seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;

            frmAltaPokemon modificar = new frmAltaPokemon(seleccionado); //Uso el constructor creado en frmAltapokemon que recibe un pokemon por parametro
            modificar.ShowDialog();                                      //"seleccionado" es el pokemon.
            cargar();
        }

        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            eliminar();
        }

        private void btnEliminarLogico_Click(object sender, EventArgs e)
        {
            eliminar(true);
        }
        private void eliminar( bool logico = false)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            Pokemon seleccionado;

            try
            {
                DialogResult respuesta = MessageBox.Show("Desea eliminar el Pokemon seleccionado?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;

                    if (logico)
                    {
                        negocio.eliminarLogico(seleccionado.Id);
                    }
                    else
                    {
                        negocio.eliminar(seleccionado.Id);

                    }
                    cargar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
       
        private bool validarFiltro()
        {

            if(cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Porfavor cargar el campo");
                    return true;
            }
            
            if(cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor cagar el criterio");
                    return true;
            }
          
            if(cboCampo.SelectedItem.ToString() == "Numero")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes cargar un numero");
                    return true;
                }

                if (!(soloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Este campo solo acepta caracteres numericos");
                    return true;
                }
            }
            return false;              
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }

            return true;
        }
        
        private void btnFiltro_Click(object sender, EventArgs e)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            try
            {
                if (validarFiltro())
                    return;

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                dgvPokemons.DataSource = negocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Pokemon> listaFiltrada;
            string filtro = txtFiltro.Text;

            if(filtro.Length >= 3) // Aca le indicamos que solo usaremos el filtro si se ingreso un minimo de 3 caracteres.
            {
                listaFiltrada = listaPokemon.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Tipo.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                listaFiltrada = listaPokemon; //Si no se ingresaron al menos 3 caracteres se mantiene la lista original.
            }

            dgvPokemons.DataSource = null;
            dgvPokemons.DataSource = listaFiltrada;
            ocultarColumnas();

        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();

            if(opcion == "Numero") // Si es numero
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("igual a");
            }
            else                   // Si es texto.
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene a");
            }
        }

        private void btnResetear_Click(object sender, EventArgs e)
        {
            cargar();
        }
    }
}
