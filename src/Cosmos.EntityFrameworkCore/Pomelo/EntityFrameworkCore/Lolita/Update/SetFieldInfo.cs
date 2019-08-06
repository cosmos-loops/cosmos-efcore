namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    /// <summary>
    /// Set field info
    /// </summary>
    public class SetFieldInfo
    {
        /// <summary>
        /// FIeld
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Index
        /// </summary>
        public int Index { get; set; }
    }
}