using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace ArticalProject.Data.Repo;

public interface ISearchByAnyProperties<T> where T : class
{
    IEnumerable<T> SearchByProperty(string property);
}
