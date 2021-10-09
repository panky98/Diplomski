using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace EventMicroservice.Services
{
    public class DatabaseClient
    {

        public IMongoDatabase MongoDatabase
        { 
            get
            {
                if(_mongoDatabase == null)
                {
                    MongoClient _client = new MongoClient("mongodb://root:pass12345@mongodb:27017");
                    _mongoDatabase = _client.GetDatabase("events");                    
                }
                return _mongoDatabase;
            } 
        }

        public DatabaseClient()
        {
        }

        private IMongoDatabase _mongoDatabase;
    }
}
