namespace View.RazorTemplate
{
    public interface IRazorTemplate<in TModel>
    {
        string Get(TModel model);
    }
}
