﻿using BL.CrossCutting.Common;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;

namespace BL.Logic.DocumentCore
{
    internal class DocumentProcessor : IDocumentProcessor
    {
        public void SaveDocument (IContext context, BaseDocument document)
        {
            Command cmd;
            if (document.Id == 0) // new document
            {
                cmd = new BaseSaveDocument(context, document);
            }
            else
            {
                cmd = new BaseUpdateDocument(context, document);
            }

            if (cmd.CanExecute())
            {
                cmd.Execute();
            }
        }

    }
}