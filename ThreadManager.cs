using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DoTuna
{
    public class ThreadManager
    {
        private IIndexRepository _indexRepository;
        public IEnumerable<JsonIndexDocument> Index
        {
            get => _indexRepository.Get();
        }

        public ThreadManager(IIndexRepository indexRepository)
        {
            _indexRepository = indexRepository;
        }
    }
}
