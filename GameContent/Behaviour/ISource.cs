using Microsoft.Xna.Framework;
using System;

namespace BaselessJumping.GameContent.Behaviour
{
    public interface ISource<TOwner> where TOwner : Entity
    {
        string Reason { get; set; }
    }
}