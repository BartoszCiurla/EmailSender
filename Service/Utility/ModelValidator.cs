using System;
using System.Linq;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;

namespace Service.Utility
{
    public static class ModelValidator
    {
        public static void Validate<TModel>(TModel model)
        {
            var validatioResult = GetModelValidator<TModel>().Validate(model);
            if (!validatioResult.IsValid)
            {
                ValidationFailure error = validatioResult.Errors.FirstOrDefault();
                if(error != null)
                throw new Exception(error.ErrorMessage);
            }
        }

        private static AbstractValidator<TModel> GetModelValidator<TModel>()
        {
            return Assembly.GetExecutingAssembly().
                GetTypes().
                Where(n =>
                    n.IsClass &&
                    typeof(AbstractValidator<TModel>).IsAssignableFrom(n) &&
                    n.GetConstructor(Type.EmptyTypes) != null).
                Select(n => Activator.CreateInstance(n) as AbstractValidator<TModel>).
                FirstOrDefault();
        }
    }
}
