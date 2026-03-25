public class NPCComponent<TData> : TML.GlobalNPC where TData : struct
{
	public override string Name => $"NPCComponent.{typeof(TData).FullName}";
	public override bool InstancePerEntity { get; } = true;
	internal TData Data;
	internal bool Enabled = false;
}

public class ItemComponent<TData> : TML.GlobalItem where TData : struct
{
	public override string Name => $"ItemComponent.{typeof(TData).FullName}";
	public override bool InstancePerEntity { get; } = true;
	internal TData Data;
	internal bool Enabled = false;
}

public class PlayerComponent<TData> : TML.ModPlayer where TData : struct
{
	public override string Name => $"PlayerComponent.{typeof(TData).FullName}";
	internal TData Data;
	internal bool Enabled = false;
}

public class ProjectileComponent<TData> : TML.GlobalProjectile where TData : struct
{
	public override string Name => $"ProjectileComponent.{typeof(TData).FullName}";
	public override bool InstancePerEntity { get; } = true;
	internal TData Data;
	internal bool Enabled = false;
}
