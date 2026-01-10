using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UseCaseCore.UseCases
{
    public abstract class UseCaseBase<TRequest, TResponse>
    {
        public abstract Task<TResponse> Execute(TRequest request);
    }
}
