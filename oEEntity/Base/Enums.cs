using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oEEntity.Base
{
    public enum EntityOperationalState
    {
        None = 0,
        New = 1,
        Update = 2,
        Delete = 3
    }

    public enum QuestionTypes
    {
        None = 0,
        Mode = 1,
        Group = 2,
        Topic = 3,
        Question = 4
    }
}
