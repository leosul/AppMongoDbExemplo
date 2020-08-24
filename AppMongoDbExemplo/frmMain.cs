using AppMongoDbExemplo.Context;
using AppMongoDbExemplo.Model;
using Bogus;
using Bogus.Extensions.Brazil;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace AppMongoDbExemplo
{
    public partial class frmMain : Form
    {
        MongoDbContext dbContext = null;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            MongoDbContext.ConnectionString = "mongodb://localhost:27017";
            MongoDbContext.DatabaseName = "ClientesDb";
            MongoDbContext.IsSSL = false;

            dbContext = new MongoDbContext();
            ListarClientes();
        }

        private async void ListarClientes()
        {
            labelCount.Text = "...";

            List<Cliente> listaClientes = await dbContext.Clientes.Find(_ => true).ToListAsync();

            labelCount.Text = String.Format("{0:N0}", listaClientes.Take(1000).Count());
            labelCount.Refresh();

            bindingSourceMain.DataSource = listaClientes.ToList().Take(1000);
            dgvMain.DataSource = bindingSourceMain;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            gbMain.Enabled = true;
            txbNome.Focus();
        }

        private void btnIncluir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Você irá incluir mais 100 registros fakes, confirma?", "Incluir", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            try
            {
                var faker = new Faker<Cliente>("pt_BR").StrictMode(true)
                .RuleFor(p => p.Id, f => ObjectId.GenerateNewId())
                .RuleFor(p => p.Nome, f => f.Person.FullName)
                .RuleFor(p => p.Cpf, f => f.Person.Cpf())
                .RuleFor(p => p.DataNascimento, f => f.Person.DateOfBirth)
                .RuleFor(p => p.Idade, f => f.Random.Number(10, 80))
                .RuleFor(p => p.Renda, f => Math.Round(f.Random.Decimal(1500, 17560), 2))
                .RuleFor(p => p.Ativo, f => f.Random.Bool())
                .Generate(100);

                dbContext.Clientes.InsertMany(faker);

                ListarClientes();
                MessageBox.Show("Registros incluídos com sucesso!", "Incluir", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Incluir", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            try
            {
                var id = (ObjectId)dgvMain[0, dgvMain.CurrentRow.Index].Value;
                var entity = dbContext.Clientes.Find(m => m.Id == id).FirstOrDefault();
                dbContext.Clientes.DeleteOne(m => m.Id == id);

                ListarClientes();

                MessageBox.Show("Registros incluídos com sucesso!", "Incluir", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Deletar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgvMain_SelectionChanged(object sender, EventArgs e)
        {
            txbId.Text = dgvMain[0, dgvMain.CurrentRow.Index].Value.ToString();
            txbNome.Text = dgvMain[1, dgvMain.CurrentRow.Index].Value.ToString();
            txbCpf.Text = dgvMain[2, dgvMain.CurrentRow.Index].Value.ToString();
            dtpDataNascimento.Value = (DateTime)dgvMain[3, dgvMain.CurrentRow.Index].Value;
            txbIdade.Text = dgvMain[4, dgvMain.CurrentRow.Index].Value.ToString();
            txbRenda.Text = dgvMain[5, dgvMain.CurrentRow.Index].Value.ToString();
            ckbAtivo.Checked = (bool)dgvMain[6, dgvMain.CurrentRow.Index].Value;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            gbMain.Enabled = false;

            try
            {
                var _id = ObjectId.Parse(dgvMain["Id", dgvMain.CurrentRow.Index].Value.ToString());
                Expression<Func<Cliente, bool>> filter = x => x.Id.Equals(_id);

                var entity = dbContext.Clientes.Find(filter).FirstOrDefault();

                entity.Nome = txbNome.Text;
                entity.Cpf = txbCpf.Text;
                entity.DataNascimento = dtpDataNascimento.Value;
                entity.Idade = Convert.ToInt32(txbIdade.Text);
                entity.Renda = Convert.ToDecimal(txbRenda.Text);
                entity.Ativo = ckbAtivo.Checked;

                dbContext.Clientes.ReplaceOne(m => m.Id == entity.Id, entity);

                ListarClientes();

                MessageBox.Show("Registro salvo com sucesso!", "Editar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Editar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSair_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
