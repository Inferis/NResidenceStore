namespace ResidenceStore
{
    public interface IResidenceStoreProvider
    {
        IResidenceStore ResidenceStore { get; }
    }
}