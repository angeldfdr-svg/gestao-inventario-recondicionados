using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace GestaoInventarioRecondicionados
{
    public partial class FormPrincipal : Form
    {
        // Listas para guardar os dados dos equipamentos
        private List<string> listaNomes = new List<string>();
        private List<string> listaCategorias = new List<string>();
        private List<string> listaEstados = new List<string>();
        private List<decimal> listaPrecos = new List<decimal>();

        // Índice do item selecionado para edição (-1 = nenhum)
        private int indiceSelecionado = -1;

        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            // Preencher a combobox de categorias
            cbCategoria.Items.Clear();
            cbCategoria.Items.Add("Desktop");
            cbCategoria.Items.Add("Portátil");
            cbCategoria.SelectedIndex = 0;

            // Preencher a combobox de estados de recondicionamento
            cbEstado.Items.Clear();
            cbEstado.Items.Add("Por avaliar");
            cbEstado.Items.Add("Em reparação");
            cbEstado.Items.Add("Recondicionado");
            cbEstado.Items.Add("Pronto para venda");
            cbEstado.SelectedIndex = 3;

            // Preencher as comboboxes de filtro
            cbFiltroCategoria.Items.Clear();
            cbFiltroCategoria.Items.Add("Todas as Categorias");
            cbFiltroCategoria.Items.Add("Desktop");
            cbFiltroCategoria.Items.Add("Portátil");
            cbFiltroCategoria.SelectedIndex = 0;

            cbFiltroEstado.Items.Clear();
            cbFiltroEstado.Items.Add("Todos os Estados");
            cbFiltroEstado.Items.Add("Por avaliar");
            cbFiltroEstado.Items.Add("Em reparação");
            cbFiltroEstado.Items.Add("Recondicionado");
            cbFiltroEstado.Items.Add("Pronto para venda");
            cbFiltroEstado.SelectedIndex = 0;

            // Carregar dados de teste iniciais
            CarregarDadosIniciais();

            // Atualizar a grelha
            AtualizarTabela();
            DefinirMensagemStatus("Aplicação pronta a utilizar.");
        }

        private void CarregarDadosIniciais()
        {
            listaNomes.Clear();
            listaCategorias.Clear();
            listaEstados.Clear();
            listaPrecos.Clear();

            listaNomes.Add("Lenovo ThinkPad T480 Intel Core i5 16GB 256GB SSD");
            listaCategorias.Add("Portátil");
            listaEstados.Add("Pronto para venda");
            listaPrecos.Add(320.00m);

            listaNomes.Add("Dell OptiPlex 7070 Micro Intel Core i7 16GB 512GB SSD");
            listaCategorias.Add("Desktop");
            listaEstados.Add("Pronto para venda");
            listaPrecos.Add(385.00m);

            listaNomes.Add("HP EliteBook 840 G5 Intel Core i5 8GB 256GB SSD");
            listaCategorias.Add("Portátil");
            listaEstados.Add("Recondicionado");
            listaPrecos.Add(280.00m);

            listaNomes.Add("HP ProDesk 600 G4 Mini i5 8GB");
            listaCategorias.Add("Desktop");
            listaEstados.Add("Em reparação");
            listaPrecos.Add(190.00m);

            listaNomes.Add("Apple MacBook Pro 13\" Retina Core i5 8GB");
            listaCategorias.Add("Portátil");
            listaEstados.Add("Por avaliar");
            listaPrecos.Add(350.00m);
        }

        private void AtualizarTabela()
        {
            dgvEquipamentos.Rows.Clear();

            string termoPesquisa = txtPesquisa.Text.Trim().ToLower();
            string filtroCategoria = cbFiltroCategoria.SelectedItem != null ? cbFiltroCategoria.SelectedItem.ToString() : "Todas as Categorias";
            string filtroEstado = cbFiltroEstado.SelectedItem != null ? cbFiltroEstado.SelectedItem.ToString() : "Todos os Estados";

            int totalItens = listaNomes.Count;

            for (int i = 0; i < totalItens; i++)
            {
                string nome = listaNomes[i];
                string categoria = listaCategorias[i];
                string estado = listaEstados[i];
                decimal preco = listaPrecos[i];

                if (!string.IsNullOrEmpty(termoPesquisa) && !nome.ToLower().Contains(termoPesquisa))
                {
                    continue;
                }

                if (filtroCategoria != "Todas as Categorias" && categoria != filtroCategoria)
                {
                    continue;
                }

                if (filtroEstado != "Todos os Estados" && estado != filtroEstado)
                {
                    continue;
                }

                int indiceLinha = dgvEquipamentos.Rows.Add(
                    nome,
                    categoria,
                    estado,
                    preco.ToString("C2")
                );

                dgvEquipamentos.Rows[indiceLinha].Tag = i;
            }

            lblStatusTotal.Text = "Total: " + totalItens + " equipamento(s)";
            dgvEquipamentos.ClearSelection();
        }

        private bool ValidarCampos(out string mensagemErro, out decimal precoValido)
        {
            precoValido = 0;

            string nome = txtNome.Text.Trim();
            if (string.IsNullOrEmpty(nome))
            {
                mensagemErro = "O campo 'Nome do Equipamento' é obrigatório.";
                txtNome.Focus();
                return false;
            }

            if (cbCategoria.SelectedItem == null)
            {
                mensagemErro = "Selecione uma categoria.";
                cbCategoria.Focus();
                return false;
            }

            if (cbEstado.SelectedItem == null)
            {
                mensagemErro = "Selecione o estado do equipamento.";
                cbEstado.Focus();
                return false;
            }

            string textoPreco = txtPreco.Text.Trim().Replace("€", "").Trim();
            if (string.IsNullOrEmpty(textoPreco))
            {
                mensagemErro = "O campo 'Preço' é obrigatório.";
                txtPreco.Focus();
                return false;
            }

            string textoNormalizado = textoPreco.Replace(",", ".");
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out precoValido) || precoValido <= 0)
            {
                mensagemErro = "O Preço deve ser um valor numérico positivo (ex: 299.90).";
                txtPreco.Focus();
                return false;
            }

            mensagemErro = string.Empty;
            return true;
        }

        private void LimparCampos()
        {
            indiceSelecionado = -1;
            txtNome.Clear();
            txtPreco.Clear();

            if (cbCategoria.Items.Count > 0) cbCategoria.SelectedIndex = 0;
            if (cbEstado.Items.Count > 3) cbEstado.SelectedIndex = 3;

            btnAdicionar.Enabled = true;
            btnAtualizar.Enabled = false;
            btnRemover.Enabled = false;

            DefinirMensagemStatus("Campos limpos.");
        }

        private void CarregarEquipamentoParaEdicao(int indice)
        {
            if (indice < 0 || indice >= listaNomes.Count) return;

            indiceSelecionado = indice;
            txtNome.Text = listaNomes[indice];
            cbCategoria.SelectedItem = listaCategorias[indice];
            cbEstado.SelectedItem = listaEstados[indice];
            txtPreco.Text = listaPrecos[indice].ToString("F2");

            btnAdicionar.Enabled = false;
            btnAtualizar.Enabled = true;
            btnRemover.Enabled = true;

            DefinirMensagemStatus("Equipamento selecionado: " + listaNomes[indice]);
        }

        private void DefinirMensagemStatus(string mensagem)
        {
            lblStatusMensagem.Text = mensagem;
        }

        private void BtnAdicionar_Click(object sender, EventArgs e)
        {
            string mensagemErro;
            decimal precoValido;

            if (!ValidarCampos(out mensagemErro, out precoValido))
            {
                MessageBox.Show(mensagemErro, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nome = txtNome.Text.Trim();
            string categoria = cbCategoria.SelectedItem.ToString();
            string estado = cbEstado.SelectedItem.ToString();

            listaNomes.Add(nome);
            listaCategorias.Add(categoria);
            listaEstados.Add(estado);
            listaPrecos.Add(precoValido);

            AtualizarTabela();
            DefinirMensagemStatus("Equipamento '" + nome + "' registado com sucesso.");

            MessageBox.Show(
                "Equipamento registado com sucesso!",
                "Informação",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            LimparCampos();
        }

        private void BtnAtualizar_Click(object sender, EventArgs e)
        {
            if (indiceSelecionado < 0 || indiceSelecionado >= listaNomes.Count)
            {
                MessageBox.Show("Nenhum equipamento selecionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensagemErro;
            decimal precoValido;

            if (!ValidarCampos(out mensagemErro, out precoValido))
            {
                MessageBox.Show(mensagemErro, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            listaNomes[indiceSelecionado] = txtNome.Text.Trim();
            listaCategorias[indiceSelecionado] = cbCategoria.SelectedItem.ToString();
            listaEstados[indiceSelecionado] = cbEstado.SelectedItem.ToString();
            listaPrecos[indiceSelecionado] = precoValido;

            AtualizarTabela();
            DefinirMensagemStatus("Equipamento atualizado com sucesso.");

            MessageBox.Show(
                "Dados atualizados com sucesso!",
                "Informação",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            LimparCampos();
        }

        private void BtnRemover_Click(object sender, EventArgs e)
        {
            if (indiceSelecionado < 0 || indiceSelecionado >= listaNomes.Count)
            {
                MessageBox.Show("Nenhum equipamento selecionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nomeParaRemover = listaNomes[indiceSelecionado];

            DialogResult confirmacao = MessageBox.Show(
                "Tem a certeza de que deseja remover o equipamento seguinte?\n\n" + nomeParaRemover,
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacao == DialogResult.Yes)
            {
                listaNomes.RemoveAt(indiceSelecionado);
                listaCategorias.RemoveAt(indiceSelecionado);
                listaEstados.RemoveAt(indiceSelecionado);
                listaPrecos.RemoveAt(indiceSelecionado);

                AtualizarTabela();
                DefinirMensagemStatus("Equipamento removido com sucesso.");

                MessageBox.Show(
                    "Equipamento removido com sucesso!",
                    "Informação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LimparCampos();
            }
        }

        private void BtnLimpar_Click(object sender, EventArgs e)
        {
            LimparCampos();
        }

        private void BtnLimparFiltros_Click(object sender, EventArgs e)
        {
            txtPesquisa.Clear();
            cbFiltroCategoria.SelectedIndex = 0;
            cbFiltroEstado.SelectedIndex = 0;
            AtualizarTabela();
            DefinirMensagemStatus("Filtros limpos.");
        }

        private void DgvEquipamentos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvEquipamentos.Rows.Count)
            {
                object tagValor = dgvEquipamentos.Rows[e.RowIndex].Tag;
                if (tagValor is int)
                {
                    int indiceOriginal = (int)tagValor;
                    CarregarEquipamentoParaEdicao(indiceOriginal);
                }
            }
        }

        private void CbFiltroCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarTabela();
        }

        private void CbFiltroEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarTabela();
        }

        private void TxtPesquisa_TextChanged(object sender, EventArgs e)
        {
            AtualizarTabela();
        }
    }
}
