// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;
using System.Runtime.Serialization;
using Vortice.Mathematics;

namespace Alimer.Engine;

[DefaultEntitySystem(typeof(TransformSystem))]
public sealed class TransformComponent : EntityComponent//, IEnumerable<TransformComponent>, INotifyPropertyChanged
{
    private Vector3 _position;
    private Quaternion _rotation = Quaternion.Identity;
    private Vector3 _scale = Vector3.One;

    private Matrix4x4 _localMatrix = Matrix4x4.Identity;
    private Matrix4x4 _worldMatrix = Matrix4x4.Identity;

    public Vector3 Position
    {
        get => _position;
        set => _position = value;
    }

    public Quaternion Rotation
    {
        get => _rotation;
        set => _rotation = value;
    }

    public Vector3 Scale
    {
        get => _scale;
        set => _scale = value;
    }

    [IgnoreDataMember]
    public Vector3 RotationEuler
    {
        get => Rotation.ToEuler();
        set => Rotation = value.FromEuler();
    }

    [IgnoreDataMember]
    public ref Matrix4x4 LocalMatrix => ref _localMatrix;

    [IgnoreDataMember]
    public ref Matrix4x4 WorldMatrix => ref _worldMatrix;

    public void UpdateLocalMatrix()
    {
        LocalMatrix = Matrix4x4.CreateScale(Scale)
            * Matrix4x4.CreateFromQuaternion(Rotation)
            * Matrix4x4.CreateTranslation(Position);
    }

    public void UpdateWorldMatrix()
    {
        UpdateLocalMatrix();
        UpdateWorldMatrixInternal(true);
    }

    internal void UpdateWorldMatrixInternal(bool recursive)
    {
        //if (Parent is null)
        {
            WorldMatrix = LocalMatrix;
        }
        //else
        //{
        //    if (recursive)
        //    {
        //        Parent.UpdateWorldMatrix();
        //    }
        //
        //    WorldMatrix = LocalMatrix * Parent.WorldMatrix;
        //}
    }

    public override string ToString() => $"Position: {Position}, Rotation: {RotationEuler}, Scale: {Scale}";
}
