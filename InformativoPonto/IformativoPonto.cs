using Firebase.Database;
using MetroFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InformativoPonto
{
    public partial class IformativoPonto : MetroFramework.Forms.MetroForm
    {
        #region [ATRIBUTOS]

        private OpenFileDialog ofdArquivo = new OpenFileDialog();

        #endregion

        #region [CONTRUTOR]

        public IformativoPonto()
        {
            InitializeComponent();

            //var auth = "ABCDE"; // your app secret
            //var firebaseClient = new FirebaseClient(
            //  "https://informativo-9fc24-default-rtdb.firebaseio.com/",
            //  new FirebaseOptions
            //  {
            //      AuthTokenAsyncFactory = () => Task.FromResult(auth)
            //  });

            mtxtDataAtual.Text = DateTime.Today.ToShortDateString();
        }

        #endregion

        #region [MÉTODOS]

        private void CarregaGrid(DataTable dt)
        {
            mgrdTempo.DataSource = null;
            mgrdTempo.DataSource = dt;

            for (int j = 0; j < mgrdTempo.ColumnCount; j++)
            {
                mgrdTempo.Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                for (int i = 0; i < mgrdTempo.RowCount; i++)
                {
                    mgrdTempo[j, i].Value = mgrdTempo[j, i].Value.ToString().Replace("\"", "");
                }
            }

            mgrdTempo.Rows.Remove(mgrdTempo.Rows[0]);
            mgrdTempo.Columns.Remove(mgrdTempo.Columns[2]);
            mgrdTempo.Columns.Remove(mgrdTempo.Columns[2]);
            mgrdTempo.Columns.Remove(mgrdTempo.Columns[2]);

            var JsonString = new StringBuilder();

            JsonString.Append("[");//https://www.macoratti.net/16/03/c_dtjson1.htm
            for (int i = 0; i < mgrdTempo.Rows.Count; i++)
            {
                JsonString.Append("{");
                for (int j = 0; j < mgrdTempo.Columns.Count; j++)
                {
                    if (j < mgrdTempo.Columns.Count -1)
                    {
                        //JsonString.Append("\"" + mgrdTempo.Columns[j].Name + "\":" + "\"" + mgrdTempo.Rows[i][j].ToString() + "\",");
                    }
                }
            };

            //ConverterJSON(mgrdTempo.DataSource);

            /*
                 public static void ConverterJSON(List<Tabela> listaAlterada)
                 {
                     string json = JsonConvert.SerializeObject(listaAlterada);
                     File.WriteAllText(@"meuCaminho", json);
                 }
             */
        }

        private void CamposVisiveisInvisiveis(int intTitulo)
        {
            switch (intTitulo)
            {
                case 1:
                    metroPanel2.Visible = true;
                    metroPanel3.Visible = false;
                    metroPanel4.Visible = false;
                    break;
                case 2:
                    metroPanel2.Visible = false;
                    metroPanel3.Visible = true;
                    metroPanel4.Visible = false;
                    break;
                case 3:
                    metroPanel2.Visible = false;
                    metroPanel3.Visible = false;
                    metroPanel4.Visible = true;
                    break;
            }
        }

        private void ImportarCSV()
        {
            ofdArquivo.InitialDirectory = "@" + "C:\\";
            ofdArquivo.Multiselect = false;

            if ((ofdArquivo.ShowDialog() != DialogResult.OK) || (ofdArquivo.FileName.Length == 0))
                return;

            string nome_arquivo = ofdArquivo.FileName;
            if (nome_arquivo.Contains(".csv"))
            {
                CarregaGrid(LerArquivoExcel(nome_arquivo));
                MetroMessageBox.Show(this, "Arquivo salvo com sucesso.\n\nVerifique o Mês Corrente!", "Sucesso!");
            }
            else
            {
                MetroMessageBox.Show(this,"Não foi possível carregar, Verifique o formato do arquivo!", "Erro!");
                return;
            }
        }

        private DataTable LerArquivoExcel(string arquivo)
        {
            DataTable dt = null;
            string ext = VerificarExtensaoExcel(arquivo);
            if (ext != string.Empty)
            {
                if (ext == ".csv")
                {
                    dt = LerArquivoCSV(arquivo);
                }
            }
            return dt;
        }

        public static DataTable LerArquivoCSV(string strArquivo, char chDelimitadorDeColuna = ';', char chDelimitadorDeLinha = '\n')
        {
            DataTable dt = new DataTable();

            if (strArquivo != string.Empty)
            {
                string conteudo = LerArquivo(strArquivo).ToString();
                string[] linha = conteudo.Split(chDelimitadorDeLinha);
                for (int i = 0; i < linha.Length; i++)
                {
                    string[] celula = linha[i].Split(chDelimitadorDeColuna);
                    if (i == 0)
                    {
                        for (int j = 0; j < celula.Length; j++)
                        {
                            dt.Columns.Add();
                        }
                    }
                    dt.Rows.Add(celula);
                }
            }
            return dt;
        }

        public static StringBuilder LerArquivo(string arquivo)
        {
            StringBuilder texto;
            StreamReader input;

            FileInfo file = new FileInfo(arquivo);

            if (!file.Exists)
            {
                throw new FileNotFoundException();
            }
            input = new StreamReader(arquivo, Encoding.GetEncoding(28591));
            texto = new StringBuilder();

            while (!input.EndOfStream)
            {
                texto.Append(input.ReadLine() + "\n");
            }
            input.Close();
            return texto;
        }

        private string VerificarExtensaoExcel(string arquivo)
        {
            string[] ext = { ".csv", ".xls", ".xlsx" };
            foreach (string e in ext)
            {
                if (arquivo.EndsWith(e)) { return e; }
            }
            return string.Empty;
        }

        #endregion

        #region [EVENTOS]

        private void mtInformarHora_Click(object sender, EventArgs e)
        {
            CamposVisiveisInvisiveis(1);
        }

        private void mtMesCorrente_Click(object sender, EventArgs e)
        {
            CamposVisiveisInvisiveis(2);
        }

        private void metroTile3_Click(object sender, EventArgs e)
        {
            CamposVisiveisInvisiveis(3);
        }

        private void mbtnImportar_Click(object sender, EventArgs e)
        {
            ImportarCSV();
        }

        private void mbtnSalvar_Click(object sender, EventArgs e)
        {
            if (mtxtEntrada.Text != string.Empty && mtxtSaidaAlmoco.Text != string.Empty
                && mtxtRetornoAlmoco.Text != string.Empty && mtxtSaida.Text != string.Empty && (mradRelogio.Checked || mradWeb.Checked))
            {

            }
        }

        #endregion
    }
}