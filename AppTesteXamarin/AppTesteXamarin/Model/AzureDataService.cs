using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace AppTesteXamarin.Model
{
    public class AzureDataService
    {
        public MobileServiceClient MobileService { get; set; }
        IMobileServiceSyncTable bookTable;

        public async Task Initialize()
        {
            MobileService = new MobileServiceClient("http://apptestexamarin.azurewebsites.net/");

            const string path = "syncstore.db";
            var store = new MobileServiceSQLiteStore(path);
            store.DefineTable(); 
            await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            bookTable = MobileService.GetSyncTable();

        }

        public async Task<IEnumerable> GetBooks()
        {
            await SyncBook();
            return await bookTable.OrderBy(c => c.DateUtc).ToEnumerableAsync();
        }

        public async Task AddBook(bool madeAtHome)
        {
            var book = new Book
            {
                DateUtc = DateTime.UtcNow,
                MadeAtHome = madeAtHome
            };

            await bookTable.InsertAsync(book);

            //Synchronize coffee
            await SyncBook();

        }

        public async Task SyncBook()
        {
            await bookTable.PullAsync("allBooks", bookTable);
            await MobileService.SyncContext.PushAsync();

        }

    }
}
