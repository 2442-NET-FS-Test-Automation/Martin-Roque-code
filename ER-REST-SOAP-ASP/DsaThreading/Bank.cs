namespace DsaThreading;

public class Bank
{
    public long Balance;

    //Lock object
    private readonly object _gate = new();
    public void DepositUnsafe(long amount) => Balance += amount;

    public void DepositSafe(long amount)
    {
        lock (_gate) //Only one thread can enter this code block at a time
        {
            Balance += amount;
        }
    }
}