using System.IO;

namespace Kentico.KInspector.Modules.Export
{
    /// <summary>
    /// Memory stream with Close lock
    /// </summary>
    public class NpoiMemoryStream : MemoryStream
    {
        public NpoiMemoryStream(bool allowClose = true)
        {
            AllowClose = allowClose;
        }

        /// <summary>
        /// Set wheter stream should be close-able. Remember to allow closing when you no longer need the stream.
        /// </summary>
        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
            {
                base.Close();
            }
        }
    }
}
