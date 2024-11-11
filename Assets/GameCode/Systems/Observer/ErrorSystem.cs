namespace Legacy.Observer
{
    /*[UpdateInGroup(typeof(TechnicalSystemGroup))]
	public class ErrorSystem : ComponentSystem
	{
		private EntityQuery _errors;
		protected override void OnCreate()
		{
			_errors = GetEntityQuery(
					ComponentType.ReadOnly<UserErrorData>(),
					ComponentType.ReadOnly<TechnicalErrorMessage>()
				);
		}
		protected override void OnUpdate()
		{
			var error_entities = _errors.ToEntityArray(Allocator.TempJob);
			var error_datas = _errors.ToComponentDataArray<UserErrorData>(Allocator.TempJob);

			var window_entity = EntityManager.CreateEntity();
			EntityManager.AddComponentData(window_entity, default(PopupWindow));
			EntityManager.AddComponentData(window_entity,
				new WindowTypeComponent
				{
					type = WindowType.ErrorWindow
				});
			UserErrorData data = error_datas[0];
			EntityManager.AddComponentData(window_entity, data);

			foreach(var e in error_entities)
				PostUpdateCommands.DestroyEntity(e);
			error_entities.Dispose();
			error_datas.Dispose();
		}
	}*/
}