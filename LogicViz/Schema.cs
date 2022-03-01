using Godot;
using System;
using System.Collections.Generic;

namespace LogicViz
{
	public abstract class Schema
	{
		[Serializable]
		public class OneToOneNode : INode
		{
			// TODO: unnecessary dictionary usage
			public string Label => "1:1 Node";
			public void Tick(float deltaTime)
			{
				foreach (var node in _inputs.Keys)
				{
					float dir = Math.Sign(node.Output - _inputs[node]);
					_inputs[node] += (dir * _propagationRate * deltaTime) % 1f;
				}
			}

			public int MaxInputs => 1;
			private Dictionary<INode, float> _inputs; 
			public Dictionary<INode, float> Inputs => _inputs;

			private float _propagationRate = 1f;
			public float PropagationRate => _propagationRate;

			private float _output;
			public float Output => _output;

			public OneToOneNode(float propagationRate, INode input = null)
			{
				_propagationRate = propagationRate;

				Init(input); 
			}

			public OneToOneNode(INode input = null)
			{
				Init(input);
			}
			
			public void Init(INode input = null)
			{
				_inputs = new Dictionary<INode, float>();
				
				if (input != null)
				{
					_inputs[input] = 0f;
				}
			}
			
			public bool TryRegisterInput(INode node)
			{
				if (_inputs.Keys.Count >= MaxInputs) return false;

				_inputs[node] = 0f;
				return true; 
			}
		}

		[Serializable]
		public abstract class AbstractGate : IGate
		{
			public string Label => "Gate"; 
			private Dictionary<INode, float> _inputs; 
			public Dictionary<INode, float> Inputs => _inputs;
			
			private float _propagationRate = 1f;
			public float PropagationRate => _propagationRate;

			public AbstractGate()
			{
				_inputs = new Dictionary<INode, float>();
			}
			
			public int MaxInputs => 2; 
			
			public bool TryRegisterInput(INode node)
			{
				if (_inputs.Keys.Count >= MaxInputs) return false;

				_inputs[node] = 0f;
				return true; 
			}

			public void Tick(float deltaTime)
			{
				foreach (var node in _inputs.Keys)
				{
					float dir = Math.Sign(node.Output - _inputs[node]);
					_inputs[node] += (dir * _propagationRate * deltaTime) % 1f;
				}
			}

			public float Output => ProcessInputs();
			
			public float ProcessInputs()
			{
				throw new NotImplementedException();
			}
		}

		[Serializable]
		public abstract class Container : IContainer
		{
			public string Label => "Container";
			public Dictionary<INode, float> Inputs => _inputs;
			public HashSet<INode> Contents => _contents;  

			private Dictionary<INode, float> _inputs; 
			private HashSet<INode> _contents; 
			
			public int MaxInputs => _inputs.Count;

			public Container()
			{
				_inputs = new Dictionary<INode, float>();
				_contents = new HashSet<INode>(); 
			}
			
			public bool TryRegisterInput(INode node)
			{
				_inputs[node] = 0f;
				return true; 
			}

			public void Tick(float deltaTime)
			{
				foreach (var node in _inputs.Keys)
				{
					float dir = Math.Sign(node.Output - _inputs[node]);
					_inputs[node] += (dir * _propagationRate * deltaTime) % 1f;
				}
				
				foreach (var c in _contents)
				{
					c.Tick(deltaTime);
				}
			}

			private float _propagationRate = 1f;
			public float PropagationRate => _propagationRate;

			public float Output => ProcessInputs();
			public float ProcessInputs()
			{
				throw new NotImplementedException();
			}

			public Dictionary<INode, float> ProcessContents()
			{
				throw new NotImplementedException();
			}
		}

		interface IContainer : IGate
		{
			Dictionary<INode, float> ProcessContents();
		}
		
		public interface INode
		{
			Dictionary<INode, float> Inputs { get; }
			int MaxInputs { get; }
			bool TryRegisterInput(INode node); 
			float PropagationRate { get; }
			string Label { get; }
			float Output { get; }
			void Tick(float deltaTime);
		}

		interface IGate : INode
		{
			float ProcessInputs(); 
		}
	}
}