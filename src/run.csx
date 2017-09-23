using System.Net;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");
    
    dynamic body = await req.Content.ReadAsStringAsync();
    var e = JsonConvert.DeserializeObject<Person>(body as string);
    
    // Define the row,
    string sRow = e.email + e.lastname;

    // Create the Entity and set the partition to signup, 
    PersonEntity _person = new PersonEntity("signup", sRow);

    _person.First_Name_VC = e.firstname;
    _person.Last_Name_VC = e.lastname;
    _person.Email_VC = e.email;
    
    // Connect to the Storage account.
    CloudStorageAccount storageAccount = CloudStorageAccount.Parse("XXX");

    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

    CloudTable table = tableClient.GetTableReference("personitems");

    table.CreateIfNotExists();

    TableOperation insertOperation = TableOperation.Insert(_person);

    table.Execute(insertOperation);

    return req.CreateResponse(HttpStatusCode.OK, "Ok");
}

public class Person{
    public string firstname {get;set;}
    public string lastname {get;set;}
    public string email {get;set;}
}



    public class PersonEntity : TableEntity
    {
        public PersonEntity(string skey, string srow)
        {
            this.PartitionKey = skey;
            this.RowKey = srow;
        }

        public PersonEntity() { }
        public string First_Name_VC { get; set; }
        public string Last_Name_VC { get; set; }
        public string Email_VC { get; set;}
    }