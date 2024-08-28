using System;using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using negocio;
using dominio;
using System.Configuration;
using System.IO;

namespace WinFormPokebola
{
    public partial class frmAltaPokemon : Form
    {
        private Pokemon pokemon = null;
        private OpenFileDialog archivo = null;
        public frmAltaPokemon()  //Si viene a este constructor es porque presionamos "Agregar"
        {                        //y no recibe nada por parametro asi que mi variable pokemon(blanco) queda en null.
            InitializeComponent();
        }
        public frmAltaPokemon(Pokemon pokemon)
        {
            InitializeComponent();    //Aca lo que hago es aclarar que si vino al segundo constructor
            this.pokemon = pokemon;   //es porque recibio un pokemon por parametro asi que mi variable pokemon(blanco) ya no esta en null.
            Text = "Modificar Pokemon"; 
        }
      
        private void btnAceptar_Click(object sender, EventArgs e)
        {
          //  Pokemon poke = new Pokemon(); Esta linea la eliminamos porque vamos a utilizar la 
                                         // Variable "pokemon" creada en el constructor del frmAltaPokemon.
            PokemonNegocio negocio = new PokemonNegocio();

            try
            {
                if(pokemon == null)
                {
                    pokemon = new Pokemon(); //Si el Pokemon es nulo quiere decir que estamos agregando uno nuevo
                }                            // Poe eso creamos el nuevo pokemon, sino es que estamos modificando.
                pokemon.Numero = int.Parse(txtNumero.Text);
                pokemon.Nombre = txtNombre.Text;
                pokemon.Descripcion = txtDescripcion.Text;
                pokemon.UrlImagen = txtUrlImagen.Text;
                pokemon.Tipo = (Elemento)cboTipo.SelectedItem;
                pokemon.Debilidad = (Elemento)cboDebilidad.SelectedItem;
                
                if(pokemon.Id != 0)
                {
                    negocio.modificar(pokemon);
                    MessageBox.Show("Modificado Exitosamente");
                }
                else
                {
                    negocio.agregar(pokemon);
                    MessageBox.Show("Agregado Exitosamente");
                }
              
                //guardo Pokemon si levanto imagen localmente: 
                if (archivo != null && !(txtUrlImagen.Text.ToUpper().Contains("HTTP")))
                {
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["image-folder"] + archivo.SafeFileName);
                }
                
               Close();

            }
            catch (Exception ex)
            {   
                MessageBox.Show(ex.ToString());              
            }
        }
        
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmAltaPokemon_Load(object sender, EventArgs e)
        {
            ElementoNegocio elementoNegocio = new ElementoNegocio();
            try
            {
                cboTipo.DataSource = elementoNegocio.listar(); //De esta manera me traigo la lista de elementos
                cboTipo.ValueMember = "Id";
                cboTipo.DisplayMember = "Descripcion";
                cboDebilidad.DataSource = elementoNegocio.listar();
                cboDebilidad.ValueMember = "Id";
                cboDebilidad.DisplayMember = "Descripcion";

                if(pokemon != null) //VALIDACION: A traves de la validacion vamos a capturar los datos del 
                                    // Pokemon que se quiere modificar para que se pre carguen.
                {
                    txtNumero.Text = pokemon.Numero.ToString();
                    txtNombre.Text = pokemon.Nombre;
                    txtDescripcion.Text = pokemon.Descripcion;
                    txtUrlImagen.Text = pokemon.UrlImagen;
                    cargarImagen(pokemon.UrlImagen);
                    cboTipo.SelectedValue = pokemon.Tipo.Id;
                    cboDebilidad.SelectedValue = pokemon.Debilidad.Id;

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtUrlImagen.Text);
        }
        private void cargarImagen(string imagen)
        {
            try
            {
              pbxPokemon.Load(imagen);
            }
            catch (Exception ex)
            {
                pbxPokemon.Load("https://developers.elementor.com/docs/assets/img/elementor-placeholder-image.png");
            }
        }


        private void btnAgregarImagen_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg; |png|*.png";

            if (archivo.ShowDialog() == DialogResult.OK)
            {
                txtUrlImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);
            }
        }
    }
}
