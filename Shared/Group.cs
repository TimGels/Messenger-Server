using System;

namespace Shared
{
    /// <summary>
    /// Represents a group as defined by the database.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// The unique Id of the group.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the group.
        /// </summary>
        public string Name { get; set; }

        public Group(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Returns a string with the id and the group name encoded in base64.
        /// </summary>
        /// <returns>The encoded string.</returns>
        internal static string ToFormatString(Group group)
        {
            return String.Format("{0},{1}", group.Id, Helper.StringToBase64(group.Name));
        }

        /// <summary>
        /// Create a new group from an encoded base64 string.
        /// </summary>
        /// <param name="encoded">The base64-encoded string.</param>
        /// <returns>The new group.</returns>
        internal static Group FromFormatString(string encoded)
        {
            string[] splitted = encoded.Split(',');
            return new Group(Int32.Parse(splitted[0]), Helper.Base64ToString(splitted[1]));
        }
    }
}
