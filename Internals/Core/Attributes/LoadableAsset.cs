using System;

namespace BaselessJumping.Internals.Core.Attributes
{
    /// <summary>
    /// Used for assets to be loaded to Ryengine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class LoadableAsset : Attribute
    {
        public LoadableAsset()
        {
            /*
             * Steps:
             * 1) Get the XNA asset compiler to compile files to XNB.
             * 2) Get said LoadableAsset to load into the ProjectPath/Assets Folder.
             * 3) Then, export assets to the .exe directory.
             * 
             * PSA: This should only work for things like SpriteFonts, Effects, or others of the sort.
             * 
             */
        }
    }
}