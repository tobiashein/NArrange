﻿// <auto-generated /> Disable StyleCop on this file
namespace SampleNamespace
{
    public class PropertyCode
    {
        #region Fields

        private bool _property;

        #endregion Fields

        #region Properties

        /// <summary>
        /// NArrange should not alter properties with code.
        /// </summary>
        public bool Property
        {
            get { return _property; }
            set
            {
                // setter even has a comment, if we inline code will definitely no longer compile
                _property = value;
            }
        }

        #endregion Properties
    }
}