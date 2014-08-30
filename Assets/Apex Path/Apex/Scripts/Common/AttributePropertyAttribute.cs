namespace Apex.Common
{
    using UnityEngine;

    /// <summary>
    /// Marker for Apex Attribute fields
    /// </summary>
    public class AttributePropertyAttribute : PropertyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributePropertyAttribute"/> class.
        /// </summary>
        public AttributePropertyAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributePropertyAttribute"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        public AttributePropertyAttribute(string label)
        {
            this.label = label;
        }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string label
        {
            get;
            private set;
        }
    }
}
