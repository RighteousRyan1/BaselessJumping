using Microsoft.Xna.Framework;
using System;

namespace BaselessJumping.GameContent.Behaviour
{
    public interface ISource<TOwner> where TOwner : Entity
    {
        string Reason { get; set; }
    }

    public interface IUnsafeSource<TOwner>
    {
        string Reason { get; set; }
    }

    public interface ISource
    {
        string Reason { get; set; }
    }
}