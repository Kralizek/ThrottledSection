namespace Kralizek.ThrottledSection
{
    public interface IThrottledSection
    {
        bool CanEnter();

        bool TryEnter();
    }
}