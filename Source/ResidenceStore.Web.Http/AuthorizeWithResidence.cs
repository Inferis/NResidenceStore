namespace ResidenceStore.Web.Http
{
    public static class AuthorizeWithResidence
    {
        public static IResidenceStore Store { get; private set; }

        public static void UseStore(IResidenceStore store)
        {
            Store = store;
        }

    }
}