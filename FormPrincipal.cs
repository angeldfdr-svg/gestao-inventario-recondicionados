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

        // Flag para evitar eventos recursivos durante a atualização da grelha
        private bool aAtualizarTabela = false;

        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            aAtualizarTabela = true;

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

            aAtualizarTabela = false;

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
            aAtualizarTabela = true;

            try
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

                    // Pesquisa abrangente (nome, categoria ou estado)
                    if (!string.IsNullOrEmpty(termoPesquisa))
                    {
                        bool corresponde = nome.ToLower().Contains(termoPesquisa) ||
                                           categoria.ToLower().Contains(termoPesquisa) ||
                                           estado.ToLower().Contains(termoPesquisa);
                        if (!corresponde) continue;
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
                        preco.ToString("N2") + " €"
                    );

                    dgvEquipamentos.Rows[indiceLinha].Tag = i;
                }

                if (dgvEquipamentos.Rows.Count == totalItens)
                {
                    lblStatusTotal.Text = "Total: " + totalItens + " equipamento(s)";
                }
                else
                {
                    lblStatusTotal.Text = "Apresentados: " + dgvEquipamentos.Rows.Count + " de " + totalItens + " equipamento(s)";
                }

                dgvEquipamentos.ClearSelection();
            }
            finally
            {
                aAtualizarTabela = false;
            }
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

            if (cbCategoria.SelectedItem == null && string.IsNullOrEmpty(cbCategoria.Text.Trim()))
            {
                mensagemErro = "Selecione uma categoria válida.";
                cbCategoria.Focus();
                return false;
            }

            if (cbEstado.SelectedItem == null && string.IsNullOrEmpty(cbEstado.Text.Trim()))
            {
                mensagemErro = "Selecione o estado do equipamento.";
                cbEstado.Focus();
                return false;
            }

            string textoPreco = txtPreco.Text.Trim().Replace("€", "").Replace("EUR", "").Replace("eur", "").Trim();
            if (string.IsNullOrEmpty(textoPreco))
            {
                mensagemErro = "O campo 'Preço' é obrigatório.";
                txtPreco.Focus();
                return false;
            }

            // Suporta formatos com vírgula ou ponto decimal
            bool conversaoComSucesso = decimal.TryParse(textoPreco.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out precoValido)
                                    || decimal.TryParse(textoPreco, NumberStyles.Any, CultureInfo.CurrentCulture, out precoValido);

            if (!conversaoComSucesso || precoValido <= 0)
            {
                mensagemErro = "O Preço deve ser um valor numérico positivo (ex: 299.90 ou 299,90).";
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

            aAtualizarTabela = true;
            dgvEquipamentos.ClearSelection();
            aAtualizarTabela = false;

            btnAdicionar.Enabled = true;
            btnAtualizar.Enabled = false;
            btnRemover.Enabled = false;

            DefinirMensagemStatus("Campos limpos. Pronto para registar novo equipamento.");
        }

        private void CarregarEquipamentoParaEdicao(int indice)
        {
            if (indice < 0 || indice >= listaNomes.Count) return;

            indiceSelecionado = indice;
            txtNome.Text = listaNomes[indice];
            cbCategoria.SelectedItem = listaCategorias[indice];
            cbEstado.SelectedItem = listaEstados[indice];
            txtPreco.Text = listaPrecos[indice].ToString("0.00");

            // Mantém Adicionar habilitado e ativa Atualizar e Remover
            btnAdicionar.Enabled = true;
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
            string categoria = cbCategoria.SelectedItem != null ? cbCategoria.SelectedItem.ToString() : cbCategoria.Text.Trim();
            string estado = cbEstado.SelectedItem != null ? cbEstado.SelectedItem.ToString() : cbEstado.Text.Trim();

            listaNomes.Add(nome);
            listaCategorias.Add(categoria);
            listaEstados.Add(estado);
            listaPrecos.Add(precoValido);

            // Limpa pesquisa/filtros para garantir que o novo registo aparece de imediato
            aAtualizarTabela = true;
            txtPesquisa.Clear();
            cbFiltroCategoria.SelectedIndex = 0;
            cbFiltroEstado.SelectedIndex = 0;
            aAtualizarTabela = false;

            AtualizarTabela();

            // Destaca a linha do equipamento acabado de inserir
            if (dgvEquipamentos.Rows.Count > 0)
            {
                int ultimaLinha = dgvEquipamentos.Rows.Count - 1;
                dgvEquipamentos.ClearSelection();
                dgvEquipamentos.Rows[ultimaLinha].Selected = true;
                dgvEquipamentos.FirstDisplayedScrollingRowIndex = ultimaLinha;
            }

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
            // Se o índice não estiver definido mas houver uma linha selecionada na grelha, obtém o índice
            if ((indiceSelecionado < 0 || indiceSelecionado >= listaNomes.Count) && dgvEquipamentos.SelectedRows.Count > 0)
            {
                if (dgvEquipamentos.SelectedRows[0].Tag is int)
                {
                    indiceSelecionado = (int)dgvEquipamentos.SelectedRows[0].Tag;
                }
            }

            if (indiceSelecionado < 0 || indiceSelecionado >= listaNomes.Count)
            {
                MessageBox.Show("Por favor selecione um equipamento na lista para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensagemErro;
            decimal precoValido;

            if (!ValidarCampos(out mensagemErro, out precoValido))
            {
                MessageBox.Show(mensagemErro, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string categoria = cbCategoria.SelectedItem != null ? cbCategoria.SelectedItem.ToString() : cbCategoria.Text.Trim();
            string estado = cbEstado.SelectedItem != null ? cbEstado.SelectedItem.ToString() : cbEstado.Text.Trim();

            listaNomes[indiceSelecionado] = txtNome.Text.Trim();
            listaCategorias[indiceSelecionado] = categoria;
            listaEstados[indiceSelecionado] = estado;
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
            // Se o índice não estiver definido mas houver uma linha selecionada na grelha, obtém o índice
            if ((indiceSelecionado < 0 || indiceSelecionado >= listaNomes.Count) && dgvEquipamentos.SelectedRows.Count > 0)
            {
                if (dgvEquipamentos.SelectedRows[0].Tag is int)
                {
                    indiceSelecionado = (int)dgvEquipamentos.SelectedRows[0].Tag;
                }
            }

            if (indiceSelecionado < 0 || indiceSelecionado >= listaNomes.Count)
            {
                MessageBox.Show("Por favor selecione um equipamento na lista para remover.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nomeParaRemover = listaNomes[indiceSelecionado];

            DialogResult confirmacao = MessageBox.Show(
                "Tem a certeza de que deseja remover o equipamento seguinte?\n\n" + nomeParaRemover,
                "Confirmar Remoção",
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
            aAtualizarTabela = true;
            txtPesquisa.Clear();
            if (cbFiltroCategoria.Items.Count > 0) cbFiltroCategoria.SelectedIndex = 0;
            if (cbFiltroEstado.Items.Count > 0) cbFiltroEstado.SelectedIndex = 0;
            aAtualizarTabela = false;

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

        private void DgvEquipamentos_SelectionChanged(object sender, EventArgs e)
        {
            if (aAtualizarTabela) return;

            if (dgvEquipamentos.SelectedRows.Count > 0)
            {
                DataGridViewRow linha = dgvEquipamentos.SelectedRows[0];
                if (linha.Tag is int)
                {
                    int indiceOriginal = (int)linha.Tag;
                    CarregarEquipamentoParaEdicao(indiceOriginal);
                }
            }
        }

        private void CbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCategoria.SelectedItem != null)
            {
                DefinirMensagemStatus("Categoria selecionada: " + cbCategoria.SelectedItem.ToString());
            }
        }

        private void CbEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbEstado.SelectedItem != null)
            {
                DefinirMensagemStatus("Estado selecionado: " + cbEstado.SelectedItem.ToString());
            }
        }

        private void CbFiltroCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!aAtualizarTabela)
            {
                AtualizarTabela();
            }
        }

        private void CbFiltroEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!aAtualizarTabela)
            {
                AtualizarTabela();
            }
        }

        private void TxtPesquisa_TextChanged(object sender, EventArgs e)
        {
            if (!aAtualizarTabela)
            {
                AtualizarTabela();
            }
        }
    }
}
