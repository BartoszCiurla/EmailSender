using Model;
using RazorEngine;

namespace View.RazorTemplate
{
    public class PersonRazorTemplate:IRazorTemplate<Person>
    {
        private string messageTemplate = @"Template for Person. Hello @Model.FirstName @Model.LastName !";
       // private string ViewPath = @"..\..\..\View\PersonMail.cshtml";
        public string Get(Person model)
        {
            return Razor.Parse(messageTemplate, model);
            //return ViewPath;
        }
    }
}
