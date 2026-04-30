using UnityEngine;

public class TargetFactory : FactoryBase<Target>
{
    public override Target CreateInstance() =>
        Instantiate(spawnObjects[Random.Range(0, spawnObjects.Length)]);

    public Target[] GetRawPrefabs() => spawnObjects;
}