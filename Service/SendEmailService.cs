using FluentMailer.Interfaces;
using Model;
using Service.Utility;
using View.RazorTemplate;

namespace Service
{
    public interface ISendEmailService<in T>
    {
        void Send(T model);
    }
    public class SendEmailService<T> :ISendEmailService<T> where T : EmailAddress
    {
        private readonly IFluentMailer _fluentMailer;
        private readonly IRazorTemplate<T> _razorTemplate; 
        public SendEmailService(IFluentMailer fluentMailer,IRazorTemplate<T> razorTemplate)
        {            
            _fluentMailer = fluentMailer;
            _razorTemplate = razorTemplate;
        }
        public void Send(T model)
        {
            ModelValidator.Validate(model);
            _fluentMailer.CreateMessage()
                .WithViewBody(_razorTemplate.Get(model))
                //.WithView(_razorTemplate.Get(model),model)
                .WithReceivers(model.Email)
                .Send();
        }

       
    }
}
