using System;
using System.Linq;
using System.Windows;
using LicenceManager.DB;
using LicenceManager.Licence;
using LicenceManager.Models;

namespace LicenceManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string GetConnectionString()
        {
            var p = Environment.CurrentDirectory;
            return $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={p}\\LicenceDatabase.mdf;Integrated Security=True";
        }

        public MainWindow()
        {
            InitializeComponent();

            using (var ctx = new LicenceManagerDb(GetConnectionString()))
            {
                foreach (string nm in ctx.LicenceTypes.Select(x=>x.Name))
                {
                    cbLicenceType.Items.Add(nm);
                }
            }

            cbLicenceType.SelectedIndex = 0;
            RefreshClientsInfo();

        }

        private void RefreshClientsInfo()
        {
            using (var ctx = new LicenceManagerDb(GetConnectionString()))
            {
                var dt = ctx.ClientsLicences.Select(x => new ClientInfoModel
                {
                    Id = x.Clients.Id,
                    Name = x.Clients.Name,
                    CNC = x.Licences.ConcurenteNumberOfConnections,
                    NNC = x.Licences.NamedNumberOfConnections,
                    DateStart = x.StartDate,
                    ClientCode = x.VerificationCode,
                    LicenceCode = x.LicenceKey,
                    IsValid = x.Licences.Activ,
                    LicenceDescription = x.Licences.Description,
                    LicenceName = x.Licences.Name,
                    DayLimit = x.Licences.DurationDay,
                    Functionality = x.Licences.Functionals
                }).ToList();
                clientsGrid.ItemsSource = dt;
            }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            using (var ctx = new LicenceManagerDb(GetConnectionString()))
            {
                var clInfo = new ClientsInfo {Name = txtClientName.Text};
                clInfo.Licences.Add(new ClientsLicence { IsTrial = false, LicenceKey = txtLicenceCode.Text, VerificationCode = txtClientCode.Text, StartDate = DateTime.Now,LicenceId = ctx.LicenceTypes.First(x => x.Name == cbLicenceType.Text).Id});
                ctx.ClientsInfos.Add(clInfo);
                ctx.SaveChanges();
            }
            RefreshClientsInfo();
        }

        private LicenceInfo GetLicenceInfo()
        {
            using (var ctx = new LicenceManagerDb(GetConnectionString()))
            {
                return ctx.LicenceTypes.Where(x => x.Name == cbLicenceType.Text).Select(x => new LicenceInfo
                {
                    ClientName = txtClientName.Text,
                    LicenceId = x.Id,
                    DateLimit = x.DurationDay,
                    ConcurenteNumberOfConnections = x.ConcurenteNumberOfConnections,
                    Functionals = x.Functionals,
                    NamedNumberOfConnections = x.NamedNumberOfConnections
                }).First();
            }
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            var li = GetLicenceInfo();
            txtLicenceCode.Text = LicenceGenerator.CreateLicenceKey(txtClientCode.Text, li);
            btnAdd.IsEnabled = true;
        }
    }
}
