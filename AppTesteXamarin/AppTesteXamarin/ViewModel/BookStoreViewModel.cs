using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppTesteXamarin.Model;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace AppTesteXamarin.ViewModel
{
    public class BookStoreViewModel : BaseViewModel
    {
        private AzureDataService azureService;

        private string _nome;
        public string Nome
        {
            get { return _nome; }
            set { _nome = value; Notify("Nome"); }
        }

        private bool _loadin;

        public bool loading
        {
            get { return _loadin; }
            set { _loadin = value; Notify("loading"); }
        }



        private List<Book> _books;
        public List<Book> Books
        {
            get { return _books; }
            set { _books = value; Notify("Books"); }
        }

        public BookStoreViewModel()
        {
            azureService = new AzureDataService();
        }

        ICommand addBookCommand;
        public ICommand AddBookCommand =>
            addBookCommand ?? (addBookCommand = new Command(async () => await ExecuteAddBookCommandAsync()));

        async Task ExecuteAddBookCommandAsync()
        {
            try
            {
                loading = true;
                await azureService.Initialize();
                var books = await azureService.GetBooks();
                var lista = books.ToList();
                var coffee = await azureService.AddBook(Nome);
                Nome = string.Empty;
                lista.Add(coffee);
                Books = new List<Book>(lista);
                loading = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OH NO!" + ex);

            }
        }


    }
}
