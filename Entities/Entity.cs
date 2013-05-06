using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Each entity has a name, unique components for data and handlers for logic attached to them.
	/// Entities are used for rendering, game objects, ui, physics, etc.
	/// </summary>
	public abstract class Entity
	{
		public Entity Add(object component)
		{
			if (component is EntityHandler)
				throw new InstantiatedEntityHandlerAddedToEntity();

			components.Add(component);
			return this;
		}

		public class InstantiatedEntityHandlerAddedToEntity : Exception { }

		internal readonly List<object> components = new List<object>();

		public Entity Add<T>() where T : EntityHandler
		{
			if (!handlerTypesToAdd.Contains(typeof(T)))
				handlerTypesToAdd.Add(typeof(T));

			return this;
		}

		internal readonly List<Type> handlerTypesToAdd = new List<Type>();

		public Entity Add<T1, T2>()
			where T1 : EntityHandler
			where T2 : EntityHandler
		{
			Add<T1>();
			Add<T2>();
			return this;
		}

		public Entity Add<T1, T2, T3>()
			where T1 : EntityHandler
			where T2 : EntityHandler
			where T3 : EntityHandler
		{
			Add<T1>();
			Add<T2>();
			Add<T3>();
			return this;
		}

		internal void AddHandler(EntityHandler handler)
		{
			if (activeHandlers.Contains(handler))
				return;

			AddHandlerSortedByPriority(handler);
		}

		private void AddHandlerSortedByPriority(EntityHandler handler)
		{
			for (int index = 0; index < activeHandlers.Count; index++)
			{
				if (activeHandlers[index].Priority <= handler.Priority)
					continue;
				activeHandlers.Insert(index, handler);
				return;
			}
			activeHandlers.Add(handler);
		}

		internal readonly List<EntityHandler> activeHandlers = new List<EntityHandler>();

		public void Remove<T>()
		{
			if (typeof(EntityHandler).IsAssignableFrom(typeof(T)))
				RemoveHandler<T>();
			else
				components.RemoveAll(c => c is T);
		}

		private void RemoveHandler<T>()
		{
			if (handlerTypesToAdd.Contains(typeof(T)))
				handlerTypesToAdd.Remove(typeof(T));
			else
				handlerTypesToRemove.Add(typeof(T));
		}

		internal readonly List<Type> handlerTypesToRemove = new List<Type>();

		public int NumberOfComponents
		{
			get { return components.Count; }
		}

		public bool Contains<T>()
		{
			return components.OfType<T>().Any();
		}

		public bool Contains<T1, T2>()
		{
			return Contains<T1>() && Contains<T2>();
		}

		public bool Contains<T1, T2, T3>()
		{
			return Contains<T1>() && Contains<T2>() && Contains<T3>();
		}

		public T Get<T>()
		{
			foreach (T component in components.OfType<T>())
				return component;

			throw new ComponentNotFound(typeof(T));
		}

		public void Set<T>(T component)
		{
			for (int index = 0; index < components.Count; index++)
				if (components[index] is T)
				{
					components[index] = component;
					return;
				}
			throw new ComponentNotFound(typeof(T));
		}

		public class ComponentNotFound : Exception
		{
			public ComponentNotFound(Type component)
				: base(component.ToString()) { }
		}

		public void MessageAllListeners(object message)
		{
			foreach (var listener in activeHandlers.OfType<EntityListener>())
				listener.ReceiveMessage(this, message);

			if (Messaged != null)
				Messaged(message);
		}

		public event Action<object> Messaged;

		/// <summary>
		/// Optional tag that can be set to identify entities, use EntitySystem.GetFromTag
		/// </summary>
		public string Tag { get; set; }

		/// <summary>
		/// Entities start out inactive until added to the EntitySystem. Entities can be activated or
		/// deactivated at any time to not be processed. To disable just one handler use RemoveHandler.
		/// </summary>
		public bool IsActive
		{
			get { return isActive; }
			set
			{
				if (isActive != value)
					isActiveChanged = true;

				isActive = value;
			}
		}

		private bool isActive;
		internal bool isActiveChanged;

		public bool HandlersChanged
		{
			get
			{
				return handlerTypesToAdd.Count > 0 || handlerTypesToRemove.Count > 0 || isActiveChanged;
			}
		}

		public override string ToString()
		{
			return (IsActive ? "" : "<Inactive> ") + GetType().Name + (Tag != null ? " Tag=" + Tag : "") +
				(components.Count > 0 ? ": " + GetTypesText(components) : "") +
				(activeHandlers.Count + handlerTypesToAdd.Count == 0
					? "" : " [" + GetTypesText(activeHandlers) + GetTypesText(handlerTypesToAdd) + "]");
		}

		private static string GetTypesText<T>(IEnumerable<T> typesList)
		{
			if (typeof(T) == typeof(Type))
				return typesList.Aggregate("",
					(text, component) => (text.Length > 0 ? text + ", " : "") + GetTypeName(component));

			return typesList.Aggregate("",
				(text, instance) => (text.Length > 0 ? text + ", " : "") + GetTypeName(instance.GetType()));
		}

		private static string GetTypeName<T>(T component)
		{
			var type = (component as Type);
			if (typeof(IList).IsAssignableFrom(type) && !type.IsArray)
				return type.Name.Replace("`1", "") + "<" + GetTypesText(type.GetGenericArguments()) + ">";

			return type.Name;
		}
	}
}