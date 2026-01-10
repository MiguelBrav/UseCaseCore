using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UseCaseCore.UseCases
{
    public class UseCaseDispatcher
    {
        // Virtual 
        public virtual async Task<TResponse> Dispatch<TRequest, TResponse>(UseCaseBase<TRequest, TResponse> useCase, TRequest request)
        {
            var response = await useCase.Execute(request);
            return response;
        }
    }
}
