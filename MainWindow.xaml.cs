using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Business;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;


namespace BtmApp
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SearchPage s1 = new SearchPage();
        public NewItemPage n1 = new NewItemPage();
        public ModifyPage m1 = new ModifyPage();
        public Home home = new Home();
        cnBusiness cnBusiness;


        public MainWindow()
        {
            InitializeComponent();
            cnBusiness = new cnBusiness();
            Main.Content = home;
            InitializeDb();
        }

        private void InitializeDb()
        {
            cnBusiness.Database.EnsureCreated();
            if (cnBusiness.Customers != null)
            {
                if (!cnBusiness.Customers.Any())
                {
                    SeedDb();
                }
                //else
                //    ShowData();
            }
        }

        private void ShowData()
        {
            throw new NotImplementedException();
        }

        private void SeedDb()
        {
            var c1 = new Customer { Name = "Alice Johnson", Email = "alice@example.com", Phone = "+36-70-1234567" };
            var c2 = new Customer { Name = "Bob Smith", Email = "bob@example.com", Phone = "+36-30-7654321" };

            var p1 = new Product { Name = "Laptop", Price = 399990 };
            var p2 = new Product { Name = "Mouse", Price = 4990 };
            var p3 = new Product { Name = "Keyboard", Price = 9990 };

            var t1 = new Transaction
            {
                Date = DateTime.Parse("2025-05-01"),
                Amount = 409980,
                Status = "Completed",
                Customer = c1
            };

            var t2 = new Transaction
            {
                Date = DateTime.Parse("2025-05-02"),
                Amount = 14980,
                Status = "Pending",
                Customer = c2
            };

            var ti1 = new TransactionItem { Transaction = t1, Product = p1, Quantity = 1 };
            var ti2 = new TransactionItem { Transaction = t1, Product = p2, Quantity = 2 };
            var ti3 = new TransactionItem { Transaction = t2, Product = p3, Quantity = 1 };
            var ti4 = new TransactionItem { Transaction = t2, Product = p2, Quantity = 2 };

            cnBusiness.Customers.AddRange(c1, c2);
            cnBusiness.Products.AddRange(p1, p2, p3);
            cnBusiness.Transactions.AddRange(t1, t2);
            cnBusiness.TransactionItems.AddRange(ti1, ti2, ti3, ti4);

            cnBusiness.SaveChanges();
        }



        private void miSearchClick(object sender, RoutedEventArgs e)
        {
            Main.Content = s1;

        }

        private void miNewClick(object sender, RoutedEventArgs e)
        {
            Main.Content = n1;
        }

        private void miModifyPageClick(object sender, RoutedEventArgs e)
        {
            Main.Content = m1;
        }

        private void miHomeClick(object sender, RoutedEventArgs e)
        {
            Main.Content = home;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            DetachDb(cnBusiness);
        }

        private static void DetachDb(cnBusiness cn)
        {
            cn.Database.OpenConnection();
            var connection = cn.Database.GetDbConnection();
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
            string[] commands =
            { "USE master",
                $"ALTER DATABASE [{connection.Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE",
                $"ALTER DATABASE [{connection.Database}] SET OFFLINE WITH ROLLBACK IMMEDIATE",
                $"EXEC sp_detach_db '{connection.Database}'"
            };
            using (var sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = connection as SqlConnection;
                foreach (string command in commands)
                {
                    sqlCommand.CommandText = command;
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

    }
}