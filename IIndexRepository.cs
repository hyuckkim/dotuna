using System.Collections.Generic;

namespace DoTuna
{
    public interface IIndexRepository
    {
        public List<JsonIndexDocument> Get();
    }
}