using System.IO;

namespace Kentico.KInspector.Modules.Export
{
    /// <summary>
    /// Memory stream with Close lock. Used to bypass NPOI closing stream after Write.
    /// </summary>
    public class NpoiMemoryStream : MemoryStream
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="allowClose">Allow Close for stream. Property <see cref="AllowClose"/>.</param>
        public NpoiMemoryStream(bool allowClose = true)
        {
            AllowClose = allowClose;
        }

        /// <summary>
        /// Set wheter stream should be close-able. Remember to allow closing when you no longer need the stream.
        /// </summary>
        public bool AllowClose { get; set; }

        /// <summary>
        /// Overload of close method taking into account property <see cref="AllowClose"/>.
        /// </summary>
        public override void Close()
        {
            if (AllowClose)
            {
                base.Close();
            }
        }
    }
}
