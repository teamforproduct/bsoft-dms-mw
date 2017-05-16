namespace BL.Model.Enums
{
    /// <summary>
    /// Describe type of the user's file. 
    /// </summary>
    public enum EnumDocumentFileType
    {
        /// <summary>
        /// Original user file
        /// </summary>
        UserFile = 0,
        /// <summary>
        /// Pdf copy of the file
        /// </summary>
        PdfFile = 1,
        /// <summary>
        /// Short preview for PDF file
        /// </summary>
        PdfPreview = 2,

    }
}