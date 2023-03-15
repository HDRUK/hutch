namespace ROCrates;

public interface IEntitySerializable<T>
{
  public abstract string Serialize();
  public abstract T Deserialize();
}
