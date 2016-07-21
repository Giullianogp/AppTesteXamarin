using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace AppTesteXamarin.Model
{
    public class AzureDataService
    {
        public MobileServiceClient MobileService { get; set; }
        IMobileServiceSyncTable<Book> bookTable;
        bool isInitialized;
        public async Task Initialize()
        {
            if (isInitialized)
                return;

            MobileService = new MobileServiceClient("http://apptestexamarin.azurewebsites.net/");

            const string path = "synBook3.db";
            var store = new MobileServiceSQLiteStore(path);
            store.DefineTable<Book>();
            await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            bookTable = MobileService.GetSyncTable<Book>();

            isInitialized = true;

        }

        public async Task<IEnumerable<Book>> GetBooks()
        {
            await Initialize();
            await SyncBook();
            return await bookTable.OrderBy(c => c.Name).ToEnumerableAsync();
        }

        public async Task<Book> AddBook(string name)
        {
            await Initialize();
            var book = new Book()
            {
                Name = name
            };

            await bookTable.InsertAsync(book);

            //Synchronize coffee
            await SyncBook();

            return book;

        }

        public async Task SyncBook()
        {
            await bookTable.PullAsync("allBooks", bookTable.CreateQuery());
            await MobileService.SyncContext.PushAsync();
        }

    }
}
